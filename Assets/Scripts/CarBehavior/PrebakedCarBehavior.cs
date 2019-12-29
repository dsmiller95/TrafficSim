using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PrebakedCarBehavior: MonoBehaviour, ICarBehavior
{
    public float maxAcceleration = 2;
    public GameObject sensors;

    public void ExecuteBehavior(ICarActionable target)
    {
        var acceleration = maxAcceleration;
        var farFrontSensor = target.GetCarSensor<bool>(CarSensorTypes.FrontFar);
        var frontSensor = target.GetCarSensor<bool>(CarSensorTypes.Front);
        if (farFrontSensor.Sense())
        {
            acceleration = -maxAcceleration / 2;
        }
        if (frontSensor.Sense())
        {
            acceleration = -maxAcceleration;
        }
        target.SetForwardAcceleration(acceleration);
        //this.GetCarAction<ICarFloatAction>(actions, CarActionTypes.SetAcceleration)?.Execute(acceleration);
    }
}