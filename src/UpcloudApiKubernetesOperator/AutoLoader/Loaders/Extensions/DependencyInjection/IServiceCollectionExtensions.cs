using Microsoft.Extensions.Options;
using UpcloudApiKubernetesOperator.AutoLoader.Options;

namespace UpcloudApiKubernetesOperator.AutoLoader.Loaders.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddLoaders(this IServiceCollection services) => services
        .AddSingleton<ObjectStorageV2Loader>();
}
