using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TED.API.Configuration;
using TED.API.Context;
using TED.API.Entities;

namespace TED.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);
            configBuilder.AddJsonFile("appsettings.Local.json", optional: false, reloadOnChange: true);
        });

        builder.ConfigureServices(services =>
        {
            builder.UseEnvironment("Test");

            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDBContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDBContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            services.Configure<SinacorConfiguration>(configuration.GetSection(nameof(SinacorConfiguration)));

            // Adicionando dados iniciais à tabela LimiteTed
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDBContext>();
                db.Database.EnsureCreated();

                db.LimiteTed.Add(new LimiteTed { 
                    ValorMaximoPorSaque = 5000,
                    ValorMaximoDia = 10000,
                    QuantidadeMaximaDia = 5,
                });

                db.SaveChanges();
            }
        });
    }
}
