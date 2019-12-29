using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.ScriptableBuilder.ScriptLinking
{
    public class DragDropBase : EventTrigger
    {
        // A dragabble of any type could potentially be dragging at any point
        protected static DragDropBase CurrentDragging;

        protected Image baseImage;
        private bool dragging;

        // A set of other dragabbles which this object is currently being dragged over
        //  The linkable parameter could be of any type
        private ISet<DragDropBase> dragLinked = new HashSet<DragDropBase>();

        public RectTransform rectTransform
        {
            get => this.GetComponent<RectTransform>();
        }

        // Use this for initialization
        public virtual void Start()
        {
            this.baseImage = this.GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        /// <summary>
        /// This object has another Lockable hovering over it, ready to link when dropped
        /// </summary>
        /// <param name="other">the other Lockable which is about to link</param>
        public virtual void DragabbleLinkEnter(DragDropBase other)
        {
            this.baseImage.color = Color.green;
        }

        /// <summary>
        /// This object no longer has another DragDropLockable about to pair with it
        /// </summary>
        /// <param name="other">The other Lockable which was previously hovering to link</param>
        public virtual void DraggableLinkExit(DragDropBase other)
        {
            this.baseImage.color = Color.white;
        }

        /// <summary>
        /// This draggable has had another dragabble dropped on top of it
        /// </summary>
        /// <param name="other">the draggable which was dropped</param>
        public virtual void DraggableDroppedOnto(DragDropBase other)
        {
        }

        /// <summary>
        /// Called when this object begins being dragged
        /// </summary>
        public virtual void OnDragStart()
        {
            this.baseImage.color = Color.blue;
        }

        /// <summary>
        /// Called whenever the current object has been moved as a result of a drag
        /// </summary>
        public virtual void OnDragging(PointerEventData eventData)
        {
        }

        /// <summary>
        /// Called whenever the current object was dragging, but no longer is
        /// Always called before <see cref="DraggableDroppedOnto(DragDropBase)"/> if on top of other draggable
        /// </summary>
        public virtual void OnDragEnd()
        {
            this.baseImage.color = Color.white;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (this.dragging)
            {
                return;
            }

            if (CurrentDragging != null)
            {
                this.DragabbleLinkEnter(CurrentDragging);
                CurrentDragging.dragLinked.Add(this);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (this.dragging)
            {
                return;
            }
            if (CurrentDragging != null)
            {
                this.DraggableLinkExit(CurrentDragging);
                CurrentDragging.dragLinked.Remove(this);
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            dragging = true;
            this.transform.SetAsFirstSibling();

            CurrentDragging = this;
            this.OnDragStart();
        }
        public override void OnDrag(PointerEventData eventData)
        {
            this.transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
            this.OnDragging(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            this.OnDragEnd();
            dragging = false;
            CurrentDragging = null;
            var target = this.dragLinked.FirstOrDefault();
            if (target)
            {
                target.DraggableDroppedOnto(this);
            }
            this.dragLinked.Clear();
        }

        private Vector2 GetMousePosOnCanvas()
        {
            var pos = Input.mousePosition;
            return new Vector2(pos.x, pos.y);
        }
    }
}