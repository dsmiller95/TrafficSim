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

        public override void OnDragStart()
        {
            base.OnDragStart();
            this.container?.OnContainedRemoved(this);
        }

        public virtual void OnContainerUpdated(Vector2 slotTranslation)
        {
            this.PositionSelfRelativeToContainer(slotTranslation);
        }

        protected virtual void PositionSelfRelativeToContainer(Vector2 containerSlotPosition)
        {
            this.transform.position = containerSlotPosition;
            base.OnPositionChanged();
        }

        protected IInputElementContainer container;
        public void SetContainer(IInputElementContainer container)
        {
            this.container = container;
        }

        public Vector2 GetRectSize()
        {
            return this.rectTransform.sizeDelta;
        }
    }
}