﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SW.Logger.ElasticSerach
{
    public class LoggerOptions
    {
        public const string ConfigurationSection = "SwLogger";

        public LoggerOptions()
        {
            ElasticsearchEnvironments = "Development,Staging,Production";
            LoggingLevel = 3;
            ApplicationName = "unknownapp";
            ElasticsearchDeleteIndexAfterDays = 90;
        }

        public int LoggingLevel { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public string ElasticsearchUrl { get; set; }
        public string ElasticsearchUser { get; set; }
        public string ElasticsearchPassword { get; set; }
        public string ElasticsearchEnvironments { get; set; }
        
        public string ElasticsearchCertificatePath { get; set; }
        public int ElasticsearchDeleteIndexAfterDays { get; set; }

    }
}
