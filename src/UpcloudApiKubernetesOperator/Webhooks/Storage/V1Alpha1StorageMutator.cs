using KubeOps.Operator.Webhooks;
using UpcloudApiKubernetesOperator.Entities.Storage;

namespace UpcloudApiKubernetesOperator.Webhooks.Storage;

public class V1Alpha1StorageMutator : IMutationWebhook<V1Alpha1Storage>
{
    public AdmissionOperations Operations => AdmissionOperations.Create;
    public MutationResult Create(V1Alpha1Storage newEntity, bool dryRun) => MutationResult.Modified(newEntity);
}
