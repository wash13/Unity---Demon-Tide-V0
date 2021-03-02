using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class limitClose : MonoBehaviour
{
    int count = 0;
    public UnityEvent action;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (count > 0) action.Invoke();
        else count++;
    }
}
