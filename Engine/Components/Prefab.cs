using SourceRewrite.Files;
using SourceRewrite.Maps;

namespace SourceRewrite.Components
{
    public class Prefab : GameComponent
    {
        [MapProperty("file")]
        public string PrefabPath;

        public override void Start()
        {
            // Editor stores prefabs in .vmf format, we use .bsp
            string truePath = PrefabPath.Replace(".vmf", ".bsp");

            Map prefab = new Map(FileSystem.GetMapPath(truePath));

            prefab.LoadMap();
        }
    }
}
