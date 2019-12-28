using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScriptTriggers
{
    class OnStartTrigger : DragDropLockable, ICarAction
    {
        public OnStartTrigger()
        {
            this.canHaveParents = false;
        }

        public ILinkedScriptingObject Execute(ICarActionable target)
        {
            return this.GetChild();
        }

        public CarActionTypes GetActionType()
        {
            return CarActionTypes.Start;
        }
    }
}
