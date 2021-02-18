using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTargetText : MonoBehaviour
{
    public GameObject display;
    // Start is called before the first frame update
    void Start()
    {
        display = GameObject.Find("TargetCanvas");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
