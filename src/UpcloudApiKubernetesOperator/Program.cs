using KubeOps.Operator;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddKubernetesOperator();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.UseKubernetesOperator();

await app.RunOperatorAsync(args);
