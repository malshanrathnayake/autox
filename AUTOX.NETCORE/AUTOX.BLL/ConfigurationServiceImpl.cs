using AUTOX.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AUTOX.BLL
{
    public class ConfigurationServiceImpl : IConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ConfigurationServiceImpl(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetGoogleClientId()
        {
            return _configuration["Google:ClientId"] ?? string.Empty;
        }

        public string GetGoogleClientSecret()
        {
            return _configuration["Google:ClientSecret"] ?? string.Empty;
        }

        public string GetAppBaseUrl()
        {
            return _configuration["AppSettings:BaseUrl"] ?? string.Empty;
        }
    }
}
