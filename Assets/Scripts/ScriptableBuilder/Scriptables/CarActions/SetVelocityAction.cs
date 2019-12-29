using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.ScriptableBuilder.ScriptLinking;
using UnityEngine;

namespace Assets.Scripts.ScriptableBuilder.SeriesScriptable.CarActions
{
    public class SetVelocityAction : ScriptableEntry, IScriptableEntryWithInputs
    {
        public override IScriptableEntry Execute(ICarActionable reciever)
        {
            if(this.firstBoolInput != null)
            {
                if (this.firstBoolInput.Sense(reciever))
                {
                    reciever.SetForwardVelocity(0);
                }
                else
                {
                    reciever.SetForwardVelocity(1);
                }
            }
            else
            {
                //TODO: grab velocity from linked sensor object
                reciever.SetForwardVelocity(1);
            }
            return this.child;
        }

        public CarActionTypes GetActionType()
        {
            return CarActionTypes.SetVelocity;
        }

        public override bool GetCanHaveChildren()
        {
            return true;
        }

        public override bool GetCanHaveParents()
        {
            return true;
        }

        public override bool GetCompatabilityWithDraggable(DragDropBase draggable)
        {
            return draggable is DragDropInput;
        }

        public IList<ScriptableEntryInputConfig> GetInputConfigs()
        {
            return new List<ScriptableEntryInputConfig>()
            {
                new ScriptableEntryInputConfig
                {
                    droppableZone = new Rect(0, 0, 50, 20),
                    acceptedType = typeof(bool)
                }
            };
        }

        private IScriptableInput<bool> firstBoolInput;

        public void SetInputElements(IList<IScriptableInput> inputs)
        {
            if(inputs != null)
            {
                this.firstBoolInput = inputs.FirstOrDefault() as IScriptableInput<bool>;
            }
        }
    }
}