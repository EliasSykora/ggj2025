using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidbody;

    [Header("Objects")]
    [SerializeField] GameObject bubbleSprite;
    //[SerializeField] GameObject insideBubble;
    [SerializeField] CircleCollider2D bubbleCollider;

    [Header("Speed")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float airAmount = 1f;
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
        scaleChange = new Vector3(-0.001f, -0.001f, -0.001f);
        minSize = new Vector3(0.5f, 0.5f, 0.5f);
        pushedAir = new Vector3(pumpAirAmount, pumpAirAmount, pumpAirAmount);
        InvokeRepeating("RepeatedTests", 0.05f, 0.05f);
    }

    void Update()
    {
        Move();
    }

    void RepeatedTests()
    {
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
            return;
        }
        // Touching water
        currentGravitation = 1f - airAmount;
        myRigidbody.gravityScale = currentGravitation;
        //Debug.Log(currentGravitation);
    }

    void UpdateAir()
    {
        if (airAmount > 0f)  airAmount -= 0.001f;

        bubbleSprite.transform.localScale += scaleChange;

        if (bubbleSprite.transform.localScale.x < minSize.x)
        {
            Debug.Log("Chcíp!");
            Death();
        }
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene(0);
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
        if (!bubbleCollider.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            return;
        }

        if (value.isPressed)
        {
            airAmount += pumpAirAmount;
            bubbleSprite.transform.localScale += pushedAir;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectable")
        {
            collision.gameObject.SetActive(false);
            shellsCollected++;
        }

        if (collision.tag == "Water")
        {
            Debug.Log("Water");
            myRigidbody.velocity = new Vector2(0.0f, 0.0f);
        }
    }

    private void Death()
    {
        Invoke("ReloadLevel", 2f);
    }
}
