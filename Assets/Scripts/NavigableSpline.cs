using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.U2D;
using UnityEngine;
using Unity.Collections;
using UnityEditor;
//using System.Numerics;

namespace Assets.Scripts
{
    class NavigableSpline : MonoBehaviour
    {
        public int segmentsPerBezier = 10;
        internal List<Waypoint> waypoints;

        private Spline spriteShapeSpline;

        // Start is called before the first frame update
        void Start()
        {
            var spriteShapeController = this.GetComponent<SpriteShapeController>();
            if (spriteShapeController == null)
            {
                throw new System.Exception("Error: no sprite shape renderer on GameObject");
            }
            this.spriteShapeSpline = spriteShapeController.spline;

            this.waypoints = this.GenerateWaypointsFromSpline(this.spriteShapeSpline);
        }

        // Update is called once per frame
        void Update()
        {
        }

        private List<Waypoint> GenerateWaypointsFromSpline(Spline spline)
        {
            List<Vector3> vectorWaypoints = new List<Vector3>();

            var numberOfPoints = spline.GetPointCount();
            for (var i = 0; i < numberOfPoints; i++)
            {
                var position = spline.GetPosition(i);
                vectorWaypoints.Add(position);
                int nextPointIndex = i + 1;
                if (i + 1 >= numberOfPoints)
                {
                    if (spline.isOpenEnded)
                    {
                        continue;
                    }
                    nextPointIndex = 0;
                }
                var nextPosition = spline.GetPosition(nextPointIndex);

                var path = Handles.MakeBezierPoints(
                    position,
                    nextPosition,
                    position + spline.GetRightTangent(i),
                    nextPosition + spline.GetLeftTangent(nextPointIndex),
                    this.segmentsPerBezier);

                vectorWaypoints.AddRange(path);
            }

            var waypoints = vectorWaypoints
                .Select(v => this.transform.TransformPoint(v))
                .Select(v => new Waypoint(v)).ToList();

            for (var i = 0; i < waypoints.Count - 1; i++)
            {
                waypoints[i].SetNext(waypoints[i + 1]);
            }

            if (!spline.isOpenEnded)
            {
                waypoints.Last().SetNext(waypoints.First());
            }

            return waypoints;
        }

        private System.Numerics.Vector2 ConvertToNumericVector2(Vector3 vect)
        {
            return new System.Numerics.Vector2(vect.x, vect.y);
        }

        private IEnumerable<Vector3> GetCoordsForBezier(Vector3 start, Vector3 end, Vector3 startTan, Vector3 endTan)
        {
            var inputBezier = new System.Geometry.Bezier(
                    ConvertToNumericVector2(start),
                    ConvertToNumericVector2(startTan),
                    ConvertToNumericVector2(endTan),
                    ConvertToNumericVector2(end));
            //TODO: implement offset??
            return null;
        }

        public void OnDrawGizmosSelected()
        {
            var spriteShapeControl = this.GetComponent<SpriteShapeController>();
            this.RenderWaypointGizmos(spriteShapeControl.spline);
        }

        private void RenderWaypointGizmos(Spline spline)
        {
            Gizmos.color = Color.red;
            var wayPoints = this.GenerateWaypointsFromSpline(spline);

            Vector3 lastPoint = Vector3.zero;
            foreach (var wayPoint in wayPoints)
            {
                var point = wayPoint.position;
                if (lastPoint != Vector3.zero)
                {
                    Gizmos.DrawLine(point, lastPoint);
                    Gizmos.DrawWireSphere(point, 0.05f);
                }
                lastPoint = point;
            }

        }
    }
}
