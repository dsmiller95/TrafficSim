using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking
{
    public interface IScriptableWithInputs : IDragDropBehavior
    {
        bool ValidateInputs(IEnumerable<Type> inputTypes);
        void SetInputElements(IList<IScriptableInput> inputs);
    }

    /// <summary>
    /// Base class for any action which can have children under it or parents above
    ///     Examples: SetAcceleration, SetVelocity, Start, End
    /// </summary>
    public interface IScriptableEntryWithInputs : IScriptableEntry, IScriptableWithInputs
    {
    }
}
