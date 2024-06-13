using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //�̵�=============================================

    public float moveSpeed = 3.0f;
    public float runSpeed = 5.0f;
    public float currentSpeed = 3.0f;

    bool isMove = false;

    Vector2 inputDir = Vector2.zero;

    //����=============================================

    public float jumpPower = 1.0f;
    public float smallJumpPower = 0.1f;
    public float jumpLimit = 0.3f;

    float jumpTimer = 0.0f;
    bool jumpKey = false;           //���� �������� ����
    bool isJumping = false;

    //ü��=============================================

    public float HP 
    {
        get => hp;
        set 
        {
            if(hp != value) 
            {
                hp = value;
                hp = Mathf.Clamp(hp, 0, maxHp);
                if (!IsAlive) 
                {
                    Die();
                }
                OnLifeChange?.Invoke();
            }
        }
    }
    public float maxHp = 100.0f;
    float hp;
    bool IsAlive => hp > 0.1;

    //�׼�=============================================

    public Action onDie;
    public Action OnLifeChange;

    //��Ÿ=============================================

    PlayerInputAction actions;
    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        actions = new PlayerInputAction();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentSpeed = moveSpeed;
        hp = maxHp;
    }

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (IsAlive)
        {
            rb.velocity = new Vector2(inputDir.x * moveSpeed, rb.velocity.y);        //MovePosition�� �����̵��̶� �߷� ���뷮�� ������
        }

        if (isJumping && jumpKey && jumpTimer < jumpLimit)
        {
            rb.AddForce(smallJumpPower * ((jumpTimer) + 1f) * Vector2.up, ForceMode2D.Impulse);
            jumpTimer += Time.fixedDeltaTime;
        }
    }

    void Die() 
    {
        actions.Player.Disable();
        onDie?.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D collision)  //�ݸ��� ���� ���̸� ����غ��� �ٴ� ���̰� �׶����϶��� �������̰� ���϶� ����
    {
        if(collision.gameObject.layer == 7) 
        {
            isJumping = false;
            if (jumpKey)
            {
                isJumping = true;
                jumpKey = true;
                jumpTimer = 0;

                rb.AddForce(jumpPower * Vector2.up, ForceMode2D.Impulse);
            }
            //�÷����� ���鿡 �΋H������ �۵� ���ϴ� ��� �����ϱ�
        }
    }

    //�̵�ó��===========================================

    private void OnEnable()
    {
        actions.Player.Enable();
        actions.Player.Move.performed += OnMove;
        actions.Player.Move.canceled += OnMove;
        actions.Player.Run.performed += OnRun;
        actions.Player.Run.canceled += OnRun;
        actions.Player.UpDown.performed += OnUpDown;
        actions.Player.Jump.performed += OnJump;
        actions.Player.Jump.canceled += OnJump;
        actions.Player.Attack.performed += OnAttack;
        actions.Player.Reload.performed += OnReload;
    }
    private void OnDisable()
    {
        actions.Player.Move.performed -= OnMove;
        actions.Player.Move.canceled -= OnMove;
        actions.Player.Run.performed -= OnRun;
        actions.Player.Run.canceled -= OnRun;
        actions.Player.UpDown.performed -= OnUpDown;
        actions.Player.Jump.performed -= OnJump;
        actions.Player.Jump.canceled -= OnJump;
        actions.Player.Attack.performed -= OnAttack;
        actions.Player.Reload.performed -= OnReload;
        actions.Player.Disable();
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            inputDir = Vector2.zero;

            isMove = false;
        }
        else
        {
            inputDir = context.ReadValue<Vector2>();

            isMove = true;
        }
    }
    private void OnRun(InputAction.CallbackContext context)
    {
        if (context.canceled) 
        {
            currentSpeed = moveSpeed;
        }
        else 
        {
            currentSpeed = runSpeed;    
        }
    }
    private void OnUpDown(InputAction.CallbackContext context)
    {
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log(isJumping);
        if (context.canceled)
        {
            jumpKey = false;
        }
        else 
        {
            if (!isJumping) 
            {
                isJumping = true;
                jumpKey = true;
                jumpTimer = 0;

                rb.AddForce(jumpPower * Vector2.up, ForceMode2D.Impulse);
                Debug.Log("jump");
            }
        }
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
    }
    private void OnReload(InputAction.CallbackContext context)
    {
    }
}
