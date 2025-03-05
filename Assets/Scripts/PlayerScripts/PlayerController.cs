using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static GameObject player;
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

    [Header("Sound")]
    LocalSoundComposer lsc;

    [SerializeField]
    List<string> walkSounds;

    [SerializeField]
    float walkSoundInterval = 3f;

    float walkMagnitude;

    // Start is called before the first frame update
    private void Start() {
        player = this.gameObject;
        rb = GetComponent<Rigidbody2D>();
        lsc = GetComponent<LocalSoundComposer>();
    }

    // Update is called once per frame
    private void Update() {
        takeInput();

        if (walkMagnitude >= walkSoundInterval) {
            lsc.PlayRandomFx(walkSounds);
            walkMagnitude = 0;
        }
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
        walkMagnitude += velocity.magnitude;

        rb.linearVelocity = velocity;
    }
}
