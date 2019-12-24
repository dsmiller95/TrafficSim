using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.U2D;
using UnityEngine;
using Unity.Collections;
using UnityEditor;
using System.Geometry;
//using System.Numerics;

namespace Assets.Scripts
{
    class NavigableSpline : MonoBehaviour
    {
        public int segmentsPerBezier = 10;
        public float offsetDistance;
        public int numOffset = 1;

        public ISplineNavigationStrategy navigationStrategy = new SplineNavigationStrategyReducer();

        internal List<List<Waypoint>> navigableRoutes;

        // Start is called before the first frame update
        void Start()
        {
            var spriteShapeController = this.GetComponent<SpriteShapeController>();
            if (spriteShapeController == null)
            {
                throw new System.Exception("Error: no sprite shape renderer on GameObject");
            }
            this.SetWaypoints(spriteShapeController.spline);
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void SetWaypoints(Spline spline)
        {
            float startOffset = (numOffset - 1)  * - (this.offsetDistance / 2);

            this.navigableRoutes = new List<List<Waypoint>>();
            for(var offsetNum = 0; offsetNum < numOffset; offsetNum++)
            {
                var waypoints = this.navigationStrategy.GetWaypointsFromSplineAtOffset(spline, startOffset + offsetNum * this.offsetDistance, this.segmentsPerBezier).ToList();
                waypoints.ForEach(waypoint => waypoint.PlaceSelfInsideTransform(this.transform));
                this.navigableRoutes.Add(waypoints);
            }
        }

        public void OnDrawGizmosSelected()
        {
            var spriteShapeControl = this.GetComponent<SpriteShapeController>();
            this.RenderWaypointGizmos(spriteShapeControl.spline);
        }

        private void RenderWaypointGizmos(Spline spline)
        {
            Gizmos.color = Color.red;
            this.SetWaypoints(spline);

            foreach (var navigableRoute in this.navigableRoutes)
            {
                Vector3 lastPoint = Vector3.zero;
                foreach (var wayPoint in navigableRoute)
                {
                    var point = wayPoint.position;
                    if (lastPoint != Vector3.zero)
                    {
                        Gizmos.DrawLine(point, lastPoint);
                        Gizmos.DrawWireSphere(point, 0.05f);
                    }
                    lastPoint = point;
                }
                if (!spline.isOpenEnded)
                {
                    var point = navigableRoute[0].position;
                    Gizmos.DrawLine(point, lastPoint);
                    Gizmos.DrawWireSphere(point, 0.05f);
                }
            }
        }
    }
}
