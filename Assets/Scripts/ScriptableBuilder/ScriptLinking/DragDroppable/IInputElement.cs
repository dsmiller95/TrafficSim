using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking.DragDroppable
{
    /// <summary>
    /// Models any interactions necessary for things like resizing and repositioning IInputElements
    ///     Necessary to allow parent elements to resize based on their children,
    ///     for example if an element gets dragged into the input slot of one of their children and changes the vertical size
    /// Can be thought of as a way for Inputelements to talk to each other
    /// </summary>
    public interface IInputElementContainer
    {
        /// <summary>
        /// bubbles up to the top of the tree to update sizing
        /// </summary>
        void OnContainedUpdated(IInputElement child);
        /// <summary>
        /// Called when a child has been removed from its ownership
        /// </summary>
        void OnContainedRemoved(IInputElement child);
    }

    public interface IInputElement
    {


        /// <summary>
        /// Sets the "parent" of this component, AKA the element which will recieve any bottom-up updates
        /// </summary>
        /// <param name="container"></param>
        void SetContainer(IInputElementContainer container);

        /// <summary>
        /// bubbles back down the tree to update positioning of elements
        /// </summary>
        void OnContainerUpdated(Vector2 slotTranslation);

        Vector2 GetRectSize();
    }
}
