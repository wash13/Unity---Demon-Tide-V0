using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDemon : MonoBehaviour
{
    //public float spawntime = 8f;
    //public float variance = 3f;
    //public GameObject spawns;
    //public int maxActive = 5;
    //private int numSpawned = 0;
    public bool working = false;
    private ParticleSystem portal;
    public bool stop = false;

    //private float lastspawn = 0;

    // Start is called before the first frame update
    void Start()
    {
        portal = GetComponentInChildren<ParticleSystem>();
        portal.Stop();
    }


    public IEnumerator spawnGroup (GameObject demon, int max, float interval, float timeVariance)
    {
        GetComponentInChildren<ParticleSystem>().Play();
        //Debug.Log("one worker activated");
        working = true;
        for (int i = 0; i < max; i++)
        {
            if (stop) break;
            GameObject spawnedDemon = Instantiate(demon, transform.position, transform.rotation);
            spawnedDemon.transform.SetParent(transform);
            yield return new WaitForSeconds(interval + Random.Range(-timeVariance, timeVariance));
        }
        working = false;
        GetComponentInChildren<ParticleSystem>().Stop();
    }
}
