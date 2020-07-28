[![Build Status](https://dev.azure.com/simplify9/Github%20Pipelines/_apis/build/status/simplify9.Logger?branchName=master)](https://dev.azure.com/simplify9/Github%20Pipelines/_build/latest?definitionId=168&branchName=master) 

![Azure DevOps tests](https://img.shields.io/azure-devops/tests/Simplify9/Github%20Pipelines/168?style=for-the-badge)


| **Package**       | **Version** |
| :----------------:|:----------------------:|
|[```SimplyWorks.Logger```](https://www.nuget.org/packages/SimplyWorks.Logger/)| ![Nuget](https://img.shields.io/nuget/v/SimplyWorks.Logger?style=for-the-badge)


## Introduction 
*Logger* is a library extension method that documents any events or incidents onto an [Elasticsearch](https://www.elastic.co/blog/found-elasticsearch-as-nosql) database. With *Logger*, the backend logs every event that occurs, and saves it. 


## Getting Started
*Logger* is available as a package on [NuGet](https://www.nuget.org/packages/SimplyWorks.Logger/). 

To use *Logger*, you will require the [`SeriLog`](https://serilog.net) library. 

## Setting Up *Logger*
```csharp
 public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDfaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSwLogger();
    }
        
```
## Getting support 👷
If you encounter any bugs, don't hesitate to submit an [issue](https://github.com/simplify9/Logger/issues). We'll get back to you promptly!

