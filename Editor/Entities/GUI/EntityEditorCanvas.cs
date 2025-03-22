using Editor.Logic;
using FileFormats.KeyValues;
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
            Canvas.RegisterEvent("KeyValueChanged", (args) =>
            {
                // Extract field name, value, and type
                string fieldName = args[0];
                string inputValue = args[1];

                // Remove all backspace characters from the input value
                inputValue = inputValue.Replace("\b", "");

                // Try to get the field from the selected entity
                var field = Selection.SelectedEntity.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    object convertedValue = KeyValuesUtility.ConvertValueToType(inputValue);

                    // Check if the field type matches the converted value's type
                    if (field.FieldType.IsAssignableFrom(convertedValue.GetType()))
                    {
                        // Types match, set the value
                        field.SetValue(Selection.SelectedEntity, convertedValue);
                    }
                }
            });

            Canvas.RegisterEvent("ChangeClass", (args) =>
            {
                // Get the inputted class name
                VistaElement classNameElement = Canvas.GetElement("class-name");
                string className = classNameElement.GetProperty("value");

                Type entityType = Type.GetType(className);
                if (entityType == null)
                    return;

                // Delete currently selected entity
                Selection.SelectedEntity.DestroyDeferred();
                Selection.SelectedEntity = null;

                // Try to get entity of className
                BaseEntity newEntity = (BaseEntity)Activator.CreateInstance(Type.GetType(className));

                // Replace with newEntity
                EntityManager.AddMapEntity(newEntity);
                Selection.SelectedEntity = newEntity;
            });
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
                        propertiesTable.InnerHTML = propertiesTable.InnerHTML + $"<tr><td>{field.Name}</td><td><input type=\"text\" value=\"{value}\" onkeyup=\"KeyValueChanged('{field.Name}', this.value)\" onkeydown=\"checkEnter(event, this)\"></td></tr>";
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            Canvas.UnregisterEvent("Quit");
            Canvas.UnregisterEvent("KeyValueChanged");
            Canvas.UnregisterEvent("ChangeClass");
        }
    }
}
