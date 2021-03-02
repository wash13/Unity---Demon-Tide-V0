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
        bool inStagger = false;
        bool interruptable = false;
        bool wandering = false;
        bool waiting = false;
        public float rangeMod = 1f;
        float timer;
        float originalDetect;

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
        public float wanderChance = .25f;
        public float wanderThreshold = 3f;
        public float wanderDistance = 5f;
        public float waitChance = .25f;
        public float maxWaitTime = .7f;
        public float minWaitTime = .2f;
        private float waitTime;
        float waitStart;
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
            player = GameObject.Find("arthur_custom").GetComponent<PlayerControlAlt>();
            scale();
            timer = Time.time;
            originalDetect = playerScanner.detectionRadius;
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("time is " + Time.time + " and timer is " + timer);
            if (Time.time - .5 > timer && !wandering && !attacking)
            {
                timer = Time.time;
                if (Random.Range(0f, 1f) < wanderChance)
                {
                    wandering = true;
                    playerScanner.detectionRadius = wanderThreshold;
                    target = playerScanner.Detect(transform, "Player", m_Target == null);
                    Vector3 randomDestination = new Vector3(transform.position.x + Random.Range(-wanderDistance, wanderDistance),
                        transform.position.y, transform.position.z + Random.Range(-wanderDistance, wanderDistance));
                    if (target == null) myAgent.SetDestination(randomDestination);
                    else
                    {
                        playerScanner.detectionRadius = originalDetect;
                        wandering = false;
                    }
                    anim.SetFloat("move", 1);
                }
                else if (Random.Range(0f, 1f) < wanderChance)
                {
                    playerScanner.detectionRadius = wanderThreshold;
                    target = playerScanner.Detect(transform, "Player", m_Target == null);
                    if (target == null)
                    {
                        waiting = true;
                        waitTime = Random.Range(minWaitTime, maxWaitTime);
                        myAgent.SetDestination(transform.position);
                        waitStart = Time.time;
                        anim.SetFloat("move", 0);
                    }
                    else playerScanner.detectionRadius = originalDetect;
                }
            }
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


            //first check if wandering
            if (wandering)
            {
                if (!myAgent.hasPath || myAgent.velocity.sqrMagnitude == 0f || target != null)
                {
                    if (target == null)
                    {
                        waiting = true;
                        waitTime = Random.Range(minWaitTime, maxWaitTime);
                        myAgent.SetDestination(transform.position);
                        waitStart = Time.time;
                        anim.SetFloat("move", 0);
                    }
                    else playerScanner.detectionRadius = originalDetect;
                    wandering = false;                    
                    anim.SetFloat("move", 0);
                }
                else anim.SetFloat("move", 1);
            }

            else if (waiting)
            {
                if (target != null || Time.time > waitTime + waitStart )
                {
                    waiting = false;
                    playerScanner.detectionRadius = originalDetect;
                }
            }

            //move to target if its far away, not attacking, and not staggering
            else if (target != null && Vector3.Distance(transform.position, target.transform.position) > (1.5 * rangeMod) && !attacking && !inStagger)
            {
                //Debug.Log(target.transform.position);
                myAgent.SetDestination(target.transform.position);
                anim.SetFloat("move", 1);

            }
            //if not staggering and close to the target
            else if (target != null && !inStagger)
            {
                Quaternion targetAngle = Quaternion.LookRotation((target.transform.position - transform.position).normalized);
                Quaternion rotat = Quaternion.RotateTowards(transform.rotation, targetAngle, 180 * Time.deltaTime);
                transform.rotation = rotat;
                //if target is close, attack
                if (!attacking && Vector3.Distance(transform.position, target.transform.position) < (1.5 * rangeMod))
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
            //else standing still
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
                    ragdollInstance.transform.localScale = transform.localScale;
                    player.awardXp(xpWorth);
                    Destroy(gameObject);
                    break;
                case Message.MessageType.DAMAGED:
                    //ApplyDamage((Damageable.DamageMessage)msg);
                    //Debug.Log("demon hurt");
                    if (hitAudio != null && Random.Range(1,3) == 1) hitAudio.PlayRandomClip();
                    if(interruptable)
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

        public void interruptHit()
        {
            attacking = false;
            inStagger = true;
            anim.SetBool("interrupt", true);
            anim.SetInteger("attacking", 0);
            //Debug.Log("demon state is  " + anim.GetInteger("moving"));
            weapon.EndAttack();
        }

        public void setInterruptable(int set)
        {
            interruptable = (set == 1);
            //Debug.Log("interruptable is " + interruptable);
        }

        public void endInterrupt()
        {
            anim.SetBool("interrupt", false);
            
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

        public void idleNotAttack()
        {
            attacking = false;
        }

        public void modifyAttackSpeed(float modifier)
        {
            anim.SetFloat("speedMod", modifier);
        }
    }
}
