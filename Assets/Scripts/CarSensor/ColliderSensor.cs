using UnityEngine;
using System.Collections;

public class ColliderSensor : MonoBehaviour, ICarSensorInstance<bool>
{
    public LayerMask colliderLayerMask;
    public CarSensorTypes sensorType;

    private CapsuleCollider2D capsuleCollider;

    public CarSensorTypes GetSensorType()
    {
        return this.sensorType;
    }

    public bool Sense()
    {
        return this.capsuleCollider.IsTouchingLayers(this.colliderLayerMask);
    }

    // Use this for initialization
    void Start()
    {
        this.capsuleCollider = this.GetComponent<CapsuleCollider2D>();  
    }

    // Update is called once per frame
    void Update()
    {

    }
}
