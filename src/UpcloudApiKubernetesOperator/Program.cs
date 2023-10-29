using KubeOps.Operator;
using UpcloudApiKubernetesOperator.Entities;
using UpcloudApiKubernetesOperator.UpCloudApi.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureServices((context, services) => {
    services.AddUpCloudApiClient(context.Configuration);
    services.AddKubernetesOperator();
    services.AddControllers();

});

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.UseKubernetesOperator();

await app.RunOperatorAsync(args);
