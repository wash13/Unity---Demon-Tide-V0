using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamekit3D
{
    public class StatScaling : MonoBehaviour
    {
        private Damageable unit;
        //the increments by which modifier increases
        public float healthScale = .2f;
        public float damageScale = .2f;
        public float speedScale = .02f;
        public float attackScale = .02f;
        public float xpScale = .08f;
        //current modifier
        public float healthMod = 1f;
        public float damageMod = 1f;
        public float speedMod = 1f;
        public float attackMod = 1f;
        public float xpMod = 1f;

        // Start is called before the first frame update
        void Start()
        {
            unit = gameObject.GetComponent<Damageable>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void scale()
        {
            healthMod += healthScale;
            damageMod += damageScale;
            speedMod += speedScale;
            attackMod += attackScale;
            xpMod += xpScale;
        }

        public void resetScale()
        {
            healthMod = 1;
            damageMod = 1;
            speedMod = 1;
            attackMod = 1;
            xpMod = 1;
        }
    }
}