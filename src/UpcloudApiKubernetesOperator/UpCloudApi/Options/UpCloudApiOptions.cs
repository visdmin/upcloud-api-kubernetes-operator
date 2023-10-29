using System.Collections.ObjectModel;
using Microsoft.Extensions.Options;

namespace UpcloudApiKubernetesOperator.UpCloudApi.Options;

public class UpCloudApiOptions
{
    public const string DEFAULT_SECTION_NAME = "UpCloudApiOptions";
    public string Endpoint           { get; init; } = "https://api.upcloud.com/";
    public string Username           { get; init; } = string.Empty;
    public string Password           { get; init; } = string.Empty;
    public int RequestTimeoutSeconds { get; init; } = default;

    public class Validator : IValidateOptions<UpCloudApiOptions>
    {
        public ValidateOptionsResult Validate(string? name, UpCloudApiOptions options)
        {
            Collection<string>? errors = null;

            if (string.IsNullOrEmpty(options.Endpoint)) {
                (errors ??= new ()).Add($"'{nameof(options.Endpoint)}' cannot be null or empty");
            }

            if (!Uri.TryCreate(uriString: options.Endpoint, creationOptions: new UriCreationOptions(), out _)) {
                (errors ??= new ()).Add($"'{nameof(options.Endpoint)}' has to be parsable to valid uri");
            }

            if (string.IsNullOrEmpty(options.Username)) {
                (errors ??= new ()).Add($"'{nameof(options.Username)}' cannot be null or empty");
            }

            if (string.IsNullOrEmpty(options.Password)) {
                (errors ??= new ()).Add($"'{nameof(options.Password)}' cannot be null or empty");
            }

            if (options.RequestTimeoutSeconds < 1) {
                (errors ??= new ()).Add($"'{nameof(options.RequestTimeoutSeconds)}' has to be positive integer");
            }

            if (errors is not null) {
                return ValidateOptionsResult.Fail(errors);
            }

            return ValidateOptionsResult.Success;
        }
    }
}
