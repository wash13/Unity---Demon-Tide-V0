using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class levelHealthVal : MonoBehaviour
{
    playerStats source;
    Text t;
    bool toggle;

    // Start is called before the first frame update
    void Start()
    {
        source = FindObjectOfType<playerStats>();
        t = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        t.text = "" + source.hp;
    }

    public void upHp()
    {
        if (!toggle)
        {
            source.hp += 5;
            toggle = true;
        }
        else
        {
            source.hp -= 5;
            toggle = false;
        }
    }

    public void off()
    {
        if (toggle)
        {
            source.hp -= 5;
            toggle = false;
        }
    }
}
