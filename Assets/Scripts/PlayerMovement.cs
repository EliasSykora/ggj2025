using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    [Header("Objects")]
    [SerializeField] GameObject bubbleSprite;
    [SerializeField] GameObject insideBubble;
    [SerializeField] CircleCollider2D bubbleCollider;
    [Header("Speed")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float floatSpeed = 5f;
    [SerializeField] float airAmount = 1f;
    [SerializeField] float airMinimalAmount = 0.0001f;
    [SerializeField] float pumpAirAmount = 0.2f;
    [Header("Collectables")]
    [SerializeField] float shellsToCollect = 1;
    private float shellsCollected = 0;
    private Vector3 scaleChange;
    private Vector3 pushedAir;
    private Vector3 minSize;
    private float currentGravitation = 1;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        scaleChange = new Vector3(-0.0005f, -0.0005f, -0.0005f);
        minSize = new Vector3(0.5f, 0.5f, 0.5f);
        pushedAir = new Vector3(pumpAirAmount, pumpAirAmount, pumpAirAmount);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        UpdateGravity();
        UpdateAir();
    }

    void UpdateGravity()
    {
        // Not touching water
        if (!bubbleCollider.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            Debug.Log("Not touching water");
            myRigidbody.gravityScale = 1f;
            airAmount = 1f;
            bubbleSprite.transform.localScale = new Vector3(1f, 1f, 1f);
            return;
        }

        // Touching water
        //Debug.Log(airAmount);
        currentGravitation = 1f - airAmount;
        myRigidbody.gravityScale = currentGravitation;
        Debug.Log(currentGravitation);


    }

    void UpdateAir()
    {
        
        if (bubbleSprite.transform.localScale.x > minSize.x)
        {
            bubbleSprite.transform.localScale += scaleChange;
        } else { return; }

        if (airAmount > 0f) {
        airAmount -= 0.0005f;
        }
    }

    void Move()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);

    }

    void OnFloat(InputValue value)
    {

        if (value.isPressed)
        {
            airAmount += pumpAirAmount;
            bubbleSprite.transform.localScale += pushedAir;
            //myRigidbody.velocity += new Vector2(0f, floatSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectable")
        {
            collision.gameObject.SetActive(false);
            shellsCollected++;
        }
    }
}
