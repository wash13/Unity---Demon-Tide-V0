using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Gamekit3D.Message;
using UnityEngine.UI;

namespace Gamekit3D
{
    public class PlayerControlAlt : MonoBehaviour, IMessageReceiver
    {
        public RandomAudioPlayer footstep;         // Random Audio Players used for various situations.
        public RandomAudioPlayer hurtAudioPlayer;
        public RandomAudioPlayer landingPlayer;
        public RandomAudioPlayer emoteLandingPlayer;
        public RandomAudioPlayer emoteDeathPlayer;
        public RandomAudioPlayer emoteAttackPlayer;
        public RandomAudioPlayer emoteJumpPlayer;
        public AudioSource slamSound;
        public AudioSource blockSound;
        public AudioSource levelSound;

        private NavMeshAgent myAgent;
        private Animator anim;
        public LayerMask clickable;
        //private float attackTime;
       // private float atkStart;
        private MeleeWeapon weapon;
        private bool alive;
        private bool isBlocking = false;  
        private Damageable dam;
        private bool isCasting = false;
        public GameObject bolt;
        public GameObject targetCanvas;
        private bool moveToHit = false; //this is checking if an attack is queued
        public GameObject healthBar;
        public GameObject manaBar;
        public GameObject xpBar;
        public int spellCost = 2;
        public int manaGain = 1;
        public float blockAngle = 120;
        private bool manaReady = false;
        public GameObject worldControl;

        //experience stuff
        public int xp = 200;
        private int xpToNextLevel = 300; //doing level increase as fibonacci x 100
        private int xpToLastLevel = 200;
        public int level = 1;
        public bool leveled = false;

        private playerStats stats;

        



        // Start is called before the first frame update
        void Start()
        {
            myAgent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            //attackTime = 1.0f / 2; //true value is 1.667, anim ix 2x speed
            weapon = GetComponentInChildren<MeleeWeapon>();
            weapon.SetOwner(gameObject);
            alive = true;
            dam = GetComponent<Damageable>();
            targetCanvas = GameObject.Find("target bar");
            healthBar = GameObject.Find("health fill");
            manaBar = GameObject.Find("mana fill");
            manaBar.GetComponent<Image>().fillAmount = ((float)dam.curMana / (float)dam.maxMana);
            //stats = FindObjectOfType<playerStats>();
        }

        // Update is called once per frame
        void Update()
        {
            if (alive)
            {
                //check if moving to hit a target, and if in range
                if (moveToHit && DistanceToTarget(targetCanvas.GetComponent<targetReference>().target) < 4)
                {
                    rotateToPoint();
                    if (myAgent.velocity != Vector3.zero) myAgent.SetDestination(transform.position); //only called if already moving, to stop
                    anim.SetBool("Attacking", true);
                    anim.SetBool("Moving", false);
                    moveToHit = false;
                }

                //blocking controller
                if (Input.GetMouseButtonDown(1) && !anim.GetBool("Attacking"))
                {
                    myAgent.SetDestination(transform.position);
                    rotateToPoint();
                    manaReady = true;
                    anim.SetBool("Blocking", true);
                }
                
                else if (Input.GetMouseButtonUp(1))
                {
                    myAgent.SetDestination(transform.position);
                    anim.SetBool("Blocking", false);
                }
                

                //movement / primary attack
                if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift) && !anim.GetBool("Attacking") && !isBlocking && !isCasting)
                {
                    //if in range of the target, 
                    float targetDistance = DistanceToTarget(targetCanvas.GetComponent<targetReference>().target);
                    //Debug.Log(targetDistance);
                    if (targetDistance < 4)
                    {
                        rotateToPoint();
                        if (myAgent.velocity != Vector3.zero) myAgent.SetDestination(transform.position); //only called if already moving, to stop
                        anim.SetBool("Attacking", true);
                        anim.SetBool("Moving", false);
                        moveToHit = false;
                    }
                    //not in range of the target
                    else
                    {
                        if (targetDistance < 90) moveToHit = true; 
                        else moveToHit = false;

                        Ray movePoint = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hitInfo;
                        if (Physics.Raycast(movePoint, out hitInfo, 100, clickable))
                        {
                            myAgent.SetDestination(hitInfo.point);
                        }
                    }
                }


                //magic
                if (Input.GetKeyDown(KeyCode.Q)  && !isBlocking && !isCasting)
                {
                    if (dam.curMana < spellCost) return;
                    dam.curMana -= 2;
                    rotateToPoint();
                    myAgent.SetDestination(transform.position);
                    isCasting = true;
                    anim.SetBool("Casting", true);
                    moveToHit = false;
                    manaBar.GetComponent<Image>().fillAmount = ((float) dam.curMana / (float) dam.maxMana);
                }

                    //stand ground attack start
                if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift) && !anim.GetBool("Attacking") && !isBlocking && !isCasting)
                {
                    //atkStart = Time.fixedTime;
                    rotateToPoint();
                    myAgent.SetDestination(transform.position);
                    anim.SetBool("Attacking", true);
                    anim.SetBool("Moving", false);
                    //weapon.BeginAttack(false);
                    //Debug.Log("one call to attack");
                    moveToHit = false;
                }

                //set moving if moving
                else if (myAgent.velocity != Vector3.zero)
                {
                    //Debug.Log("moving");
                    anim.SetBool("Moving", true);
                }

                //else not moving
                else anim.SetBool("Moving", false);
            }

        }

        void rotateToPoint()
        {
            Ray movePoint = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(movePoint, out hitInfo, 100, clickable))
            {
                Vector3 direc = (hitInfo.point - transform.position).normalized;
                Quaternion rotat = Quaternion.LookRotation(direc);
                transform.rotation = rotat;
            }

        }

        public void step()
        {
            if (footstep != null) footstep.PlayRandomClip();
        }

        public void OnReceiveMessage(MessageType type, object sender, object msg)
        {
            switch (type)
            {
                case Message.MessageType.DEAD:
                    //Death((Damageable.DamageMessage)msg);
                    alive = false;
                    anim.Play("arthur_dead_01");
                    healthBar.GetComponent<Image>().fillAmount = 0;
                    break;
                case Message.MessageType.DAMAGED:
                    healthBar.GetComponent<Image>().fillAmount = ((float)dam.currentHitPoints / (float)dam.maxHitPoints);
                    break;
                default:
                    break;
            }
        }

        public float DistanceToTarget(InteractableGlow target)
        {
            if (target == null) return 100; //anything over a small number causes movement
            if (target.tag == "button") return 100;
            return Vector3.Distance(transform.position, target.transform.position);
        }


        public void packStats()
        {
            stats = FindObjectOfType<playerStats>();
            dam = GetComponent<Damageable>();
            weapon = GetComponentInChildren<MeleeWeapon>();
            myAgent = GetComponent<NavMeshAgent>();

            stats = FindObjectOfType<playerStats>();
            stats.hp = dam.maxHitPoints;
            stats.spellCost = spellCost;
            stats.xp = xp;
            stats.level = level;
            stats.xpToNextLevel = xpToNextLevel;
            stats.xpToLastLevel = xpToLastLevel;
            stats.damage = weapon.damage;
            stats.speed = myAgent.speed;
            stats.manaGain = manaGain;
            stats.blockAngle = blockAngle;
        }

        public void unpackStats()
        {
            stats = FindObjectOfType<playerStats>();
            dam = GetComponent<Damageable>();
            weapon = GetComponentInChildren<MeleeWeapon>();
            myAgent = GetComponent<NavMeshAgent>();

            Debug.Log("stat unpack " + stats);
            Debug.Log("damagable " + dam);
            dam.maxHitPoints = stats.hp;
            dam.ResetDamage();
            spellCost = stats.spellCost;
            xp = stats.xp;
            level = stats.level;
            xpToNextLevel = stats.xpToNextLevel;
            xpToLastLevel = stats.xpToLastLevel;
            weapon.damage = stats.damage;
            myAgent.speed = stats.speed;
            manaGain = stats.manaGain;
            blockAngle = stats.blockAngle;
        }

        ///// Stuff for damagable event calls

        public void blockAudio()
        {
            blockSound.Play();
        }

        public void gainMana(int amount)
        {
            if (manaReady) //this is to limit mana gain.  one gain per use of shield
            {
                dam.curMana += manaGain; //used to be amount, hunt that loose end down later
                if (dam.curMana > dam.maxMana) dam.curMana = dam.maxMana;
                manaBar.GetComponent<Image>().fillAmount = ((float)dam.curMana / (float)dam.maxMana);
                manaReady = false;
            }
        }


        ///
        /// this is stuff for xp management
        /// 

        public void awardXp(int amount)
        {
            xp = xp + amount;

            if (xp > xpToNextLevel && !leveled)
            {
                xp = xpToNextLevel;
                level++;
                int temp = xpToNextLevel;
                xpToNextLevel = xpToNextLevel + xpToLastLevel;
                xpToLastLevel = temp;
                leveled = true;
                gameObject.GetComponentInChildren<ParticleSystem>().Play();
                //Debug.Log(levelSound);
                levelSound.Play();
            }
            //Debug.Log(((xp - xpToLastLevel) / xpToNextLevel));
            xpBar.GetComponent<Slider>().value = ((float) (xp - xpToLastLevel) / (float) (xpToNextLevel - xpToLastLevel));
        }

        public void levelUp()
        {
                level++;
                int temp = xpToNextLevel;
                xpToNextLevel = xpToNextLevel + xpToLastLevel;
                xpToLastLevel = temp;
            
        }

        /// <summary>
        /// This is stuff for use with animation events
        /// </summary>


        public void startBlock()
        {
            myAgent.SetDestination(transform.position);
            isBlocking = true;
            dam.hitAngle = (360 - blockAngle);
        }

        public void endBlock()
        {
            anim.SetBool("Blocking", false);
            isBlocking = false;
            dam.hitAngle = 360;
        }

        public void endCast()
        {
            anim.SetBool("Casting", false);
            isCasting = false;
        }

        public void attackStart()
        {
            weapon.BeginAttack(false);
        }

        public void attackEnd()
        {
            anim.SetBool("Attacking", false);
            weapon.EndAttack();
        }

        

        /////// Player Magic Effects
        ///

        public void magicFlash()
        {
            slamSound.Play();
        }

        public void magicBolt()
        {
            //Debug.Log("bolt called");
            GameObject boltInstance = Instantiate(bolt, transform.position, transform.rotation);
            //Debug.Log(boltInstance);
            boltInstance.GetComponent<Rigidbody>().AddRelativeForce(0, 0, 500);
        }

        public bool checkAlive()
        {
            return alive;
        }
    }
}