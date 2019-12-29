using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking
{
    /// <summary>
    /// Base class for any action which can have children under it or parents above
    ///     Examples: SetAcceleration, SetVelocity, Start, End
    /// </summary>
    public abstract class ScriptableEntryNested : ScriptableEntry, IScriptableEntryNested
    {
        protected IScriptableEntry nestedChild;
        public void SetNestedChild(IScriptableEntry child)
        {
            this.nestedChild = child;
        }

        public abstract void SetInputElements(IList<IScriptableInput> inputs);
        public abstract bool ValidateInputs(IEnumerable<Type> inputTypes);
    }
}
