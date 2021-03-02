using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class levelDamageVal : MonoBehaviour
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
        t.text = "" + source.damage;
    }

    public void upDam()
    {
        if (!toggle)
        {
            source.damage += 10;
            toggle = true;
        }
        
    }

    public void off()
    {
        if (toggle)
        {
            source.damage -= 10;
            toggle = false;
        }
    }
}
