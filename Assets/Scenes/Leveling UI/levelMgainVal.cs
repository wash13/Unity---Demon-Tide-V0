using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class levelMgainVal : MonoBehaviour
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
        t.text = "" + source.manaGain;
    }

    public void upGain()
    {
        if (!toggle)
        {
            source.manaGain += 1;
            toggle = true;
        }
        else
        {
            source.manaGain -= 1;
            toggle = false;
        }
    }

    public void off()
    {
        if (toggle)
        {
            source.manaGain -= 1;
            toggle = false;
        }
    }
}
