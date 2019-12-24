using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Waypoint
    {
        public Vector3 position;
        public Waypoint previous;
        private float prevDist;
        public Waypoint next;
        private float nextDist;

        public Waypoint(Vector3 position)
        {
            this.position = position;
        }

        public Waypoint(System.Numerics.Vector2 numericsPosition)
        {
            this.position = new Vector3(numericsPosition.X, numericsPosition.Y);
        }

        public void SetNext(Waypoint next)
        {
            var dist = Vector3.Distance(this.position, next.position);
            this.nextDist = dist;
            this.next = next;

            next.prevDist = dist;
            next.previous = this;
        }

        public float GetDistanceToNext()
        {
            return this.nextDist;
        }

        public float GetDistanceToPrev()
        {
            return this.prevDist;
        }

        public void PlaceSelfInsideTransform(Transform transform)
        {
            this.position = transform.TransformPoint(this.position);
        }
    }
}