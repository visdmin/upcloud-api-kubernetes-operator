using System.Text.Json.Serialization;

namespace UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Responses;

public readonly struct DeleteInstanceResponse
{
    public readonly bool Success;
    public readonly ErrorResponse? ErrorResponse;

    public DeleteInstanceResponse(bool success, ErrorResponse? errorResponse) => (Success, ErrorResponse) = (success, errorResponse);
}
