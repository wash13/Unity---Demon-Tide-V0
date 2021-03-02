using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnCamera : MonoBehaviour
{
    public float rotation = 0;
    public IsometricCamera isoCamera;
    private Quaternion originalRot;
    private Vector3 originalPos;
    public bool restore = false;

    // Start is called before the first frame update
    void Start()
    {
        originalRot = isoCamera.transform.rotation;
        originalPos = isoCamera.offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ontrigger worked");
        if (!restore && isoCamera.transform.rotation == originalRot) rotate();
        else if (restore && isoCamera.transform.rotation != originalRot)
        {
            isoCamera.transform.rotation = originalRot;
            isoCamera.offset = originalPos;
        }
    }

    private void rotate()
    {
        Debug.Log(Mathf.Sin(Mathf.Deg2Rad * rotation));
        Debug.Log(isoCamera.offset.z * Mathf.Sin(Mathf.Deg2Rad * rotation));
        Debug.Log(Mathf.Cos(Mathf.Deg2Rad * rotation));
        Debug.Log(isoCamera.offset.z* Mathf.Cos(Mathf.Deg2Rad * rotation));
        float dx = (isoCamera.offset.x * Mathf.Cos(Mathf.Deg2Rad * rotation)) + (isoCamera.offset.z * Mathf.Sin(Mathf.Deg2Rad * rotation));
        float dz = (isoCamera.offset.x * Mathf.Sin(Mathf.Deg2Rad * rotation)) + (isoCamera.offset.z * Mathf.Cos(Mathf.Deg2Rad * rotation));
        isoCamera.offset.x = dx;
        isoCamera.offset.z = dz;
        isoCamera.transform.Rotate(0, rotation, 0, Space.World);
        //isoCamera.transform.RotateAround(isoCamera.GetComponent<IsometricCamera>().target.transform.position, new Vector3(isoCamera.transform.rotation.x, isoCamera.transform.rotation.y + rotation, isoCamera.transform.rotation.z), rotation);
    }
}
