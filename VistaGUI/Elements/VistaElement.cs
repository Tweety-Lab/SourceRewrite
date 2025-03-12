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
}
