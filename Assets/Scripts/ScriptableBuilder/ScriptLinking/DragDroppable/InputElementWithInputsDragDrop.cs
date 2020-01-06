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
    public class InputElementWithInputsDragDrop : InputElementDragDrop, IInputElementContainer
    {
        /// <summary>
        /// all input slots, ordered from left to right in increasing x-order
        /// </summary>
        private List<InputDropSlot> inputSlots;
        public override void Start()
        {
            base.Start();
            if (!(this.myScript is IScriptableWithInputs) || !this.myScript.GetCompatabilityWithDraggable(this))
            {
                throw new System.Exception($"Critical: behavior {this.myScript} is incompatable with the current dragabble implementation");
            }

            this.inputSlots = new List<InputDropSlot>(this.GetComponentsInChildren<InputDropSlot>())
                .OrderBy(slot => slot.transform.position.x)
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

            InputDropSlot slotFit = null;
            foreach (var slot in this.inputSlots)
            {
                if (slot.AttemptToFitElement(inputElement, Input.mousePosition))
                {
                    slotFit = slot;
                    break;
                }
            }
            if (!slotFit)
            {
                return;
            }

            (this.myScript as IScriptableWithInputs).SetInputElements(this.inputSlots
                .Select(slot => slot?.GetInputScript())
                .ToList());

            this.OnContainedUpdated(inputElement);
            this.inputSlots.ForEach(slot =>
            {
                slot.PassContainerUpdated();
            });
            //slotFit.PassContainerUpdated();
        }

        /// <summary>
        /// look at current children or contents and adjust size accordingly
        /// Will also adjust the size of the slots accordingly, but won't update the positions of the elments inside the slots
        /// </summary>
        public void AdjustSizeBasedOnCurrentChildren()
        {
            var currentLeftOffset = 0f;
            var maxVerticalSlotSize = 0f;
            for (var i = 0; i < this.inputSlots.Count; i++)
            {
                var diff = this.inputSlots[i].ChangeSizeToFitLinkedElement();
                maxVerticalSlotSize = Mathf.Max(maxVerticalSlotSize, diff.y);

            }
            //TODO: finish method implementation
        }

        protected override void PositionSelfRelativeToContainer(Vector2 containerPosition)
        {
            base.PositionSelfRelativeToContainer(containerPosition);
        }

        public override void OnContainerUpdated(Vector2 slotTranslation)
        {
            base.OnContainerUpdated(slotTranslation);
            this.inputSlots.ForEach(slot =>
            {
                slot.PassContainerUpdated();
            });
        }

        /// <summary>
        /// When any of the children elements are updated. In this implementation, pass the event upwards if possible
        /// </summary>
        public void OnContainedUpdated(IInputElement child)
        {
            this.AdjustSizeBasedOnCurrentChildren();
            this.container?.OnContainedUpdated(this);
            this.inputSlots
                .Where(input => input != child).ToList()
                .ForEach(slot =>
                {
                    slot.PassContainerUpdated();
                });
        }

        /// <summary>
        /// Called when a child has been removed from its ownership
        /// </summary>
        public void OnContainedRemoved(IInputElement child)
        {
            this.inputSlots.ForEach(slot => slot.AttemptAbortChild(child as InputElementDragDrop));
            this.OnContainedUpdated(null);
        }

        public override void OnDragging(PointerEventData data)
        {
            base.OnDragging(data);
            this.inputSlots.ForEach(slot =>
            {
                slot.PassContainerUpdated();
            });
        }
    }
}