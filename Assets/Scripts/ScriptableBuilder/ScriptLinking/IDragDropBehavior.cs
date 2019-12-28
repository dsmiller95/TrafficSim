using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking
{
    /// <summary>
    /// Defines any type which can be paired with a <see cref="DragDropSeries"/>
    ///     And any methods needed to setup or establish a linking between the two
    /// </summary>
    public interface IDragDropBehavior
    {
        bool GetCompatabilityWithDraggable(DragDropSeries draggable);
    }
}
