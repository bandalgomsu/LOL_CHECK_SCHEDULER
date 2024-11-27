using dotenv.net;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using lol_check_scheduler.src.app.devices.repository;
using lol_check_scheduler.src.app.devices.repository.interfaces;
using lol_check_scheduler.src.app.devices.service;
using lol_check_scheduler.src.app.devices.service.interfaces;
using lol_check_scheduler.src.app.scheduler;
using lol_check_scheduler.src.app.subscribers.repository;
using lol_check_scheduler.src.app.subscribers.repository.interfaces;
using lol_check_scheduler.src.app.subscribers.service;
using lol_check_scheduler.src.app.subscribers.service.interfaces;
using lol_check_scheduler.src.app.summoners.repository;
using lol_check_scheduler.src.app.summoners.repository.interfaces;
using lol_check_scheduler.src.app.summoners.service;
using lol_check_scheduler.src.app.summoners.service.interfaces;
using lol_check_scheduler.src.infrastructure.database;
using lol_check_scheduler.src.infrastructure.firebase;
using lol_check_scheduler.src.infrastructure.firebase.interfaces;
using lol_check_scheduler.src.infrastructure.riotclient;
using lol_check_scheduler.src.infrastructure.riotclient.interfaces;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;


DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    // builder.Logging.ClearProviders();
    // builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Warning);
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

builder.Configuration.GetConnectionString("");

var host = Environment.GetEnvironmentVariable("DB_HOST");
var name = Environment.GetEnvironmentVariable("DB_NAME");
var username = Environment.GetEnvironmentVariable("DB_USERNAME");
var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
var port = Environment.GetEnvironmentVariable("DB_PORT");

var connection = $"Server={host};Database={name};User={username};Password={password};Port={port};";

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseMySQL(connection)
);

builder.Configuration["RiotApiKey"] = Environment.GetEnvironmentVariable("RIOT_API_KEY");

// SUMMONER DI
builder.Services.AddSingleton<ISummonerService, SummonerService>();
builder.Services.AddSingleton<ISummonerRepository, SummonerRepository>();

// DEVICE DI
builder.Services.AddSingleton<IDeviceRepository, DeviceRepository>();
builder.Services.AddSingleton<IDeviceService, DeviceService>();

//SUBSCRIBER DI 
builder.Services.AddSingleton<ISubscriberRepository, SubscriberRepository>();
builder.Services.AddSingleton<ISubscriberService, SubscriberService>();

// RIOT_CLIENT_DI
builder.Services.AddSingleton<IRiotClient, RiotClient>();

// FCM_CLIENT_DI
builder.Services.AddSingleton<IFcmClient, FcmClient>();

// SCHEDULER_DI
builder.Services.AddQuartz(q =>
    {
        var jobKey = new JobKey("CHECK_PLAYING_GAME_JOB");
        q.AddJob<CheckPlayingGameScheduler>(opts => opts.WithIdentity(jobKey));
        q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("CHECK_PLAYING_GAME")
            .WithCronSchedule("1 * * * * ?"));
    }
);

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("google-services.json")
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
