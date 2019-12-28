using Assets.Scripts.ScriptableBuilder.ScriptLinking;
using Assets.Scripts.ScriptableBuilder.SeriesScriptable.CarActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScriptableBuilder.ScriptTriggers
{
    class OnStart : ScriptableEntry, ICarAction
    {
        public override IScriptableEntry Execute(ICarActionable reciever)
        {
            return this.child;
        }

        public override bool GetCompatabilityWithDraggable(DragDropSeries draggable)
        {
            if (draggable is DragDropSeries)
            {
                return true;
            }
            return false;
        }

        public override bool GetCanHaveChildren()
        {
            return true;
        }

        public override bool GetCanHaveParents()
        {
            return false;
        }

        public CarActionTypes GetActionType()
        {
            return CarActionTypes.Start;
        }
    }
}
