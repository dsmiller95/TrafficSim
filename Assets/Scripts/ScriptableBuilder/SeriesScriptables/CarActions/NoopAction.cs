using Assets.Scripts.ScriptableBuilder.ScriptLinking;
using UnityEngine;

namespace Assets.Scripts.ScriptableBuilder.SeriesScriptable.CarActions
{
    /// <summary>
    /// Set the forward acceleration of the car.
    ///     Relative to current velocity: negative will always brake, positive always speeds up
    /// </summary>
    public class NoopAction : ScriptableEntry, ICarAction
    {
        public override IScriptableEntry Execute(ICarActionable reciever)
        {
            return this.child;
        }

        public override bool GetCompatabilityWithDraggable(DragDropSeries draggable)
        {
            if(draggable is DragDropSeries)
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
            return true;
        }

        public CarActionTypes GetActionType()
        {
            return CarActionTypes.Noop;
        }
    }
}