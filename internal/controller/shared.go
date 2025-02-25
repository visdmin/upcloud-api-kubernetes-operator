package controller

import (
	"context"
	"fmt"

	v1 "k8s.io/api/core/v1"
	"sigs.k8s.io/controller-runtime/pkg/client"

	upcloudApi "github.com/UpCloudLtd/upcloud-go-api/v8/upcloud/client"
	upcloudService "github.com/UpCloudLtd/upcloud-go-api/v8/upcloud/service"
	upcloudv1alpha1 "github.com/visdmin/upcloud-api-kubernetes-operator/api/v1alpha1"
)

func getUpCServiceByAccount(ctx context.Context, r client.Client, acc *upcloudv1alpha1.Account) (*upcloudService.Service, error) {
	secretRef := acc.Spec.ApiCredentialSecretRef
	apiCredentialsSecret, err := getApiCredentials(ctx, r, secretRef)
	if err != nil {
		return nil, err
	}

	return getUpCService(*apiCredentialsSecret), nil
}

func getAccountByReference(ctx context.Context, r client.Client, ref v1.ObjectReference) (*upcloudv1alpha1.Account, error) {
	var account upcloudv1alpha1.Account
	err := r.Get(ctx, client.ObjectKey{
		Namespace: ref.Namespace,
		Name:      ref.Name,
	}, &account)

	if err != nil {
		return nil, err
	}

	return &account, nil
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
