var builder = WebApplication.CreateBuilder(args);


Host.CreateDefaultBuilder(args)
    .UseWindowsService() // 👈 Enables running as a Windows Service
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;

        // Register services
        MappingConfig.RegisterMappings(config);
        services.AddHttpClient();
        services.AddScoped<IFetcher<Candidate>, Fetcher>();
        services.AddScoped<IPoster, Talkpush_BackgroundService.Implementation.Poster>();
        services.AddScoped<ICreatedTicket_InsertData, CreatedTicket_InsertData>();
        services.AddScoped<ICheckedCandidateId, CheckedCandidateId>();
        services.AddSharedServices();
        services.AddSingleton<BranchDictionary>();
        services.AddScoped<ITransformer<Candidate, CreateTicketCandidateRecord>, CandidateTransformer>();

        services.AddDbContext<TalkpushDBContext>(options =>
            options.UseSqlServer(config.GetConnectionString("OMS")),
            ServiceLifetime.Scoped);

        services.AddSingleton(new WorkerConsumerOptions(
            new Dictionary<string, string> {
                { "api_key", config["TalkpushRequest:ApiKey"]! },
                { "filter[others][bi_check]", config["TalkpushRequest:FilterQuery"]! },
                { "include_documents", config["TalkpushRequest:IncludeDocuments"]! },
                { "include_attachments", config["TalkpushRequest:IncludeAttachments"]! }
            },
            config["Urls:urlTalkPushLeads"]!,
            config["Urls:urlCreateTicket"]!,
            config["Urls:urlGenerateToken"]!,
            config["Credentials:Username"]!,
            config["Credentials:Password"]!
        ));

        services.ConfigureLogger(config);
        services.AddHostedService<BackgroundWorker<Candidate, CreateTicketCandidateRecord>>();
    })
    .UseSerilog()
    .Build()
    .Run();
