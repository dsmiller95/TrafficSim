using UnityEngine;

public interface ICarAction
{
    void SetAcionReceiver(ICarActionable reciever);
    CarActionTypes GetActionType();
}

public enum CarActionTypes
{
    SetVelocity
}