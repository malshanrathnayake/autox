using System;
using System.Collections.Generic;
using System.Text;

namespace AUTOX.BLL.Interfaces
{
    public interface IConfigurationService
    {
        string GetGoogleClientId();
        string GetGoogleClientSecret();
        string GetAppBaseUrl();
    }
}
