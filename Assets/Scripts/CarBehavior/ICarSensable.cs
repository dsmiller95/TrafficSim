public interface ICarSensable
{
    ICarSensorInstance<T> GetCarSensor<T>(CarSensorTypes type);
}