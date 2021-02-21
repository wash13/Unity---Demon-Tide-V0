using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private void Awake()
    {
        target = FindObjectOfType<Gamekit3D.PlayerControlAlt>().transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position + offset;
    }
}
