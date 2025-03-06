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

    [Header("Animation")]
    [SerializeField]
    Animator animator;

    [SerializeField]
    SpriteRenderer sr;

    // Start is called before the first frame update
    private void Start() {
        player = this.gameObject;
        rb = GetComponent<Rigidbody2D>();
        lsc = GetComponent<LocalSoundComposer>();
    }

    // Update is called once per frame
    private void Update() {
        takeInput();
        updateVisuals();

        if (walkMagnitude >= walkSoundInterval) {
            lsc.PlayRandomFx(walkSounds);
            walkMagnitude = 0;
        }
    }

    private void FixedUpdate() {
        movementLogic();
    }

    void updateVisuals() {
        animator.SetFloat("MovementMagnitude", movementInput.magnitude);

        if (movementInput.magnitude == 0) {
            walkMagnitude = walkSoundInterval - 1;
            return;
        }

        if (movementInput.x > 0) {
            sr.flipX = true;
        } else if (movementInput.x < 0) {
            sr.flipX = false;
        }

        animator.SetFloat("XValue", movementInput.x);
        animator.SetFloat("YValue", movementInput.y);
    }
    void takeInput() {
        movementInput = move.action.ReadValue<Vector2>().normalized;
    }

    void movementLogic() {
        //Basic lerp movement
        Vector2 velocity = rb.linearVelocity;
        float lerpFloatValue = accelerate;

        if(movementInput == Vector2.zero)
        {
            lerpFloatValue = deAccelerate;
        }

        velocity = Vector2.Lerp(velocity, movementInput * speed, lerpFloatValue * Time.fixedDeltaTime);
        walkMagnitude += velocity.magnitude;

        rb.linearVelocity = velocity;
    }
}
