using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Gamekit3D.Message;

namespace Gamekit3D
{
    public class DemonAI : MonoBehaviour, IMessageReceiver
    {
        public EnemyController controller { get { return m_Controller; } }
        //public PlayerController target { get { return m_Target; } }
        //scale stuff
        private float dmgScale = 1f;
        private float hpScale = 1f;
        private float atkScale = 1f;
        private float spdScale = 1f;


        public TargetScannerAlt playerScanner;
        protected PlayerController m_Target = null;
        protected EnemyController m_Controller;
        private Vector3 origin;
        private NavMeshAgent myAgent;
        private Animator anim;
        private bool alive = true;
        private MeleeWeapon weapon;
        bool attacking = false;
        bool interruptable = false;

        //sound
        public RandomAudioPlayer attackAudio;
        public AudioSource stepAudio;
        public RandomAudioPlayer backStepAudio;
        public RandomAudioPlayer hitAudio;
        public RandomAudioPlayer gruntAudio;
        public RandomAudioPlayer deathAudio;
        public RandomAudioPlayer spottedAudio;
        private GameObject target;

        public GameObject scythe;
        public GameObject ragdollPrefab;
        public PlayerControlAlt player;
        public int xpWorth = 9;


        //private float range;


        // Start is called before the first frame update
        void Start()
        {
            origin = transform.position;
            myAgent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            //Debug.Log("got my agent " + myAgent);
            anim.SetInteger("battle", 1);
            weapon = GetComponentInChildren<MeleeWeapon>();
            weapon.SetOwner(scythe);
            player = GameObject.Find("arthur_custom").GetComponent<PlayerControlAlt>();
            scale();
        }

        // Update is called once per frame
        void Update()
        {
            if (alive) FindTarget();
        }

        public void scale()
        {
            StatScaling statScale = GameObject.FindObjectOfType<StatScaling>();
            dmgScale = statScale.damageMod;
            hpScale = statScale.healthMod;
            spdScale = statScale.speedMod;
            atkScale = statScale.attackMod;
            Damageable damg = GetComponent<Damageable>();

            GetComponentInChildren<MeleeWeapon>().damage = Mathf.RoundToInt(GetComponentInChildren<MeleeWeapon>().damage * dmgScale);
            damg.maxHitPoints = Mathf.RoundToInt(damg.maxHitPoints * dmgScale);
            damg.ResetDamage();
            xpWorth = Mathf.RoundToInt(xpWorth * statScale.xpMod);
            GetComponent<NavMeshAgent>().speed *= spdScale;
            modifyAttackSpeed(atkScale);
        }


        public void FindTarget()
        {
            //we ignore height difference if the target was already seen
            target = playerScanner.Detect(transform, "Player", m_Target == null);
            //Debug.Log("target is " + target);
            //if (target != null) Debug.Log("distance is " + Vector3.Distance(transform.position, target.transform.position));
            if (target != null && Vector3.Distance(transform.position, target.transform.position) > 2.5 && !attacking)
            {
                //Debug.Log(target.transform.position);
                myAgent.SetDestination(target.transform.position);
                anim.SetInteger("moving", 1);

            }
            else if (target != null)
            {
                Quaternion targetAngle = Quaternion.LookRotation((target.transform.position - transform.position).normalized);
                Quaternion rotat = Quaternion.RotateTowards(transform.rotation, targetAngle, 180 * Time.deltaTime);
                transform.rotation = rotat;
                if (!attacking && Vector3.Distance(transform.position, target.transform.position) < 2.5)
                {
                     if (myAgent.velocity != Vector3.zero) myAgent.SetDestination(transform.position);
                    //Debug.Log("set 3");


                    anim.SetInteger("moving", 3);
                    attacking = true;
                }
            }
            else anim.SetInteger("moving", 0);
          
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
                    if (hitAudio != null) hitAudio.PlayRandomClip();
                    if (interruptable)
                    {
                        interruptable = false;
                        interruptHit();
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

            
        }

        public void grunt()
        {
            if (gruntAudio != null && Random.Range(0f, 1f) > .9f) gruntAudio.PlayRandomClip();
        }

        public void step()
        {
            if (stepAudio != null) stepAudio.Play();
        }

        public void interruptHit()
        {
            attacking = false;
            //inStagger = true;
            anim.SetBool("interrupt", true);
            anim.SetInteger("attacking", 0);
            //Debug.Log("demon state is  " + anim.GetInteger("moving"));
            weapon.EndAttack();
        }

        public void setInterruptable(int set)
        {
            interruptable = (set == 1);
            Debug.Log("interruptable is " + interruptable);
        }

        public void endInterrupt()
        {
            anim.SetBool("interrupt", false);
        }


        public void stagger()
        {
            attacking = false;
            anim.Play("Weakness");
            anim.SetInteger("moving", 0);
            //Debug.Log("demon state is  " + anim.GetInteger("moving"));
            weapon.EndAttack();
        }

        public void staggerEnd()
        {
            attacking = false;
            anim.SetInteger("moving", 0);
            weapon.EndAttack();
        }

        public void modifyAttackSpeed(float modifier)
        {
            anim.SetFloat("speedMod", modifier);
        }
    }
}
