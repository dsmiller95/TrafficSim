using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragDropLockable : EventTrigger
{
    private bool dragging;
    private Vector2 dragStartDiff;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            var newPos = this.GetMousePosOnCanvas() + this.dragStartDiff;
            Debug.Log($"dragging to ${newPos}");
            transform.position = newPos;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        this.dragStartDiff = new Vector2(this.transform.position.x, this.transform.position.y) - this.GetMousePosOnCanvas();
        dragging = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }

    private Vector2 GetMousePosOnCanvas()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(pos.x, pos.y);
    }
}
