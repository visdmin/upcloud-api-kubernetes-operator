# upcloud-api-kubernetes-operator
Kubernetes operator hobby project for managing services running in UpCloud via the public [UpCloud API](https://developers.upcloud.com/1.3/1-introduction/)

## Disclaimer:
This project is an independent tool and is not affiliated with, officially supported, or endorsed by UpCloud Ltd. or any of its affiliates.
Any references to "UpCloud" are for identification purposes only. Users of this tool should understand that it operates via UpCloud's public API and should be used with caution.
While we strive to ensure compatibility and correct functionality, any actions taken or operations performed using this tool are at the user's own risk.
Always refer to the official UpCloud documentation and support channels for authoritative information and assistance.

## This operator project aims to:
- [ ] Implement UpCloud API objects as custom resource types.
- [ ] To keep up with UpCloud API object states by pulling changes from the UpCloud API.
- [ ] To be able to display the current UpCloud API object states via Kubernetes API. 
- [ ] To be able to maintain the desired state of the UpCloud API objects.

These features can be implemented one by one, and the features can be implemented only for selected UpCloud API objects.

These feature goals may change during the development of the operator when we gain better understanding of the UpCloud API, and Kubernetes operator framework.

## Implementation choices:
This operator is build using [**KubeOps** - dotnet-operator-sdk](https://github.com/buehler/dotnet-operator-sdk).

KubeOps .NET sdk was selected purely because the original author of the project happened to be most experinced with .NET development.

Original author of the project is aware that current standard for building kubernetes operator seems to be to use Go lang sdk's like the [kubebuilder](https://book.kubebuilder.io).
