using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D;
using UnityEngine.U2D;

public class CarMovement : MonoBehaviour
{
    public float velocity = 1;

    private SplineNavigator navigator;

    // Start is called before the first frame update
    void Start()
    {
        this.navigator = this.GetComponent<SplineNavigator>();
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdatePosition(Time.deltaTime);
    }

    private void UpdatePosition(float deltaT)
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
