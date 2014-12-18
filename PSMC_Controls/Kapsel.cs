using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSMC
{
    public static class Kapsel
    {
        public static void setHlmvPath(string path)
        {
            PSMC.Properties.Settings.Default.hlmv_path = path;
            PSMC.Properties.Settings.Default.Save();
        }
    }
}
