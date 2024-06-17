using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour                             //�Ĺֽ������������� �κ��丮���� �޵� ��ġ �� ���ⱳü �Ұ�
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

    Vector2 jumpDir = Vector2.zero; //��������

    const float NormalGravity = 1.0f;
    const float ZeroGravity = 0.0f;
    const float WallGravity = 0.1f;

    //����=============================================

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
    public Rigidbody2D rb2;     //����׿�
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
        {           //�̰� y��ν�Ƽ0���� �ϰ� ���� �Ҵ� �����ؼ� ���߾� �ҵ� +=����
            rb.velocity = new Vector2(inputDir.x * currentSpeed, rb.velocity.y);        //MovePosition�� �����̵��̶� �߷� ���뷮�� ������
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
        if (collision.gameObject.layer == 7)                //���� ����, ��Ÿ�� ���¿��� ���� ���������� �ذ��ϱ�
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

    //����ó��===========================================

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
    private void OnMove(InputAction.CallbackContext context)    //Vaine�ÿ� �۵�����
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
    private void OnRun(InputAction.CallbackContext context)     //Shoot�ÿ� �۵� ����
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
    private void OnJump(InputAction.CallbackContext context)    //Vaine, Wall�ÿ� �ٸ��� ����
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

        rb.velocity = Vector2.zero;         //����ġ�� �ݴ�� ����
        rb.AddForce(jumpPower * Vector2.up * -dir, ForceMode2D.Impulse);
    }
    private void OnVaine(InputAction.CallbackContext context)
    {
        if (isVaineAble) 
        {
            State = PlayerState.Vaine;
        }
    }
    private void OnUpDown(InputAction.CallbackContext context)  //���°� vaine�϶��� �۵�
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
    private void OnAttack(InputAction.CallbackContext context)      //���� ��������� �޸��� ����
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
