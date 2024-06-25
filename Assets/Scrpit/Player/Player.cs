using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //ü��=============================================

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

    //����=============================================

    WeaponBase[] weapons;
    WeaponBase currentWeapon;

    bool isAttackMode = false;

    //�׼�=============================================

    public Action onDie;
    public Action OnLifeChange;
    public Action<WeaponBase> onGunChange;

    //��Ÿ=============================================

    PlayerInputAction actions;
    PlayerMove playerMove;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        actions = new PlayerInputAction();

        Transform c = transform.GetChild(1);
        weapons = c.GetComponentsInChildren<WeaponBase>();

        hp = maxHp;
    }

    private void OnEnable()
    {
        actions.Player.Enable();
        actions.Player.Attack.performed += OnAttack;
        actions.Player.Reload.performed += OnReload;

        actions.UI.Enable();
        actions.UI.Weapon1.performed += OnWeapon1;
        actions.UI.Weapon2.performed += OnWeapon2;
        actions.UI.Weapon3.performed += OnWeapon3;
        actions.UI.Weapon4.performed += OnWeapon4;
        actions.UI.Menu.performed += OnMenu;
    }

    private void OnDisable()
    {
        actions.Player.Attack.performed -= OnAttack;
        actions.Player.Reload.performed -= OnReload;
        actions.Player.Disable();

        actions.UI.Weapon1.performed -= OnWeapon1;
        actions.UI.Weapon2.performed -= OnWeapon2;
        actions.UI.Weapon3.performed -= OnWeapon3;
        actions.UI.Weapon4.performed -= OnWeapon4;
        actions.UI.Menu.performed -= OnMenu;
        actions.UI.Disable();
    }

    void Die()
    {
        onDie?.Invoke();
    }



    private void OnAttack(InputAction.CallbackContext context)      //���� ��������� �޸��� ����
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

    private void OnMenu(InputAction.CallbackContext context)
    {
    }

    //���⺯ȯ==============================================�޵� ��ġ�� ��ȯ �Ұ�

    private void OnWeapon4(InputAction.CallbackContext context)
    {
    }

    private void OnWeapon3(InputAction.CallbackContext context)
    {
    }

    private void OnWeapon2(InputAction.CallbackContext context)
    {
    }

    private void OnWeapon1(InputAction.CallbackContext context)
    {
    }
}
