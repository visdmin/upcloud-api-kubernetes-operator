using System.Collections.ObjectModel;

using KubeOps.Operator.Webhooks;
using UpcloudApiKubernetesOperator.Entities.Storage;

namespace UpcloudApiKubernetesOperator.Webhooks;

public class V1Alpha1StorageValidator : IValidationWebhook<V1Alpha1Storage>
{
    public AdmissionOperations Operations => AdmissionOperations.Create;
    private const int MAXIMUM_SIZE = 4096;
    private const int MINIMUM_SIZE = 4096;
    private static readonly ReadOnlyCollection<string> ACCEPTED_TIERS = new (new string[] { "hdd", "maxiops" });
    private static readonly ReadOnlyCollection<string> ACCEPTED_BACKUP_RULE_INTERVALS = new (
        new string[] {
            "daily",
            "mon",
            "tue",
            "wed",
            "thu",
            "fri",
            "sat",
            "sun"
        }
    );

    private const int MINIMUM_BACKUP_RULE_RETENTION = 1;
    private const int MAXIMUM__BACKUP_RULE_RETENTION = 1095;

    public ValidationResult Create(V1Alpha1Storage newEntity, bool dryRun)
    {
        if (newEntity.Spec.Size < MINIMUM_SIZE || newEntity.Spec.Size > MAXIMUM_SIZE) {
            return ValidationResult.Fail(StatusCodes.Status400BadRequest, $"Invalid size, accepted values: {MINIMUM_SIZE}-{MAXIMUM_SIZE}");
        }

        if (ACCEPTED_TIERS.Contains(newEntity.Spec.Tier, EqualityComparer<string>.Default)) {
            return ValidationResult.Fail(StatusCodes.Status400BadRequest, $"Invalid tier, accepted values: {string.Join(',', ACCEPTED_TIERS)}");
        }

        if (newEntity.Spec.Title.Length > 255) {
            return ValidationResult.Fail(StatusCodes.Status400BadRequest, "Invalid title, accepted length: 0-255 characters");
        }

        if (string.IsNullOrEmpty(newEntity.Spec.Zone)) {
            return ValidationResult.Fail(StatusCodes.Status400BadRequest, "Zone is required");
        }

        if (newEntity.Spec.BackupRule is not null) {
            if (!ACCEPTED_BACKUP_RULE_INTERVALS.Contains(newEntity.Spec.BackupRule.Interval)) {
                return ValidationResult.Fail(
                    StatusCodes.Status400BadRequest,
                    $"Invalid backup rule interval, accepted values: {string.Join(',', ACCEPTED_BACKUP_RULE_INTERVALS)}"
                );
            }

            if (!int.TryParse(newEntity.Spec.BackupRule.Time, out _)) {
                return ValidationResult.Fail(StatusCodes.Status400BadRequest, $"Invalid backup rule time, accepted values: 0000-2359");
            }

            if (newEntity.Spec.BackupRule.Retention < MINIMUM_BACKUP_RULE_RETENTION || newEntity.Spec.BackupRule.Retention > MAXIMUM__BACKUP_RULE_RETENTION) {
                return ValidationResult.Fail(
                    StatusCodes.Status400BadRequest,
                    $"Invalid backup rule retention, accepted values: {MINIMUM_BACKUP_RULE_RETENTION}-{MAXIMUM__BACKUP_RULE_RETENTION}"
                );
            }
        }

        if (newEntity.Spec.Labels is not null && newEntity.Spec.Labels.Count > 0) {
            foreach (var label in newEntity.Spec.Labels) {
                if (string.IsNullOrEmpty(label.Name)) {
                    return ValidationResult.Fail(StatusCodes.Status400BadRequest, "Empty label name is not valid");
                }
            }
        }

        return ValidationResult.Success();
    }
}
