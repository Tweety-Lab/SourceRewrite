using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceRewrite.Maths;

namespace SourceRewrite.Objects
{
    public class GameObject
    {
        // Every object that exists
        public static List<GameObject> ActiveObjects { get; private set; } = new List<GameObject>();

        // Every Object needs a Transform
        public Transform Transform { get; set; } = new Transform();

        // Add Object to list of Objects for later rendering (placeholder)
        public GameObject()
        {
            ActiveObjects.Add(this);
        }
    }
}
