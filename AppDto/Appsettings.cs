using System;

namespace AppDto
{
    public class ConfigData
    {
        public Appsettings Appsettings { get; set; }
    }
    public class Appsettings
    {
        public string AWSUniqueDbKey { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecrteKey { get; set; }
    }

}
