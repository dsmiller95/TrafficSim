using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    class Waypoint
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
    }
}