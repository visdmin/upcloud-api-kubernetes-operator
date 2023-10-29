using IdentityModel.Client;
using Microsoft.Extensions.Options;

using UpcloudApiKubernetesOperator.UpCloudApi.Options;
using UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2;

namespace UpcloudApiKubernetesOperator.UpCloudApi.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddUpCloudApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<UpCloudApiOptions>(
            config: configuration.GetRequiredSection(key: UpCloudApiOptions.DEFAULT_SECTION_NAME)
        );

        services.AddHttpClient<UpCloudApiClient>((serviceProvider, httpClient) => {
            var apiConfiguration = serviceProvider.GetRequiredService<IOptions<UpCloudApiOptions>>().Value;

            httpClient.Timeout     = TimeSpan.FromSeconds(apiConfiguration.RequestTimeoutSeconds);
            httpClient.BaseAddress = new Uri(apiConfiguration.Endpoint);

            httpClient.SetBasicAuthentication(
                userName: apiConfiguration.Username,
                password: apiConfiguration.Password
            );
        });

        services.AddSingleton<IObjectStorageV2Client, ObjectStorageV2Client>();

        return services;
    }
}
