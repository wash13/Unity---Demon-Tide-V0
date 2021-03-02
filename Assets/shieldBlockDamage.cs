using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class shieldBlockDamage : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<MeleeWeapon>().SetOwner(gameObject);
        }

        private void Awake()
        {
            StartCoroutine(active());
        }

        private IEnumerator active()
        {
            GetComponent<MeleeWeapon>().BeginAttack(false);
            yield return new WaitForSeconds(.4f);
            GetComponent<MeleeWeapon>().EndAttack();
            Destroy(gameObject);
        }
    }
}