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

package v1alpha1

import (
	corev1 "k8s.io/api/core/v1"
	metav1 "k8s.io/apimachinery/pkg/apis/meta/v1"
)

// SimplePlanSpec defines the desired state of SimplePlan
type SimplePlanSpec struct {
	AccountRef corev1.ObjectReference `json:"account_ref,omitempty"`
	PlanName   string                 `json:"plan_name,omitempty"`
}

// SimplePlanStatus defines the observed state of SimplePlan
type SimplePlanStatus struct {
	Status           string `json:"status,omitempty"`
	CoreNumber       int    `json:"core_number,omitempty"`
	MemoryAmount     int    `json:"memory_amount,omitempty"`
	PublicTrafficOut int    `json:"public_traffic_out,omitempty"`
	StorageSize      int    `json:"storage_size,omitempty"`
	StorageTier      string `json:"storage_tier,omitempty"`
}

// SimplePlan is the Schema for the simpleplans API
// +k8s:openapi-gen=true
// +kubebuilder:object:root=true
// +kubebuilder:subresource:status
// +kubebuilder:resource:shortName=plan
// +kubebuilder:subresource:spec
// +kubebuilder:printcolumn:name="Status",type="string",JSONPath=".status.status",description="Server plan status"
// +kubebuilder:printcolumn:name="Account",type="string",JSONPath=".spec.account_ref",description="Server plan CPU core number"
// +kubebuilder:printcolumn:name="PlanName",type="string",JSONPath=".spec.plan_name",description="Server plan CPU core number"
// +kubebuilder:printcolumn:name="CoreNumber",type="string",JSONPath=".status.core_number",description="Server plan CPU core number"
// +kubebuilder:printcolumn:name="MemoryAmount",type="string",JSONPath=".status.memory_amount",description="Server plan memory amount"
// +kubebuilder:printcolumn:name="PublicTrafficOut",type="string",JSONPath=".status.public_traffic_out",description="Server plan public traffic out"
// +kubebuilder:printcolumn:name="StorageSize",type="string",JSONPath=".status.storage_size",description="Server plan storage size"
// +kubebuilder:printcolumn:name="StorageTier",type="string",JSONPath=".status.storage_tier",description="Server plan storage tier"
type SimplePlan struct {
	metav1.TypeMeta   `json:",inline"`
	metav1.ObjectMeta `json:"metadata,omitempty"`

	Spec   SimplePlanSpec   `json:"spec,omitempty"`
	Status SimplePlanStatus `json:"status,omitempty"`
}

// +kubebuilder:object:root=true

// SimplePlanList contains a list of SimplePlan
type SimplePlanList struct {
	metav1.TypeMeta `json:",inline"`
	metav1.ListMeta `json:"metadata,omitempty"`
	Items           []SimplePlan `json:"items"`
}

func init() {
	SchemeBuilder.Register(&SimplePlan{}, &SimplePlanList{})
}
