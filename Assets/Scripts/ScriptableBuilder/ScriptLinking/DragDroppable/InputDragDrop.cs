using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.ScriptableBuilder.ScriptLinking.DragDroppable;
using System;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking
{
    /// <summary>
    /// An element which in addition to forming a chain, can also accept input elements.
    /// EX: accelerate, setVelocity
    /// </summary>
    public class InputDragDrop : SeriesDragDrop, IInputElementContainer
    {
        private List<InputDropSlot> inputSlots;

        public override void Start()
        {
            base.Start();
            if (!(this.myScript is IScriptableEntryWithInputs))
            {
                throw new System.Exception("ERROR: incompatable script attached");
            }
            this.inputSlots = new List<InputDropSlot>(this.GetComponentsInChildren<InputDropSlot>())
                .ToList();

            if(!(this.myScript as IScriptableWithInputs).ValidateInputs(this.inputSlots.Select(slot => slot.acceptedType)))
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
            slotFit.PassContainerUpdated();
        }
        /// <summary>
        /// look at current children or contents and adjust size accordingly
        /// Will also adjust the size of the slots accordingly, but won't update the positions of the elments inside the slots
        /// </summary>
        private void AdjustSizeBasedOnCurrentChildren()
        {
            //TODO: finish method implementation. link with InputElementWithInputsDragDrop
        }

        public override void OnDragging(PointerEventData data)
        {
            base.OnDragging(data);
            this.inputSlots.ForEach(slot =>
            {
                slot.PassContainerUpdated();
            });
        }

        public override void OnParentUpdated(Vector2 childTransform)
        {
            base.OnParentUpdated(childTransform);
            this.inputSlots.ForEach(slot =>
            {
                slot.PassContainerUpdated();
            });
        }

        public void OnContainedUpdated(IInputElement child)
        {
            this.AdjustSizeBasedOnCurrentChildren();
            this.GetChild()?.OnParentUpdated(this.GetChildTransform()); 
            this.GetParent()?.OnChildUpdated(this);
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
    }
}