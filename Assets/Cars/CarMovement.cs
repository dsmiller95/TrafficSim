using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D;
using UnityEngine.U2D;

public class CarMovement : MonoBehaviour, ICarActionable
{
    public GameObject farFront;
    public GameObject closeFront;
    public GameObject actionSet;

    private SplineNavigator navigator;
    private ICarBehavior carBehavior;

    private Dictionary<CarSensorTypes, ICarSensor> sensors;
    private Dictionary<CarActionTypes, ICarAction> actions;

    private float velocity = 0;

    // Start is called before the first frame update
    void Start()
    {
        this.navigator = this.GetComponent<SplineNavigator>();
        this.carBehavior = this.GetComponent<ICarBehavior>();

        var sensorsList = new List<ICarSensor>();
        sensorsList.Add(this.farFront.GetComponent<ColliderSensor>());
        sensorsList.Add(this.closeFront.GetComponent<ColliderSensor>());
        this.sensors = sensorsList.ToDictionary(sense => sense.GetSensorType());

        var actionList = new List<ICarAction>(this.actionSet.GetComponents<ICarAction>());
        actionList.ForEach(act => act.SetAcionReceiver(this));
        this.actions = actionList
            .ToDictionary(act => act.GetActionType());
    }

    // Update is called once per frame
    void Update()
    {
        if(this.velocity != 0)
        {
            this.UpdatePosition(Time.deltaTime, this.velocity);
        }
        this.carBehavior.ExecuteBehavior(this.sensors, this.actions);
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

    public void SetForwardVelocity(float newVelocity)
    {
        this.velocity = newVelocity;
    }
}
