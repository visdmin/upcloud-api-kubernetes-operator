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
	metav1 "k8s.io/apimachinery/pkg/apis/meta/v1"
)

// SimplePlanSpec defines the desired state of SimplePlan
type SimplePlanSpec struct {
	CoreNumber       int    `json:"core_number,omitempty"`
	MemoryAmount     int    `json:"memory_amount,omitempty"`
	PublicTrafficOut int    `json:"public_traffic_out,omitempty"`
	StorageSize      int    `json:"storage_size,omitempty"`
	StorageTier      string `json:"storage_tier,omitempty"`
}

// SimplePlanStatus defines the observed state of SimplePlan
type SimplePlanStatus struct {
	CoreNumber       int    `json:"core_number,omitempty"`
	MemoryAmount     int    `json:"memory_amount,omitempty"`
	PublicTrafficOut int    `json:"public_traffic_out,omitempty"`
	StorageSize      int    `json:"storage_size,omitempty"`
	StorageTier      string `json:"storage_tier,omitempty"`
}

// +kubebuilder:object:root=true
// +kubebuilder:subresource:status

// SimplePlan is the Schema for the simpleplans API
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
