using Assets.Scripts.ScriptableBuilder.ScriptLinking;
using UnityEngine;

namespace Assets.Scripts.ScriptableBuilder.SeriesScriptable.CarActions
{
    /// <summary>
    /// Set the forward acceleration of the car.
    ///     Relative to current velocity: negative will always brake, positive always speeds up
    /// </summary>
    public class SetAccelerationAction : ScriptableEntry, ICarFloatAction
    {
        public float acceleraton = 1;

        public override IScriptableEntry Execute(ICarActionable reciever)
        {
            //TODO: get acceleration value from linked sensor
            Debug.Log($"Set acceleration to {this.acceleraton}");
            reciever.SetForwardAcceleration(this.acceleraton);
            return this.child;
        }

        public CarActionTypes GetActionType()
        {
            return CarActionTypes.SetAcceleration;
        }

        public override bool GetCompatabilityWithDraggable(DragDropSeries draggable)
        {
            return draggable is DragDropSeries;
        }

        public override bool GetCanHaveChildren()
        {
            return true;
        }

        public override bool GetCanHaveParents()
        {
            return true;
        }
    }
}