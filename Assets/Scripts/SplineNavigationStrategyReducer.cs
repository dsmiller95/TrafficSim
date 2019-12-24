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
    class SplineNavigationStrategyReducer : ISplineNavigationStrategy
    {
        private Spline spriteShapeSpline;

        public List<Waypoint> GetWaypointsFromSplineAtOffset(Spline spline, float offset, int segmentsPerBezier)
        {
            List<Vector3> vectorWaypoints = new List<Vector3>();

            var numberOfPoints = spline.GetPointCount();
            for (var i = 0; i < numberOfPoints; i++)
            {
                var position = spline.GetPosition(i);
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

                var path = this.GetInnerCoordsForOffsetBezier(
                    position,
                    nextPosition,
                    position + spline.GetRightTangent(i),
                    nextPosition + spline.GetLeftTangent(nextPointIndex),
                    offset,
                    segmentsPerBezier);

                vectorWaypoints.AddRange(path);
            }

            var waypoints = vectorWaypoints
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

        private IEnumerable<Vector3> GetInnerCoordsForOffsetBezier(Vector3 start, Vector3 end, Vector3 startTan, Vector3 endTan, float offsetDist, int resolution)
        {
            var inputBezier = new Bezier(
                    ConvertToNumericVector2(start),
                    ConvertToNumericVector2(startTan),
                    ConvertToNumericVector2(endTan),
                    ConvertToNumericVector2(end));

            var offsetBezier = inputBezier.Offset(offsetDist);
            if(offsetBezier == null || offsetBezier.Count <= 0)
            {
                offsetBezier = new List<Bezier> { inputBezier };
            }

            var workingPath = new List<Vector3>();
            var segmentPerSubBezier = resolution / offsetBezier.Count;
            foreach(var bez in offsetBezier)
            {
                workingPath.AddRange(this.GetInnerCoordsFromBezier(bez, segmentPerSubBezier).Select(v => new Vector3(v.X, v.Y)));
            }
            return workingPath;
        }

        private IEnumerable<System.Numerics.Vector2> GetInnerCoordsFromBezier(Bezier bez, int numSegments)
        {
            float stepPerSegment = 1f / numSegments;
            for(var i = 0; i < numSegments; i++)
            {
                yield return bez.Position(stepPerSegment * i);
            }
        }
    }
}
