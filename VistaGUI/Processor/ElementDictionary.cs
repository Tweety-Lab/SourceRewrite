using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VistaGUI.Elements;

namespace VistaGUI.Processor
{
    public static class ElementDictionary
    {
        public static Dictionary<string, Type> Elements = new Dictionary<string, Type>
        {
            { "test-element", typeof(TestElement) },
        };
            
    }
}
