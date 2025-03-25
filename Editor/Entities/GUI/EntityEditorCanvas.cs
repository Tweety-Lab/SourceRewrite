using Editor.Logic;
using FileFormats.KeyValues;
using SourceRewrite.Attributes;
using SourceRewrite.Entities;
using SourceRewrite.Entities.GUI;
using System.Reflection;
using VistaGUI.Scripting.References;

namespace Editor.Entities.GUI
{
    internal class EntityEditorCanvas : ScreenspaceGUICanvas
    {
        public override void Start()
        {
            PanelName = "editor/entity_editor.html";
            base.Start();

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
            RegisterEvent("Quit", () => DestroyDeferred());
            RegisterEvent("KeyValueChanged", (args) =>
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

            RegisterEvent("ChangeClass", (args) =>
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
            UnregisterEvent("Quit");
            UnregisterEvent("KeyValueChanged");
            UnregisterEvent("ChangeClass");
            base.OnDestroy();
        }
    }
}
