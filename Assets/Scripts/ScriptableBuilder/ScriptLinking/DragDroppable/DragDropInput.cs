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
    public class DragDropInput : DragDropSeries
    {
        private class InputSlot
        {
            public Type acceptedType;
            public DragDropInputElement linkedElement;
            public InputDropSlot slotObj;
            public InputSlot(InputDropSlot slotObj)
            {
                this.slotObj = slotObj;
                this.acceptedType = slotObj.acceptedType;
            }
            public bool AttemptToFitElement(DragDropInputElement element, Vector2 mousePos)
            {
                if (element.outputType == this.acceptedType
                    && this.slotObj.rectTransform.rect.Contains(this.slotObj.rectTransform.InverseTransformPoint(mousePos)))
                {
                    this.linkedElement = element;
                    Debug.Log($"Linked input element: {element}");
                    return true;
                }
                return false;
            }

            public void PositionLinkedRelativeToSlot()
            {
                if (this.linkedElement != null)
                {
                    this.linkedElement.PositionSelfRelativeToContainer(this.slotObj.rectTransform.position);
                }
            }
        }

        private List<InputSlot> inputSlots;

        public override void Start()
        {
            base.Start();
            if (!(this.myScript is IScriptableEntryWithInputs))
            {
                throw new System.Exception("ERROR: incompatable script attached");
            }
            this.inputSlots = new List<InputDropSlot>(this.GetComponentsInChildren<InputDropSlot>())
                .Select(dropSlot => new InputSlot(dropSlot))
                .ToList();

            if(!(this.myScript as IScriptableEntryWithInputs).ValidateInputs(this.inputSlots.Select(slot => slot.acceptedType)))
            {
                throw new System.Exception($"Critical Error: GameObject {this.name} not set up with correct InputDropSlots");
            }
        }

        public override void DraggableDroppedOnto(DragDropBase other)
        {
            Debug.Log($"Draggable {other.name} dropped onto {this.name}");
            var inputElement = other as DragDropInputElement;
            if (inputElement == null)
            {
                base.DraggableDroppedOnto(other);
                return;
            }

            foreach(var slot in this.inputSlots)
            {
                if (slot.AttemptToFitElement(inputElement, Input.mousePosition))
                {
                    inputElement.PositionSelfRelativeToContainer(slot.slotObj.rectTransform.position);
                    break;
                }
            }

            (this.myScript as IScriptableEntryWithInputs).SetInputElements(this.inputSlots
                .Where(slot => slot.linkedElement)
                .Select(slot => slot.linkedElement.myScript)
                .ToList());
        }

        public override void OnPositionChanged()
        {
            base.OnPositionChanged();
            this.inputSlots.ForEach(slot =>
            {
                slot.PositionLinkedRelativeToSlot();
            });
        }
    }
}