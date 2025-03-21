using SourceRewrite.Entities;
using SourceRewrite.Entities.GUI;
using SourceRewrite.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Entities.GUI
{
    internal class EntityEditorCanvas : BaseEntity
    {
        public GUICanvasEntity Canvas;

        public override void Start()
        {
            Canvas = new GUICanvasEntity();
            Canvas.IsTransparent = true;
            Canvas.PanelName = "editor/entity_editor.html";
            Canvas.Parent = this;

            // HACK: Manually start the canvas component
            Canvas.Start();
        }

        public override void OnDestroy()
        {
        }
    }
}
