using System.Collections.ObjectModel;

using Microsoft.Extensions.Options;

namespace UpcloudApiKubernetesOperator.AutoLoader.Options;

internal sealed class AutoLoaderOptions
{
    public const string DEFAULT_SECTION_NAME = "AutoLoader";
    public bool Enabled        { get; init; } = false;
    public int RefreshInterval { get; init; } = 120;
    public string Namespace    { get; init; } = string.Empty;

    public AutoLoaderOptions() {}

    public class Validator : IValidateOptions<AutoLoaderOptions>
    {
        public ValidateOptionsResult Validate(string? name, AutoLoaderOptions options)
        {
            Collection<string>? errors = null;

            if (options.RefreshInterval < 1) {
                (errors ??= new ()).Add($"'{nameof(options.RefreshInterval)}' has to be positive integer, auto loader refresh interval in seconds");
            }

            if (string.IsNullOrEmpty(options.Namespace)) {
                (errors ??= new ()).Add($"'{nameof(options.Namespace)}' cannot be null or empty");
            }

            if (errors is not null) {
                return ValidateOptionsResult.Fail(errors);
            }

            return ValidateOptionsResult.Success;
        }
    }
}
