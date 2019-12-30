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
    ///     This variation accepts other inputs and performs some operation on the two. Used for things like arithmetic operations
    /// </summary>
    public class InputElementWithInputsDragDrop : InputElementDragDrop
    {
        private List<InputDropSlot> inputSlots;
        public override void Start()
        {
            base.Start();
            if (!(this.myScript is IScriptableWithInputs) || !this.myScript.GetCompatabilityWithDraggable(this))
            {
                throw new System.Exception($"Critical: behavior {this.myScript} is incompatable with the current dragabble implementation");
            }

            this.inputSlots = new List<InputDropSlot>(this.GetComponentsInChildren<InputDropSlot>())
                .ToList();

            if (!(this.myScript as IScriptableWithInputs).ValidateInputs(this.inputSlots.Select(slot => slot.acceptedType)))
            {
                throw new System.Exception($"Critical Error: GameObject {this.name} not set up with correct InputDropSlots");
            }
        }

        public override void DraggableDroppedOnto(BaseDragDrop other)
        {
            var inputElement = other as InputElementDragDrop;
            if (inputElement == null)
            {
                base.DraggableDroppedOnto(other);
                return;
            }

            foreach (var slot in this.inputSlots)
            {
                if (slot.AttemptToFitElement(inputElement, Input.mousePosition))
                {
                    slot.PositionLinkedRelativeToSlot();
                    break;
                }
            }

            (this.myScript as IScriptableWithInputs).SetInputElements(this.inputSlots
                .Select(slot => slot?.GetInputScript())
                .ToList());
        }
        public override void PositionSelfRelativeToContainer(Vector2 containerPosition)
        {
            base.PositionSelfRelativeToContainer(containerPosition);
            this.inputSlots.ForEach(slot =>
            {
                slot.PositionLinkedRelativeToSlot();
            });
        }
    }
}