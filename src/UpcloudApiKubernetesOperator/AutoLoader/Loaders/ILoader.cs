namespace UpcloudApiKubernetesOperator.AutoLoader.Loaders;

internal interface ILoader
{
    abstract Task Run(CancellationToken cancellationToken);
}
