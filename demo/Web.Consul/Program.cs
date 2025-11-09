using EasyCore.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.AddEasyCoreConsul()
    .AddEasyCoreConsulCache()
    .AddEasyCoreConsulLocking()
    .AddEasyCoreConsulServer();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();

app.UseEasyCoreConsul();
app.MapControllers();
app.Run();
