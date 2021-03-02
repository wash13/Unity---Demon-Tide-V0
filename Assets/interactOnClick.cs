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
        if (close && Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100, 1 << 13);
            if (hitInfo.collider.name == this.name)   OnClick.Invoke();
        }
    }


    


    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0) && close)
        { // if left button pressed...
            //OnClick.Invoke();
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
