using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking.DragDroppable
{
    /// <summary>
    /// Models any interactions necessary for things like resizing and repositioning IInputElements
    ///     Necessary to allow parent elements to resize based on their children,
    ///     for example if an element gets dragged into the input slot of one of their children and changes the vertical size
    /// </summary>
    interface IInputElement
    {
    }
}
