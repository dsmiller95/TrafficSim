using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.U2D;
using UnityEngine;
using Unity.Collections;
using UnityEditor;

namespace Assets.Scripts
{
    class SplineNavigator : MonoBehaviour
    {
        public GameObject spriteShape;

        private Waypoint lastWaypoint;
        private float distanceFromLastWaypoint;


        // Start is called before the first frame update
        void Start()
        {
            var navigableSpline = this.spriteShape.GetComponent<NavigableSpline>();
            if (navigableSpline == null)
            {
                throw new System.Exception("Error: no sprite shape renderer on GameObject");
            }

            if (navigableSpline.waypoints == null)
            {
                throw new System.Exception("Error: waypoints list not initialized");
            }
            lastWaypoint = navigableSpline.waypoints[0];
            if (lastWaypoint == null)
            {
                throw new System.Exception("Error: no waypoints found");
            }
            distanceFromLastWaypoint = 0;
        }

        // Update is called once per frame
        void Update()
        {
        }

        /// <summary>
        /// Translates the linked transform across the spline it is locked to by a certain distance
        /// </summary>
        /// <param name="distance"></param>
        public void TranslateAcrossSpline(float distance)
        {
            var newDistance = distance + distanceFromLastWaypoint;
            while (true)
            {
                if (newDistance > lastWaypoint.GetDistanceToNext())
                {
                    newDistance -= lastWaypoint.GetDistanceToNext();
                    if (lastWaypoint.next == null)
                    {
                        return;
                    }
                    lastWaypoint = lastWaypoint.next;
                }
                else if (newDistance < 0)
                {
                    newDistance += lastWaypoint.GetDistanceToPrev();
                    if (lastWaypoint.previous == null)
                    {
                        return;
                    }
                    lastWaypoint = lastWaypoint.previous;
                }
                else
                {
                    this.distanceFromLastWaypoint = newDistance;
                    break;
                }
            }

            var newPosition = this.GetPositionFromCurrentWaypointAndDistance();

            this.transform.position = newPosition;
            var nextPosition = distance > 0 ? this.lastWaypoint.next.position : this.lastWaypoint.position;
            //var lookNormalVector = (this.transform.position - nextPosition).normalized;
            //var tmpQuat = new Quaternion();
            //tmpQuat.SetFromToRotation(this.transform.position, nextPosition);
            //this.transform.rotation = tmpQuat;
            this.transform.right = nextPosition - transform.position;
            //this.transform.LookAt(nextPosition, new Vector3(0, 0, 1));
            //this.transform.rotation.SetLookRotation(lookNormalVector);
        }

        private Vector3 GetPositionFromCurrentWaypointAndDistance()
        {
            return this.lastWaypoint.position + (this.lastWaypoint.next.position - this.lastWaypoint.position).normalized * this.distanceFromLastWaypoint;
        }
    }
}
