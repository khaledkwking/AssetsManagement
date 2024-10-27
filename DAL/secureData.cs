using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public static class secureData
    {
        public static string ConString = Properties.Settings.Default.GPFAAssetsConnectionString;
        public static string HRconString = Properties.Settings.Default.GPFAHRConnectionString;
        //@"Data Source=./;Initial Catalog=GPFAssetsV1;Persist Security Info=True;UserID=DBUSER;Password=geAn9s#>>A";
    }
}
