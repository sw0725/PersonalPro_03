using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : RecycleObject
{
    [Header("Enemy Data")]
    public float moveSpeed = 2.0f;
    public float maxHP = 100;

    protected float HP 
    {
        get => hp;
        set 
        {
            if (hp != value) 
            {
                hp = value;
                if (hp < 0.1) 
                {
                    hp = 0;
                    Die();
                }
            }
        }
    }
    float hp;

    Rigidbody2D rb;

    protected override void OnEnable()
    {
        base.OnEnable();
        HP = maxHP;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public virtual void Attack() 
    {
    
    }

    public void Attacked(float damege) 
    {
        HP -= damege;
        //��������� �ڷ� �������� rb.Addforce
    }

    void Die() //���ͷ��� ���� -> ������� �Ĺ�
    {
        gameObject.SetActive(false);
    }
}
