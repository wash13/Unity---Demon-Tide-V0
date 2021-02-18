using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D.Message;

namespace Gamekit3D
{
    public class HolyBolt : MonoBehaviour
    {
        private MeleeWeapon wep;
        private Vector3 origin;
        public float distance = 100f;


        // Start is called before the first frame update
        void Awake()
        {
            origin = transform.position;
            wep = GetComponent<MeleeWeapon>();
            wep.SetOwner(gameObject);
            wep.BeginAttack(true);
        }

    
        // Update is called once per frame
        void Update()
        {
            if (Vector3.Distance(origin, transform.position) > distance )
            {
                wep.EndAttack();
                Destroy(gameObject);
            }
        }
    }
}