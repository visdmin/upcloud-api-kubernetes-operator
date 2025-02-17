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

// ServerSpec defines the desired state of Server
type ServerSpec struct {
	Title        string  `json:"title,omitempty"`
	Hostname     string  `json:"hostname,omitempty"`
	Zone         string  `json:"zone,omitempty"`
	Labels       []Label `json:"labels,omitempty"`
	CoreNumber   int64   `json:"core_number,omitempty"`
	MemoryAmount int64   `json:"memory_amount,omitempty"`
	Plan         string  `json:"plan,omitempty"`
}

// ServerStatus defines the observed state of Server
type ServerStatus struct {
	UUID         string `json:"uuid,omitempty"`
	Title        string `json:"title,omitempty"`
	MemoryAmount int64  `json:"memory_amount,omitempty"`
	CoreNumber   int64  `json:"core_number,omitempty"`
	Plan         string `json:"plan,omitempty"`
	State        string `json:"state,omitempty"`
}

type IpAddress struct {
	Address string `json:"address,omitempty"`
	Access  string `json:"access,omitempty"`
	Family  string `json:"family,omitempty"`
}

type Label struct {
	Key   string `json:"key,omitempty"`
	Value string `json:"value,omitempty"`
}

// +kubebuilder:object:root=true
// +kubebuilder:subresource:status

// Server is the Schema for the servers API
type Server struct {
	metav1.TypeMeta   `json:",inline"`
	metav1.ObjectMeta `json:"metadata,omitempty"`

	Spec   ServerSpec   `json:"spec,omitempty"`
	Status ServerStatus `json:"status,omitempty"`
}

// +kubebuilder:object:root=true

// ServerList contains a list of Server
type ServerList struct {
	metav1.TypeMeta `json:",inline"`
	metav1.ListMeta `json:"metadata,omitempty"`
	Items           []Server `json:"items"`
}

func init() {
	SchemeBuilder.Register(&Server{}, &ServerList{})
}
