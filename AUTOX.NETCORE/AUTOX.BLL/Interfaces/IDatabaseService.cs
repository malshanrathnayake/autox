using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTOX.BLL.Interfaces
{
    public interface IDatabaseService
    {
        bool SetConnectionString(string connectionString = "");
        string GetConnectionString();
    }
}
