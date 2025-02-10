using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Objects;

namespace SourceRewrite.Components
{
    public class GameComponent
    {
        /// <summary>
        /// Reference to the parent GameObject, This is just a reference, setting it does nothing.
        /// </summary>
        public GameObject GameObject { get; set; }
    }
}
