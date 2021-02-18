using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes : MonoBehaviour
{
    public string lorename;

    // Start is called before the first frame update
    void Start()
    {
        if (lorename.Equals(""))
        {
            lorename = gameObject.name;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
