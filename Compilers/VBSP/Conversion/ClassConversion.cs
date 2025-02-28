using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VBSP.Conversion
{
    public static class ClassConversion
    {
        // Create dictionary to store Valve to Our types
        public static Dictionary<string, string> ClassMap = new Dictionary<string, string>()
        {
            // Valve Class Name, Our Class Name
            { "light", "SourceRewrite_Components_PointLight" }
        };
    }
}
