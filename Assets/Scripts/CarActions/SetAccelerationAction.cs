using UnityEngine;

public class SetAccelerationAction : MonoBehaviour, ICarFloatAction
{
    private ICarActionable reciever;

    public void Execute(float acceleration)
    {
        //TODO: allow for acceleration
        throw new System.NotImplementedException();
    }

    public void SetAcionReceiver(ICarActionable reciever)
    {
        this.reciever = reciever;
    }

    public CarActionTypes GetActionType()
    {
        return CarActionTypes.SetAcceleration;
    }
}