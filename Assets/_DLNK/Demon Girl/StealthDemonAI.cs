using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Gamekit3D.Message;

namespace Gamekit3D
{
    public class StealthDemonAI : MonoBehaviour, IMessageReceiver
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
        private bool attacking = false;
        private bool retreating = false;
        private Vector3 retreatFrom;
        //private Vector3 retreatTo;
        private bool stealth = true;

        //sound
        public RandomAudioPlayer attackAudio;
        public RandomAudioPlayer weaponAudio;
        public RandomAudioPlayer backstabAudio;
        public RandomAudioPlayer hitAudio;
        public RandomAudioPlayer laughAudio;
        public RandomAudioPlayer deathAudio;
        public RandomAudioPlayer screamAudio;
        private GameObject target;

        public GameObject ragdollPrefab;
        public PlayerControlAlt player;
        public int xpWorth = 8;
        public float backstabMultiplier = 2f;
        private int baseDamage;

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
            baseDamage = weapon.damage;
            retreatFrom = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (alive) FindTarget();
        }


        public void FindTarget()
        {
           
           target = playerScanner.Detect(transform, "Player", m_Target == null);
            if (target == null)
            {
                myAgent.SetDestination(retreatFrom);
                retreating = false;
                attacking = false;
                stealth = true;
                goStealth();
                anim.SetInteger("attacking", 0);
                anim.SetInteger("moving", 1);
                myAgent.speed = 4;

            }
            //Debug.Log("target is " + target);
            //if (target != null) Debug.Log("distance is " + Vector3.Distance(transform.position, target.transform.position));

            if (attacking)
            {
                //always rotate to target
                Quaternion targetAngle = Quaternion.LookRotation((target.transform.position - transform.position).normalized);
                Quaternion rotat = Quaternion.RotateTowards(transform.rotation, targetAngle, 180 * Time.deltaTime);
                transform.rotation = rotat;
            }

            //first check if retreating
            
            if (target != null && retreating)
            {
                anim.SetInteger("attacking", 0);
                attacking = false;
                Debug.Log("running");
                myAgent.SetDestination(transform.position - target.transform.position);
                anim.SetInteger("moving", 2);
                myAgent.speed = 8;
            }
           

            //cases for movement
           else if (target != null && Vector3.Distance(transform.position, target.transform.position) > 1.5 && !attacking)
            {
                //stealth, move to target.  stealth and retreat impossible
                if (stealth)
                {
                    //Debug.Log(target.transform.position);
                    myAgent.SetDestination(target.transform.position);
                    anim.SetInteger("moving", 1);
                    myAgent.speed = 4;
                }

                //case for out of stealth and not retreating
                else if (!stealth && !retreating)
                {
                    //Debug.Log(target.transform.position);
                    myAgent.SetDestination(target.transform.position);
                    anim.SetInteger("moving", 2);
                    myAgent.speed = 8;
                }

                //last possible case //wasn't working when invoked while in range
                /*
                else
                {
                    myAgent.SetDestination(transform.position - target.transform.position);
                    anim.SetInteger("moving", 2);
                    myAgent.speed = 8;
                }
                */
            }

            //cases for attacking 
            else if (target != null && !attacking && Vector3.Distance(transform.position, target.transform.position) <= 1.5)
            {
                //always rotate to target
                Quaternion targetAngle = Quaternion.LookRotation((target.transform.position - transform.position).normalized);
                Quaternion rotat = Quaternion.RotateTowards(transform.rotation, targetAngle, 180 * Time.deltaTime);
                transform.rotation = rotat;

                Debug.Log("attack decision is " + attacking + " " + stealth);
                //first attack out of stealth
                if (stealth)
                {
                    if (myAgent.velocity != Vector3.zero) myAgent.SetDestination(transform.position);

                    attacking = true;
                    stealth = false;
                    unStealth();
                    anim.SetInteger("attacking", 1);
                    anim.SetInteger("moving", 0);
                    weapon.damage = Mathf.CeilToInt(baseDamage * backstabMultiplier);
                    weapon.hitAudio = backstabAudio;
                    if (Random.Range(1, 3) == 1) laughAudio.PlayRandomClip();
                }

                //any other attack
                else
                {
                    if (myAgent.velocity != Vector3.zero) myAgent.SetDestination(transform.position);
                    Debug.Log("should now be attacking 2");

                    weapon.damage = baseDamage;
                    weapon.hitAudio = weaponAudio;
                    anim.SetInteger("moving", 0);
                    anim.SetInteger("attacking", 2);
                    attacking = true;
                    if (Random.Range(1, 4) == 1) attackAudio.PlayRandomClip();
                }
            }

            //target is null, is retreating
            else if (retreating)
            {
                anim.SetInteger("moving", 1);
                retreating = false;
                stealth = true;
                goStealth();
                myAgent.SetDestination(retreatFrom);
            }
            
            //target is null, not retreating
            else
            {
                //Debug.Log("else happened");
                anim.SetInteger("moving", 0);
                //anim.SetInteger("attacking", 0);
                //attacking = false;
            }
          
        }

        public void goStealth()
        {
            setStealth[] workers = GetComponentsInChildren<setStealth>();

            for (int i = 0; i < workers.Length; i++)
            {
                workers[i].Stealth();
            }
        }

        public void unStealth()
        {
            setStealth[] workers = GetComponentsInChildren<setStealth>();

            for (int i = 0; i < workers.Length; i++)
            {
                workers[i].Unstealth();
            }
        }


        public void OnReceiveMessage(Message.MessageType type, object sender, object msg)
        {
            //Debug.Log("message recieved");
            switch (type)
            {
                case Message.MessageType.DEAD:
                    //Death((Damageable.DamageMessage)msg);

                    deathAudio.PlayRandomClip();
                    GameObject ragdollInstance = Instantiate(ragdollPrefab, transform.position, transform.rotation);
                    player.awardXp(xpWorth);
                    Destroy(gameObject);
                    break;
                case Message.MessageType.DAMAGED:
                    //ApplyDamage((Damageable.DamageMessage)msg);
                    //Debug.Log("demon hurt");
                    if (hitAudio != null && Random.Range(1,5) == 1) hitAudio.PlayRandomClip();
                    if (Random.Range(0, 100) > 55)
                    {
                        retreating = true;
                        retreatFrom = transform.position;
                        //retreatTo = transform.position - target.transform.position;
                        screamAudio.PlayRandomClip();
                    }
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
            //Debug.Log("damage ended");
            weapon.EndAttack();
        }

        public void attackEnd()
        {
            //Debug.Log("attack ended");
            attacking = false;
            anim.SetInteger("attacking", 0);
            
        }

        public void grunt()
        {
            //if (gruntAudio != null && Random.Range(0f, 1f) > .9f) gruntAudio.PlayRandomClip();
        }

        public void step()
        {
            //if (stepAudio != null) stepAudio.Play();
        }

        public void stagger()
        {
            attacking = false;
            anim.SetBool("staggered", true);
            anim.SetInteger("moving", 0);
            anim.SetInteger("attacking", 0);
            //Debug.Log("demon state is  " + anim.GetInteger("moving"));
            weapon.EndAttack();
        }

        public void setStagger()
        {
            anim.SetBool("staggered", false);
        }

        public void staggerEnd()
        {
            anim.SetBool("staggered", false);
            attacking = false;
            anim.SetInteger("moving", 0);
            anim.SetInteger("attacking", 0);
        }
    }
}
