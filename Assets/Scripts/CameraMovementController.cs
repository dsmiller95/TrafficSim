using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovementController : MonoBehaviour
{
    public float sensitivity;
    public float panSpeedPercent;
    private Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        /*EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Scroll;
        entry.callback.AddListener((data) => { OnScroll((PointerEventData)data); });
        trigger.triggers.Add(entry);
        Debug.Log("Registered scroll trigger.");*/

        this.camera = this.GetComponent<Camera>();
        if (this.camera == null) {
            throw new System.Exception("No camera found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        var scrollInput = Input.GetAxis("Mouse ScrollWheel") * this.sensitivity;
        this.camera.orthographicSize += scrollInput;

        if (Input.GetKey(KeyCode.W))
        {
            this.camera.transform.position += Vector3.up * (this.panSpeedPercent * this.camera.orthographicSize);
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.camera.transform.position += Vector3.left * (this.panSpeedPercent * this.camera.orthographicSize);
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.camera.transform.position += Vector3.down * (this.panSpeedPercent * this.camera.orthographicSize);
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.camera.transform.position += Vector3.right * (this.panSpeedPercent * this.camera.orthographicSize);
        }
    }

        //public override void OnScroll(PointerEventData data)
        //{
        //    Debug.Log("On Scroll delegate called.");
        //    var scrollDist = data.scrollDelta.magnitude;
        //    this.camera.orthographicSize += 1;
        //}

        //public override void OnPointerClick(PointerEventData data)
        //{
        //    Debug.Log("OnPointerClick called.");
        //}
}
