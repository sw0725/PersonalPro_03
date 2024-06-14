using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //이동=============================================

    public float moveSpeed = 3.0f;
    public float runSpeed = 5.0f;
    public float currentSpeed = 3.0f;

    Vector2 inputDir = Vector2.zero;

    //점프=============================================

    public float jumpPower = 1.0f;
    public float smallJumpPower = 0.1f;
    public float jumpLimit = 0.3f;

    float jumpTimer = 0.0f;
    bool jumpKey = false;           //점프 높이조절 위함
    bool isJumping = false;

    //상태=============================================

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

    //체력=============================================

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

    //액션=============================================

    public Action onDie;
    public Action OnLifeChange;

    //기타=============================================

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
            rb.velocity = new Vector2(inputDir.x * currentSpeed, rb.velocity.y);        //MovePosition은 순간이동이라 중력 적용량이 증발함
        }

        if (isJumping && jumpKey && jumpTimer < jumpLimit)
        {
            rb.AddForce(smallJumpPower * ((jumpTimer) + 1f) * Vector2.up, ForceMode2D.Impulse);
            jumpTimer += Time.fixedDeltaTime;
        }

        Debug.DrawRay(rb.position, Vector2.down * groundRayRange, new(0, 0, 1));
        Debug.DrawRay(rb.position, inputDir * wallRayRange, new(0, 0, 1));
    }

    private void OnCollisionEnter2D(Collision2D collision)  //콜리젼 말고 레이를 사용해보자 바닥 레이가 그라운드일때와 무릎레이가 월일때 적용
    {
        if (collision.gameObject.layer == 7)                //상태 적용, 벽타기 상태에서 땅에 내려왔을때 해결하기
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
            //벽타기로
            Debug.Log("WallRay");
        }
    }

    //상태처리===========================================

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

    //이동처리===========================================

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
    private void OnMove(InputAction.CallbackContext context)    //Vaine시에 작동안함
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
    private void OnRun(InputAction.CallbackContext context)     //Shoot시에 작동 안함
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
    private void OnJump(InputAction.CallbackContext context)    //Vaine, Wall시에 다르게 점프
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
    private void OnAttack(InputAction.CallbackContext context)      //무기 들었을때는 달리기 봉인
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
