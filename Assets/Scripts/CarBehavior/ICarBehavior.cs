using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ICarBehavior
{
    void ExecuteBehavior(IDictionary<CarSensorTypes, ICarSensor> sensors, IDictionary<CarActionTypes, ICarAction> actions);
}