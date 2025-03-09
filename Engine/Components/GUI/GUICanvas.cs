using SourceRewrite.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Components
{
    /// <summary>
    /// GUI Renderer.
    /// </summary>
    public class GUICanvas : GameComponent
    {
        [MapProperty("height")]
        public int height; // Panel Height

        [MapProperty("width")]
        public int width; // Panel Width

        public override void Start()
        {
            Console.WriteLine(height);
            Console.WriteLine(width);
        }
    }
}
