using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

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
            ScriptableEntryInputConfig config;
            public DragDropInputElement linkedElement;
            public InputSlot(ScriptableEntryInputConfig conf)
            {
                this.config = conf;
            }
            public bool AttemptToFitElement(DragDropInputElement element, Vector2 mousePosInParent)
            {
                if (element.outputType == config.acceptedType && this.config.droppableZone.Contains(mousePosInParent))
                {
                    this.linkedElement = element;
                    Debug.Log($"Linked input element: {element}");
                    return true;
                }
                return false;
            }
        }

        private IList<InputSlot> inputSlots;

        public override void Start()
        {
            base.Start();
            if (!(this.myScript is IScriptableEntryWithInputs))
            {
                throw new System.Exception("ERROR: incompatable script attached");
            }
            this.inputSlots = (this.myScript as IScriptableEntryWithInputs)
                    .GetInputConfigs()
                    .Select(config => new InputSlot(config))
                    .ToList();
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
                if (slot.AttemptToFitElement(inputElement, this.rectTransform.InverseTransformPoint(Input.mousePosition)))
                {
                    break;
                }
            }

            (this.myScript as IScriptableEntryWithInputs).SetInputElements(this.inputSlots
                .Where(slot => slot.linkedElement)
                .Select(slot => slot.linkedElement.myScript)
                .ToList());
        }
    }
}