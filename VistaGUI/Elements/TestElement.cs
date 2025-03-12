using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistaGUI.Elements
{
    /// <summary>
    /// Test Vista Element.
    /// </summary>
    [VistaElement("test-element", typeof(TestElement))]
    public class TestElement : IVistaElement
    {
        private HtmlNode Node;

        public TestElement(HtmlNode node)
        {
            Node = node;
        }

        // Return the HTML for this element
        public string GenerateHTML()
        {
            string html = $"<p>{Node.InnerText}</p>";

            return html;
        }
    }
}
