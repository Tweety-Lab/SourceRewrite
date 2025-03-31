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
                _selectedEntity = value;
                if (_selectedEntity != null)
                {
                    _selectedEntity.DrawGizmosSelected();
                }
            }
        }
    }
}
