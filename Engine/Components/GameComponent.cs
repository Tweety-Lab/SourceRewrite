using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Objects;

namespace SourceRewrite.Components
{
    /// <summary>
    /// Base Component Class.
    /// </summary>
    public class GameComponent
    {
        /// <summary>
        /// Reference to the parent GameObject, This is just a reference, setting it does nothing.
        /// </summary>
        public GameObject GameObject { get; set; }

        /// <summary>
        /// Runs once per frame.
        /// </summary>
        public virtual void Update(double deltaTime) { }

        /// <summary>
        /// Runs once on start.
        /// </summary>
        public virtual void Start() { }
    }
}
