using Talkpush_BackgroundService.Data.DataAbstraction;
using Talkpush_BackgroundService.Data.DataImplementation;

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
builder.Services.AddScoped<IPoster, Talkpush_BackgroundService.Implementation.Poster>();
builder.Services.AddScoped<ICreatedTicket_InsertData, CreatedTicket_InsertData>();
builder.Services.AddScoped<ICheckedCandidateId, CheckedCandidateId>();
builder.Services.AddSharedServices();
builder.Services.ConfigureLogger(builder.Configuration);
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);
builder.Services.AddHostedService<BackgroundWorker<Candidate, CreateTicketCandidateRecord>>();
builder.Services.AddSingleton<BranchDictionary>();
builder.Services.AddScoped<ITransformer<Candidate, CreateTicketCandidateRecord>, CandidateTransformer>();

// register DBCONTEXT
builder.Services.AddDbContext<TalkpushDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("OMS"));
});


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
