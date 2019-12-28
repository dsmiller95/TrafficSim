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
    public interface IScriptableEntry : IDragDropBehavior
    {
        bool GetCanHaveChildren();
        bool GetCanHaveParents();
        void SetNextExecutingChild(IScriptableEntry child);
        IScriptableEntry Execute(ICarActionable reciever);
    }
    /// <summary>
    /// Base class for any action which can have children under it
    ///     Examples: SetAcceleration, SetVelocity, Start
    /// </summary>
    public abstract class ScriptableEntry : MonoBehaviour, IScriptableEntry
    {
        protected IScriptableEntry child;
        public void SetNextExecutingChild(IScriptableEntry child)
        {
            this.child = child;
        }
        public abstract IScriptableEntry Execute(ICarActionable reciever);

        public abstract bool GetCompatabilityWithDraggable(DragDropSeries draggable);
        public abstract bool GetCanHaveChildren();
        public abstract bool GetCanHaveParents();
    }
}
