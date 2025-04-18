using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities
{
    /// <summary>
    /// An Entity that can be interacted with using +use.
    /// </summary>
    public interface IUsable
    {
        void OnUse(BaseEntity activator);
    }
}
