using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    CircleCollider2D myBubble;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float floatSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myBubble = GetComponent<CircleCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        Move();
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
            //do stuff

            myRigidbody.velocity += new Vector2(0f, floatSpeed);
        }
    }
}
