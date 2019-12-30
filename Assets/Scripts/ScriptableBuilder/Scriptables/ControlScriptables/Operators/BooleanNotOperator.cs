using Assets.Scripts.ScriptableBuilder.ScriptLinking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScriptableBuilder.Scriptables.ControlScriptables.Operators
{
    class BooleanNotOperator : ScriptableInput<bool>, IScriptableInputWithInputs<bool>
    {
        public override bool GetCompatabilityWithDraggable(BaseDragDrop draggable)
        {
            return draggable is InputElementWithInputsDragDrop;
        }

        public override bool Sense(ICarSensable target)
        {
            return this.booleanInput.Sense(target);
        }

        private IScriptableInput<bool> booleanInput;
        public void SetInputElements(IList<IScriptableInput> inputs)
        {
            if (inputs != null)
            {
                this.booleanInput = inputs[0] as IScriptableInput<bool>;
            }
        }

        public bool ValidateInputs(IEnumerable<Type> inputTypes)
        {
            return inputTypes != null && inputTypes.SequenceEqual(new Type[] { typeof(bool) });
        }
    }
}
