using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.ScriptableBuilder.ScriptLinking;
using UnityEngine;

namespace Assets.Scripts.ScriptableBuilder.SeriesScriptables.CarSensors
{
    public class GenericCarSensor<T> : ScriptableInput<T>
    {
        public CarSensorTypes sensorType;
        public override bool GetCompatabilityWithDraggable(BaseDragDrop draggable)
        {
            return draggable is InputElementDragDrop;
        }

        public override T Sense(ICarSensable target)
        {
            var sensor = target.GetCarSensor<T>(this.sensorType);
            if (sensor != null)
            {
                return sensor.Sense();
            }
            Debug.LogError($"Error: no sensor found on {(target as MonoBehaviour).name} for type {this.sensorType}");
            return default;
        }
    }
}
