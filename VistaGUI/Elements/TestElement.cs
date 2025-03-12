using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VistaGUI.Elements
{
    /// <summary>
    /// Embedded Vista Page.
    /// </summary>
    public class TestElement : IVistaElement
    {
        private HtmlNode Node;

        public TestElement(HtmlNode node)
        {
            Node = node;
        }

        public string GenerateHTML()
        {
            string html = string.Empty;

            html = $"""
                <p>{Node.InnerText}</p>
                """;

            return html;
        }
    }
}
