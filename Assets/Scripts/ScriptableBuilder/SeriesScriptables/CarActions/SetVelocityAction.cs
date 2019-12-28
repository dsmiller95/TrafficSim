using Assets.Scripts.ScriptableBuilder.ScriptLinking;
using UnityEngine;

namespace Assets.Scripts.ScriptableBuilder.SeriesScriptable.CarActions
{
    public class SetVelocityAction : ScriptableEntry, ICarFloatAction
    {
        public override IScriptableEntry Execute(ICarActionable reciever)
        {
            //TODO: grab velocity from linked sensor object
            reciever.SetForwardVelocity(1);
            return this.child;
        }

        public CarActionTypes GetActionType()
        {
            return CarActionTypes.SetVelocity;
        }

        public override bool GetCanHaveChildren()
        {
            return true;
        }

        public override bool GetCanHaveParents()
        {
            return true;
        }

        public override bool GetCompatabilityWithDraggable(DragDropSeries draggable)
        {
            return draggable is DragDropSeries;
        }
    }
}