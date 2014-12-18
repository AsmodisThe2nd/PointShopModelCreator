using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSMC
{
    public static class Modelhandling
    {
        public static string getModelNameFromPath(string path)
        {
            string modelname = path.Substring(path.LastIndexOf('\\')+1);
            modelname = modelname.Replace(".mdl", "");
            return modelname;
        }
    }
}
