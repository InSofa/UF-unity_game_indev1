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
        Debug.Log($"Input: {animator.GetInteger("State")}, is idle: {animator.GetBool("isIdle")}");
        if (movementInput.magnitude < 0.1f) {
            walkMagnitude = walkSoundInterval - 1;
            animator.SetBool("isIdle", true);
            return;
        }
        animator.SetBool("isIdle", false);

        if (movementInput.x > 0) {
            sr.flipX = true;
        } else if (movementInput.x < 0) {
            sr.flipX = false;
        }

        /*States
         0 - Idle
         1 - Up
         2 - Side Up
         3 - Side
         4 - Side Down
         5 - Down
         */


        switch (Mathf.Abs(movementInput.x)) {
            case < 0.3826834f:
                if (movementInput.y > 0) {
                    animator.SetInteger("State", 1);
                } else if (movementInput.y < 0) {
                    animator.SetInteger("State", 5);
                }
                break;
            case > 0.3826834f:
                if (movementInput.y > 0.3826834f) {
                    animator.SetInteger("State", 2);
                } else if (movementInput.y < -0.3826834f) {
                    animator.SetInteger("State", 4);
                } else {
                    animator.SetInteger("State", 3);
                }
                break;
            default:
                animator.SetInteger("State", 0);
                break;
        }
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
