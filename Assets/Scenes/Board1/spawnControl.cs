using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class spawnControl : MonoBehaviour
    {
        public GameObject lesserDemon;
        public GameObject scytheDemon;
        public GameObject shadowDemon;
        public GameObject hellBones;

        public float interval = 30;
        public int simultaneous = 2;
        private SpawnDemon[] workers;
        public bool working = true;

        // Start is called before the first frame update
        void Start()
        {
            workers = GetComponentsInChildren<SpawnDemon>();
            Debug.Log(workers[0]);
            //lesserDemon = Resources.Load("Important/lesser demon") as GameObject;
            //StartCoroutine(spawn());
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public IEnumerator spawn()
        {
            while (working)
            {
                for (int i = 0; i < simultaneous; i++)
                {
                    Debug.Log("one call to control spawn");
                    StartCoroutine(workers[Random.Range(0, workers.Length)].spawnGroup(lesserDemon, 5, 2f, .5f));
                }
                yield return new WaitForSeconds(interval);
            }
        }

        public void stop()
        {
            working = false;
            foreach (SpawnDemon worker in workers)
            {
                worker.stop = true;
            
            }
        }
    }
}