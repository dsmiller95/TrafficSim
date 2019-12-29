using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking
{
    /// <summary>
    /// Defines a drag-drop behavior which acts as a single input, usually linked with a sensor
    /// </summary>
    public interface IScriptableInput : IDragDropBehavior
    {
        Type GetOutputType();
    }

    /// <summary>
    /// Genericized version
    /// </summary>
    public interface IScriptableInput<T> : IScriptableInput
    {
        T Sense(ICarSensable target);
    }

    public abstract class ScriptableInput<T> : MonoBehaviour, IScriptableInput<T>
    {
        public abstract bool GetCompatabilityWithDraggable(DragDropBase draggable);

        public abstract T Sense(ICarSensable target);

        public Type GetOutputType()
        {
            return typeof(T);
        }
    }
}
