using Serilog;
using Shared.DependencyInjection;
using Shared.ExceptionHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSharedServices();
builder.Services.ConfigureLogger(builder.Configuration);
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);
builder.Services.AddExceptionHandler<GlobalException>();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.Run();
