using UnityEngine;

public interface ICarSensor
{
    CarSensorTypes GetSensorType();
}

public enum CarSensorTypes
{
    FrontFar,
    Front
}