using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfficerAI : MonoBehaviour
{
    //world objects
    Camera main_camera;
    AudioSource audioSource;
    public GameObject player;
    public GameObject attackPoint;
    public GameObject firePoint;
    public Image warning;
    Image posTimer;
    Image healthBar;

    OfficerManager officerManag;
    public GameObject officerManagOb;

    //object components
    Animator bodyAnim;
    Animator legsAnim;

    //object public variables
    public AudioClip[] clips = new AudioClip[2]; //0|Fire Warning   1|Gun Shot

    public static bool trueAttack = true; //this variable tells all instances of OfficerAI when it is okay to attack
    public bool meleeMode = false;
    public bool moveToRightPos = false;
    public bool moveToLeftPos = false;

    public float fireTime = 20.0f;

    public float health = 5;
    public float walkSpeed = 3.5f;
    public float attackRadius = 1f;
    public LayerMask playerLayers; //used to find player
    public int attackTime = 8;
    public int currPos; //current pos
    public float startMaxDis = 8.0f; //max distance btw player and enemy
    public float startMinDis = 2.75f; //min distance btw player and enemy
    public static float maxDis; //static var for all officers
    public static float minDis;
    public float offsetMaxTime = 0.4f;
    public float offsetMinTime = 0.2f;

    [HideInInspector]
    public bool warned;
    public float posTime = 5.0f; //time it takes to reset pos

    //object private variables
    static int OffsetOrderInLayer = 0;

    float fireTimeLeft;

    float startHealth;
    bool trueMove = true;
    float speed;
    bool walkingForward = false;
    bool walkingBackward = false;
    int directionFacing = -1; // -1 for right of player, 1 otherwise
    bool setTime = true;
    float posTimeLeft; //time left until reset
    float posScale;
    float distance;
    float attackTimeLeft; //time until next attack

    // Start is called before the first frame update
    void Start()
    {
        officerManag = officerManagOb.GetComponent<OfficerManager>();
        main_camera = Camera.main;

        audioSource = transform.GetComponent<AudioSource>();

        warning = transform.Find("Display").gameObject.transform.Find("Warning").GetComponent<Image>();

        posTimer = transform.Find("Display").gameObject.transform.Find("Pos Timer").GetComponent<Image>();
        posTimeLeft = posTime;
        posScale = posTimer.fillAmount;

        healthBar = transform.Find("Display").gameObject.transform.Find("Health").GetComponent<Image>();
        startHealth = health;

        bodyAnim = transform.GetComponent<Animator>();
        legsAnim = transform.Find("Origin").gameObject.GetComponent<Animator>();

        currPos = Random.Range(1, 4);
        bodyAnim.SetInteger("Curr Pos", currPos);
        bodyAnim.SetBool("Melee Mode", meleeMode);

        speed = walkSpeed;
        maxDis = startMaxDis;
        minDis = startMinDis;

        attackPoint = transform.Find("Attack Point").gameObject;
        firePoint = transform.Find("Fire Point").gameObject;

        attackTimeLeft = (float)Random.Range(attackTime - 4, attackTime);
        fireTimeLeft = fireTime;

        bodyAnim.SetFloat("BufferSpeed", Random.Range(offsetMinTime, offsetMaxTime));

        if (!meleeMode)
        {
            officerManag.officerList.Add(gameObject); //add officer object to manager's list
            healthBar.enabled = posTimer.enabled = false; //turn off display
        }
        else
            legsAnim.SetBool("Melee Mode", true);
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(player.transform.position, transform.position);
        Vector3 heading = player.transform.position - transform.position;
        directionFacing = AngleDir(transform.forward, heading, transform.up);

        bodyAnim.SetFloat("Angle", Vector3.Angle(transform.position - player.transform.position, transform.forward) * directionFacing);

        if (meleeMode)
        {
            //Officer movement
            if (trueMove)
            {
                if (distance > maxDis) //max distance
                {
                    walkingForward = true;
                }
                else if (distance < minDis) // min distance
                {
                    walkingBackward = true;
                }
                else
                {
                    walkingForward = walkingBackward = false;
                }
            }
            walkForward();
            walkBackward();

            //pos timing
            posTimer.fillAmount = posScale * ((posTimeLeft + 0.5f) / posTime);

            if (setTime)
            {
                posTimeLeft -= Time.deltaTime;
                if (posTimeLeft < 0)
                {
                    currPos = Random.Range(1, 4);
                    bodyAnim.SetInteger("Curr Pos", currPos);
                    posTimeLeft = posTime;
                    posTimer.fillAmount = posScale;
                }
            }

            //attack timing
            if (trueAttack)
                attackTimeLeft -= Time.deltaTime;

            if (attackTimeLeft < 0 && health > 0)
            {
                posTimer.color = Color.red;
                trueMove = setTime = trueAttack = false;
                walkingForward = walkingBackward = false;
                if (distance > 2)
                {
                    walkingForward = true;
                    speed = walkSpeed * 1.5f;
                    legsAnim.SetFloat("Speed", 1.5f);
                }
                else
                {
                    posTimer.color = Color.white;
                    bodyAnim.SetTrigger("Attack"); //trueMove, setTime, attackTimeLeft, and walking bools are reset in anim event attack(), trueAttack in resetTrueAttack()
                    attackTimeLeft = (float)Random.Range(attackTime - 4, attackTime);
                }
            }
        }

        else
        {
            //moving to pos when ready
            if (moveToRightPos)
            {
                Vector3 rightPos = player.transform.position + player.transform.right * maxDis;
                distance = Vector3.Distance(rightPos, transform.position);
                if (distance > 0.1)
                {
                    walkToPos(rightPos);
                    legsAnim.SetBool("Walk Downward", true);
                }
                else
                {
                    legsAnim.SetBool("Walk Downward", false);
                    healthBar.enabled = posTimer.enabled = true; //turn on display
                    moveToRightPos = false;
                    meleeMode = true;
                    bodyAnim.SetBool("Melee Mode", true);
                }
            }

            else if (moveToLeftPos)
            {
                Vector3 leftPos = player.transform.position + player.transform.right * -maxDis;
                distance = Vector3.Distance(leftPos, transform.position);
                if (distance > 0.1)
                {
                    walkToPos(leftPos);
                    legsAnim.SetBool("Walk Downward", true);
                }
                else
                {
                    legsAnim.SetBool("Walk Downward", false);
                    healthBar.enabled = posTimer.enabled = true; //turn on display
                    moveToLeftPos = false;
                    meleeMode = true;
                    bodyAnim.SetBool("Melee Mode", true);
                }
            }

            //firing
            else
            {
                fireTimeLeft -= Time.deltaTime;
                if (fireTimeLeft < 2.0f && fireTimeLeft > 0 && trueAttack)
                {
                    trueAttack = false;
                    officerManag.setWarning();
                    audioSource.PlayOneShot(clips[0], 1);

                    minDis = startMinDis * 5; //distances are increased so the melee soldiers back up, it is reset in fire()
                    maxDis = startMaxDis * 5;
                }
                else if (fireTimeLeft < 0)
                {
                    if (warned)
                    {
                        warned = false;

                        var tempColor = Color.white;
                        tempColor.a = 0.0f;
                        warning.color = tempColor;

                        bodyAnim.SetTrigger("Fire");
                    }
                    else
                    {
                        trueAttack = true;
                    }
                    fireTimeLeft = fireTime;
                }
            }
        }
    }


    int AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) //returns 1 if target is on right of fwd and -1 if on left
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    void fire()
    {
        audioSource.PlayOneShot(clips[1], 1.0f);
        minDis = startMinDis;
        maxDis = startMaxDis;
        Vector3 offset = player.transform.right * Random.Range(-3, 3) + new Vector3(0, Random.Range(3, 5), 0);
        DrawLine(firePoint.transform.position, player.transform.position + offset, Color.white);
        trueAttack = true;
        if (player.GetComponent<PlayerController>() != null && player.GetComponent<PlayerController>().firedAt(player.transform.position + offset))
            return;
    }

    public void resetDis() //called by officerManager incase the distances aren't reset if the last officer moves up
    {
        minDis = startMinDis;
        maxDis = startMaxDis;

        trueAttack = true;
    }

    void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        firePoint.AddComponent<LineRenderer>();
        LineRenderer lr = firePoint.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = 0.1f;
        lr.endWidth = 0.01f;
        lr.startColor = lr.endColor = color;
        lr.numCapVertices = 4;
        lr.sortingOrder = -3;
        firePoint.AddComponent<DestroyLine>();
    }

    void flip()
    {
        Vector3 newVect = transform.localScale;
        newVect.x *= -1;
        transform.localScale = newVect;
    }

    void walkForward()
    {
        if (walkingForward && !walkingBackward)
        {
            transform.localPosition += main_camera.transform.right * Time.deltaTime * speed * directionFacing;
            legsAnim.SetBool("Walking Forward", true);
        }
        else
            legsAnim.SetBool("Walking Forward", false);
    }

    void walkBackward()
    {
        if (walkingBackward && !walkingForward)
        {
            transform.localPosition += main_camera.transform.right * Time.deltaTime * speed * -directionFacing;
            legsAnim.SetBool("Walking Backward", true);
        }
        else
            legsAnim.SetBool("Walking Backward", false);
    }

    void walkToPos(Vector3 pos)
    {
        if (warning.color.a > 0)
        {
            var tempColor = Color.white;
            tempColor.a = 0.0f;
            warning.color = tempColor;
        }

        transform.localPosition += Vector3.Normalize(pos - transform.position) * Time.deltaTime * speed * 2;
    }

    //called by player script. returns true if enemy took damage. returns false otherwise. also triggers the enemy's counter/damage
    public bool enemyCountered(int playerPos) 
    {
        if (playerPos == currPos)
        {
            trueMove = false; //must be freed by countered call
            walkingForward = walkingBackward = false;
            bodyAnim.SetTrigger("Counter");
            return true;
        }
        else
        {
            bodyAnim.SetTrigger("Damage");
            health--;
            healthBar.fillAmount = (float)health / (float)startHealth;
            return false;
        }
    }

    void countered()
    {
        trueMove = true;
    }

    void attack()
    {
        walkingForward = walkingBackward = false;
        speed = walkSpeed;
        legsAnim.SetFloat("Speed", 1.0f);

        trueMove = setTime = true;

        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.transform.position, attackRadius, playerLayers);
        foreach (Collider player in hitPlayers)
        {
            PlayerController script = player.GetComponent<PlayerController>();
            if (script != null && script.playerBlocked(currPos, directionFacing)) //if the enemy countered
            {
                bodyAnim.SetTrigger("Blocked");
            }
        }
    }

    void resetTrueAttack()
    {
         trueAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint)
            Gizmos.DrawWireSphere(attackPoint.transform.position, attackRadius);
    }

    void push()
    {
        trueMove = false;
        walkingForward = false;
        walkingBackward = true;
    }

    void pushed()
    {
        walkingForward = walkingBackward = false;
        if (health <= 0)
        {
            trueAttack = true; //incase the enemy somehow dies before it is reset in attack
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<CapsuleCollider>());
            bodyAnim.SetBool("Death", true);
            return;
        }
        trueMove = true;
    }

    void updateLegsDeath()
    {
        legsAnim.SetTrigger("Death");
    }

    void updateDisplayColor()
    {
        //revealing the healthBar and posTimer
        var tempColor = Color.red;
        tempColor.a = 1.0f;
        healthBar.color = tempColor;
        posTimer.color = tempColor;
    }

    void dead()
    {
        player.GetComponent<PlayerController>().score++;

        officerManag.setPos((int)transform.localScale.x);
        transform.Find("Origin").gameObject.transform.Find("Body").GetComponent<SpriteRenderer>().sortingOrder -= OffsetOrderInLayer;
        transform.Find("Origin").gameObject.transform.Find("Legs").GetComponent<SpriteRenderer>().sortingOrder -= OffsetOrderInLayer;
        OffsetOrderInLayer += 2;
        Destroy(bodyAnim);
        Destroy(legsAnim);
        Destroy(this);
    }
}
