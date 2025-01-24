using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    [SerializeField] CircleCollider2D bubbleCollider;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float floatSpeed = 5f;
    [SerializeField] float airAmount = 1f;
    [SerializeField] float airMinimalAmount = 0.0001f;
    [SerializeField] GameObject bubbleSprite;
    private Vector3 scaleChange;
    private Vector3 pushedAir;
    private Vector3 minSize;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        bubbleCollider = GetComponent<CircleCollider2D>();
        scaleChange = new Vector3(-0.0005f, -0.0005f, -0.0005f);
        minSize = new Vector3(0.5f, 0.5f, 0.5f);
        pushedAir = new Vector3(0.2f, 0.2f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        UpdateAir();
    }

    void UpdateAir()
    {
        
        if (bubbleSprite.transform.localScale.x > minSize.x)
        {
            bubbleSprite.transform.localScale += scaleChange;
        } else { return; }
        airAmount-= 0.0005f;

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
            airAmount += 0.1f;
            bubbleSprite.transform.localScale += pushedAir;
            myRigidbody.velocity += new Vector2(0f, floatSpeed);
        }
    }
}
