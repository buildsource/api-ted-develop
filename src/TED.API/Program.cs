using BancoMaster.LogManager.Extensions;
using BancoMaster.LogManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog.Events;
using System.Reflection;
using System.Text.Json;
using TED.API.Configuration;
using TED.API.Context;
using TED.API.Filters;
using TED.API.Helpers;
using TED.API.Interceptors;
using TED.API.Interfaces;
using TED.API.Mappers;
using TED.API.Repositories;
using TED.API.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configurao do arquivo de configurao
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);


        if (!File.Exists("appsettings.Local.json"))
        {
            var secrets = await SecretsManagerHelper.ConnectionString("API-GTW-TED");

            var appConfiguration = JsonSerializer.Deserialize<AppConfiguration>(JsonDocument.Parse(secrets));

            builder.Services.Configure<SinacorConfiguration>(config =>
            {
                config.BaseUrl = appConfiguration.BaseUrl;
                config.ClienteSecret = appConfiguration.ClienteSecret;
                config.ClienteId = appConfiguration.ClienteId;
                config.HorarioInicio = DateTime.Parse(appConfiguration.HorarioInicio);
                config.HorarioFim = DateTime.Parse(appConfiguration.HorarioFim);
                config.IsLocal = builder.Configuration.GetValue<bool>("SinacorConfiguration:IsLocal");
            });

            builder.Services.AddDbContext<AppDBContext>(options =>
                options.UseSqlServer(appConfiguration.DefaultConnection));
        }
        else
        {
            builder.Services.Configure<SinacorConfiguration>(builder.Configuration.GetSection("SinacorConfiguration"));

            builder.Services.AddDbContext<AppDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        }

        // Configuraes adicionais
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        // Configurao dos Controllers e Filtros
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
        });

        // Configurao do Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = $"TED API - IsLocal: {builder.Configuration.GetValue<bool>("SinacorConfiguration:IsLocal")}", Version = "v1" });

            //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //{
            //    In = ParameterLocation.Header,
            //    Description = "JWT Authorization header using the Bearer scheme.",
            //    Name = "Authorization",
            //    Type = SecuritySchemeType.ApiKey
            //});

            c.DocumentFilter<IncludeInSwaggerDocumentFilter>();

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        // Configurao do AutoMapper
        builder.Services.AddAutoMapper(typeof(ProfileMapper));

        // Configurao dos Interceptadores
        builder.Services.AddSingleton<ErrorLoggingInterceptor>();
        builder.Services.AddSingleton<SuccessLoggingInterceptor>();

        // Configurao dos Servios e Repositrios
        builder.Services.AddScoped<IClienteTedService, ClienteTedService>();
        builder.Services.AddScoped<IAdminTedService, AdminTedService>();
        builder.Services.AddScoped<ISinacorTedService, SinacorTedService>();

        builder.Services.AddScoped<IClienteTedRepository, ClienteTedRepository>();
        builder.Services.AddScoped<IAdminTedRepository, AdminTedRepository>();

        // Configurao do HttpClient
        builder.Services.AddHttpClient();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
            });
        });


        //Configurao da Telemetria
        var options = new ConfigureOpenTelemetryOptions(builder.Services);
        options.Fill("api-gtw-ted",
        SenderType.System,
        "apiGTWTED",
        LogEventLevel.Information,
        "release/v1.0.0");
        builder.Services.ConfigureObservability(options);


        // Construo do aplicativo
        var app = builder.Build();

        app.UseCors();
        app.ConfigureCorrelationID();

        // Configurao do ambiente de desenvolvimento
        if (app.Environment.IsDevelopment())
        {
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TED API");
                //c.InjectStylesheet("/swagger-dark.css");
            });
        }

        // Middleware
        app.UseAuthorization();
        app.MapControllers();
        app.MapGet("/health", () => "OK");


        // Execuo do aplicativo
        app.Run();

    }
}
