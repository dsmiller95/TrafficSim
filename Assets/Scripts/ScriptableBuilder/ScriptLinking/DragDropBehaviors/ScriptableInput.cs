using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking
{
    public abstract class ScriptableInput<T> : MonoBehaviour, IScriptableInput<T>
    {
        public abstract bool GetCompatabilityWithDraggable(BaseDragDrop draggable);

        public abstract T Sense(ICarSensable target);

        public Type GetOutputType()
        {
            return typeof(T);
        }
    }
}
