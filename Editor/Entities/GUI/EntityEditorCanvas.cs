using Editor.Logic;
using SourceRewrite.Attributes;
using SourceRewrite.Entities;
using SourceRewrite.Entities.GUI;
using SourceRewrite.Files;
using SourceRewrite.InputSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VistaGUI.Scripting.References;

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

            // Register GUI Events
            RegisterGUIEvents();

            if (Selection.SelectedEntity != null)
            {
                // Get Selected Entities classname
                string className = Selection.SelectedEntity.GetType().ToString();

                // Split the string
                string[] parts = className.Split('.');

                // Check if there are parts and remove the first part
                if (parts.Length > 1)
                {
                    // Join the remaining parts back into a single string
                    className = string.Join(".", parts, 1, parts.Length - 1);
                }

                // Set Classname
                VistaElement classNameElement = Canvas.GetElement("class-name");
                classNameElement.SetProperty("value", className);
            }

            PopulateProperties();
        }

        // Register GUI Events
        private void RegisterGUIEvents()
        {
            Canvas.RegisterEvent("Quit", () => DestroyDeferred());
        }

        // Populates table with entity properties
        private void PopulateProperties()
        {
            VistaElement propertiesTable = Canvas.GetElement("keyvalues-table");

            var fieldsWithAttribute = Selection.SelectedEntity.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);


            foreach (var field in fieldsWithAttribute)
            {
                // Check if the field has the EntityPropertyAttribute
                if (Attribute.IsDefined(field, typeof(EntityPropertyAttribute)))
                {
                    // Get the value of the field from the selected entity
                    var value = field.GetValue(Selection.SelectedEntity);

                    // Set propertiesTable innerHTML to have new row with name and value
                    if (value != null)
                    {
                        propertiesTable.InnerHTML = propertiesTable.InnerHTML + $"<tr><td>{field.Name}</td><td>{value}</td></tr>";
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            Canvas.UnregisterEvent("Quit");
        }
    }
}
