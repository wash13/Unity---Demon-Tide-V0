using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InteractableGlow : MonoBehaviour
{
    private Color original;
    Material material;

    public GameObject displayGroup;
    private GameObject displayName;
    private GameObject displayBar;
    private Attributes abt;

    private Gamekit3D.Damageable health;

    private void OnMouseOver()
    {
        //Debug.Log("entered");
        //original = material.color;
        //material.color = Color.yellow;
        //material.EnableKeyword("_EMISSION");
        displayGroup.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        //Debug.Log(temp);
        if (health != null) displayGroup.GetComponent<Slider>().value = ((float)health.currentHitPoints / (float)health.maxHitPoints);
        displayName.GetComponent<Text>().text = abt.lorename;
        displayGroup.GetComponent<targetReference>().target = this.gameObject;
        
    }

    /*
    private void OnMouseOver()
    {
        if (health != null) displayGroup.GetComponent<Slider>().value = ((float)health.currentHitPoints / (float)health.maxHitPoints);
        displayGroup.GetComponent<targetReference>().target = this;
    }
    */

    private void OnDestroy()
    {
        if (displayGroup != null)
        {
            displayGroup.GetComponent<Slider>().value = 0;
            displayGroup.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            displayGroup.GetComponent<targetReference>().target = null;
        }
    }

    
    private void OnMouseExit()
    {

        Debug.Log("exited");
        //material.color = original;
        material.DisableKeyword("_EMISSION");
        //displayName.GetComponent<Text>().text = "";
        displayGroup.GetComponent<Slider>().value = 0;
        displayGroup.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        displayGroup.GetComponent<targetReference>().target = null;
    }
    

    // Start is called before the first frame update
    void Start()
    {
        Physics.queriesHitTriggers = true;
        displayName = GameObject.Find("TargetText");
        //displayBar = GameObject.Find("front bar");
        displayGroup = GameObject.Find("target bar");
        abt = GetComponent<Attributes>();
        material = GetComponent<Renderer>().material;
        health = GetComponentInParent<Gamekit3D.Damageable>();
        if (health == null) health = GetComponent<Gamekit3D.Damageable>();
        //Debug.Log("got health " + health);
        //Debug.Log("got group " + displayGroup);
       
    }

    private void Update()
    {

    }
}
