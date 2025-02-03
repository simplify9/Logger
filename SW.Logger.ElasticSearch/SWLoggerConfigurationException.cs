using System;

namespace SW.Logger.ElasticSerach
{
    public class SWLoggerConfigurationException: Exception
    {
        public SWLoggerConfigurationException() { }
        public SWLoggerConfigurationException(string message): base(message) {}

        public SWLoggerConfigurationException(string message, Exception InnerException): base(message, InnerException) { }
    }
}