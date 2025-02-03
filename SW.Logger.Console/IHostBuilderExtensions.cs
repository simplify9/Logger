using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace SW.Logger.Console;

public static class IHostBuilderExtensions
{

    public static IHostBuilder UseSwConsoleLogger(this IHostBuilder builder,
        Action<LoggerOptions> configure = null)
    {
        var loggerOptions = new LoggerOptions
        {
            ApplicationVersion = Assembly.GetCallingAssembly().GetName().Version.ToString()
        };

        if (configure != null) configure.Invoke(loggerOptions);
        
        

        return builder.UseSerilog((hostBuilderContext, loggerConfiguration) =>
            ConfigureSerilog(hostBuilderContext, loggerConfiguration, loggerOptions));
    }

    private static LoggerConfiguration ConfigureSerilog(HostBuilderContext hostBuilderContext,
        LoggerConfiguration loggerConfiguration, LoggerOptions loggerOptions)
    {
        hostBuilderContext.Configuration.GetSection(LoggerOptions.ConfigurationSection).Bind(loggerOptions);
        
        loggerConfiguration = loggerConfiguration
            .MinimumLevel.Is((LogEventLevel)loggerOptions.LoggingLevel)
            //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", hostBuilderContext.HostingEnvironment.EnvironmentName)
            .Enrich.WithProperty("ApplicationVersion", loggerOptions.ApplicationVersion)
            .Enrich.WithProperty("Application", loggerOptions.ApplicationName)
            .WriteTo.Console();

        if (hostBuilderContext.HostingEnvironment.IsDevelopment())
            loggerConfiguration = loggerConfiguration
                .WriteTo.Debug();


        loggerConfiguration = loggerConfiguration
            .WriteTo.Console(new CompactJsonFormatter());


        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        Log.Information("Serilog started from SwLogger.");

        return loggerConfiguration; //.ReadFrom.Configuration(hostBuilderContext.Configuration);
    }
    
}