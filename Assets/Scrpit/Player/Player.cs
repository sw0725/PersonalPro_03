using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //체력=============================================

    public float HP
    {
        get => hp;
        set
        {
            if (hp != value)
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
    public bool IsAlive => hp > 0.1;

    //액션=============================================

    public Action onDie;
    public Action OnLifeChange;

    //기타=============================================

    PlayerInputAction actions;
    PlayerMove playerMove;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        actions = new PlayerInputAction();

        hp = maxHp;
    }

    private void OnEnable()
    {
        actions.Player.Enable();
        actions.Player.Attack.performed += OnAttack;
        actions.Player.Reload.performed += OnReload;
    }

    private void OnDisable()
    {
        actions.Player.Attack.performed -= OnAttack;
        actions.Player.Reload.performed -= OnReload;
        actions.Player.Disable();
    }

    void Die()
    {
        onDie?.Invoke();
    }



    private void OnAttack(InputAction.CallbackContext context)      //무기 들었을때는 달리기 봉인
    {
        if (playerMove.State == PlayerMoveState.Shoot)
        {
        }
    }
    private void OnReload(InputAction.CallbackContext context)
    {
        if (playerMove.State == PlayerMoveState.Shoot)
        {
        }
    }
}
