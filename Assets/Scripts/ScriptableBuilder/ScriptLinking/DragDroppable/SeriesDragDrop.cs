using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking
{
    public class SeriesDragDrop : BaseDragDrop, IChainableSeries<IScriptableEntry>
    {
        // The DragDropSeries can only support IScriptableEntry, since it inherently links in a parent-child chain
        public IScriptableEntry myScript;

        public IChainableSeries<IScriptableEntry> parent;
        private IChainableSeries<IScriptableEntry> _child;
        public IChainableSeries<IScriptableEntry> nextExecutingChild {
            get {
                return _child;
            }
            set {
                _child = value;
                if(value != null)
                {
                    this.ChildLinked(value);
                }
                else
                {
                    this.DraggableChildUnlinked();
                }
            }
        }
        private IChainableSeries<IScriptableEntry> childAsChainable
        {
            get => this.nextExecutingChild as IChainableSeries<IScriptableEntry>;
        }

        // Use this for initialization
        public override void Start()
        {
            base.Start();
            this.myScript = this.GetComponent<IScriptableEntry>();
            if (this.myScript == null || !this.myScript.GetCompatabilityWithDraggable(this))
            {
                throw new System.Exception($"Critical: behavior {this.myScript} is incompatable with the current dragabble implementation");
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        //TODO: refactor to use IChainable
        private bool IsCompatableChild(BaseDragDrop other)
        {
            // Only change color if a link is possible
            var compatableSibling = other as SeriesDragDrop;
            return this.myScript.GetCanHaveChildren()
                   && compatableSibling != null
                   && compatableSibling.myScript.GetCanHaveParents();
        }

        /// <summary>
        /// This object has another DragDrop hovering over it, ready to link when dropped
        /// </summary>
        /// <param name="other">the other DragDrop which is about to link</param>
        public override void DragabbleLinkEnter(BaseDragDrop other)
        {
            // Only respond if a link was possible
            if (this.IsCompatableChild(other))
            {
                base.DragabbleLinkEnter(other);
            }
        }

        /// <summary>
        /// This object no longer has another DragDropLockable about to pair with it
        /// </summary>
        /// <param name="other">The other Lockable which was previously hovering to link</param>
        public override void DraggableLinkExit(BaseDragDrop other)
        {
            // Only respond if a link was possible
            if (this.IsCompatableChild(other))
            {
                base.DraggableLinkExit(other);
            }
        }

        public override void OnDragStart()
        {
            base.OnDragStart();
            if (this.parent != null)
            {
                this.parent.AbortChild(this);
                this.SetParent(null);
            }
        }

        public override void OnDragging(PointerEventData data)
        {
            base.OnDragging(data);
            if (this.GetCanHaveChildren())
            {
                this.childAsChainable?.OnParentUpdated(this.GetChildTransform());
                //this.childAsDragDrop?.UpdatePositionRelativeToParent(this.GetChildTransform());
            }
        }

        public override void DraggableDroppedOnto(BaseDragDrop other)
        {
            if (this.IsCompatableChild(other))
            {
                var compatableOther = other as SeriesDragDrop;
                this.SpliceChildIn(compatableOther);
                //compatableOther.AttachToParent(this);
            }
        }

        /// <summary>
        /// Called whenever the child of this dragabble is set to anything not null
        /// </summary>
        /// <param name="child">The child which has been linked to this object</param>
        public virtual void ChildLinked(IChainableSeries<IScriptableEntry> child)
        {
            this.baseImage.color = Color.white;
            this.myScript.SetNextExecutingChild(child.GetData());
        }

        /// <summary>
        /// Called whenever the child of this dragabble is set to null
        /// </summary>
        public virtual void DraggableChildUnlinked()
        {
            this.myScript.SetNextExecutingChild(null);
        }


        /// <summary>
        /// Return a transform to apply to a child relative to this object when linking
        /// </summary>
        /// <returns></returns>
        protected virtual Vector3 GetChildTransform()
        {
            return Vector3.down * this.rectTransform.sizeDelta.y;
        }

        public void UpdatePositionRelativeToParent(Vector3 childTransform)
        {
            var compatableParent = this.parent as SeriesDragDrop;
            this.transform.position = compatableParent.transform.position + childTransform;
            this.OnPositionChanged();
            //this.childAsDragDrop?.UpdatePositionRelativeToParent(this.GetChildTransform());
        }

        public void OnSelfEjected()
        {
            this.rectTransform.position = this.rectTransform.position + this.rectTransform.sizeDelta.x * Vector3.right;
            this.childAsChainable?.OnParentUpdated(this.GetChildTransform());
            //var childAsDragDrop = this.nextExecutingChild as SeriesDragDrop;
            //if(childAsDragDrop != null)
            //{
            //    childAsDragDrop.UpdatePositionRelativeToParent(this.GetChildTransform());
            //}
        }

        private Vector2 GetMousePosOnCanvas()
        {
            var pos = Input.mousePosition;
            return new Vector2(pos.x, pos.y);
        }

        public bool GetCanHaveChildren()
        {
            return this.myScript.GetCanHaveChildren();
        }

        public bool GetCanHaveParents()
        {
            return this.myScript.GetCanHaveParents();
        }

        public IChainableSeries<IScriptableEntry> GetChild()
        {
            return this.nextExecutingChild;
        }

        public IChainableSeries<IScriptableEntry> GetParent()
        {
            return this.parent;
        }

        public void SpliceChildIn(IChainableSeries<IScriptableEntry> newChild)
        {
            if(newChild == null || !this.GetCanHaveChildren() || !newChild.GetCanHaveParents())
            {
                return;
            }
            var originalChild = this.GetChild();

            this.nextExecutingChild = newChild;
            newChild.SetParent(this);

            ChainableSeriesUtilities.UpdateOriginalChildAfterSplice(originalChild, newChild);

            newChild.OnParentUpdated(this.GetChildTransform());
            this.OnChildUpdated(this);

            //(newChild as SeriesDragDrop)?.UpdatePositionRelativeToParent(this.GetChildTransform());
        }

        public IScriptableEntry GetData()
        {
            return this.myScript;
        }

        public void SetParent(IChainableSeries<IScriptableEntry> parent)
        {
            this.parent = parent;
        }

        public virtual bool AbortChild(IChainableSeries<IScriptableEntry> child)
        {
            if (this.GetChild() != null && this.GetChild() == child)
            {
                this.nextExecutingChild = null;
                return true;
            }
            return false;
        }

        public virtual void OnChildUpdated(IChainableSeries<IScriptableEntry> child)
        {
            this.parent?.OnChildUpdated(this);
        }

        public virtual void OnParentUpdated(Vector2 childTransform)
        {
            this.UpdatePositionRelativeToParent(childTransform);
            this.childAsChainable?.OnParentUpdated(this.GetChildTransform());
        }

        public void SimpleAppendChild(IChainableSeries<IScriptableEntry> child)
        {
            if (!this.GetCanHaveChildren() || this.nextExecutingChild != null)
            {
                throw new InvalidOperationException($"Cannot append child to {this.name}");
            }
            this.nextExecutingChild = child;
        }
    }
}