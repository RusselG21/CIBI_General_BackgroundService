
var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

// Register services

builder.Services.AddHttpClient();

// Add services to the container. // Add the carter service
builder.Services.AddCarter(configurator: c =>
{
    // Specify the assembly containing your modules
    var modulesAssembly = assembly;
    var modules = modulesAssembly.GetTypes()
        .Where(t => typeof(ICarterModule).IsAssignableFrom(t) && !t.IsAbstract)
        .ToArray();
    c.WithModules(modules);
});

// Add MediatR
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

// Add Exception Handler
builder.Services.AddExceptionHandler<CustomExceptionHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.

// Configure the HTTP request pipeline.
app.MapCarter(); // Scan all the ICarterModule in the project and map the necessary route

// use exception handler after register
app.UseExceptionHandler(options => { });

app.Run();
