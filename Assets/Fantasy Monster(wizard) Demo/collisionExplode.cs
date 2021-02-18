using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionExplode : MonoBehaviour
{
    private Gamekit3D.MeleeWeapon explosion;
    public ParticleSystem part1;
    public ParticleSystem part2;
    public AudioSource burning;
    public Gamekit3D.RandomAudioPlayer explodeSound;
    private GameObject targetcircle;

    private void Start()
    {
        explosion = gameObject.GetComponent<Gamekit3D.MeleeWeapon>();
        explosion.SetOwner(gameObject);
    }

    public void setTarget(GameObject theTarget)
    {
        targetcircle = theTarget;
    }


    private IEnumerator OnTriggerEnter(Collider target)
    {
        Debug.Log("fireball hit " + target);
        if (target.gameObject.layer != 23 && target.gameObject.layer != 0)
        {
            gameObject.GetComponent<SphereCollider>().enabled = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<Gamekit3D.MeleeWeapon>().EndAttack();
            burning.enabled = false;
            Debug.Log("happened");
            part1.transform.parent = null;
            part2.transform.parent = null;
            part1.Play();
            part2.Play();
            if (explodeSound != null) explodeSound.PlayRandomClip();

            explosion.BeginAttack(false);
            Destroy(targetcircle.gameObject);
            yield return new WaitForSeconds(.02f);
            explosion.EndAttack();
            yield return new WaitForSeconds(2f);
            Destroy(part1.gameObject);
            Destroy(part2.gameObject);
            Destroy(gameObject);
            //explosion.EndAttack();
        }

    }

   
  


}
