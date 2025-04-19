using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Rendering.OpenGL
{
    /// <summary>
    /// Least Recently Used (LRU) cache for texture unit management
    /// </summary>
    public class LRUTextureUnitCache
    {
        private readonly LinkedList<int> _lruList = new LinkedList<int>();
        private readonly Dictionary<int, LinkedListNode<int>> _unitMap = new Dictionary<int, LinkedListNode<int>>();
        private readonly int _capacity;

        public LRUTextureUnitCache(int capacity)
        {
            _capacity = capacity;

            // Initialize with all available texture units
            for (int i = 0; i < capacity; i++)
            {
                var node = _lruList.AddLast(i);
                _unitMap[i] = node;
            }
        }

        public int GetTextureUnit()
        {
            // Get the least recently used unit
            int unit = _lruList.First.Value;
            Touch(unit);
            return unit;
        }

        public void Touch(int unit)
        {
            // Move this unit to the end of the list (most recently used)
            if (_unitMap.TryGetValue(unit, out var node))
            {
                _lruList.Remove(node);
                _lruList.AddLast(node);
            }
            else
            {
                // This shouldn't happen if the cache is properly maintained
                var newNode = _lruList.AddLast(unit);
                _unitMap[unit] = newNode;
            }
        }
    }
}
