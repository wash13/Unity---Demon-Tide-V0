using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setStealth : MonoBehaviour
{
    public Material normalMaterial;
    public Material stealthMaterial;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Stealth()
    {
        if (stealthMaterial != null) gameObject.GetComponent<SkinnedMeshRenderer> ().material= stealthMaterial;
    }

    public void Unstealth()
    {
        if (normalMaterial != null) gameObject.GetComponent<SkinnedMeshRenderer>().material = normalMaterial;
    }
}
