using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace SW.Logger.Console;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSWConsoleLogger(this IServiceCollection services,
        Action<LoggerOptions> configure = null)
    {
        var loggerOptions = new LoggerOptions
        {
            ApplicationVersion = Assembly.GetCallingAssembly().GetName().Version.ToString()
        };


        if (configure != null) configure.Invoke(loggerOptions);
        services.AddSingleton(loggerOptions);

        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        configuration.GetSection(LoggerOptions.ConfigurationSection).Bind(loggerOptions);

        var hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();

        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is((LogEventLevel)loggerOptions.LoggingLevel)
            //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", hostEnvironment.EnvironmentName)
            .Enrich.WithProperty("ApplicationVersion", loggerOptions.ApplicationVersion)
            .Enrich.WithProperty("Application", loggerOptions.ApplicationName);
        
        loggerConfiguration = Debugger.IsAttached
            ? loggerConfiguration.WriteTo.Console()
            : loggerConfiguration.WriteTo.Console(new CompactJsonFormatter());

        Log.Information("Serilog started from SwLogger.");

        services.AddSerilog(loggerConfiguration.CreateLogger());

        return services;
    }
}