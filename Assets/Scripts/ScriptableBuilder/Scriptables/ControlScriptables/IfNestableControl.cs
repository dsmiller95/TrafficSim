using Assets.Scripts.ScriptableBuilder.ScriptLinking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScriptableBuilder.Scriptables.ControlScriptables
{
    class IfNestableControl : ScriptableEntryNested
    {
        private IScriptableInput<bool> switchInput;

        public override IScriptableEntry Execute(ICarActionable reciever)
        {
            if (this.switchInput.Sense(reciever))
            {
                return this.nestedChild;
            }
            return this.child;
        }

        public override bool GetCanHaveChildren()
        {
            return true;
        }

        public override bool GetCanHaveParents()
        {
            return true;
        }

        public override bool GetCompatabilityWithDraggable(BaseDragDrop draggable)
        {
            return draggable is NestedDragDrop;
        }

        public override void SetInputElements(IList<IScriptableInput> inputs)
        {
            if(inputs != null)
            {
                this.switchInput = inputs[0] as IScriptableInput<bool>;
            }
        }

        public override bool ValidateInputs(IEnumerable<Type> inputTypes)
        {
            return inputTypes.SequenceEqual(new Type[] { typeof(bool) });
        }
    }
}
