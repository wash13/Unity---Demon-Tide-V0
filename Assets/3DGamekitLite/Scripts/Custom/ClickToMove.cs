using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
    public LayerMask clickable;
    private NavMeshAgent myAgent;
    
    // Start is called before the first frame update
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent> ();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown (0))
        {
            Ray movePoint = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast (movePoint, out hitInfo, 100, clickable))
            {
                myAgent.SetDestination (hitInfo.point);
            }
        }
    }
}
