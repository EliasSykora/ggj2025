using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidbody;

    [Header("Objects")]
    [SerializeField] GameObject bubbleSprite;
    [SerializeField] CircleCollider2D bubbleCollider;
    GameObject[] Pearls;

    [Header("Speed")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float airAmount = 1f;
    [SerializeField] float pumpAirAmount = 0.2f;
    [SerializeField] float airReduction = 0.001f;

    [Header("Collectables")]
    [SerializeField] float shellsToCollect = 1f;
    private float shellsCollected = 0f;

    [Header("Air")]
    [SerializeField] float breathableAir = 1f;
    [SerializeField] Image AirBar;
    private RectTransform AirBarMask;
    private Vector3 scaleChange;
    private Vector3 pushedAir;
    private Vector3 minSize;
    private float currentGravitation = 1f;
    private bool isAlive = true;
    private float airBarCapacity = 1310f;
    private float airBarDeficit = 1f;
    [SerializeField] private float startFloat = 1f;
    [SerializeField] private float startDrown = 0.3f;
    [SerializeField] private float floatDrownSpeed = 0.01f;
    [SerializeField] private float pumpSpeedUp = 5f;

    public GameObject ExitObject;
    public Animator ExitAnimator;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        scaleChange = new Vector3(-airReduction, -airReduction, -airReduction);
        minSize = new Vector3(0.5f, 0.5f, 0.5f);
        pushedAir = new Vector3(pumpAirAmount, pumpAirAmount, pumpAirAmount);
        InvokeRepeating("RepeatedTests", 0.05f, 0.05f);
        AirBarMask = AirBar.GetComponent<RectTransform>();
        Pearls = GameObject.FindGameObjectsWithTag("Collectable");
        ExitObject = GameObject.Find("Exit");
        ExitAnimator = ExitObject.GetComponent<Animator>();

        //Debug.Log("Perel je: " + Pearls.Length);

    }

    void Update()
    {
        if (!isAlive) return;
            
            Move();
    }

    void RepeatedTests()
    {
        if (!isAlive) return;

        UpdateGravity();

        if (bubbleCollider.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            UpdateAir();
        } 
    }

    void UpdateGravity()
    {
        // Not touching water
        if (!bubbleCollider.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            myRigidbody.gravityScale = 1f;
            airAmount = 1f;
            bubbleSprite.transform.localScale = new Vector3(1f, 1f, 1f);
            AirBarMask.anchoredPosition = new Vector2(-5f, AirBarMask.anchoredPosition.y);

            return;
        } else
        {
            myRigidbody.gravityScale = 0;
        }

        if(airAmount > startFloat)
        {
            myRigidbody.velocity  = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y-floatDrownSpeed);
        } else if (airAmount > startDrown) {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 0f);
        }  else if (airAmount < startDrown) {
           myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y - floatDrownSpeed);
        }
        // Touching water
        // currentGravitation = 1f - (0.5+airAmount);s
        // myRigidbody.gravityScale = currentGravitation;
        // Debug.Log(currentGravitation);
    }

    void UpdateAir()
    {
       // Death by empty airtank
       if(AirBarMask.anchoredPosition.x < -1310) Death();

       // Air reduction 
       if (airAmount > 0f)  airAmount -= airReduction;
        bubbleSprite.transform.localScale += scaleChange;

        /* 
         airBarDeficit -= 0.005f;
         AirBarMask.anchoredPosition = new Vector2(AirBarMask.anchoredPosition.x - 1f, AirBarMask.anchoredPosition.y);

         if(AirBarMask.anchoredPosition.x > -5f)
         {
             AirBarMask.anchoredPosition = new Vector2(-5f, AirBarMask.anchoredPosition.y);
         }
        */

        // Death by bubble reduction
        if (bubbleSprite.transform.localScale.x < minSize.x) 
        {
            Death();
        }
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Move()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
       // Debug.Log(moveInput);
    }

    void OnFloat(InputValue value)
    {
        if (!isAlive) return;

        if (!bubbleCollider.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            return;
        }

        if (value.isPressed)
        {
            airAmount += pumpAirAmount;
            bubbleSprite.transform.localScale += pushedAir;
            AirBarMask.anchoredPosition = new Vector2(AirBarMask.anchoredPosition.x - (pumpAirAmount*200), AirBarMask.anchoredPosition.y);
            // myRigidbody.velocity = new Vector2(0f, 0f);
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y + pumpSpeedUp);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectable")
        {
            collision.gameObject.SetActive(false);
            shellsCollected++;

            if (shellsCollected >= Pearls.Length)
            {
                ExitAnimator.SetBool("CollectedAll", true);
                //gameObject.SetActive(false);
                myRigidbody.velocity = new Vector2(0.0f, 0.0f);
            }
        }
        /*
        if (collision.tag == "Exit")
        {
            ExitAnimator.SetBool("CollectedAll", true);
        }
        */
            if (collision.tag == "Water")
        {
            Debug.Log("Water");
            myRigidbody.velocity = new Vector2(0.0f, 0.0f);
        }

        if (collision.tag == "Danger")
        {
            Death();
        }

        if (collision.tag == "AirTank")
        {
            collision.gameObject.SetActive(false);

            AirBarMask.anchoredPosition = new Vector2(AirBarMask.anchoredPosition.x + 600f, AirBarMask.anchoredPosition.y);
            if (AirBarMask.anchoredPosition.x > -5f)
            {
                AirBarMask.anchoredPosition = new Vector2(-5f, AirBarMask.anchoredPosition.y);
            }
        }
    }

    private void Death()
    {
        isAlive = false;
        bubbleSprite.transform.localScale = minSize;
        myRigidbody.gravityScale = 2;
        AirBarMask.anchoredPosition = new Vector2(-airBarCapacity, AirBarMask.anchoredPosition.y);
        Invoke("ReloadLevel", 2f);
    }
}
