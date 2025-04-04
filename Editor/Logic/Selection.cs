using SourceRewrite;
using SourceRewrite.Entities;

namespace Editor.Logic
{
    public static class Selection
    {
        private static BaseEntity _selectedEntity;

        public static BaseEntity SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                if (_selectedEntity != value)
                {
                    // Remove the previous selected gizmos, if any
                    if (_selectedEntity != null)
                    {
                        foreach (var gizmo in _selectedEntity.Children)
                        {
                            if (gizmo.Name == "SelectedGizmo")
                            {
                                gizmo.DestroyDeferred();
                            }
                        }
                    }

                    // Set the new selected entity
                    _selectedEntity = value;

                    if (_selectedEntity != null)
                    {
                        // Create new Gizmos
                        Gizmos.Name = "SelectedGizmo";
                        _selectedEntity.DrawGizmosSelected();
                        Gizmos.Name = "Gizmo";
                    }
                }
            }
        }
    }
}
