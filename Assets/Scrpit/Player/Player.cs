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
    Inventory inven;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        actions = new PlayerInputAction();

        Transform c = transform.GetChild(1);
        weapons = c.GetComponentsInChildren<WeaponBase>();

        hp = maxHp;
    }

    private void Start()
    {
        inven = Inventory.Instance;
    }

    void Die()
    {
        onDie?.Invoke();
    }

    void WeaponChange(WeaponType weapon) 
    {
        currentWeapon.UnEquip();
        currentWeapon.gameObject.SetActive(false);

        currentWeapon = weapons[(int)weapon];
        currentWeapon.Equip();
        currentWeapon.gameObject.SetActive(true);

        onGunChange?.Invoke(currentWeapon);
    }

    private void OnAttack(InputAction.CallbackContext context)      //���� ��������� �޸��� ����
    {
        if (isAttackMode)
        {
            //currentWeapon.Fire();//��ġ���� �ʿ�
        }
    }
    private void OnReload(InputAction.CallbackContext context)
    {
        if (isAttackMode)
        {
            currentWeapon.Reload();
        }
    }

    private void OnMenu(InputAction.CallbackContext context)
    {
    }

    //���⺯ȯ==============================================�޵� ��ġ�� ��ȯ �Ұ�

    private void OnWeapon4(InputAction.CallbackContext context)
    {
        if (!isAttackMode)
        {
            isAttackMode = true;
            playerMove.OnShootMode(true);
        }
        else if (currentWeapon.weaponType == WeaponType.AssaultRifle)
        {
            isAttackMode = false;
            playerMove.OnShootMode(false);
        }
        else 
        {
            WeaponChange(WeaponType.AssaultRifle);
        }
    }

    private void OnWeapon3(InputAction.CallbackContext context)
    {
        if (!isAttackMode)
        {
            isAttackMode = true;
            playerMove.OnShootMode(true);
        }
        else if (currentWeapon.weaponType == WeaponType.ShotGun)
        {
            isAttackMode = false;
            playerMove.OnShootMode(false);
        }
        else
        {
            WeaponChange(WeaponType.ShotGun);
        }
    }

    private void OnWeapon2(InputAction.CallbackContext context)
    {
        if (!isAttackMode)
        {
            isAttackMode = true;
            playerMove.OnShootMode(true);
        }
        else if (currentWeapon.weaponType == WeaponType.Pistol)
        {
            isAttackMode = false;
            playerMove.OnShootMode(false);
        }
        else
        {
            WeaponChange(WeaponType.Pistol);
        }
    }

    private void OnWeapon1(InputAction.CallbackContext context)
    {
        if (!isAttackMode)
        {
            isAttackMode = true;
            playerMove.OnShootMode(true);
        }
        else if (currentWeapon.weaponType == WeaponType.Bat)
        {
            isAttackMode = false;
            playerMove.OnShootMode(false);
        }
        else
        {
            WeaponChange(WeaponType.Bat);
        }
    }

    //�Է¿���==============================================

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
}
