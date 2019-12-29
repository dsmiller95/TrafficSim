using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking
{
    public class DragDropSeries : DragDropBase
    {
        // The DragDropSeries can only support IScriptableEntry, since it inherently links in a parent-child chain
        protected IScriptableEntry myScript;

        public DragDropSeries parent;
        private DragDropSeries _child;
        public DragDropSeries nextExecutingChild {
            get {
                return _child;
            }
            set {
                _child = value;
                if(value != null)
                {
                    this.DraggableChildLinked(value);
                }
                else
                {
                    this.DraggableChildUnlinked();
                }
            }
        }

        public RectTransform rectTransform
        {
            get => this.GetComponent<RectTransform>();
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

        private bool IsCompatableChild(DragDropBase other)
        {
            // Only change color if a link is possible
            var compatableSibling = other as DragDropSeries;
            return this.myScript.GetCanHaveChildren()
                   && compatableSibling != null
                   && compatableSibling.myScript.GetCanHaveParents();
        }

        /// <summary>
        /// This object has another Lockable hovering over it, ready to link when dropped
        /// </summary>
        /// <param name="other">the other Lockable which is about to link</param>
        public override void DragabbleLinkEnter(DragDropBase other)
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
        public override void DraggableLinkExit(DragDropBase other)
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
                this.parent.nextExecutingChild = null;
                this.parent = null;
            }
        }

        public override void OnDragging(PointerEventData data)
        {
            base.OnDragging(data);
            if (this.myScript.GetCanHaveChildren())
            {
                this.nextExecutingChild?.UpdatePositionRelativeToParent();
            }
        }

        public override void DraggableDroppedOnto(DragDropBase other)
        {
            if (this.IsCompatableChild(other))
            {
                var compatableOther = other as DragDropSeries;
                compatableOther.AttachToParent(this);
            }
        }

        /// <summary>
        /// Called whenever the child of this dragabble is set to anything not null
        /// </summary>
        /// <param name="child">The child which has been linked to this object</param>
        public virtual void DraggableChildLinked(DragDropSeries child)
        {
            this.baseImage.color = Color.white;
            this.myScript.SetNextExecutingChild(child.myScript);
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
        private Vector3 GetChildTransform()
        {
            return Vector3.down * this.rectTransform.sizeDelta.y;
        }

        private void UpdatePositionRelativeToParent()
        {
            this.transform.position = parent.transform.position + parent.GetChildTransform();
            this.nextExecutingChild?.UpdatePositionRelativeToParent();
        }

        private void AttachToParent(DragDropSeries parent)
        {
            if (parent == null || !this.myScript.GetCanHaveParents())
            {
                return;
            }
            if (this.myScript.GetCanHaveChildren())
            {
                this.parent = parent;
                if (this.parent.nextExecutingChild)
                {
                    this.CascadeFloatingChild(this.parent.nextExecutingChild);
                }
                this.parent.nextExecutingChild = this;
            }
            else
            {
                //cascade to the bottom and kick out any existing terminators
                parent.CascadeFloatingChild(this, true);
            }

            if (this.parent)
            {
                this.UpdatePositionRelativeToParent();
            }
        }

        /// <summary>
        /// After a drag completes on top of a Lockable with children, insert the new block of Lockables directly below the target
        ///     And cascade all previous children to the bottom of the chain
        /// If this element can't have children, by default it will eject the floating child without a parent
        /// </summary>
        /// <param name="child">The child to be dropped to the bottom of the current list</param>
        private void CascadeFloatingChild(DragDropSeries child, bool ejectSelfIfNoChildren = false)
        {
            if (this.nextExecutingChild)
            {
                this.nextExecutingChild.CascadeFloatingChild(child, ejectSelfIfNoChildren);
            }
            else
            {
                if (child.myScript.GetCanHaveParents())
                {
                    if (this.myScript.GetCanHaveChildren())
                    {
                        this.nextExecutingChild = child;
                        this.nextExecutingChild.parent = this;
                        this.nextExecutingChild.UpdatePositionRelativeToParent();
                        return;
                    }
                    else
                    {
                        if (ejectSelfIfNoChildren)
                        {
                            // since the floating cannot be ejected, eject this element
                            child.parent = this.parent;
                            child.parent.nextExecutingChild = child;

                            this.parent = null;
                            this.EjectFromPosition();
                            return;
                        }
                    }
                }
                //If child cannot be placed, it is orphaned
                child.parent = null;
                child.EjectFromPosition();
            }
        }

        /// <summary>
        /// this item has been severed from its chain due to a drag taking its place
        ///     By default this will shift the element over by its width to offset and make it visible
        /// </summary>
        private void EjectFromPosition()
        {
            this.rectTransform.position = this.rectTransform.position + this.rectTransform.sizeDelta.x * Vector3.right;
            this.nextExecutingChild?.UpdatePositionRelativeToParent();
        }

        private Vector2 GetMousePosOnCanvas()
        {
            var pos = Input.mousePosition;
            return new Vector2(pos.x, pos.y);
        }
    }
}