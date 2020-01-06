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
        private IChainableSeries<IScriptableEntry> _nestedSeries;
        private IChainableSeries<IScriptableEntry> nestedSeries
        {
            get {
                return _nestedSeries;
            }
            set
            {
                (this.myScript as IScriptableEntryNested).SetNestedChild(value?.GetData());
                this._nestedSeries = value;
            }
        }

        private NestedChildChainTerminator pairedNestedBlockTerminator;

        private class NestedChildChainTerminator : IChainableSeries<IScriptableEntry>
        {
            private IChainableSeries<IScriptableEntry> parent;
            private NestedDragDrop owner;
            private IScriptableEntryNestedBlockTerminator terminatorBehavior;
            public NestedChildChainTerminator(NestedDragDrop owner)
            {
                this.owner = owner;
                var ownerScriptTyped = (owner.myScript as IScriptableEntryNested);
                this.terminatorBehavior = ownerScriptTyped.GetNestedBlockTerminator();
                this.terminatorBehavior.SetPairedNestingParent(ownerScriptTyped);
            }

            public void AbortSelf()
            {
                this.parent?.AbortChild(this);
                this.parent = null;
            }

            /// <summary>
            /// used to set the child of this node as associated with the Terminator. 
            /// </summary>
            /// <param name="childScript"></param>
            public void SetScriptChild(IScriptableEntry childScript)
            {
                Debug.Log($"Script child set {childScript}");
                this.terminatorBehavior.SetNextExecutingChild(childScript);
            }

            public bool AbortChild(IChainableSeries<IScriptableEntry> child)
            {
                return false;
            }

            public bool GetCanHaveChildren()
            {
                return false;
            }

            public bool GetCanHaveParents()
            {
                return true;
            }

            public IChainableSeries<IScriptableEntry> GetChild()
            {
                return null;
            }

            public IScriptableEntry GetData()
            {
                return this.terminatorBehavior;
            }

            public IChainableSeries<IScriptableEntry> GetParent()
            {
                return this.parent;
            }

            public void OnSelfEjected()
            {
                //Todo: maybe do nothing
                //throw new NotImplementedException();
            }
            public void SetParent(IChainableSeries<IScriptableEntry> parent)
            {
                this.parent = parent;
            }

            public void SpliceChildIn(IChainableSeries<IScriptableEntry> chainHead)
            {
            }

            public void OnChildUpdated(IChainableSeries<IScriptableEntry> child)
            {
            }

            public void OnParentUpdated(Vector2 childTranslation)
            {
            }

            public void SimpleAppendChild(IChainableSeries<IScriptableEntry> child)
            {
                throw new InvalidOperationException("Cannot append child to component, no children allowed");
            }
        }

        public override void Start()
        {
            base.Start();
            if (!(this.myScript is IScriptableEntryNested))
            {
                throw new System.Exception("ERROR: incompatable script attached");
            }
            this.pairedNestedBlockTerminator = new NestedChildChainTerminator(this);
            this.pairedNestedBlockTerminator.SetScriptChild(this.nextExecutingChild?.GetData());
        }

        public override void ChildLinked(IChainableSeries<IScriptableEntry> child)
        {
            base.ChildLinked(child);
            this.pairedNestedBlockTerminator.SetScriptChild(this.nextExecutingChild?.GetData());
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

        public override void OnDragging(PointerEventData data)
        {
            base.OnDragging(data);
            this.nestedSeries?.OnParentUpdated(this.GetNestedChildTransform());
        }

        public override bool AbortChild(IChainableSeries<IScriptableEntry> child)
        {
            if (base.AbortChild(child))
            {
                return true;
            }

            if (this.nestedSeries == child)
            {
                this.nestedSeries = null;
                return true;
            }
            return false;
        }

        protected override Vector3 GetChildTransform()
        {
            if (this.nestedSeries != null)
            {
                var currentPhysicalTerminator = ChainableSeriesUtilities.GetChainTerminator(this.nestedSeries);
                if (currentPhysicalTerminator == this.pairedNestedBlockTerminator)
                {
                    currentPhysicalTerminator = currentPhysicalTerminator.GetParent();
                }
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                if (currentPhysicalTerminator == this || !(currentPhysicalTerminator is SeriesDragDrop))
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
                {
                    return base.GetChildTransform();
                }
                var draggableTerminator = currentPhysicalTerminator as SeriesDragDrop;
                return base.GetChildTransform() + (draggableTerminator.transform.position + -this.transform.position + -this.GetNestedOffset());
            }
            return base.GetChildTransform();
        }

        /// <summary>
        /// Nestedchild is shifted over by half-width
        /// </summary>
        /// <returns></returns>
        private Vector3 GetNestedChildTransform()
        {
            return base.GetChildTransform() + this.GetNestedOffset();
        }

        private Vector3 GetNestedOffset()
        {
            return (this.rectTransform.sizeDelta.x / 2) * Vector3.right;
        }

        private void DropInNestedSeries(IChainableSeries<IScriptableEntry> newNested)
        {
            Debug.Log($"adding {(newNested as MonoBehaviour)?.name} as nested under {this.name}");
            if (newNested == null || !newNested.GetCanHaveParents())
            {
                throw new Exception("ERROR: invalid nesting. see logs");
            }

            var originalNested = this.nestedSeries;
            this.nestedSeries = newNested;
            newNested.SetParent(this);

            ChainableSeriesUtilities.UpdateOriginalChildAfterSplice(originalNested, newNested);

            this.OnChildUpdated(this.nestedSeries);
            //var currentTerminator = ChainableSeriesUtilities.GetChainTerminator(this.nestedSeries);
            //if (currentTerminator.GetCanHaveChildren())
            //{
            //    this.pairedNestedBlockTerminator.GetParent()?.AbortChild(this.pairedNestedBlockTerminator);
            //    this.pairedNestedBlockTerminator.SetParent(null);
            //    currentTerminator.SpliceChildIn(this.pairedNestedBlockTerminator);
            //}

            //(newNested as SeriesDragDrop).UpdatePositionRelativeToParent(this.GetNestedChildTransform());
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
            if (mousePositionRelativeToLocal.x > this.rectTransform.sizeDelta.x / 2)
            {
                return true;
            }
            return false;
        }

        public override void OnChildUpdated(IChainableSeries<IScriptableEntry> child)
        {
            if(child == this.nestedSeries)
            {
                var currentTerminator = ChainableSeriesUtilities.GetChainTerminator(this.nestedSeries);
                if(currentTerminator != this.pairedNestedBlockTerminator)
                {
                    this.pairedNestedBlockTerminator.AbortSelf();

                    if (currentTerminator.GetCanHaveChildren())
                    {
                        currentTerminator.SimpleAppendChild(this.pairedNestedBlockTerminator);
                    }
                }
                this.GetChild().OnParentUpdated(this.GetChildTransform());
            }
            base.OnChildUpdated(child);
        }
    }
}