using UnityEngine;

public interface ICarAction : ILinkedScriptingObject
{
    CarActionTypes GetActionType();

    ILinkedScriptingObject Execute(ICarActionable target);
}

public enum CarActionTypes
{
    SetVelocity,
    Noop,
    Start
}