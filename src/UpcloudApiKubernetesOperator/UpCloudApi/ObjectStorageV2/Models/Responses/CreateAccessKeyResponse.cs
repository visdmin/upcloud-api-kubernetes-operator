using System.Diagnostics.CodeAnalysis;

namespace UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Responses;

public readonly struct CreateAccessKeyResponse
{
    public readonly bool Success;
    public readonly ErrorResponse? ErrorResponse;
    public readonly NewAccessKeyDetails? Details;

    [MemberNotNullWhen(true, nameof(Details))]
    public bool IsSuccessful => Success is true && Details is not null;

    public CreateAccessKeyResponse(bool success, ErrorResponse? errorResponse = null, NewAccessKeyDetails? details = null)
        => (Success, ErrorResponse, Details) = (success, errorResponse, details);
}
