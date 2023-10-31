using System.Text.Json.Serialization;

namespace UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Responses;

public readonly struct ErrorResponse
{
    [JsonPropertyName("type")]
    public readonly string Type;

    [JsonPropertyName("title")]
    public readonly string Title;

    [JsonPropertyName("correlation_id")]
    public readonly string CorrelationId;

    [JsonPropertyName("status")]
    public readonly int Status;

    [JsonConstructor]
    public ErrorResponse(string type, string title, string correlationId, int status) => (Type, Title, CorrelationId, Status) = (type, type, correlationId, status);
}
