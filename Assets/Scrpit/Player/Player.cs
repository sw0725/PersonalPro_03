using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour                             //파밍스테이지에서는 인벤토리없음 앵두 납치 시 무기교체 불가
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

    Vector2 jumpDir = Vector2.zero; //벽점프용

    const float NormalGravity = 1.0f;
    const float ZeroGravity = 0.0f;
    const float WallGravity = 0.1f;

    //상태=============================================

    enum PlayerState : byte
    {
        Move = 0,
        Wall,
        Vaine,
        Shoot,
        Dead,
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
                Debug.Log(state);
            }
        }
    }
    PlayerState state = PlayerState.Move;

    bool isVaineAble = false;

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

    PlayerInputAction actions;
    Rigidbody2D rb;
    public Rigidbody2D rb2;     //디버그용
    Animator animator;

    const float GroundRayRange = 0.5f;
    const float WallRayRange = 0.4f;

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
        {           //이건 y밸로시티0으로 하고 가속 촤대 지정해서 막야야 할듯 +=쓰자
            rb.velocity = new Vector2(inputDir.x * currentSpeed, rb.velocity.y);        //MovePosition은 순간이동이라 중력 적용량이 증발함
            //if (State != PlayerState.Vaine)
            //{
            //    rb.velocity += new Vector2(inputDir.x * currentSpeed, 0);
            //    if (rb.velocity.x > currentSpeed) 
            //    {
            //        rb.velocity = new Vector2 (currentSpeed, rb.velocity.y);
            //    }
            //    else if (rb.velocity.x < -currentSpeed)
            //    {
            //        rb.velocity = new Vector2(-currentSpeed, rb.velocity.y);
            //    }
            //}
            //else 
            //{
            //    rb.velocity = new Vector2(rb.velocity.x, inputDir.y * currentSpeed); 
            //}
        }

        if (isJumping && jumpKey && jumpTimer < jumpLimit)
        {
            rb.AddForce(smallJumpPower * ((jumpTimer) + 1f) * Vector2.up, ForceMode2D.Impulse);
            jumpTimer += Time.fixedDeltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
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

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                State = PlayerState.Move;
            }
        }
    }

    void GroundRay() 
    {
        RaycastHit2D hit2D = Physics2D.Raycast(rb.position, Vector2.down, GroundRayRange, LayerMask.GetMask("Ground"));
        if (hit2D.collider != null)
        {
            State = PlayerState.Move;
            isJumping = false;
            if (jumpKey)
            {
                Jump();
            }
        }
    }
    void WallRay()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(rb.position, inputDir, WallRayRange, LayerMask.GetMask("Ground"));
        if (hit2D.collider != null)
        {
            isJumping = false;
            jumpDir = hit2D.transform.position - transform.position;
            State = PlayerState.Wall;
        }
    }

    void Die() 
    {
        actions.Player.Disable();
        onDie?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Vaine")) 
        {
            isVaineAble = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Vaine")) 
        {
            isVaineAble = false;
        }
    }

    //상태처리===========================================

    void StateEnter(PlayerState state)
    {
        switch (state) 
        {
            case PlayerState.Move:
                break;
            case PlayerState.Wall:
                rb.velocity = Vector2.zero;
                rb.gravityScale = WallGravity;
                break;
            case PlayerState.Vaine:
                rb.velocity = Vector2.zero;
                rb.gravityScale = ZeroGravity;
                break;
            case PlayerState.Shoot:
                break;
            case PlayerState.Dead: 
                break;
        }
    }

    void StateExit(PlayerState state) 
    {
        switch (state)
        {
            case PlayerState.Move:
                break;
            case PlayerState.Wall:
                rb.gravityScale = NormalGravity;
                break;
            case PlayerState.Vaine:
                rb.gravityScale = NormalGravity;
                break;
            case PlayerState.Shoot:
                break;
            case PlayerState.Dead:
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
        actions.Player.Vaine.performed += OnVaine;
        actions.Player.UpDown.performed += OnUpDown;
        actions.Player.UpDown.canceled += OnUpDown;
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
        actions.Player.Vaine.performed -= OnVaine;
        actions.Player.UpDown.performed -= OnUpDown;
        actions.Player.UpDown.canceled -= OnUpDown;
        actions.Player.Attack.performed -= OnAttack;
        actions.Player.Reload.performed -= OnReload;
        actions.Player.Disable();
    }
    private void OnMove(InputAction.CallbackContext context)    //Vaine시에 작동안함
    {
        if (State != PlayerState.Vaine)
        {
            if (context.canceled)
            {
                inputDir = Vector2.zero;
            }
            else
            {
                inputDir = context.ReadValue<Vector2>();

                State = PlayerState.Move;
            }
        }
    }
    private void OnRun(InputAction.CallbackContext context)     //Shoot시에 작동 안함
    {
        if (State != PlayerState.Shoot)
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
    }
    private void OnJump(InputAction.CallbackContext context)    //Vaine, Wall시에 다르게 점프
    {
        if (State == PlayerState.Vaine || State == PlayerState.Wall)
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
                    WallJump(jumpDir);
                }
            }
        }
        else
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
    }
    void Jump()
    {
        isJumping = true;
        jumpTimer = 0;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(jumpPower * Vector2.up, ForceMode2D.Impulse);
    }
    void WallJump(Vector2 dir) 
    {
        isJumping = true;
        jumpTimer = 0;

        rb.velocity = Vector2.zero;         //벽위치의 반대로 점프
        rb.AddForce(jumpPower * Vector2.up * -dir, ForceMode2D.Impulse);
    }
    private void OnVaine(InputAction.CallbackContext context)
    {
        if (isVaineAble) 
        {
            State = PlayerState.Vaine;
        }
    }
    private void OnUpDown(InputAction.CallbackContext context)  //상태가 vaine일때만 작동
    {
        if (State == PlayerState.Vaine)
        {
            if (context.canceled)
            {
                inputDir = Vector2.zero;
            }
            else
            {
                inputDir = context.ReadValue<Vector2>();
            }
        }
    }
    private void OnAttack(InputAction.CallbackContext context)      //무기 들었을때는 달리기 봉인
    {
        if (State == PlayerState.Shoot) 
        {
        }
    }
    private void OnReload(InputAction.CallbackContext context)
    {
        if (State == PlayerState.Shoot)
        {
        }
    }
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;

        Handles.DrawLine(rb2.position, rb2.position + Vector2.down * GroundRayRange);
        Handles.DrawLine(rb2.position, rb2.position + inputDir * WallRayRange);
    }

#endif
}
