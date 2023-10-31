using System.Text.Json.Serialization;

namespace UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Responses;

public readonly struct ReplaceInstanceResponse
{
    public readonly bool Success;
    public readonly ErrorResponse? ErrorResponse;

    public ReplaceInstanceResponse(bool success, ErrorResponse? errorResponse) => (Success, ErrorResponse) = (success, errorResponse);
}
