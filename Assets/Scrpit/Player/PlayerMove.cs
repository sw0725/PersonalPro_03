using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerMoveState : byte
{
    Move = 0,
    Wall,
    Vaine,
    Shoot,
    Dead,
}

public class PlayerMove : MonoBehaviour                             //�Ĺֽ������������� �κ��丮���� �޵� ��ġ �� ���ⱳü �Ұ�
{
    //�̵�=============================================

    public float moveSpeed = 3.0f;
    public float runSpeed = 5.0f;
    public float currentSpeed = 3.0f;

    Vector2 inputDir = Vector2.zero;

    //����=============================================

    public float jumpPower = 1.0f;
    public float smallJumpPower = 7.0f;
    public float wallJumpPower = 1.0f;
    public float jumpLimit = 0.3f;

    float jumpTimer = 0.0f;
    bool jumpKey = false;               //���� �������� ����
    bool isJumping = false;

    Vector2 jumpDir = Vector2.zero;     //��������
    Vector2 vainePos = Vector2.zero;    //������

    const float NormalGravity = 1.0f;
    const float ZeroGravity = 0.0f;
    const float WallGravity = 0.1f;

    //����=============================================

    public PlayerMoveState State //�����¸� ������ ��ü���� ���ϴ°� ������
    {
        get => state;
        private set 
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
    PlayerMoveState state = PlayerMoveState.Move;

    bool isVaineAble = false;
    //��Ÿ=============================================

    public Rigidbody2D rb2;     //����׿�
    Player player;
    PlayerInputAction actions;
    Rigidbody2D rb;
    Animator animator;

    const float GroundRayRange = 0.5f;
    const float WallRayRange = 0.4f;

    private void Awake()
    {
        player = GetComponent<Player>();
        player.onDie += Die;

        actions = new PlayerInputAction();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentSpeed = moveSpeed;
    }

    private void FixedUpdate()
    {
        if (player.IsAlive)
        {           
            if (State != PlayerMoveState.Vaine)
            {
                rb.velocity += new Vector2(inputDir.x * currentSpeed, 0);
                if (rb.velocity.x > currentSpeed)
                {
                    rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
                }
                else if (rb.velocity.x < -currentSpeed)
                {
                    rb.velocity = new Vector2(-currentSpeed, rb.velocity.y);
                }
            }
            else
            {
                rb.velocity += new Vector2(0, inputDir.y * currentSpeed);
                if (rb.velocity.y > currentSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, currentSpeed);
                }
                else if (rb.velocity.y < -currentSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -currentSpeed);
                }
            }

            if (State != PlayerMoveState.Wall) 
            {
                if (isJumping && jumpKey && jumpTimer < jumpLimit)
                {
                    rb.AddForce(smallJumpPower * ((jumpTimer) + 1f) * Vector2.up);      //������Ʈ�������� AddForce�� �ϰ� �ʹٸ� ForceMode2D.Impulse �� ����
                    jumpTimer += Time.fixedDeltaTime;
                }
            }
        }
    }
    void Die()
    {
        State = PlayerMoveState.Dead;
        actions.Player.Disable();
    }

    public void OnShootMode(bool shootMode = true) 
    {
        if (shootMode) 
        {
            State = PlayerMoveState.Shoot;
        }
        else 
        {
            State = PlayerMoveState.Move;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)                //���� ����, ��Ÿ�� ���¿��� ���� ���������� �ذ��ϱ�
        {
            if (State != PlayerMoveState.Shoot)
            {
                if (collision.gameObject.CompareTag("Wall"))
                {
                    WallRay();
                }
                GroundRay();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            if (State != PlayerMoveState.Shoot)
            {
                if (collision.gameObject.CompareTag("Wall"))
                {
                    State = PlayerMoveState.Move;
                }
            }
        }
    }

    void GroundRay() 
    {
        RaycastHit2D hit2D = Physics2D.Raycast(rb.position, Vector2.down, GroundRayRange, LayerMask.GetMask("Ground"));
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
        RaycastHit2D hit2D = Physics2D.Raycast(rb.position, inputDir, WallRayRange, LayerMask.GetMask("Ground"));
        if (hit2D.collider != null)
        {
            isJumping = false;
            jumpDir = hit2D.transform.position - transform.position;
            jumpDir.y = 0;
            jumpDir.Normalize();
            State = PlayerMoveState.Wall;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Vaine")) 
        {
            isVaineAble = true; 
            jumpDir = collision.transform.position - transform.position;
            jumpDir.y = 0;
            jumpDir.Normalize();

            vainePos = collision.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Vaine")) 
        {
            isVaineAble = false;
            State = PlayerMoveState.Move;

            vainePos = Vector2.zero;
        }
    }

    //����ó��===========================================

    void StateEnter(PlayerMoveState state)
    {
        switch (state) 
        {
            case PlayerMoveState.Move:
                break;
            case PlayerMoveState.Wall:
                rb.velocity = Vector2.zero;
                rb.gravityScale = WallGravity;
                break;
            case PlayerMoveState.Vaine:
                rb.velocity = Vector2.zero;
                rb.gravityScale = ZeroGravity;
                break;
            case PlayerMoveState.Shoot:
                break;
            case PlayerMoveState.Dead: 
                break;
        }
    }

    void StateExit(PlayerMoveState state) 
    {
        switch (state)
        {
            case PlayerMoveState.Move:
                break;
            case PlayerMoveState.Wall:
                rb.gravityScale = NormalGravity;
                break;
            case PlayerMoveState.Vaine:
                rb.gravityScale = NormalGravity;
                break;
            case PlayerMoveState.Shoot:
                break;
            case PlayerMoveState.Dead:
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
        actions.Player.Disable();
    }
    private void OnMove(InputAction.CallbackContext context)    //Vaine�ÿ� �۵�����
    {
        if (State != PlayerMoveState.Vaine)
        {
            if (context.canceled)
            {
                inputDir = Vector2.zero;
                rb.velocity = new Vector2(rb.velocity.normalized.x * 0.001f, rb.velocity.y);    //�̲����� ����
            }
            else
            {
                inputDir = context.ReadValue<Vector2>();

                State = PlayerMoveState.Move;
            }
        }
    }
    private void OnRun(InputAction.CallbackContext context)     //Shoot�ÿ� �۵� ����
    {
        if (State != PlayerMoveState.Shoot)
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
    private void OnJump(InputAction.CallbackContext context)    //Vaine, Wall�ÿ� �ٸ��� ����, ���������� ���� ����
    {
        if (State == PlayerMoveState.Wall)
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
                    WallJump();
                }
            }
        } 
        else if (State == PlayerMoveState.Vaine) 
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
                    VaineJump();
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
        State = PlayerMoveState.Move;
        isJumping = true;
        jumpTimer = 0;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(jumpPower * Vector2.up, ForceMode2D.Impulse);
    }
    void WallJump() 
    {
        State = PlayerMoveState.Move;
        isJumping = true;
        jumpTimer = 0;

        rb.velocity = Vector2.zero;         //����ġ�� �ݴ�� ����
        rb.AddForce(jumpPower * -jumpDir, ForceMode2D.Impulse);
        rb.AddForce(wallJumpPower * Vector2.up, ForceMode2D.Impulse);
        jumpDir = Vector2.zero;
    }
    void VaineJump()
    {
        State = PlayerMoveState.Move;
        isJumping = true;
        jumpTimer = 0;

        if (Keyboard.current.aKey.isPressed) 
        {
            jumpDir = Vector2.left;
        }
        else if (Keyboard.current.dKey.isPressed) 
        {
            jumpDir = Vector2.right;
        }

        rb.velocity = Vector2.zero;
        rb.AddForce(jumpPower * jumpDir, ForceMode2D.Impulse);
        rb.AddForce(wallJumpPower * Vector2.up, ForceMode2D.Impulse);
        jumpDir = Vector2.zero;
    }
    private void OnVaine(InputAction.CallbackContext context)
    {
        if (isVaineAble) 
        {
            State = PlayerMoveState.Vaine;
            transform.position = new(vainePos.x, transform.position.y);
            isJumping = false;
        }
    }
    private void OnUpDown(InputAction.CallbackContext context)  //���°� vaine�϶��� �۵�
    {
        if (State == PlayerMoveState.Vaine)
        {
            if (context.canceled)
            {
                inputDir = Vector2.zero;
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.normalized.y * 0.001f);    //�۵��ϳ� ������ �ȵ�
            }
            else
            {
                inputDir = context.ReadValue<Vector2>();
            }
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
