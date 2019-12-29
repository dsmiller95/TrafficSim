using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking
{
    /// <summary>
    /// Models a draggable which can nest other DragDropSeries inside of it in addition to directly after it; optionally
    /// </summary>
    public class NestedDragDrop : InputDragDrop
    {
        private SeriesDragDrop _nestedSeries;
        private SeriesDragDrop nestedSeries
        {
            get {
                return _nestedSeries;
            }
            set
            {
                (this.myScript as IScriptableEntryNested).SetNestedChild(value.myScript);
                this._nestedSeries = value;
            }
        }


        public override void Start()
        {
            base.Start();
            if (!(this.myScript is IScriptableEntryNested))
            {
                throw new System.Exception("ERROR: incompatable script attached");
            }
        }

        public override void DraggableDroppedOnto(BaseDragDrop other)
        {
            Debug.Log($"Draggable {other.name} dropped onto {this.name}");
            var inputElement = other as SeriesDragDrop;
            if (inputElement != null && this.ShouldNestDragged(inputElement))
            {
                this.DropInNestedSeries(inputElement);
                return;
            }
            base.DraggableDroppedOnto(other);
        }

        public override void OnPositionChanged()
        {
            this.nestedSeries?.UpdatePositionRelativeToParent(this.GetNestedChildTransform());
            base.OnPositionChanged();
        }

        private void DropInNestedSeries(SeriesDragDrop nested)
        {
            Debug.Log($"adding {nested.name} as nested under {this.name}");
            if (nested.myScript.GetCanHaveChildren())
            {
                nested.parent = this;
                if (this.nestedSeries)
                {
                    nested.CascadeFloatingChild(this.nestedSeries);
                }
                this.nestedSeries = nested;
            }
            else
            {
                if (this.nestedSeries)
                {
                    //cascade to the bottom and kick out any existing terminators
                    this.nestedSeries.CascadeFloatingChild(nested, true);
                }
                else
                {
                    nested.parent = this;
                    this.nestedSeries = nested;
                }
            }
            if (this.nestedSeries)
            {
                this.nestedSeries.UpdatePositionRelativeToParent(this.GetNestedChildTransform());
            }
        }

        protected override Vector3 GetChildTransform()
        {
            //TODO: find the end of the nestedSeries
            return base.GetChildTransform();
        }

        /// <summary>
        /// Nestedchild is shifted over by half-width
        /// </summary>
        /// <returns></returns>
        private Vector3 GetNestedChildTransform()
        {
            return base.GetChildTransform() + (Vector3.right * this.rectTransform.sizeDelta.x / 2);
        }

        /// <summary>
        /// Gets whether or not the drag-e should be nested inside the component, or after it
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if other should be nested inside, false otherwise</returns>
        private bool ShouldNestDragged(SeriesDragDrop other)
        {
            var mousePositionRelativeToLocal = this.rectTransform.InverseTransformPoint(Input.mousePosition);
            // Only nest if the mouse position is on right half of this element
            if(mousePositionRelativeToLocal.x > this.rectTransform.sizeDelta.x / 2)
            {
                return true;
            }
            return false;
        }
    }
}