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
    public class InputDragDrop : SeriesDragDrop
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
                //.Select(dropSlot => new InputSlot(dropSlot))
                .ToList();

            if(!(this.myScript as IScriptableEntryWithInputs).ValidateInputs(this.inputSlots.Select(slot => slot.acceptedType)))
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

            foreach(var slot in this.inputSlots)
            {
                if (slot.AttemptToFitElement(inputElement, Input.mousePosition))
                {
                    slot.PositionLinkedRelativeToSlot();
                    break;
                }
            }

            (this.myScript as IScriptableEntryWithInputs).SetInputElements(this.inputSlots
                .Select(slot => slot?.GetInputScript())
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