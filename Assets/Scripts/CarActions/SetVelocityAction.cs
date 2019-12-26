using UnityEngine;

public class SetVelocityAction : MonoBehaviour, ICarFloatAction
{
    private ICarActionable reciever;

    public void Execute(float acceleration)
    {
        this.reciever?.SetForwardVelocity(acceleration);
    }
    public void SetAcionReceiver(ICarActionable reciever)
    {
        this.reciever = reciever;
    }

    public CarActionTypes GetActionType()
    {
        return CarActionTypes.SetVelocity;
    }
}