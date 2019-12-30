using Assets.Scripts.ScriptableBuilder.ScriptLinking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class CarSpawner : MonoBehaviour
    {
        public GameObject targetRoad;
        public GameObject spawnTarget;
        public GameObject behaviorFactory;
        public KeyCode spawnKey;
        public bool shouldAutoSpawn;
        public List<int> navigableRouteIndexes = new List<int>(0);
        public float spawnSpeed = 4;
        public bool spawnDirection;

        public float avgSpawnInterval = 1;


        // Start is called before the first frame update
        void Start()
        {
            //SpawnObject();
        }

        private void SpawnObject()
        {
            // Instantiate at position (0, 0, 0) and zero rotation.
            var spawned = Instantiate(this.spawnTarget, new Vector3(0, 0, -10000), Quaternion.identity);
            var navigator = spawned.GetComponent<SplineNavigator>();
            navigator.spriteShape = targetRoad;
            navigator.navigableRouteIndex = this.navigableRouteIndexes[Random.Range(0, this.navigableRouteIndexes.Count)];

            var carMover = spawned.GetComponent<CarMovement>();
            carMover.SetForwardVelocity(this.spawnSpeed);
            carMover.SetDirection(this.spawnDirection);

            var rigidbody = spawned.GetComponent<Rigidbody2D>();
            rigidbody.isKinematic = false;

            var behaviorFactoryElement = this.behaviorFactory.GetComponent<ScriptableObjectContainerController>();
            behaviorFactoryElement.AttachCarBehaviorFromScriptField(spawned);
        }

        private float timeSinceLastSpawn = 0;
        // Update is called once per frame
        void Update()
        {
            if (shouldAutoSpawn)
            {
                timeSinceLastSpawn += Time.deltaTime;
                if (timeSinceLastSpawn > this.avgSpawnInterval)
                {
                    this.timeSinceLastSpawn = 0;
                    this.SpawnObject();
                }
            }else
            {
                if (Input.GetKeyDown(this.spawnKey))
                {
                    this.SpawnObject();
                }
            }
        }
    }
}
