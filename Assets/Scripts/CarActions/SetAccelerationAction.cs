using UnityEngine;

/// <summary>
/// Set the forward acceleration of the car.
///     Relative to current velocity: negative will always brake, positive always speeds up
/// </summary>
public class SetAccelerationAction : MonoBehaviour, ICarFloatAction
{
    private ICarActionable reciever;

    public void Execute(float acceleration)
    {
        this.reciever.SetForwardAcceleration(acceleration);
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