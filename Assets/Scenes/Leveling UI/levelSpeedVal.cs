using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class levelSpeedVal : MonoBehaviour
{
    playerStats source;
    Text t;
    bool toggle = false;

    // Start is called before the first frame update
    void Start()
    {
        source = FindObjectOfType<playerStats>();
        t = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        t.text = "" + source.speed;
    }

    public void upSpd()
    {
        if (!toggle)
        {
            source.speed += .2f;
            toggle = true;
        }
        else
        {
            source.speed -= .2f;
            toggle = false;
        }
    }

    public void off()
    {
        if(toggle)
        {
            source.speed -= .2f;
            toggle = false;
        }
    }
}
