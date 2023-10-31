using Microsoft.Extensions.Options;

using UpcloudApiKubernetesOperator.AutoLoader.Options;
using UpcloudApiKubernetesOperator.AutoLoader.Loaders.Extensions.DependencyInjection;

namespace UpcloudApiKubernetesOperator.AutoLoader.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddAutoLoaderService(this IServiceCollection services, IConfiguration configuration) =>
        configuration.GetRequiredSection(key: AutoLoaderOptions.DEFAULT_SECTION_NAME) switch {
            var autoLoaderConfig => autoLoaderConfig.GetValue("Enabled", defaultValue: false) switch {
                false => services,
                true  => services
                    .Configure<AutoLoaderOptions>(config: autoLoaderConfig)
                    .AddSingleton<IValidateOptions<AutoLoaderOptions>, AutoLoaderOptions.Validator>()
                    .AddLoaders()
                    .AddHostedService<AutoLoaderService>()
            }
        };
}
