namespace UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Responses;

public readonly struct CreateInstanceResponse
{
    public readonly bool Success;
    public readonly ErrorResponse? ErrorResponse;

    public CreateInstanceResponse(bool success, ErrorResponse? errorResponse) => (Success, ErrorResponse) = (success, errorResponse);
}
