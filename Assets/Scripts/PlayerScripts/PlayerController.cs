using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField]
    [Range(0f, 20f)]
    float speed;

    [Header("Basic lerp variables")]

    [SerializeField]
    [Range(0, 20)]
    float accelerate;

    [SerializeField]
    [Range(0,20)]
    float deAccelerate;

    [Space]

    Vector2 movementInput;

    [Header("Input table(?)")]
    [SerializeField]
    InputActionReference move;

    // Start is called before the first frame update
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update() {
        takeInput();
    }

    private void FixedUpdate() {
        movementLogic();
    }

    void takeInput() {
        movementInput = move.action.ReadValue<Vector2>();
    }

    void movementLogic() {
        //Basic lerp movement
        Vector2 velocity = rb.linearVelocity;
        float lerpFloatValue = accelerate;

        if(movementInput == Vector2.zero)
        {
            lerpFloatValue = deAccelerate;
        }
        else
        {
            movementInput.Normalize();
        }

        velocity = Vector2.Lerp(velocity, movementInput * speed, lerpFloatValue * Time.fixedDeltaTime);
        rb.linearVelocity = velocity;
    }
}
