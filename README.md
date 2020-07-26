[![Build Status](https://dev.azure.com/simplify9/Github%20Pipelines/_apis/build/status/simplify9.Logger?branchName=master)](https://dev.azure.com/simplify9/Github%20Pipelines/_build/latest?definitionId=168&branchName=master) 

![Azure DevOps tests](https://img.shields.io/azure-devops/tests/Simplify9/Github%20Pipelines/168?style=for-the-badge)


| **Package**       | **Version** |
| :----------------:|:----------------------:|
|```SimplyWorks.Logger```| ![Nuget](https://img.shields.io/nuget/v/SimplyWorks.Logger?style=for-the-badge)


## Introduction 
*Logger* is a library extension method that documents any events or incidents onto an [Elasticsearch](https://www.elastic.co/blog/found-elasticsearch-as-nosql) database. With *Logger*, the backend logs every event that occurs, and saves it. 


```csharp
public static class IHostBuilderExtensions
    {
        public static IHostBuilder UseSwLogger(this IHostBuilder builder, Action<LoggerOptions> configure = null)
        {
            var loggerOptions = new LoggerOptions
            {
                ApplicationVersion = Assembly.GetCallingAssembly().GetName().Version.ToString()
            };

            if (configure != null) configure.Invoke(loggerOptions);

            return builder.UseSerilog((hostBuilderContext, loggerConfiguration) => ConfigureSerilog(hostBuilderContext, loggerConfiguration, loggerOptions));
        }
```
## Getting Started
*Logger* is available as a package on [NuGet](https://www.nuget.org/packages/SimplyWorks.Logger/). 

To use *Logger*, you will require the [`SeriLog`](https://serilog.net) library. 

## Getting support 👷
If you encounter any bugs, don't hesitate to submit an [issue](https://github.com/simplify9/Logger/issues). We'll get back to you promptly!

