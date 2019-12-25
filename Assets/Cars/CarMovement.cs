using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D;
using UnityEngine.U2D;

public class CarMovement : MonoBehaviour
{
    public float velocity = 1;

    public GameObject farFront;
    public GameObject closeFront;

    private SplineNavigator navigator;

    private IBooleanCarSensor farFrontSensor;
    private IBooleanCarSensor closeFrontSensor;

    // Start is called before the first frame update
    void Start()
    {
        this.navigator = this.GetComponent<SplineNavigator>();
        this.farFrontSensor = this.farFront.GetComponent<ColliderSensor>();
        this.closeFrontSensor = this.closeFront.GetComponent<ColliderSensor>();
    }

    // Update is called once per frame
    void Update()
    {
        var currentVel = this.velocity;
        if (this.farFrontSensor.Sense())
        {
            currentVel /= 2;
        }
        if (!this.closeFrontSensor.Sense())
        {
            this.UpdatePosition(Time.deltaTime, currentVel);
        }
    }

    private void UpdatePosition(float deltaT, float velocity)
    {
        this.navigator.TranslateAcrossSpline(velocity * deltaT);
        this.transform.Translate(0, 0, -1);
    }

    public void OnDrawGizmos()
    {
        //this.navigator.DrawGizmos();
        //Gizmos.color = Color.blue;
        //Gizmos.DrawCube(this.transform.position, new Vector3(0.5f, 0.5f, 0.5f));
    }
}
