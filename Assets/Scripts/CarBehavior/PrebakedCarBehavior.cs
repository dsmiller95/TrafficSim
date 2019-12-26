using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrebakedCarBehavior: MonoBehaviour, ICarBehavior
{
    public float maxAcceleration = 2;

    public void ExecuteBehavior(IDictionary<CarSensorTypes, ICarSensor> sensors, IDictionary<CarActionTypes, ICarAction> actions)
    {
        var acceleration = maxAcceleration;
        var farFrontSensor = this.GetCarSensor<IBooleanCarSensor>(sensors, CarSensorTypes.FrontFar);
        var frontSensor = this.GetCarSensor<IBooleanCarSensor>(sensors, CarSensorTypes.Front);
        if (farFrontSensor.Sense())
        {
            acceleration = -maxAcceleration/2;
        }
        if (frontSensor.Sense())
        {
            acceleration = -maxAcceleration;
        }
        this.GetCarAction<ICarFloatAction>(actions, CarActionTypes.SetAcceleration)?.Execute(acceleration);
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