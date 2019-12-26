using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrebakedCarBehavior: MonoBehaviour, ICarBehavior
{
    public float baseVelocity = 2;

    public void ExecuteBehavior(IDictionary<CarSensorTypes, ICarSensor> sensors, IDictionary<CarActionTypes, ICarAction> actions)
    {
        var newVel = this.baseVelocity;
        var farFrontSensor = this.GetCarSensor<IBooleanCarSensor>(sensors, CarSensorTypes.FrontFar);
        var frontSensor = this.GetCarSensor<IBooleanCarSensor>(sensors, CarSensorTypes.Front);
        if (farFrontSensor.Sense())
        {
            newVel /= 2;
        }
        if (frontSensor.Sense())
        {
            newVel = 0;
        }
        this.GetCarAction<ICarFloatAction>(actions, CarActionTypes.SetVelocity)?.Execute(newVel);
    }
    private T GetCarAction<T>(IDictionary<CarActionTypes, ICarAction> actions, CarActionTypes type) where T : class, ICarAction
    {
        if (!actions.ContainsKey(type))
        {
            return default;
        }

        return actions[type] as T;
    }

    private T GetCarSensor<T>(IDictionary<CarSensorTypes, ICarSensor> sensors, CarSensorTypes type) where T : class, ICarSensor
    {
        if (!sensors.ContainsKey(type))
        {
            return default;
        }

        return sensors[type] as T;
    }
}