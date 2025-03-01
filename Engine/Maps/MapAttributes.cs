using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Maps
{
    /// <summary>
    /// Map Properties are variables that are defined in the BSP (map) file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MapProperty : Attribute
    {
    }
}
