namespace SW.Logger.Console;

public class LoggerOptions
{
    public const string ConfigurationSection = "SwLogger";

    public LoggerOptions()
    {
        Environments = "Development,Staging,Production";
        LoggingLevel = 1;
        ApplicationName = "unknownapp";
        
    }

    public int LoggingLevel { get; set; }
    public string ApplicationName { get; set; }
    public string ApplicationVersion { get; set; }
    public string Environments { get; set; }

}