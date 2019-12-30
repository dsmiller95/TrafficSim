using Assets.Scripts.ScriptableBuilder.ScriptLinking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScriptableBuilder.Scriptables.ControlScriptables
{
    class DoForeverNestableControl : ScriptableEntryNested
    {
        public override IScriptableEntry Execute(ICarActionable reciever)
        {
            return this.nestedChild;
        }

        public override bool GetCanHaveChildren()
        {
            return false;
        }

        public override bool GetCanHaveParents()
        {
            return true;
        }

        public override bool GetCompatabilityWithDraggable(BaseDragDrop draggable)
        {
            return draggable is NestedDragDrop;
        }

        public override IScriptableEntryNestedBlockTerminator GetNestedBlockTerminator()
        {
            return new LoopbackNestedTerminator();
        }

        public override void SetInputElements(IList<IScriptableInput> inputs)
        {
        }

        public override bool ValidateInputs(IEnumerable<Type> inputTypes)
        {
            return inputTypes.Count() == 0;
        }
    }
}
