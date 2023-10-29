using System.Text.Json;
using System.Text.Json.Serialization;

namespace UpcloudApiKubernetesOperator.UpCloudApi;

internal abstract class UpCloudApiClient
{
    protected readonly HttpClient HttpClient;
    protected readonly ILogger Logger;
    protected readonly JsonSerializerOptions JsonSerializerOptions;

    public UpCloudApiClient(IHttpClientFactory httpClientFactory, ILogger<UpCloudApiClient> logger)
    {
        HttpClient            = httpClientFactory.CreateClient(name: typeof(UpCloudApiClient).Name);
        Logger                = logger;
        JsonSerializerOptions = new JsonSerializerOptions {
            IncludeFields            = true,
            IgnoreReadOnlyProperties = false,
            IgnoreReadOnlyFields     = false,
            DefaultIgnoreCondition   = JsonIgnoreCondition.WhenWritingNull
        };
    }
}
