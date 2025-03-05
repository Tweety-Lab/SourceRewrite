using SourceRewrite.Files;
using SourceRewrite.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Components
{
    public class Prefab : GameComponent
    {
        [MapProperty("file")]
        public string PrefabPath = string.Empty;

        public override void Start()
        {
            // Editor stores prefabs in .vmf format, we use .bsp
            string truePath = PrefabPath.Replace(".vmf", ".bsp");

            Map prefab = new Map(FileSystem.GetMapPath(truePath));

            prefab.WorldTransform = GameObject.Transform;

            prefab.LoadMap();
        }
    }
}
