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
    public interface IScriptableEntryWithInputs : IScriptableEntry
    {
        IList<ScriptableEntryInputConfig> GetInputConfigs();
        void SetInputElements(IList<IScriptableInput> inputs);
    }

    public struct ScriptableEntryInputConfig
    {
        /// <summary>
        /// The area which will accept a drop, positioned relative to the current GameObject
        /// </summary>
        public Rect droppableZone;

        /// <summary>
        /// The type which this input slot expects
        /// </summary>
        public Type acceptedType;
    }
}
