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
        public bool AttemptToFitElement(InputElementDragDrop element, Vector2 mousePos)
        {
            if (element.outputType == this.acceptedType
                && this.rectTransform.rect.Contains(this.rectTransform.InverseTransformPoint(mousePos)))
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
                this.linkedElement.PositionSelfRelativeToContainer(this.rectTransform.position);
            }
        }

        public IScriptableInput GetInputScript()
        {
            return this.linkedElement.myScript;
        }
    }

    //used as a handy way to configure the dropSlot via the gameObject editor
    enum PossibleSlotType
    {
        Float,
        Boolean
    }
}
