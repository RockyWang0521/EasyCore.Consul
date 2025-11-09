using EasyCore.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasyCoreConsul(builder.Configuration);
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
