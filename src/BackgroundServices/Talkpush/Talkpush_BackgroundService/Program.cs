using Talkpush_BackgroundService.Mapping;

var builder = WebApplication.CreateBuilder(args);


// General Variable
var urlGenerateToken = builder.Configuration["Urls:urlGenerateToken"];
var urlCreateTicket =  builder.Configuration["Urls:urlCreateTicket"];
var urlTalkPushLeads = builder.Configuration["Urls:urlTalkPushLeads"];
var username = builder.Configuration["Credentials:Username"];
var password = builder.Configuration["Credentials:Password"];
var apiKey = builder.Configuration["TalkpushRequest:ApiKey"];
var filterQuery = builder.Configuration["TalkpushRequest:FilterQuery"];
var includeDocuments = builder.Configuration["TalkpushRequest:IncludeDocuments"];
var includeAttachments = builder.Configuration["TalkpushRequest:IncludeAttachments"];

// Add services to the container.
MappingConfig.RegisterMappings();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IFetcher<Candidate>, Fetcher>();
builder.Services.AddSharedServices();
builder.Services.ConfigureLogger(builder.Configuration);
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);
builder.Services.AddHostedService<BackgroundWorker<Candidate, CreateTicketCandidateRecord>>();
builder.Services.AddScoped<ITransformer<Candidate, CreateTicketCandidateRecord>, CandidateTransformer>();

// set up Worker Consumer Options
builder.Services.AddSingleton(new WorkerConsumerOptions(
    new Dictionary<string, string> {
        { "api_key", apiKey! },
        { "filter[query]", filterQuery! },
        { "include_documents", includeDocuments! },
        { "include_attachments", includeAttachments! }
    },
     urlTalkPushLeads!,
     urlCreateTicket!,
     urlGenerateToken!,
     username!,
     password!
));

var app = builder.Build();


// Configure the HTTP request pipeline.
app.Run();
