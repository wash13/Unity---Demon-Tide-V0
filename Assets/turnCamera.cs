using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnCamera : MonoBehaviour
{
    public float rotation = 0;
    public GameObject isoCamera;
    private Quaternion originalRot;
    public bool restore = false;

    // Start is called before the first frame update
    void Start()
    {
        originalRot = isoCamera.transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ontrigger worked");
        if (!restore && isoCamera.transform.rotation == originalRot) rotate();
        else if (restore && isoCamera.transform.rotation != originalRot) rotate();
    }

    private void rotate()
    {
        isoCamera.transform.RotateAround(isoCamera.GetComponent<IsometricCamera>().target.transform.position, new Vector3(isoCamera.transform.rotation.x, isoCamera.transform.rotation.y + rotation, isoCamera.transform.rotation.z),
            rotation);
    }
}
