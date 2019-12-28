﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class DragDropLockable : EventTrigger
{

    protected bool canHaveChildren = true;
    protected bool canHaveParents = true;

    private static DragDropLockable CurrentDragging;

    private Image baseImage;
    private bool dragging;
    private ISet<DragDropLockable> dragLinked = new HashSet<DragDropLockable>();

    private DragDropLockable parent;
    private DragDropLockable child;

    private RectTransform rectTransform
    {
        get => this.GetComponent<RectTransform>();
    }

    // Use this for initialization
    public void Start()
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
    public virtual void DragabbleLinkEnter(DragDropLockable other)
    {
        this.baseImage.color = Color.green;
    }

    /// <summary>
    /// This object no longer has another DragDropLockable about to pair with it
    /// </summary>
    /// <param name="other">The other Lockable which was previously hovering to link</param>
    public virtual void DraggableLinkExit(DragDropLockable other)
    {
        this.baseImage.color = Color.white;
    }

    /// <summary>
    /// This object no longer has another DragDropLockable about to pair with it
    ///     this happens either because the 
    /// </summary>
    /// <param name="child">The child which has been linked to this object</param>
    public virtual void DraggableChildLinked(DragDropLockable child)
    {
        this.baseImage.color = Color.white;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (this.dragging)
        {
            return;
        }

        if(CurrentDragging != null)
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
        this.baseImage.color = Color.blue;
        CurrentDragging = this;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if(this.parent != null)
        {
            this.parent.child = null;
            this.parent = null;
        }
        this.transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
        this.child?.UpdatePositionRelativeToParent();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        this.baseImage.color = Color.white;
        CurrentDragging = null;
        var target = this.dragLinked.FirstOrDefault();
        if (target)
        {
            this.AttachToParent(target);
            target.DraggableChildLinked(this);
        }
        this.dragLinked.Clear();
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
        this.child?.UpdatePositionRelativeToParent();
    }

    private void AttachToParent(DragDropLockable parent)
    {
        if(parent == null)
        {
            return;
        }
        this.parent = parent;
        if (this.parent.child)
        {
            this.CascadeFloatingChild(this.parent.child);
        }
        this.parent.child = this;
        this.UpdatePositionRelativeToParent();
    }

    /// <summary>
    /// After a drag completes on top of a Lockable with children, insert the new block of Lockables directly below the target
    ///     And cascade all previous children to the bottom of the chain
    /// </summary>
    /// <param name="child">The child to be dropped to the bottom of the current list</param>
    private void CascadeFloatingChild(DragDropLockable child)
    {
        if (this.child)
        {
            this.child.CascadeFloatingChild(child);
        }
        else
        {
            this.child = child;
            this.child.parent = this;
            this.child.UpdatePositionRelativeToParent();
        }
    }

    private Vector2 GetMousePosOnCanvas()
    {
        var pos = Input.mousePosition;
        return new Vector2(pos.x, pos.y);
    }
}
