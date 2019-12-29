using Assets.Scripts.ScriptableBuilder.ScriptLinking;
using System;
using UnityEngine;

//public interface ICarSensorInstance
//{
//    Type GetType();
//}
public interface ICarSensorInstance<T>
{
    T Sense();
    CarSensorTypes GetSensorType();
}

public enum CarSensorTypes
{
    FrontFar,
    Front
}