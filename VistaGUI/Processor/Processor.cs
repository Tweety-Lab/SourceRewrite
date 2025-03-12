using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using VistaGUI.Elements;

namespace VistaGUI.Processor
{
    // Turns Vista-XML into Web-HTML for use in views
    public static class VistaProcessor
    {
        public static string ProcessXML(string XML)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(XML);

            var htmlHead = doc.DocumentNode.SelectSingleNode("//head");
            var htmlBody = doc.DocumentNode.SelectSingleNode("//body");

            // Process body to replace Vista elements with generated HTML
            string webHTML = ProcessXMLNode(htmlHead) + "\n" + ProcessXMLNode(htmlBody);

            Console.WriteLine(webHTML);

            return webHTML;
        }

        // Process a single node and replace Vista elements with their HTML output
        private static string ProcessXMLNode(HtmlNode node)
        {
            string output = string.Empty;

            if (node.HasChildNodes)
            {
                foreach (var child in node.ChildNodes)
                {
                    output += ProcessXMLElement(child);
                }
            }

            return output;
        }

        // Convert Vista-XML Element to Web-HTML
        private static string ProcessXMLElement(HtmlNode node)
        {
            string output = "<p>Invalid Vista Element</p>";

            Type elementType = ElementDictionary.Elements.TryGetValue(node.Name, out elementType) ? elementType : null;

            if (elementType != null)
            {
                // Create the Vista element instance and generate its HTML
                var element = Activator.CreateInstance(elementType, node) as IVistaElement;
                if (element != null)
                {
                    output = element.GenerateHTML();
                }
            } else
            {
                // If the element is not a Vista element, keep it as it is
                output = node.OuterHtml;
            }

            return output;
        }
    }
}
