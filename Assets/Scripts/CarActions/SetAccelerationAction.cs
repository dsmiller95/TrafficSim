﻿using UnityEngine;

/// <summary>
/// Set the forward acceleration of the car.
///     Relative to current velocity: negative will always brake, positive always speeds up
/// </summary>
public class SetAccelerationAction : DragDropLockable, ICarFloatAction
{
    public float acceleraton = 1;

    public ILinkedScriptingObject Execute(ICarActionable reciever)
    {
        //TODO: get acceleration value from linked sensor
        Debug.Log($"Set acceleration to {this.acceleraton}");
        reciever.SetForwardAcceleration(this.acceleraton);
        return this.GetChild();
    }

    public CarActionTypes GetActionType()
    {
        return CarActionTypes.Noop;
    }
}