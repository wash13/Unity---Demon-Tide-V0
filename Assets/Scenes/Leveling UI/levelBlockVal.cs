using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamekit3D
{
    public class levelBlockVal : MonoBehaviour
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
            t.text = "" + source.blockAngle;
        }

        public void upBlk()
        {
            if (!toggle)
            {
                source.blockAngle += 10f;
                toggle = true;
            }
            
        }

        public void off()
        {
            if (toggle)
            {
                source.blockAngle -= 10f;
                toggle = false;
            }
        }
    }


}