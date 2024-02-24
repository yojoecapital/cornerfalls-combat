using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // required when Using UI elements
using TMPro;

public class PlayerController : MonoBehaviour
{
    //world objects
    Camera main_camera;
    GameObject crosshairs;
    GameObject cursor;
    GameObject origin, body, legs, eggface;
    GameObject attackPoint;
    Image healthBar;
    public GameObject menu;
    public GameObject scoreDisplay;

    //player components
    Animator bodyAnim;
    Animator legsAnim;
    SpriteRenderer cursorColor;

    //player public variables
    public Sprite[] eggfaceStills = new Sprite[3];

    public int score = 0;
    public float health = 5;
    public float walkSpeed = 3.5f;
    public LayerMask enemyLayers; //used to find enemies
    public float attackRadius = 1.0f;

    //player private variables
    float healthStartSize; //saves the initial size of the health image
    float startHealth; //saves the initial start health amount
    float speed;
    float angle;
    bool trueMove = true;
    bool rotateCursor = true;
    bool walkingForward = false;
    bool walkingBackward = false;
    int directionFacing = 1;
    int pushDirection = 1;
    int spriteIndex = 0;
    public bool blocking = false;
    public bool eggfaceRepel = false;

    // Start is called before the first frame update
    void Start()
    {
        main_camera = Camera.main;
        main_camera.transform.parent = transform;
        crosshairs = transform.Find("Crosshairs").gameObject;
        cursor = crosshairs.transform.Find("Cursor").gameObject;
        origin = transform.Find("Origin").gameObject;
        body = origin.transform.Find("Body").gameObject;
        legs = origin.transform.Find("Legs").gameObject;
        eggface = origin.transform.Find("Eggface").gameObject;
        attackPoint = origin.transform.Find("Attack Point").gameObject;
        healthBar = crosshairs.transform.Find("Health").GetComponent<Image>();
        healthStartSize = healthBar.fillAmount;
        startHealth = health;

        bodyAnim = GetComponent<Animator>();
        legsAnim = origin.GetComponent<Animator>();
        cursorColor = cursor.GetComponent<SpriteRenderer>();

        speed = walkSpeed;
        legsAnim.SetFloat("Speed", speed);
    }

    // Update is called once per frame
    void Update()
    {
        //player movement
        if (trueMove)
        {
            eggfaceRepel = false;
            speed = walkSpeed;
            if (Input.GetKey("d"))
                walkingForward = true;
            else
                walkingForward = false;

            if (Input.GetKey("a"))
                walkingBackward = true;
            else
                walkingBackward = false;
        }
        walkForward();
        walkBackward();

        //rotating reel
        Vector3 mousePos = Input.mousePosition;
        Vector3 cursorPos = main_camera.WorldToScreenPoint(cursor.transform.position);
        Vector3 direction = mousePos - cursorPos;
        //flipping player
        float screenMiddle = Screen.width / 2;
        if (trueMove && ((mousePos.x > screenMiddle && directionFacing < 0) || (mousePos.x < screenMiddle && directionFacing > 0)))
            bodyAnim.SetBool("Flip", true);
        float newAngle = Mathf.Atan2(direction.y, directionFacing * direction.x) * Mathf.Rad2Deg;
        //clamping btw angles
        if (newAngle < -30f)
        {
            newAngle = -30f;
        }
        else if (newAngle > 30f)
        {
            newAngle = 30f;
        }
        if (rotateCursor)
        {
            angle = newAngle;
            cursor.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            bodyAnim.SetFloat("Angle", angle);
        }

        //attacks
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            bodyAnim.SetBool("Attack", true);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            cursorColor.color = Color.white;
            bodyAnim.SetBool("Attack", false);
        }

        //blocks
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            cursorColor.color = Color.yellow;
            rotateCursor = false;
            bodyAnim.SetBool("Block", true);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            cursorColor.color = Color.white;
            rotateCursor = true;
            bodyAnim.SetBool("Block", false);
        }

        //eggface
        if (Input.GetKeyDown("s") && trueMove)
        {
            rotateCursor = trueMove = walkingForward = walkingBackward = false;
            bodyAnim.SetBool("Eggface", true);
        }
        else if (Input.GetKeyUp("s"))
        {
            rotateCursor = trueMove = true;
            bodyAnim.SetBool("Eggface", false);
            bodyAnim.SetBool("Repel", false);
            eggface.GetComponent<SpriteRenderer>().sprite = null;

            eggfaceRepel = false;
        }
    }

    //helpers and anim events
    void walkForward()
    {
        if(walkingForward && !walkingBackward)
        {
            legsAnim.SetBool("Walking Forward", true);
            transform.localPosition += main_camera.transform.right * Time.deltaTime * speed;
        }
        else
            legsAnim.SetBool("Walking Forward", false);
    }

    void walkBackward()
    {
        if (walkingBackward && !walkingForward)
        {
            legsAnim.SetBool("Walking Backward", true);
            transform.localPosition += main_camera.transform.right * Time.deltaTime * -speed;
        }
        else
            legsAnim.SetBool("Walking Backward", false);
    }

    void flip()
    {
        Vector3 newVect = crosshairs.transform.localScale;
        newVect.x *= -1;
        crosshairs.transform.localScale = newVect;

        directionFacing *= -1;

        newVect = body.transform.localScale;
        newVect.x *= -1;
        body.transform.localScale = newVect;

        newVect = attackPoint.transform.localPosition;
        newVect.x *= -1;
        attackPoint.transform.localPosition = newVect;
    }
    void flipped()
    {
        bodyAnim.SetBool("Flip", false);
    }

    void eggfaceDown()
    {
        eggfaceRepel = true; //is reset by button press
    }

    public bool firedAt(Vector3 coor) //called by officers
    {
        if (eggfaceRepel)
        {
            eggface.GetComponent<SpriteRenderer>().sprite = eggfaceStills[spriteIndex];
            spriteIndex++;
            if (spriteIndex > 2)
                spriteIndex = 0;
            eggface.transform.position = coor;
            bodyAnim.SetBool("Repel", true);
            Invoke("resetSprite", 0.6f);
            return false;
        }
        else
        {
            bodyAnim.SetTrigger("Damage");
            health -= 0.5f;
            healthBar.fillAmount = healthStartSize * ((float)health / (float)startHealth);
            return true;
        }
    }

    void resetSprite()
    {
        bodyAnim.SetBool("Repel", false);
        eggface.GetComponent<SpriteRenderer>().sprite = null;
    }

    void charge()
    {
        trueMove = false;
        rotateCursor = false;
        cursorColor.color = Color.red;
        walkingForward = walkingBackward = false;

        speed = walkSpeed * 1.5f;

        if (directionFacing == 1)
            walkingForward = true;
        else
            walkingBackward = true;
    }

    void charged()
    {
        trueMove = true;
        rotateCursor = true;
        walkingForward = walkingBackward = false;

        speed = walkSpeed;
    }

    void push()
    {
        trueMove = false;
        walkingForward = walkingBackward = false;

        if (pushDirection == -1)
            walkingForward = true;
        else
            walkingBackward = true;
    }

    void pushed()
    {
        rotateCursor = true;
        walkingForward = walkingBackward = false;
        if (health <= 0)
        {
            OfficerAI.trueAttack = false;
            trueMove = rotateCursor = false;
            bodyAnim.SetBool("Death", true);
            return;
        }
        trueMove = true;
    }

    void attack()
    {
        walkingForward = walkingBackward = false;
        speed = walkSpeed;
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.transform.position, attackRadius, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.GetComponent<OfficerAI>().enemyCountered(getPos(angle))) //if the enemy countered
            {
                bodyAnim.SetTrigger("Countered");
                pushDirection = directionFacing;
                health--;
                healthBar.fillAmount = healthStartSize * ((float)health / (float)startHealth);
            }
        }
    }

    void block()
    {
        blocking = true;
    }

    void blocked()
    {
        blocking = false;

    }

    public bool playerBlocked(int enemyPos, int directionFacing)
    {
        if (getPos(angle) == enemyPos && blocking && directionFacing != this.directionFacing)
        {
            bodyAnim.SetTrigger("Blocked");
            return true;
        }
        else
        {
            bodyAnim.SetTrigger("Damage");
            pushDirection = -directionFacing;
            health--;
            healthBar.fillAmount = healthStartSize * ((float)health / (float)startHealth);
            return false;
        }
    }

    int getPos(float angle)
    {
        if (angle > 10)
            return 3;
        else if (angle > -10)
            return 2;
        else
            return 1;
    }

    void OnDrawGizmosSelected()
    {
        if(attackPoint)
            Gizmos.DrawWireSphere(attackPoint.transform.position, attackRadius);
    }

    void updateLegsDeath()
    {
        legsAnim.SetTrigger("Death");
    }

    void dead()
    {
        menu.SetActive(true);
        if (scoreDisplay != null)
        {
            scoreDisplay.GetComponent<TextMeshProUGUI>().text = "Score: " + score;
        }

        OfficerAI.trueAttack = false;
        Destroy(bodyAnim);
        Destroy(legsAnim);
        Destroy(this);
    }
}
