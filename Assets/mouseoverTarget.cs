using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mouseoverTarget : MonoBehaviour
{
    public GameObject displayGroup;
    private GameObject displayName;
    private GameObject displayBar;
    private Attributes abt;
    private Gamekit3D.Damageable health;

    // Start is called before the first frame update
    void Start()
    {
        Physics.queriesHitTriggers = true;
        displayName = GameObject.Find("TargetText");
        displayGroup = GameObject.Find("target bar");
        abt = GetComponent<Attributes>();
        //Debug.Log("got health " + health);
        //Debug.Log("got group " + displayGroup);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100, 1 << 13))
        {
            //Debug.Log("mouse is over object " + hitInfo.collider.name);
            if (health == null) health = hitInfo.collider.GetComponentInParent<Gamekit3D.Damageable>();
            if (abt == null) abt = hitInfo.collider.GetComponent<Attributes>();
            displayGroup.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            if (health != null) displayGroup.GetComponent<Slider>().value = ((float)health.currentHitPoints / (float)health.maxHitPoints);
            displayName.GetComponent<Text>().text = abt.lorename;
            displayGroup.GetComponent<targetReference>().target = hitInfo.collider.gameObject;
        }
        else
        {
            health = null;
            abt = null;
            displayGroup.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            displayGroup.GetComponent<targetReference>().target = null;
        }
    }
}
