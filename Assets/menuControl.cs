using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuControl : MonoBehaviour
{
    bool toggle = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!toggle)
            {
                Time.timeScale = 0f;
                toggle = !toggle;
            }
            else
            {
                Time.timeScale = 1f;
                toggle = !toggle;
            }
        
        }
    }

    
}
