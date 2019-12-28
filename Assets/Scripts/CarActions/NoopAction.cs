using UnityEngine;

/// <summary>
/// Set the forward acceleration of the car.
///     Relative to current velocity: negative will always brake, positive always speeds up
/// </summary>
public class NoopAction : DragDropLockable, ICarFloatAction
{
    public ILinkedScriptingObject Execute(ICarActionable reciever)
    {
        return this.GetChild();
    }

    public CarActionTypes GetActionType()
    {
        return CarActionTypes.Noop;
    }
}