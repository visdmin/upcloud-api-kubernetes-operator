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
	"strconv"
	"time"

	"k8s.io/apimachinery/pkg/runtime"
	ctrl "sigs.k8s.io/controller-runtime"
	"sigs.k8s.io/controller-runtime/pkg/client"
	"sigs.k8s.io/controller-runtime/pkg/log"

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
		return ctrl.Result{}, client.IgnoreNotFound(err)
	}

	secretRef := account.Spec.ApiCredentialSecretRef
	apiCredentialsSecret, err := getApiCredentials(ctx, r.Client, secretRef)
	if err != nil {
		r.updateStatusMessage(ctx, &account, log, "SyncFailed")
		log.Error(err, "Account reconcile failed, unable to fetch API credentials secret",
			"accountName", account.Name,
			"secretNamespace", secretRef.Namespace,
			"secretName", secretRef.Name,
		)
		return r.RequeueAfter(accountReconcileInterval)
	}

	srv := getUpCService(*apiCredentialsSecret)
	apiCtx, cancel := context.WithTimeout(context.Background(), timeoutGetRequest)
	defer cancel()

	accountDetails, err := srv.GetAccount(apiCtx)
	if err != nil {
		r.updateStatusMessage(ctx, &account, log, "SyncFailed")
		log.Error(err, "Account reconcile failed, unable to fetch account details", "accountName", account.Name)
		return r.RequeueAfter(accountReconcileInterval)
	}

	account.Status = upcloudv1alpha1.AccountStatus{
		Status:   "Synced",
		UserName: accountDetails.UserName,
		Credits:  strconv.FormatFloat(accountDetails.Credits, 'f', 2, 64),
	}

	if err := r.Client.Status().Update(ctx, &account); err != nil {
		r.updateStatusMessage(ctx, &account, log, "SyncFailed")
		log.Error(err, "Account reconcile failed, unable to update status", "accountName", account.Name)
		return r.RequeueAfter(accountReconcileInterval)
	}

	return r.RequeueAfter(accountReconcileInterval)
}

// SetupWithManager sets up the controller with the Manager.
func (r *AccountReconciler) SetupWithManager(mgr ctrl.Manager) error {
	return ctrl.NewControllerManagedBy(mgr).
		For(&upcloudv1alpha1.Account{}).
		Complete(r)
}

func (r *AccountReconciler) RequeueAfter(duration time.Duration) (ctrl.Result, error) {
	return ctrl.Result{RequeueAfter: duration}, nil
}

func (r *AccountReconciler) updateStatusMessage(ctx context.Context, account *upcloudv1alpha1.Account, log logr.Logger, errorMsg string) {
	account.Status = r.getUpdatedStatusWithError(errorMsg, account)
	err := r.Client.Status().Update(ctx, account)
	if err == nil {
		return
	}

	log.Error(err, "Account reconcile failed unable to update status", "accountName", account.Name)
}

func (r *AccountReconciler) getUpdatedStatusWithError(err string, account *upcloudv1alpha1.Account) upcloudv1alpha1.AccountStatus {
	return upcloudv1alpha1.AccountStatus{
		Status: err,
	}
}
