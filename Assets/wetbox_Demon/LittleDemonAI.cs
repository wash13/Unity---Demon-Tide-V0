using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Gamekit3D.Message;

namespace Gamekit3D
{
    public class LittleDemonAI : MonoBehaviour, IMessageReceiver
    {
        public EnemyController controller { get { return m_Controller; } }
        //public PlayerController target { get { return m_Target; } }

        public TargetScannerAlt playerScanner;
        protected PlayerController m_Target = null;
        protected EnemyController m_Controller;
        private Vector3 origin;
        private NavMeshAgent myAgent;
        private Animator anim;
        private bool alive = true;
        private MeleeWeapon weapon;
        bool attacking = false;
        bool inStagger = false;

        //sound
        public RandomAudioPlayer attackAudio;
        public RandomAudioPlayer weaponAudio;
        public RandomAudioPlayer headbuttAudio;
        public RandomAudioPlayer hitAudio;
        public RandomAudioPlayer gruntAudio;
        public RandomAudioPlayer deathAudio;
        public RandomAudioPlayer spottedAudio;
        private GameObject target;

        public GameObject ragdollPrefab;
        public PlayerControlAlt player;
        public int xpWorth = 4;

        //private float range;


        // Start is called before the first frame update
        void Start()
        {
            origin = transform.position;
            myAgent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            //Debug.Log("got my agent " + myAgent);
            //anim.SetInteger("battle", 1);
            weapon = GetComponentInChildren<MeleeWeapon>();
            weapon.SetOwner(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            if (alive) FindTarget();
        }


        public void FindTarget()
        {
            //we ignore height difference if the target was already seen
            target = playerScanner.Detect(transform, "Player", m_Target == null);
            //Debug.Log("target is " + target);
            //if (target != null) Debug.Log("distance is " + Vector3.Distance(transform.position, target.transform.position));
            if (target != null && Vector3.Distance(transform.position, target.transform.position) > 1.5 && !attacking && !inStagger)
            {
                //Debug.Log(target.transform.position);
                myAgent.SetDestination(target.transform.position);
                anim.SetFloat("move", 1);

            }
            else if (target != null && !inStagger)
            {
                Quaternion targetAngle = Quaternion.LookRotation((target.transform.position - transform.position).normalized);
                Quaternion rotat = Quaternion.RotateTowards(transform.rotation, targetAngle, 180 * Time.deltaTime);
                transform.rotation = rotat;
                //anim.SetFloat("turn", rotat);
                if (!attacking && Vector3.Distance(transform.position, target.transform.position) < 1.5)
                {
                     if (myAgent.velocity != Vector3.zero) myAgent.SetDestination(transform.position);
                    anim.SetFloat("move", 0);

                    int rand = Random.Range(3, 10);
                    if (Random.Range(1, 5) == 1) attackAudio.PlayRandomClip();

                    anim.SetInteger("attacking", rand);
                   // Debug.Log("set attack" + rand);
                    attacking = true;
                }
            }
            else anim.SetFloat("move", 0);
          
        }

        public void OnReceiveMessage(Message.MessageType type, object sender, object msg)
        {
            //Debug.Log("message recieved");
            switch (type)
            {
                case Message.MessageType.DEAD:
                    //Death((Damageable.DamageMessage)msg);
                    GameObject ragdollInstance = Instantiate(ragdollPrefab, transform.position, transform.rotation);
                    player.awardXp(xpWorth);
                    Destroy(gameObject);
                    break;
                case Message.MessageType.DAMAGED:
                    //ApplyDamage((Damageable.DamageMessage)msg);
                    //Debug.Log("demon hurt");
                    if (hitAudio != null && Random.Range(1,3) == 1) hitAudio.PlayRandomClip();
                    break;
                default:
                    break;
            }
        }

      

        public void attack()
        {
            //Debug.Log("damage began");
            weapon.BeginAttack(false);
        }

        public void damageEnd()
        {
            Debug.Log("damage ended");
            weapon.EndAttack();

            //reduce attack count
            //check if player is in range
            //if atk count run out or player out of range
            //   attacking is false
            anim.SetInteger("attacking", (anim.GetInteger("attacking") - 3));
            if (anim.GetInteger("attacking") == 1) weapon.hitAudio = headbuttAudio;
            else weapon.hitAudio = weaponAudio;
            if (target != null && Vector3.Distance(transform.position, target.transform.position) > 1.5)
            {
                //attacking = false;
                anim.SetInteger("attacking", 0);
            }
        }

     
        public void attackEnd()
        {
            //Debug.Log("attack ended");

            //reached the end of the attack string
            if (anim.GetInteger("attacking") <= 0) attacking = false;
        }

        public void grunt()
        {
            if (gruntAudio != null && Random.Range(0f, 1f) > .9f) gruntAudio.PlayRandomClip();
        }

        public void step()
        {
            //if (stepAudio != null) stepAudio.Play();
        }

        public void stagger()
        {
            attacking = false;
            inStagger = true;
            anim.SetBool("stagger", true);
            anim.SetInteger("attacking", 0);
            //Debug.Log("demon state is  " + anim.GetInteger("moving"));
            weapon.EndAttack();
        }

        public void staggering()
        {
            anim.SetBool("stagger", false);
        }

        public void endStagger()
        {
            Debug.Log("ended stagger");
            inStagger = false;
            weapon.EndAttack();
        }
    }
}
