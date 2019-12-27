using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class DragDropLockable : EventTrigger
{
    public static DragDropLockable CurrentDragging;

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
    void Start()
    {
        this.baseImage = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (this.dragging)
        {
            return;
        }

        this.baseImage.color = Color.green;
        if(CurrentDragging != null)
        {
            CurrentDragging.dragLinked.Add(this);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (this.dragging)
        {
            return;
        }
        this.baseImage.color = Color.white;
        if (CurrentDragging != null)
        {
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
        this.AttachToParent(this.dragLinked.FirstOrDefault());
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
