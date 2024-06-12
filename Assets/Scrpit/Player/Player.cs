using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float runSpeed = 5.0f;
    public float currentSpeed = 3.0f;

    public float jumpPower = 1.0f;
    public float jumpLimit = 0.3f;
    float jumpTimer = 0.0f;
    bool isJumping = false;

    public float maxHp = 100.0f;

    public Action onDie;

    PlayerInputAction actions;
    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        actions = new PlayerInputAction();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentSpeed = moveSpeed;
    }

    private void FixedUpdate()
    {
        if (isJumping) 
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpPower * ((jumpTimer) + 1f), ForceMode2D.Impulse);
            jumpTimer += Time.fixedDeltaTime;
        }
    }

    private void OnEnable()
    {
        actions.Player.Enable();
        actions.Player.Move.performed += OnMove;
        actions.Player.Run.performed += OnRun;
        actions.Player.UpDown.performed += OnUpDown;
        actions.Player.Jump.performed += OnJump;
        actions.Player.Attack.performed += OnAttack;
        actions.Player.Reload.performed += OnReload;
    }


    private void OnDisable()
    {
        actions.Player.Move.performed -= OnMove;
        actions.Player.Run.performed -= OnRun;
        actions.Player.UpDown.performed -= OnUpDown;
        actions.Player.Jump.performed -= OnJump;
        actions.Player.Attack.performed -= OnAttack;
        actions.Player.Reload.performed -= OnReload;
        actions.Player.Disable();
    }
    private void OnMove(InputAction.CallbackContext context)
    {
    }
    private void OnRun(InputAction.CallbackContext context)
    {
    }
    private void OnUpDown(InputAction.CallbackContext context)
    {
    }
    private void OnJump(InputAction.CallbackContext context)
    {
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
    }
    private void OnReload(InputAction.CallbackContext context)
    {
    }
}
