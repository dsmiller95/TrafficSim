using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.U2D;

namespace Assets.Scripts
{
    /// <summary>
    /// Defines a strategy to break a bezier spline down into a series of wapoints
    /// </summary>
    public interface ISplineNavigationStrategy
    {
        /// <summary>
        /// Break a spline into a list of seqential waypoints, with an optional offset
        /// </summary>
        /// <param name="spline">The Spline consisting of a series of bezier cures. Either open or closed</param>
        /// <param name="offset">Distance to offset the bezier curve as a offset curve</param>
        /// <param name="segmentsPerBezier">The resultion, or number of waypoints generated per each curve</param>
        /// <returns></returns>
        List<Waypoint> GetWaypointsFromSplineAtOffset(Spline spline, float offset = 0, int segmentsPerBezier = 10);
    }
}
