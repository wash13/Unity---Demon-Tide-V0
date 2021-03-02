using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Gamekit3D
{
    public class bossUpgrade : MonoBehaviour
    {
        public float healthMod = 1;
        public float damageMod = 1;
        public float atkSpdMod = 1;
        public float staggerMod = 1;
        public float stunMod = 1;
        public float moveSpdMod = 1;
        public float rangeMod = 1;
        public Vector3 scale;


        // Start is called before the first frame update
        void Start()
        {
            transform.localScale = scale;
            gameObject.GetComponent<Damageable>().maxHitPoints = Mathf.RoundToInt(gameObject.GetComponent<Damageable>().maxHitPoints * healthMod);
            gameObject.GetComponent<Damageable>().ResetDamage();
            gameObject.GetComponentInChildren<MeleeWeapon>().damage = Mathf.RoundToInt(gameObject.GetComponentInChildren<MeleeWeapon>().damage * damageMod);
            gameObject.GetComponent<Animator>().SetFloat("speedMod", atkSpdMod);
            gameObject.GetComponent<Animator>().SetFloat("staggerMod", staggerMod);
            gameObject.GetComponent<Animator>().SetFloat("stunMod", stunMod);
            gameObject.GetComponent<NavMeshAgent>().speed *= moveSpdMod;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}