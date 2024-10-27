using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public sealed class Helper
    {
        public static string GetGPFAAssetsConnectionString()
        {
            return DAL.Properties.Settings.Default.GPFAAssetsConnectionString;
        }
    }
}
