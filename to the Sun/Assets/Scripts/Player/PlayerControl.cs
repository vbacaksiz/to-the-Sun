using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerControl : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    [SerializeField]
    private float speed = 5.0f;

    [SerializeField]
    private float slopeCheckDistance;
    private float slopeDownAngel;
    private float slopeDownAngelOld;

    [SerializeField]
    private LayerMask whatIsGround;
    private new Vector2 slopeNormalPerp;
    private new Vector2 colliderSize;
    private bool isOnSlope;

    GameObject DontDestroy;
    private Transform playerTransform;
    private PlayerAnimation playerAnimator;
    private SpriteRenderer playerSprite;
    private float tempHorizontalInput;
    private bool slide;
    private float slideTimer;
    [SerializeField]
    private float maxSlideTimer;
    private int attackNumber = 1;
    private float maxComboTime = 1.0f;
    private float comboTimer = 0.0f;
    private bool attack;
    public Text text;
    private bool nextLevelOpen = false;
    private bool equipmentDoorOpen = false;
    private bool dinnerRoomOpen = false;
    private bool backToLevelDoorOpen = false;
    private bool endGame = false;
    private bool death = false;
    public bool defense = false;
    public bool successfullDefense = false;
    private bool isEat = false;
    private int getHeal = 1;
    public int numOfEnemy = 0;

    public int currentHealth { get; set; }
    
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        playerAnimator = GetComponent<PlayerAnimation>();
        playerSprite = GetComponentInChildren<SpriteRenderer>();
        currentHealth = PlayerPrefs.GetInt("life");
        UIManager.instance.showCurrentLifeBars(currentHealth);
        StartPoint();
        colliderSize = cc.size;
    }

    void Update()
    {
        Slide();
        Attack();
        Defense();
        goNextLevel(nextLevelOpen);
        goEquipmentRoom(equipmentDoorOpen);
        goDinnerRoom(dinnerRoomOpen);
        goBreakRoom(backToLevelDoorOpen);
        forHeal(isEat);
        forEndGame(endGame);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!death)
        {
            if (!defense)
            {
                SlopeCheck();
                speed = 5.0f;
                Movement();
            }
        }
    }

    void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);
        SlopeCheckVertical(checkPos);
    }

    void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);
        if(hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngel = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngel != slopeDownAngelOld)
            {
                isOnSlope = true;
            }

            slopeDownAngelOld = slopeDownAngel;
        }
    }

    void Movement()
    {
        //horizontalInput => left or right
        float horizontalInput = Input.GetAxisRaw("Horizontal"); //CrossPlatformInputManager.GetAxis("Horizontal");

        Flip(horizontalInput);
        SlideBreak(horizontalInput);

        if(isOnSlope)
        {
            rb.velocity = new Vector2(-horizontalInput * speed * slopeNormalPerp.x, -horizontalInput * speed * slopeNormalPerp.y);
        }
        else
        {
            rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        }

        playerAnimator.Move(horizontalInput);
    }

    void Flip(float horizontalInput)
    {
        if (horizontalInput > 0)
        {
            playerSprite.flipX = false;
        }
        else if (horizontalInput < 0)
        {
            playerSprite.flipX = true;
        }
    }

    void Slide()
    {
        if ((Input.GetKeyDown(KeyCode.LeftShift) || CrossPlatformInputManager.GetButtonDown("R1Button")) && !slide)
        {
            slideTimer = 0;
            FindObjectOfType<AudioManager>().Play("Shift");
            slide = true;
            playerAnimator.Slide(true);
            tempHorizontalInput = Input.GetAxisRaw("Horizontal"); //CrossPlatformInputManager.GetAxis("Horizontal");
        }

        if (slide)
        {
            slideTimer += Time.deltaTime;
            if(slideTimer > maxSlideTimer)
            {
                slide = false;
                playerAnimator.Slide(false);
            }
        }
    }

    void SlideBreak(float horizontalInput)
    {
        //flip when slide -> stop slide
        if(horizontalInput < 0 && tempHorizontalInput > 0 || horizontalInput > 0 && tempHorizontalInput < 0)
        {
            slide = false;
            playerAnimator.Slide(false);
        }
        /*else if(horizontalInput > 0 && tempHorizontalInput < 0)
        {
            slide = false;
            playerAnimator.Slide(false);
        }*/
    }

    void Attack()
    {
        comboTimer += Time.deltaTime;
        //Debug.Log(attackNumber);
        if(comboTimer > maxComboTime)
        {
            attackNumber = 1;
            comboTimer = 0;
        }
        if ((Input.GetMouseButtonDown(0) || CrossPlatformInputManager.GetButtonDown("AButton")) && slide == false && attackNumber == 1)
        {
            FindObjectOfType<AudioManager>().Play("Attack");
            playerAnimator.Attack(attackNumber);
            attackNumber++;
            comboTimer = 0;
        }
        else if ((Input.GetMouseButtonDown(0) || CrossPlatformInputManager.GetButtonDown("AButton")) && slide == false && attackNumber == 2)
        {
            FindObjectOfType<AudioManager>().Play("Attack");
            playerAnimator.Attack(attackNumber);
            attackNumber = 1;
        }
    }

    void Defense()
    {
        bool getDefenseValue = getDefenseInput();
        playerAnimator.Defense(getDefenseValue);
    }

    bool getDefenseInput()
    {
        if ((Input.GetMouseButtonDown(1) || CrossPlatformInputManager.GetButtonDown("DButton")) && slide == false)
        {
            defense = true;
            rb.velocity = new Vector2(0,rb.velocity.y);
        }
        if((Input.GetMouseButtonUp(1) || CrossPlatformInputManager.GetButtonUp("DButton")) && slide == false)
        {
            defense = false;
        }
        return defense;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "NewLevelDoor")
        {
            if(numOfEnemy == 0)
            {
                text.text = "PRESS 'E' or 'X BUTTON' TO GO NEXT LEVEL";
                nextLevelOpen = true;
            }
            else
            {
                text.text = "ENEMIES ALIVE!!";
                nextLevelOpen = false;
            }
        }
        if (collision.tag == "EquipmentRoomDoor")
        {
            text.text = "PRESS 'E' or 'X BUTTON' TO GO EQUIPMENT ROOM";
            equipmentDoorOpen = true;
        }
        if (collision.tag == "DinnerRoomDoor")
        {
            text.text = "PRESS 'E' or 'X BUTTON' TO GO DINNER ROOM";
            dinnerRoomOpen = true;
        }
        if (collision.tag == "BackToLevelsDoor")
        {
            text.text = "PRESS 'E' or 'X BUTTON' TO GO BREAK ROOM";
            backToLevelDoorOpen = true;
        }
        if (collision.tag == "Heal")
        {
            text.text = "PRESS 'E' or 'X BUTTON' TO GET HEAL";
            isEat = true;
        }
        if (collision.tag == "EndGameDoor")
        {
            if (numOfEnemy == 0)
            {
                text.text = "PRESS 'E' or 'X BUTTON' TO END GAME";
                endGame = true;
            }
            else
            {
                text.text = "ENEMIES ALIVE!!";
                endGame = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "NewLevelDoor")
        {
            text.text = "";
            nextLevelOpen = false;
        }
        if (collision.tag == "EquipmentRoomDoor")
        {
            text.text = "";
            equipmentDoorOpen = false;
        }
        if (collision.tag == "DinnerRoomDoor")
        {
            text.text = "";
            dinnerRoomOpen = false;
        }
        if(collision.tag == "BackToLevelsDoor")
        {
            text.text = "";
            backToLevelDoorOpen = false;
        }
        if(collision.tag == "Heal")
        {
            text.text = "";
            isEat = false;
        }
        if(collision.tag == "EndGameDoor")
        {
            text.text = "";
            endGame = false;
        }
    }

    void goNextLevel(bool nextLevelOpen)
    {
        if ((Input.GetKeyDown("e") || CrossPlatformInputManager.GetButtonDown("XButton")) && nextLevelOpen == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            transform.position = new Vector3(-6.0f, 0.0f, transform.position.z);
            PlayerPrefs.SetInt("life", currentHealth);
            text.text = "";
            FindObjectOfType<AudioManager>().Play("Select");
        }
    }

    void goEquipmentRoom(bool equipmentDoorOpen)
    {
        if ((Input.GetKeyDown("e") || CrossPlatformInputManager.GetButtonDown("XButton")) && equipmentDoorOpen == true)
        {
            PlayerPrefs.SetInt("goBack", SceneManager.GetActiveScene().buildIndex);
            PlayerPrefs.SetFloat("XPosition", transform.position.x);
            PlayerPrefs.SetFloat("YPosition", transform.position.y);
            PlayerPrefs.SetFloat("ZPosition", transform.position.z);
            SceneManager.LoadScene(1); //equipmentRoom build index -> 1
            PlayerPrefs.SetInt("life", currentHealth);
            text.text = "";
            FindObjectOfType<AudioManager>().Play("Select");
        }
    }

    void goDinnerRoom(bool dinnerDoorOpen)
    {
        if ((Input.GetKeyDown("e") || CrossPlatformInputManager.GetButtonDown("XButton")) && dinnerRoomOpen == true)
        {
            PlayerPrefs.SetInt("goBack", SceneManager.GetActiveScene().buildIndex);
            PlayerPrefs.SetFloat("XPosition", transform.position.x);
            PlayerPrefs.SetFloat("YPosition", transform.position.y);
            PlayerPrefs.SetFloat("ZPosition", transform.position.z);
            PlayerPrefs.SetInt("life", currentHealth);
            SceneManager.LoadScene(2); //dinnerRoom build index -> 2
            text.text = "";
            FindObjectOfType<AudioManager>().Play("Select");
        }
    }

    void goBreakRoom(bool backToLevelDoorOpen)
    {
        if ((Input.GetKeyDown("e") || CrossPlatformInputManager.GetButtonDown("XButton")) && backToLevelDoorOpen == true)
        {
            int lastLevel = PlayerPrefs.GetInt("goBack");
            PlayerPrefs.SetInt("needStartPoint", 1);
            PlayerPrefs.SetInt("life", currentHealth);
            SceneManager.LoadScene(lastLevel);
            text.text = "";
            FindObjectOfType<AudioManager>().Play("Select");
        }
    }

    void forHeal(bool isEat)
    {
        getHeal = PlayerPrefs.GetInt("getHeal");
        if(getHeal == 0)
        {
            if ((Input.GetKeyDown("e") || CrossPlatformInputManager.GetButtonDown("XButton")) && isEat == true)
            {
                if (currentHealth < 6)
                {
                    currentHealth++;
                    PlayerPrefs.SetInt("life", currentHealth);
                    text.text = "";
                    Destroy(GameObject.FindGameObjectWithTag("Heal"));
                    getHeal = 1;
                    PlayerPrefs.SetInt("getHeal", getHeal);
                }
                else
                {
                    text.text = "";
                    Destroy(GameObject.FindGameObjectWithTag("Heal"));
                    getHeal = 1;
                    PlayerPrefs.SetInt("getHeal", getHeal);
                }
                UIManager.instance.showCurrentLifeBars(currentHealth);
                FindObjectOfType<AudioManager>().Play("Select");
            }
        }
        else
        {
            Destroy(GameObject.FindGameObjectWithTag("Heal"));
        }
    }

    void forEndGame(bool endGame)
    {
        if ((Input.GetKeyDown("e") || CrossPlatformInputManager.GetButtonDown("XButton")) && endGame == true)
        {
            text.text = "";
            FindObjectOfType<AudioManager>().Play("Select");
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(0); //--> Main Menu
        }
    }

    public void StartPoint()
    {
        int needStartPoint = PlayerPrefs.GetInt("needStartPoint");
        PlayerPrefs.SetInt("needStartPoint", 0);
        if(needStartPoint == 1)
        {
            float x = PlayerPrefs.GetFloat("XPosition");
            float y = PlayerPrefs.GetFloat("YPosition");
            float z = PlayerPrefs.GetFloat("ZPosition");
            transform.position = new Vector3(x, y, z);
        }
    }

    public void Damage(int damageAmount)
    {
        if(defense)
        {
            successfullDefense = true;
        }
        else if(slide)
        {

        }
        else
        {
            if (currentHealth < 1)
            {
                return;
            }
            currentHealth--;
            UIManager.instance.UpdateLifeBars(currentHealth);
            FindObjectOfType<AudioManager>().Play("Hit");

            if (currentHealth < 1)
            {
                playerAnimator.Death();
                FindObjectOfType<AudioManager>().Play("Death");
                death = true;
                IEnumerator coroutine = LoadForDeath();
                StartCoroutine(coroutine);
            }
            else
            {
                playerAnimator.TakeDamage();
            }
        }
        
    }

    public IEnumerator LoadForDeath()
    {
        yield return new WaitForSeconds(1.5f);
        int savedLevel = PlayerPrefs.GetInt("save");
        death = false;
        SceneManager.LoadScene(savedLevel);
    }
}
