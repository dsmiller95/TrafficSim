using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking
{
    public class SeriesDragDrop : BaseDragDrop
    {
        // The DragDropSeries can only support IScriptableEntry, since it inherently links in a parent-child chain
        public IScriptableEntry myScript;

        public SeriesDragDrop parent;
        private SeriesDragDrop _child;
        public SeriesDragDrop nextExecutingChild {
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

        private bool IsCompatableChild(BaseDragDrop other)
        {
            // Only change color if a link is possible
            var compatableSibling = other as SeriesDragDrop;
            return this.myScript.GetCanHaveChildren()
                   && compatableSibling != null
                   && compatableSibling.myScript.GetCanHaveParents();
        }

        /// <summary>
        /// This object has another Lockable hovering over it, ready to link when dropped
        /// </summary>
        /// <param name="other">the other Lockable which is about to link</param>
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
                this.parent.nextExecutingChild = null;
                this.parent = null;
            }
        }

        public override void OnDragging(PointerEventData data)
        {
            base.OnDragging(data);
            if (this.myScript.GetCanHaveChildren())
            {
                this.nextExecutingChild?.UpdatePositionRelativeToParent(this.GetChildTransform());
            }
        }

        public override void DraggableDroppedOnto(BaseDragDrop other)
        {
            if (this.IsCompatableChild(other))
            {
                var compatableOther = other as SeriesDragDrop;
                this.AttachChild(compatableOther);
                //compatableOther.AttachToParent(this);
            }
        }

        /// <summary>
        /// Called whenever the child of this dragabble is set to anything not null
        /// </summary>
        /// <param name="child">The child which has been linked to this object</param>
        public virtual void DraggableChildLinked(SeriesDragDrop child)
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
        protected virtual Vector3 GetChildTransform()
        {
            return Vector3.down * this.rectTransform.sizeDelta.y;
        }

        public void UpdatePositionRelativeToParent(Vector3 childTransform)
        {
            this.transform.position = parent.transform.position + childTransform;
            this.OnPositionChanged();
            this.nextExecutingChild?.UpdatePositionRelativeToParent(this.GetChildTransform());
        }

        private void AttachChild(SeriesDragDrop child)
        {
            if (child == null || !child.myScript.GetCanHaveParents())
            {
                return;
            }
            if (child.myScript.GetCanHaveChildren())
            {
                child.parent = this;
                if (this.nextExecutingChild)
                {
                    child.CascadeFloatingChild(this.nextExecutingChild);
                }
                this.nextExecutingChild = child;
            }
            else
            {
                //cascade to the bottom and kick out any existing terminators
                this.CascadeFloatingChild(child, true);
            }

            if (this.nextExecutingChild)
            {
                this.nextExecutingChild.UpdatePositionRelativeToParent(this.GetChildTransform());
            }
        }

        /// <summary>
        /// After a drag completes on top of a Lockable with children, insert the new block of Lockables directly below the target
        ///     And cascade all previous children to the bottom of the chain
        /// If this element can't have children, by default it will eject the floating child without a parent
        /// </summary>
        /// <param name="child">The child to be dropped to the bottom of the current list</param>
        public void CascadeFloatingChild(SeriesDragDrop child, bool ejectSelfIfNoChildren = false)
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
                        this.nextExecutingChild.UpdatePositionRelativeToParent(this.GetChildTransform());
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
            this.nextExecutingChild?.UpdatePositionRelativeToParent(this.GetChildTransform());
        }

        private Vector2 GetMousePosOnCanvas()
        {
            var pos = Input.mousePosition;
            return new Vector2(pos.x, pos.y);
        }
    }
}