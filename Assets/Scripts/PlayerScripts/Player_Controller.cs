using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player_Controller : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField]
    [Range(0f, 20f)]
    float speed;

    float useSpeed;

    [Header("Basic lerp variables")]

    [SerializeField]
    [Range(0, 20)]
    float accelerate;

    [SerializeField]
    [Range(0,20)]
    float deAccelerate;

    [Space]

    Vector2 movementInput;
    Vector2 useInput;

    [Header("Input table(?)")]
    [SerializeField]
    InputActionReference move;


    [Header("Curve acceleration variables")]
    [SerializeField]
    AnimationCurve speedCurve;

    float curveTime;

    [SerializeField]
    float accelerationTime, deAccelerationTime = 1f;

    [SerializeField]
    //REMOVE LATER
    Slider curveTimeSlider;

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
        /*Basic lerp movement
        Vector2 velocity = rb.velocity;
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
        rb.velocity = velocity;
        */

        if (useSpeed == 0 || movementInput != Vector2.zero) {
            useInput = movementInput;
        }

        Vector2 velocity = rb.linearVelocity;
        if (movementInput == Vector2.zero) {
            curveTime = Mathf.Clamp01(curveTime - (Time.deltaTime / deAccelerationTime));
        }
        else {
            curveTime = Mathf.Clamp01(curveTime + (Time.deltaTime / accelerationTime));
            useInput.Normalize();
        }
        curveTimeSlider.value = curveTime;

        useSpeed = speedCurve.Evaluate(curveTime) * speed;

        velocity = useInput * useSpeed;
        rb.linearVelocity = velocity;
    }
}
