using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Assets.Scripts.ScriptableBuilder.ScriptLinking.DragDroppable;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking
{
    /// <summary>
    /// Class to model items like sensors and variables
    ///     will allow for self to be dragged into "slots", but cannot enter into a Series chain
    /// </summary>
    public class InputElementDragDrop : BaseDragDrop, IInputElement
    {
        public IScriptableInput myScript;
        public Type outputType
        {
            get => myScript.GetOutputType();
        }

        public override void Start()
        {
            base.Start();
            this.myScript = this.GetComponent<IScriptableInput>();
            if (this.myScript == null || !this.myScript.GetCompatabilityWithDraggable(this))
            {
                throw new System.Exception($"Critical: behavior {this.myScript} is incompatable with the current dragabble implementation");
            }
        }

        public virtual void PositionSelfRelativeToContainer(Vector2 containerPosition)
        {
            this.transform.position = containerPosition;
            base.OnPositionChanged();
        }
    }
}