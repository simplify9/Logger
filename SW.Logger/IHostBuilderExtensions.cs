using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Elasticsearch.Net;
using Nest;

namespace SW.Logger
{
    public static class IHostBuilderExtensions
    {
        private const string IndexLifecycleName = "index.lifecycle.name";

        public static IHostBuilder UseSwLogger(this IHostBuilder builder, Action<LoggerOptions> configure = null)
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

            if (!string.IsNullOrEmpty(loggerOptions.ApplicationName))
            {
                var (isValid, validationError) = loggerOptions.ApplicationName.IsValidIndexName();
                if (!isValid)
                    throw new SWLoggerConfigurationException(validationError);
            }

            loggerConfiguration = loggerConfiguration
                .MinimumLevel.Is((LogEventLevel)loggerOptions.LoggingLevel)
                //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", hostBuilderContext.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("ApplicationVersion", loggerOptions.ApplicationVersion)
                .WriteTo.Console();

            if (hostBuilderContext.HostingEnvironment.IsDevelopment())
                loggerConfiguration = loggerConfiguration
                    .WriteTo.Debug();

            if (loggerOptions.ElasticsearchUrl != null && loggerOptions.ElasticsearchEnvironments != null &&
                loggerOptions.ElasticsearchEnvironments.Split(',').Select(env => env.Trim()).Contains(
                    hostBuilderContext.HostingEnvironment.EnvironmentName, StringComparer.OrdinalIgnoreCase))
            {
                CreateLifeCyclePolicy(loggerOptions);
                loggerConfiguration = loggerConfiguration
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(loggerOptions.ElasticsearchUrl))
                    {
                        ModifyConnectionSettings = connectionConfig =>
                            string.IsNullOrWhiteSpace(loggerOptions.ElasticsearchCertificatePath)
                                ? connectionConfig.BasicAuthentication(loggerOptions.ElasticsearchUser,
                                    loggerOptions.ElasticsearchPassword)
                                : connectionConfig.BasicAuthentication(loggerOptions.ElasticsearchUser,
                                        loggerOptions.ElasticsearchPassword)
                                    .ServerCertificateValidationCallback(
                                        CertificateValidations.AuthorityIsRoot(
                                            new X509Certificate(loggerOptions.ElasticsearchCertificatePath)
                                        )
                                    ),
                        TemplateCustomSettings = new Dictionary<string, string>
                        {
                            {
                                IndexLifecycleName, loggerOptions.GetPolicyName()
                            }
                        },
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                        RegisterTemplateFailure = RegisterTemplateRecovery.IndexAnyway,
                        TemplateName = $"serilog-{loggerOptions.ApplicationName.ToLower()}",
                        OverwriteTemplate = true,
                        CustomFormatter = new ElasticsearchJsonFormatter(),
                        IndexFormat = $"{loggerOptions.ApplicationName.ToLower()}-{{0:yyyy.MM}}",
                        NumberOfReplicas = 0,
                        NumberOfShards = 1,
                        EmitEventFailure = EmitEventFailureHandling.RaiseCallback,
                        FailureCallback = logEvent =>
                        {
                            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
                            Log.Error("Unable to submit event to Elasticsearch.");
                        },
                        TypeName = null
                    });
            }

            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Log.Information("Serilog started from SwLogger.");

            return loggerConfiguration; //.ReadFrom.Configuration(hostBuilderContext.Configuration);
        }

        private static void CreateLifeCyclePolicy(LoggerOptions loggerOptions)
        {
            var uri = new Uri(loggerOptions.ElasticsearchUrl);
            var settings = new ConnectionSettings(uri);
            settings.BasicAuthentication(loggerOptions.ElasticsearchUser, loggerOptions.ElasticsearchPassword);
            var client = new ElasticClient(settings);

            // create lifecycle
            client.IndexLifecycleManagement.PutLifecycle(loggerOptions.GetPolicyName(), p =>
                p.Policy(po => po.Phases(a => a.Delete(w => w
                        .MinimumAge($"{loggerOptions.ElasticsearchDeleteIndexAfterDays}d")
                        .Actions(ac => ac
                            .Delete(f => f)
                        )
                    )
                )));

            // apply lifecycle on existing if any
            client.Indices.UpdateSettings(
                new UpdateIndexSettingsRequest($"{loggerOptions.ApplicationName.ToLower()}-*")
                {
                    IndexSettings = new IndexSettings
                    {
                        { IndexLifecycleName, loggerOptions.GetPolicyName() }
                    }
                });
        }

        private static string GetPolicyName(this LoggerOptions loggerOptions) =>
            $"{loggerOptions.ApplicationName.ToLower()}-policy";
    }
}