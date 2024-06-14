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

    Vector2 inputDir = Vector2.zero;

    //����=============================================

    public float jumpPower = 1.0f;
    public float smallJumpPower = 0.1f;
    public float jumpLimit = 0.3f;

    float jumpTimer = 0.0f;
    bool jumpKey = false;           //���� �������� ����
    bool isJumping = false;

    //����=============================================

    enum PlayerState : byte
    {
        Idle =0,
        Move,
        Wall,
        Vaine,
        Shoot,
    }

    PlayerState State 
    {
        get => state;
        set 
        {
            if(state != value) 
            {
                StateExit(state);
                state = value;
                StateEnter(state);
            }
        }
    }
    PlayerState state = PlayerState.Idle;

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

    float groundRayRange = 0.5f;
    float wallRayRange = 0.4f;

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
            rb.velocity = new Vector2(inputDir.x * currentSpeed, rb.velocity.y);        //MovePosition�� �����̵��̶� �߷� ���뷮�� ������
        }

        if (isJumping && jumpKey && jumpTimer < jumpLimit)
        {
            rb.AddForce(smallJumpPower * ((jumpTimer) + 1f) * Vector2.up, ForceMode2D.Impulse);
            jumpTimer += Time.fixedDeltaTime;
        }

        Debug.DrawRay(rb.position, Vector2.down * groundRayRange, new(0, 0, 1));
        Debug.DrawRay(rb.position, inputDir * wallRayRange, new(0, 0, 1));
    }

    private void OnCollisionEnter2D(Collision2D collision)  //�ݸ��� ���� ���̸� ����غ��� �ٴ� ���̰� �׶����϶��� �������̰� ���϶� ����
    {
        if (collision.gameObject.layer == 7)                //���� ����, ��Ÿ�� ���¿��� ���� ���������� �ذ��ϱ�
        {
            if (collision.gameObject.CompareTag("Wall")) 
            {
                WallRay();
            }
            GroundRay();
        }
    }

    void Die() 
    {
        actions.Player.Disable();
        onDie?.Invoke();
    }

    void GroundRay() 
    {
        RaycastHit2D hit2D = Physics2D.Raycast(rb.position, Vector2.down, groundRayRange, LayerMask.GetMask("Ground"));
        if (hit2D.collider != null)
        {
            isJumping = false;
            if (jumpKey)
            {
                Jump();
            }
        }
    }
    void WallRay() 
    {
        RaycastHit2D hit2D = Physics2D.Raycast(rb.position, inputDir, wallRayRange, LayerMask.GetMask("Ground"));
        if (hit2D.collider != null)
        {
            //��Ÿ���
            Debug.Log("WallRay");
        }
    }

    //����ó��===========================================

    void StateEnter(PlayerState state)
    {
        switch (state) 
        {
            case PlayerState.Idle: 
                break;
            case PlayerState.Move:
                break;
            case PlayerState.Wall:
                break;
            case PlayerState.Vaine:
                break;
            case PlayerState.Shoot:
                break;
        }
    }

    void StateExit(PlayerState state) 
    {
        switch (state)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Move:
                break;
            case PlayerState.Wall:
                break;
            case PlayerState.Vaine:
                break;
            case PlayerState.Shoot:
                break;
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
        actions.Player.Jump.performed += OnJump;
        actions.Player.Jump.canceled += OnJump;
        actions.Player.UpDown.performed += OnUpDown;
        actions.Player.Attack.performed += OnAttack;
        actions.Player.Reload.performed += OnReload;
    }
    private void OnDisable()
    {
        actions.Player.Move.performed -= OnMove;
        actions.Player.Move.canceled -= OnMove;
        actions.Player.Run.performed -= OnRun;
        actions.Player.Run.canceled -= OnRun;
        actions.Player.Jump.performed -= OnJump;
        actions.Player.Jump.canceled -= OnJump;
        actions.Player.UpDown.performed -= OnUpDown;
        actions.Player.Attack.performed -= OnAttack;
        actions.Player.Reload.performed -= OnReload;
        actions.Player.Disable();
    }
    private void OnMove(InputAction.CallbackContext context)    //Vaine�ÿ� �۵�����
    {
        if (context.canceled)
        {
            inputDir = Vector2.zero;

            State = PlayerState.Idle;
        }
        else
        {
            inputDir = context.ReadValue<Vector2>();

            State = PlayerState.Move;
        }
    }
    private void OnRun(InputAction.CallbackContext context)     //Shoot�ÿ� �۵� ����
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
    private void OnJump(InputAction.CallbackContext context)    //Vaine, Wall�ÿ� �ٸ��� ����
    {
        if (context.canceled)
        {
            jumpKey = false;
        }
        else 
        {
            jumpKey = true;
            if (!isJumping) 
            {
                Jump();
            }
        }
    }
    void Jump()
    {
        isJumping = true;
        jumpTimer = 0;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(jumpPower * Vector2.up, ForceMode2D.Impulse);
    }
    private void OnUpDown(InputAction.CallbackContext context)
    {
    }
    private void OnAttack(InputAction.CallbackContext context)      //���� ��������� �޸��� ����
    {
    }
    private void OnReload(InputAction.CallbackContext context)
    {
    }
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        
    }

#endif
}
