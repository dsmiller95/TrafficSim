using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking.DragDroppable
{
    class InputDropSlot : MonoBehaviour
    {
        public PossibleSlotType externalType;
        public RectTransform rectTransform
        {
            get => this.GetComponent<RectTransform>();
        }
        private Vector2 defaultSize;
        private IInputElementContainer myContainer;

        public Type acceptedType
        {
            get
            {
                switch (this.externalType)
                {
                    case PossibleSlotType.Float:
                        return typeof(float);
                    case PossibleSlotType.Boolean:
                        return typeof(bool);
                    default:
                        return null;
                }
            }
        }

        //private Type acceptedType;
        private InputElementDragDrop linkedElement;

        public void Start()
        {
            this.defaultSize = this.rectTransform.sizeDelta;
            this.myContainer = this.gameObject.GetComponentInParent<IInputElementContainer>();
        }

        public bool AttemptToFitElement(InputElementDragDrop element, Vector2 mousePos)
        {
            if (element.outputType == this.acceptedType
                && this.rectTransform.rect.Contains(this.rectTransform.InverseTransformPoint(mousePos)))
            {
                this.linkedElement = element;
                this.linkedElement.SetContainer(this.myContainer);
                Debug.Log($"Linked input element: {element}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Change size to contain the element inside, returning the difference from its past size
        /// </summary>
        /// <param name="size">the new size to take on</param>
        /// <returns>The difference between the new size and the old size</returns>
        public Vector2 ChangeSizeToFitLinkedElement()
        {
            var newSize = this.linkedElement?.rectTransform.sizeDelta ?? this.defaultSize;
            var output = newSize - this.rectTransform.sizeDelta;
            this.rectTransform.sizeDelta = newSize;
            return output;
        }

        public bool AttemptAbortChild(InputElementDragDrop contained)
        {
            if(contained == this.linkedElement)
            {
                this.linkedElement = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// alert the contained component that the container has been updated
        /// </summary>
        public void PassContainerUpdated()
        {
            this.linkedElement?.OnContainerUpdated(this.rectTransform.position);
        }

        public IScriptableInput GetInputScript()
        {
            return this.linkedElement?.myScript;
        }

        //public bool DoesExacltyMatchInputElement(IInputElement input)
        //{
        //    return input == linkedElement;
        //}
    }

    //used as a handy way to configure the dropSlot via the gameObject editor
    enum PossibleSlotType
    {
        Float,
        Boolean
    }
}
