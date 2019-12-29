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

        public Type acceptedType {
            get {
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
    }

    //used as a handy way to configure the dropSlot via the gameObject editor
    enum PossibleSlotType
    {
        Float,
        Boolean
    }
}
