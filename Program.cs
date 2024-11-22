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
using lol_check_scheduler.src.infrastructure.riotclient;
using Microsoft.EntityFrameworkCore;


var env = DotEnv.Read();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

builder.Configuration.GetConnectionString("");

// DB CONNECTION
var host = env["DB_HOST"];
var name = env["DB_NAME"];
var username = env["DB_USERNAME"];
var password = env["DB_PASSWORD"];

var connection = $"Server={host};Database={name};User={username};Password={password}";

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseMySQL(connection)
);

builder.Configuration["RiotApiKey"] = env["RIOT_API_KEY"];

// SUMMONER DI
builder.Services.AddScoped<ISummonerService, SummonerService>();
builder.Services.AddScoped<ISummonerRepository, SummonerRepository>();

// DEVICE DI
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IDeviceService, DeviceService>();

//SUBSCRIBER DI 
builder.Services.AddScoped<ISubscriberRepository, SubscriberRepository>();
builder.Services.AddScoped<ISubscriberService, SubscriberService>();

// RIOT_CLIENT_DI
builder.Services.AddSingleton<RiotClient>();

// FCM_CLIENT_DI
builder.Services.AddSingleton<FcmClient>();

// SCHEDULER_DI
builder.Services.AddHostedService<CheckPlayingGameScheduler>();

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
