using Assets.Scripts.ScriptableBuilder.ScriptLinking;
using Assets.Scripts.ScriptableBuilder.SeriesScriptable.CarActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScriptableBuilder.ScriptTriggers
{
    class EndStack : ScriptableEntry, ICarAction
    {
        public override IScriptableEntry Execute(ICarActionable reciever)
        {
            return null;
        }

        public override bool GetCompatabilityWithDraggable(DragDropBase draggable)
        {
            if (draggable is DragDropSeries)
            {
                return true;
            }
            return false;
        }

        public override bool GetCanHaveChildren()
        {
            return false;
        }

        public override bool GetCanHaveParents()
        {
            return true;
        }

        public CarActionTypes GetActionType()
        {
            return CarActionTypes.End;
        }
    }
}
