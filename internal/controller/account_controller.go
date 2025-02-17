/*
Copyright 2025.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

package controller

import (
	"context"
	"fmt"
	"strconv"

	v1 "k8s.io/api/core/v1"
	metav1 "k8s.io/apimachinery/pkg/apis/meta/v1"
	"k8s.io/apimachinery/pkg/runtime"
	ctrl "sigs.k8s.io/controller-runtime"
	"sigs.k8s.io/controller-runtime/pkg/client"
	"sigs.k8s.io/controller-runtime/pkg/controller/controllerutil"
	"sigs.k8s.io/controller-runtime/pkg/log"

	"github.com/UpCloudLtd/upcloud-go-api/v8/upcloud"
	upcloudApi "github.com/UpCloudLtd/upcloud-go-api/v8/upcloud/client"
	upcloudService "github.com/UpCloudLtd/upcloud-go-api/v8/upcloud/service"
	"github.com/go-logr/logr"
	upcloudv1alpha1 "github.com/visdmin/upcloud-api-kubernetes-operator/api/v1alpha1"
)

// AccountReconciler reconciles a Account object
type AccountReconciler struct {
	client.Client
	Scheme *runtime.Scheme
}

// +kubebuilder:rbac:groups=upcloud.visdmin.com,resources=accounts,verbs=get;list;watch;create;update;patch;delete
// +kubebuilder:rbac:groups=upcloud.visdmin.com,resources=accounts/status,verbs=get;update;patch
// +kubebuilder:rbac:groups=upcloud.visdmin.com,resources=accounts/finalizers,verbs=update

// Reconcile is part of the main kubernetes reconciliation loop which aims to
// move the current state of the cluster closer to the desired state.
func (r *AccountReconciler) Reconcile(ctx context.Context, req ctrl.Request) (ctrl.Result, error) {
	log := log.FromContext(ctx)

	var account upcloudv1alpha1.Account
	if err := r.Get(ctx, req.NamespacedName, &account); err != nil {
		log.Error(err, "unable to fetch Account")
		return ctrl.Result{}, client.IgnoreNotFound(err)
	}

	secretRef := account.Spec.ApiCredentialSecretRef
	apiCredentialsSecret, err := getApiCredentials(ctx, r.Client, secretRef)
	if err != nil {
		updateStatusMessage(ctx, r.Client, &account, log, "SyncFailed")
		log.Error(err, "Account reconcile failed %s unable to fetch API credentials secret %s/%s", account.Name, secretRef.Namespace, secretRef.Name)
		return ctrl.Result{}, err
	}

	srv := getUpCService(*apiCredentialsSecret)
	apiCtx, cancel := context.WithTimeout(context.Background(), timeoutGetRequest)
	defer cancel()

	accountDetails, err := srv.GetAccount(apiCtx)
	if err != nil {
		updateStatusMessage(ctx, r.Client, &account, log, "SyncFailed")
		log.Error(err, "Account reconcile failed %s unable to fetch account details", account.Name)
		return ctrl.Result{}, err
	}

	account.Status = getUpdatedStatus(accountDetails, &account)
	if r.Status().Update(ctx, &account) != nil {
		log.Error(err, "Account reconcile failed %s unable to update status", account.Name)
		return ctrl.Result{}, err
	}

	plans, err := srv.GetPlans(apiCtx)
	if err != nil {
		updateStatusMessage(ctx, r.Client, &account, log, "PlanSyncFailed")
		log.Error(err, "Account reconcile failed %s unable to fetch plans", account.Name)
		return ctrl.Result{}, err
	}

	var existingPlans upcloudv1alpha1.SimplePlanList
	if r.List(ctx, &existingPlans) != nil {
		updateStatusMessage(ctx, r.Client, &account, log, "PlanSyncFailed")
		log.Error(err, "Account reconcile failed %s unable to list existing plans", account.Name)
		return ctrl.Result{}, err
	}

	for _, plan := range plans.Plans {
		found, oldPlan := tryFindPlan(&existingPlans.Items, plan.Name)
		if found {
			if !hasPlanChanged(&plan, oldPlan) {
				continue
			}

			newPlanSpec := getUpdatedPlanSpec(&plan, oldPlan)
			oldPlan.Spec = newPlanSpec
			if r.Update(ctx, oldPlan) != nil {
				updateStatusMessage(ctx, r.Client, &account, log, "PlanSyncFailed")
				log.Error(err, "Account reconcile failed %s unable to update plan %s", account.Name, plan.Name)
				return ctrl.Result{}, err
			}

			continue
		}

		newPlan, err := createOwnedPlan(r, &account, &plan)
		if err != nil {
			updateStatusMessage(ctx, r.Client, &account, log, "PlanSyncFailed")
			log.Error(err, "Account reconcile failed %s unable to create plan instance %s", account.Name, plan.Name)
			return ctrl.Result{}, err
		}

		if r.Create(ctx, &newPlan) != nil {
			updateStatusMessage(ctx, r.Client, &account, log, "PlanSyncFailed")
			log.Error(err, "Account reconcile failed %s unable to create plan %s", account.Name, plan.Name)
			return ctrl.Result{}, err
		}
	}

	return ctrl.Result{}, nil
}

// SetupWithManager sets up the controller with the Manager.
func (r *AccountReconciler) SetupWithManager(mgr ctrl.Manager) error {
	return ctrl.NewControllerManagedBy(mgr).
		For(&upcloudv1alpha1.Account{}).
		Complete(r)
}

func getApiCredentials(ctx context.Context, r client.Client, secretRef v1.SecretReference) (*v1.Secret, error) {
	var secret v1.Secret
	if err := r.Get(ctx, client.ObjectKey{
		Namespace: secretRef.Namespace,
		Name:      secretRef.Name,
	}, &secret); err != nil {
		return nil, err
	}

	data := secret.Data

	if _, ok := data["username"]; !ok {
		return nil, fmt.Errorf("missing username key in secret %s/%s", secret.Namespace, secret.Name)
	}

	if _, ok := data["password"]; !ok {
		return nil, fmt.Errorf("missing password key in secret %s/%s", secret.Namespace, secret.Name)
	}

	return &secret, nil
}

func getUpCService(secret v1.Secret) *upcloudService.Service {

	credentials := secret.Data
	client := upcloudApi.New(
		string(credentials["username"]),
		string(credentials["password"]),
	)

	return upcloudService.New(client)
}

func getUpdatedStatus(accountDetails *upcloud.Account, account *upcloudv1alpha1.Account) upcloudv1alpha1.AccountStatus {
	credits := strconv.FormatFloat(accountDetails.Credits, 'f', 2, 64)
	return upcloudv1alpha1.AccountStatus{
		Name:     account.Spec.Name,
		Status:   "Synced",
		UserName: accountDetails.UserName,
		Credits:  credits,
	}
}

func updateStatusMessage(ctx context.Context, r client.Client, account *upcloudv1alpha1.Account, log logr.Logger, errorMsg string) {
	account.Status = getUpdatedStatusWithError(errorMsg, account)
	err := r.Status().Update(ctx, account)
	if err == nil {
		return
	}

	log.Error(err, "Account reconcile failed %s unable to update status", account.Name)
}

func getUpdatedStatusWithError(err string, account *upcloudv1alpha1.Account) upcloudv1alpha1.AccountStatus {
	return upcloudv1alpha1.AccountStatus{
		Name:   account.Spec.Name,
		Status: err,
	}
}

func tryFindPlan(plans *[]upcloudv1alpha1.SimplePlan, planName string) (bool, *upcloudv1alpha1.SimplePlan) {
	for _, plan := range *plans {
		if plan.GetObjectMeta().GetName() == planName {
			return true, &plan
		}
	}

	return false, nil
}

func hasPlanChanged(newPlan *upcloud.Plan, oldPlan *upcloudv1alpha1.SimplePlan) bool {
	return newPlan.CoreNumber != oldPlan.Spec.CoreNumber ||
		newPlan.MemoryAmount != oldPlan.Spec.MemoryAmount ||
		newPlan.PublicTrafficOut != oldPlan.Spec.PublicTrafficOut ||
		newPlan.StorageSize != oldPlan.Spec.StorageSize ||
		newPlan.StorageTier != oldPlan.Spec.StorageTier
}

func createOwnedPlan(r *AccountReconciler, account *upcloudv1alpha1.Account, newPlan *upcloud.Plan) (upcloudv1alpha1.SimplePlan, error) {
	plan := upcloudv1alpha1.SimplePlan{
		ObjectMeta: metav1.ObjectMeta{
			Name:      newPlan.Name,
			Namespace: account.Namespace,
		},
		Spec: upcloudv1alpha1.SimplePlanSpec{
			CoreNumber:       newPlan.CoreNumber,
			MemoryAmount:     newPlan.MemoryAmount,
			PublicTrafficOut: newPlan.PublicTrafficOut,
			StorageSize:      newPlan.StorageSize,
			StorageTier:      newPlan.StorageTier,
		},
	}

	err := controllerutil.SetOwnerReference(account, &plan, r.Scheme)
	if err != nil {
		return upcloudv1alpha1.SimplePlan{}, err
	}

	return plan, nil
}

func getUpdatedPlanSpec(newPlan *upcloud.Plan, oldPlan *upcloudv1alpha1.SimplePlan) upcloudv1alpha1.SimplePlanSpec {
	return upcloudv1alpha1.SimplePlanSpec{
		CoreNumber:       newPlan.CoreNumber,
		MemoryAmount:     newPlan.MemoryAmount,
		PublicTrafficOut: newPlan.PublicTrafficOut,
		StorageSize:      newPlan.StorageSize,
		StorageTier:      newPlan.StorageTier,
	}
}
