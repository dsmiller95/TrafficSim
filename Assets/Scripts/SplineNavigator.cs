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
        public int navigableRouteIndex = 0;

        private NavigableSpline splineNavigable;
        private Waypoint _lastWaypoint;

        private readonly int firstWaypoint = 0;
        private Waypoint lastWaypoint {
            get {
                if(this._lastWaypoint == null)
                {
                    this._lastWaypoint = this.splineNavigable.navigableRoutes[navigableRouteIndex][this.firstWaypoint];
                }
                return this._lastWaypoint;
            }
            set
            {
                this._lastWaypoint = value;
            }
        }
        private float distanceFromLastWaypoint;


        // Start is called before the first frame update
        void Start()
        {
            this.splineNavigable = this.spriteShape.GetComponent<NavigableSpline>();
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
                var distanceToNext = this.lastWaypoint.GetDistanceToNext();
                if (newDistance > distanceToNext)
                {
                    newDistance -= distanceToNext;
                    if (this.lastWaypoint.next == null)
                    {
                        return;
                    }
                    this.lastWaypoint = this.lastWaypoint.next;
                }
                else if (newDistance < 0)
                {
                    newDistance += this.lastWaypoint.GetDistanceToPrev();
                    if (this.lastWaypoint.previous == null)
                    {
                        return;
                    }
                    this.lastWaypoint = this.lastWaypoint.previous;
                }
                else
                {
                    this.distanceFromLastWaypoint = newDistance;
                    break;
                }
            }

            var newPosition = this.GetPositionFromCurrentWaypointAndDistance(this.lastWaypoint);

            this.transform.position = newPosition;
            var nextPosition = distance > 0 ? this.lastWaypoint.next.position : this.lastWaypoint.position;
            this.transform.right = nextPosition - transform.position;
        }

        private Vector3 GetPositionFromCurrentWaypointAndDistance(Waypoint point)
        {
            return point.position + (point.next.position - point.position).normalized * this.distanceFromLastWaypoint;
        }
    }
}
