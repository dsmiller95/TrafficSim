using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.U2D;

namespace Assets.Scripts
{
    public interface ISplineNavigationStrategy
    {
        List<Waypoint> GetWaypointsFromSplineAtOffset(Spline spline, float offset, int segmentsPerBezier = 10);
    }
}
