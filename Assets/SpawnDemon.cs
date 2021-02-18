using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDemon : MonoBehaviour
{
    public float spawntime = 8f;
    public float variance = 3f;
    public GameObject spawns;
    public int maxActive = 5;
    private int numSpawned = 0;


    private float lastspawn = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > (lastspawn + spawntime) && maxActive >= numSpawned)
        {
            GameObject spawnedDemon = Instantiate(spawns, transform.position, transform.rotation);
            lastspawn = Time.time;
            numSpawned += 1;
        }
    }
}
