using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class interactOnClick : MonoBehaviour
{
    bool close = false;
    public UnityEvent OnClick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0) && close)
        { // if left button pressed...
            OnClick.Invoke();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("close");
        close = true;
    }

    private void OnTriggerExit(Collider collision)
    {
        close = false;
    }
}
