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
	"time"

	"github.com/UpCloudLtd/upcloud-go-api/v8/upcloud"
	"github.com/go-logr/logr"
	upcloudv1alpha1 "github.com/visdmin/upcloud-api-kubernetes-operator/api/v1alpha1"
	"k8s.io/apimachinery/pkg/runtime"
	ctrl "sigs.k8s.io/controller-runtime"
	"sigs.k8s.io/controller-runtime/pkg/client"
	"sigs.k8s.io/controller-runtime/pkg/log"
)

// SimplePlanReconciler reconciles a SimplePlan object
type SimplePlanReconciler struct {
	client.Client
	Scheme *runtime.Scheme
}

// +kubebuilder:rbac:groups=upcloud.visdmin.com,resources=simpleplans,verbs=get;list;watch;create;update;patch;delete
// +kubebuilder:rbac:groups=upcloud.visdmin.com,resources=simpleplans/status,verbs=get;update;patch
// +kubebuilder:rbac:groups=upcloud.visdmin.com,resources=simpleplans/finalizers,verbs=update

// Reconcile is part of the main kubernetes reconciliation loop which aims to
// move the current state of the cluster closer to the desired state.
// TODO(user): Modify the Reconcile function to compare the state specified by
// the SimplePlan object against the actual cluster state, and then
// perform operations to make the cluster state reflect the state specified by
// the user.
//
// For more details, check Reconcile and its Result here:
// - https://pkg.go.dev/sigs.k8s.io/controller-runtime@v0.19.0/pkg/reconcile
func (r *SimplePlanReconciler) Reconcile(ctx context.Context, req ctrl.Request) (ctrl.Result, error) {
	log := log.FromContext(ctx)

	var simplePlan upcloudv1alpha1.SimplePlan
	if err := r.Get(ctx, req.NamespacedName, &simplePlan); err != nil {
		return ctrl.Result{}, client.IgnoreNotFound(err)
	}

	acc, err := getAccountByReference(ctx, r.Client, simplePlan.Spec.AccountRef)
	srv, err := getUpCServiceByAccount(ctx, r.Client, acc)
	if err != nil {
		r.updateStatusMessage(ctx, &simplePlan, log, "SyncFailed")
		log.Error(err, "SimplePlan reconcile failed, unable to fetch UpCloud service",
			"simplePlanName", simplePlan.Name,
			"accountRef", simplePlan.Spec.AccountRef,
		)
		return r.RequeueAfter(simplePlanReconcileInterval)
	}

	apiCtx, cancel := context.WithTimeout(context.Background(), timeoutGetRequest)
	defer cancel()

	plans, err := srv.GetPlans(apiCtx)
	if err != nil {
		r.updateStatusMessage(ctx, &simplePlan, log, "SyncFailed")
		log.Error(err, "SimplePlan reconcile failed, unable to fetch UpCloud plans", "simplePlanName", simplePlan.Name)
		return r.RequeueAfter(simplePlanReconcileInterval)
	}

	plan := findPlanByName(plans, simplePlan.Spec.PlanName)
	if plan == nil {
		r.updateStatusMessage(ctx, &simplePlan, log, "PlanNotFound")
		log.Error(err, "SimplePlan reconcile failed, unable to find plan", "simplePlanName", simplePlan.Name, "planName", simplePlan.Spec.PlanName)
		return r.RequeueAfter(simplePlanReconcileInterval)
	}

	if err := ctrl.SetControllerReference(acc, &simplePlan, r.Scheme); err != nil {
		r.updateStatusMessage(ctx, &simplePlan, log, "SyncFailed")
		log.Error(err, "SimplePlan reconcile failed, unable to set account as owner reference", "simplePlanName", simplePlan.Name)
		return r.RequeueAfter(simplePlanReconcileInterval)
	}

	simplePlan.Status = upcloudv1alpha1.SimplePlanStatus{
		Status:           "Synced",
		CoreNumber:       plan.CoreNumber,
		MemoryAmount:     plan.MemoryAmount,
		PublicTrafficOut: plan.PublicTrafficOut,
		StorageSize:      plan.StorageSize,
		StorageTier:      plan.StorageTier,
	}

	if err := r.Client.Status().Update(ctx, &simplePlan); err != nil {
		r.updateStatusMessage(ctx, &simplePlan, log, "SyncFailed")
		log.Error(err, "SimplePlan reconcile failed, unable to update status", "simplePlanName", simplePlan.Name)
		return r.RequeueAfter(simplePlanReconcileInterval)
	}

	return r.RequeueAfter(simplePlanReconcileInterval)
}

// SetupWithManager sets up the controller with the Manager.
func (r *SimplePlanReconciler) SetupWithManager(mgr ctrl.Manager) error {
	return ctrl.NewControllerManagedBy(mgr).
		For(&upcloudv1alpha1.SimplePlan{}).
		Complete(r)
}

func (r *SimplePlanReconciler) RequeueAfter(duration time.Duration) (ctrl.Result, error) {
	return ctrl.Result{RequeueAfter: duration}, nil
}

func (r *SimplePlanReconciler) updateStatusMessage(ctx context.Context, simplePlan *upcloudv1alpha1.SimplePlan, log logr.Logger, errorMsg string) {
	simplePlan.Status = r.getUpdatedStatusWithError(errorMsg, simplePlan)
	err := r.Client.Status().Update(ctx, simplePlan)
	if err == nil {
		return
	}

	log.Error(err, "SimplePlan reconcile failed unable to update status", "simplePlanName", simplePlan.Name)
}

func (r *SimplePlanReconciler) getUpdatedStatusWithError(err string, account *upcloudv1alpha1.SimplePlan) upcloudv1alpha1.SimplePlanStatus {
	return upcloudv1alpha1.SimplePlanStatus{
		Status: err,
	}
}

func findPlanByName(plans *upcloud.Plans, name string) *upcloud.Plan {
	for _, plan := range plans.Plans {
		if plan.Name == name {
			return &plan
		}
	}
	return nil
}
