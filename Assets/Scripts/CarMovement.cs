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
    public float maxSpeed;
    public bool direction;

    private SplineNavigator navigator;
    private ICarBehavior carBehavior {
        get => this.GetComponent<ICarBehavior>();
    }

    //private Dictionary<CarSensorTypes, ICarSensor> sensors;
    //private Dictionary<CarActionTypes, ICarAction> actions;

    private float velocity = 0;
    private float acceleration = 0;

    // Start is called before the first frame update
    void Start()
    {
        this.navigator = this.GetComponent<SplineNavigator>();

        //var sensorsList = new List<ICarSensor>();
        //sensorsList.Add(this.farFront.GetComponent<ColliderSensor>());
        //sensorsList.Add(this.closeFront.GetComponent<ColliderSensor>());
        //this.sensors = sensorsList.ToDictionary(sense => sense.GetSensorType());

        //var actionList = new List<ICarAction>(this.actionSet.GetComponents<ICarAction>());
        //actionList.ForEach(act => act.SetAcionReceiver(this));
        //this.actions = actionList
        //    .ToDictionary(act => act.GetActionType());
    }

    // Update is called once per frame
    void Update()
    {
        this.velocity += this.acceleration * Time.deltaTime;
        this.velocity = Mathf.Min(this.maxSpeed, Mathf.Max(0, this.velocity));
        if(this.velocity != 0)
        {
            this.UpdatePosition(Time.deltaTime, this.velocity * (this.direction ? 1 : -1));
        }
        //if (Input.GetKeyDown(KeyCode.F))
        //{
            this.carBehavior?.ExecuteBehavior(this);
        //}
    }

    private void UpdatePosition(float deltaT, float velocity)
    {
        this.navigator.TranslateAcrossSpline(velocity * deltaT);
        //this.transform.Translate(0, 0, -1);
    }

    public void OnDrawGizmos()
    {
        //this.navigator.DrawGizmos();
        //Gizmos.color = Color.blue;
        //Gizmos.DrawCube(this.transform.position, new Vector3(0.5f, 0.5f, 0.5f));
    }

    public void SetForwardVelocity(float newVelocity)
    {
        this.velocity = Mathf.Abs(newVelocity);
    }

    public void SetForwardAcceleration(float newAcceleration)
    {
        Debug.Log($"Accelerate set to {newAcceleration}");
        this.acceleration = newAcceleration;
    }

    public void SetDirection(bool direction)
    {
        this.direction = direction;
    }

    public ICarSensorInstance<T> GetCarSensor<T>(CarSensorTypes type)
    {
        var sensors = new List<ICarSensorInstance<T>>(this.GetComponentsInChildren<ICarSensorInstance<T>>());
        return sensors.Where(s => s.GetSensorType() == type).FirstOrDefault();
    }
}
