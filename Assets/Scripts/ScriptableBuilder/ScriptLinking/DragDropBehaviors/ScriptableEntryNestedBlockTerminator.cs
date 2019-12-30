using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking
{
    /// <summary>
    /// This class is used in conjunction with nested blocks.
    /// The behaviors derived from this are not going to be assembled by the user, but are generated and used as a placeholder 
    ///     at the end of a nested chain. For Ex and If behavior will create a nestedBlockTerminator which just passes through to the next child
    /// </summary>
    public abstract class ScriptableNestedBlockTerminator : IScriptableEntryNestedBlockTerminator
    {
        protected IScriptableEntryNested pairedNestedParent;
        protected IScriptableEntry child;

        public abstract IScriptableEntry Execute(ICarActionable reciever);

        public bool GetCanHaveChildren()
        {
            return false;
        }

        public bool GetCanHaveParents()
        {
            return true;
        }

        public bool GetCompatabilityWithDraggable(BaseDragDrop draggable)
        {
            throw new NotImplementedException();
        }

        public void SetNextExecutingChild(IScriptableEntry child)
        {
            this.child = child;
        }

        public void SetPairedNestingParent(IScriptableEntryNested parent)
        {
            this.pairedNestedParent = parent;
        }
    }
}
