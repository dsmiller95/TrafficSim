using UnityEngine;

public class SetVelocityAction : DragDropLockable, ICarFloatAction
{
    public ILinkedScriptingObject Execute(ICarActionable target)
    {
        //TODO: grab velocity from linked sensor object
        target.SetForwardVelocity(1);
        return this.GetChild();
    }

    public CarActionTypes GetActionType()
    {
        return CarActionTypes.SetVelocity;
    }
}