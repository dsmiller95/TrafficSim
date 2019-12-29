public interface ICarActionable : ICarSensable
{
    void SetForwardVelocity(float newVelocity);
    void SetForwardAcceleration(float newcAceleration);

    void SetDirection(bool direction);
}