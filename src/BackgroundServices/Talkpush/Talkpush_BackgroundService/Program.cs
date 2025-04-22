var builder = WebApplication.CreateBuilder(args);


// General Variable
var urlGenerateToken = "https://uat-api.cibi.com.ph/OMSWebAPI/token/generatetoken";
var urlCreateTicket = "https://uat-api.cibi.com.ph/OMSWebAPI/oms/createticket";
var urlTalkPushLeads = "https://concentrix-ph.talkpush.com/api/talkpush_services/campaign_invitations";
var username = builder.Configuration["Credentials:Username"];
var password = builder.Configuration["Credentials:Password"];

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddScoped<IFetcher<Candidate>, Fetcher>();
builder.Services.AddSharedServices();
builder.Services.ConfigureLogger(builder.Configuration);
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);
builder.Services.AddHostedService<BackgroundWorker<Candidate>>();


// set up WorkerConsumerOptions
builder.Services.AddSingleton(new WorkerConsumerOptions(
    new Dictionary<string, string> {
        { "api_key", "fe5b9baf4a79c19b9f1de4a62b2de990" },
        { "filter[query]", "AP11845036" },
        { "include_documents", "True" },
        { "include_attachments", "True" }
    },
     urlTalkPushLeads,
     urlCreateTicket,
     urlGenerateToken
));

var app = builder.Build();


// Configure the HTTP request pipeline.

app.Run();
