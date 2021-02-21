using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Gamekit3D.Message;

namespace Gamekit3D
{
    public class WarlockAI : MonoBehaviour, IMessageReceiver
    {
        public EnemyController controller { get { return m_Controller; } }
        //public PlayerController target { get { return m_Target; } }
        private StatScaling statScale;
        //private Damageable damg;

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
        bool shouldTeleport = false; //apparently, can't change position mid animator call

        //sound
        public RandomAudioPlayer attackAudio;
        public RandomAudioPlayer teleportAudio;

        public RandomAudioPlayer backStepAudio;
        public RandomAudioPlayer hitAudio;
        public RandomAudioPlayer gruntAudio;
        public RandomAudioPlayer deathAudio;
        public RandomAudioPlayer spottedAudio;
        private GameObject target;
        public int teleportDistance = 20;

        public GameObject ragdollPrefab;
        public GameObject fireballPrefab;
        public GameObject targetPrefab;
        public PlayerControlAlt player;
        public int xpWorth = 10;


      


        // Start is called before the first frame update
        void Start()
        {
            origin = transform.position;
            myAgent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            player = GameObject.Find("arthur_custom").GetComponent<PlayerControlAlt>();
            statScale = FindObjectOfType<StatScaling>();
            scale();
            //damg = GetComponent<Damageable>();
            //Debug.Log("got my agent " + myAgent);
            //anim.SetInteger("battle", 1);
            //weapon = GetComponentInChildren<MeleeWeapon>();
            //weapon.SetOwner(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            if (shouldTeleport) teleporting();
            if (alive) FindTarget();
        }

        public void scale()
        {
            dmgScale = statScale.damageMod;
            hpScale = statScale.healthMod;
            spdScale = statScale.speedMod;
            atkScale = statScale.attackMod;
            Damageable damg = GetComponent<Damageable>();

            //fireballPrefab.GetComponentInChildren<MeleeWeapon>().damage = Mathf.RoundToInt(GetComponentInChildren<MeleeWeapon>().damage * dmgScale);
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
            /*
            if (target == null && !attacking)
            {
                //Debug.Log(target.transform.position);
                myAgent.SetDestination(target.transform.position);
                anim.SetBool("move", true);
                anim.SetBool("combat", false);

            }
            */

            if (target != null && Vector3.Distance(transform.position, target.transform.position) < 3 && !attacking)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, teleportDistance, 1<<23);
                //Debug.Log("collider " + hitColliders[0]);
                if (hitColliders.Length > 1)
                {
                    attacking = true;
                    anim.SetBool("teleport", true);
                    //teleport();
                }
                else
                {
                    anim.SetBool("move", true);
                    myAgent.SetDestination((transform.position - target.transform.position) * 3);
                }
            }
            else if (target != null)
            {
                Quaternion targetAngle = Quaternion.LookRotation((target.transform.position - transform.position).normalized);
                Quaternion rotat = Quaternion.RotateTowards(transform.rotation, targetAngle, 180 * Time.deltaTime);
                transform.rotation = rotat;
                if (!attacking && Vector3.Distance(transform.position, target.transform.position) > 3)
                {
                     if (myAgent.velocity != Vector3.zero) myAgent.SetDestination(transform.position);
                    //Debug.Log("set 3");


                    anim.SetBool("combat", true);
                    anim.SetBool("move", false);
                    //attacking = true;
                }
            }
            else anim.SetBool("move", false);
          
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
                    break;
                default:
                    break;
            }
        }

        public void castFireball()
        {
            //Debug.Log("damage began");
            //weapon.BeginAttack(false);
            if (target != null)
            {
                if (Random.Range(1,3) == 1) attackAudio.PlayRandomClip();
                GameObject fireballInstance = Instantiate(fireballPrefab, (transform.position + new Vector3(0f, 1f, 0f)), transform.rotation);
                GameObject targetInstance = Instantiate(targetPrefab, target.transform.position, transform.rotation);
                fireballInstance.GetComponent<MeleeWeapon>().damage = Mathf.RoundToInt(fireballInstance.GetComponent<MeleeWeapon>().damage * dmgScale);

                fireballInstance.GetComponent<collisionExplode>().setTarget(targetInstance); //this is target circle
                fireballInstance.GetComponent<ThrowArc>().Targ = target.transform;             //this is target - the player. I didn't think this through
            }
        }

        public void dead()
        {
            anim.SetBool("dead", false);  //for future reference, this is to stop the any state transition.  dead animation state is a trap
        }

        public void endAttack()
        {
            anim.SetBool("combat", false);
            attacking = false;
        }

        public void startAttack()
        {
            attacking = true;
        }

        public void teleport()
        {
            shouldTeleport = true;
        }

       public void teleporting()
        {
            //Debug.Log("teleport called");
            teleportAudio.PlayRandomClip();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, teleportDistance, 1 << 23);
            //Debug.Log("collider " + hitColliders[0]);
            if (hitColliders.Length > 1)
            {
                Collider furthest = hitColliders[0];
                foreach (var hitCollider in hitColliders)
                {
                    //Debug.Log("Near " + hitCollider);
                    if (Vector3.Distance(transform.position, hitCollider.transform.position) >
                        Vector3.Distance(transform.position, furthest.transform.position))
                        furthest = hitCollider;
                }
                Debug.Log("furthest is " + furthest);
                if (furthest != null)
                {
                    Vector3 tempPos = gameObject.transform.position;
                    Debug.Log(tempPos);
                    Debug.Log("switching " + gameObject + ", " + furthest);
                    gameObject.transform.position = furthest.transform.position;
                    furthest.transform.position = tempPos;
                }
            }
            attacking = false;
            anim.SetBool("teleport", false);
            shouldTeleport = false;
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
        }


        public void modifyAttackSpeed(float modifier)
        {
            anim.SetFloat("speedMod", modifier);
        }
    }
}
