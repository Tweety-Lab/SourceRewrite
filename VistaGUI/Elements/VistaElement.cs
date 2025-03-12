using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistaGUI.Elements
{
    // Base GUI Element interface
    public interface IVistaElement
    {
        string GenerateHTML();
    }

    // Base GUI Element attribute
    [AttributeUsage(AttributeTargets.Class)]
    public class VistaElementAttribute : Attribute
    {
        public string Name { get; set; }
        public Type ElementType { get; set; }

        public VistaElementAttribute(string name, Type elementType)
        {
            Name = name;
            ElementType = elementType;
        }
    }
}
