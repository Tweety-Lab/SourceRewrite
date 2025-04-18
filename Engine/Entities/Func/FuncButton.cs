using SourceRewrite.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRewrite.Entities.Func
{
    [Entity("func_button")]
    public class FuncButton : BrushEntity, IUsable
    {
        public void OnUse(BaseEntity activator)
        {
            FireOutput("OnPressed");
        }
    }
}
