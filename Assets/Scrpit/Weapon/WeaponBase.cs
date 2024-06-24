using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum WeaponType : byte
{
    bat = 0,
    Revolver,
    ShotGun,
    AssaultRifle
}

public class WeaponBase : MonoBehaviour
{
    public float range;
    public int clipSize;
    public float damege;
    public float fireCoolTime;
    public float spread;
    public float recoil;        //ȭ�� ��鸲 ����

    public Action<int> onBulletCountChange;
    public Action NoAmmo;
    public Action<float> onFire;

    protected Transform firePos;    //���� ������ ������ ��ġ
    Vector2 targetDir;              //Ŀ�� ��ġ
    protected bool canFire = true;
    protected int BulletCount 
    {
        get => bulletCount;
        set 
        {
            if(bulletCount != value) 
            {
                bulletCount = value;
                onBulletCountChange?.Invoke(bulletCount);

                if(bulletCount < 1) 
                {
                    NoAmmo?.Invoke();
                }
            }
        }
    }
    int bulletCount;

    protected virtual void Awake()
    {
        firePos = transform.parent;
        bulletCount = clipSize;
    }

    public void Fire(Vector2 targetPos) 
    {
        targetDir = (targetPos - (Vector2)firePos.position).normalized;
        Quaternion dir = Quaternion.LookRotation(targetDir, Vector2.up);
        firePos.rotation = dir;
        if(canFire && BulletCount > 0) 
        {
            FireProcess();
        }
    }

    protected virtual void FireProcess() 
    {
        canFire = false;
        StartCoroutine(FireReady());
        BulletCount--;
    }

    IEnumerator FireReady() 
    {
        yield return new WaitForSeconds(fireCoolTime);
        canFire = true;
    }

    protected void HitProcess ()
    {
        Ray ray = new(firePos.position, GetFireDirection());
    }

    protected Vector2 GetFireDirection()   //�÷��̾�� ���� Ŀ���� ��ġ
    {
        Vector2 result = targetDir;
        result = Quaternion.AngleAxis(Random.Range(-spread, spread), transform.right) * result;
        return result;
    }

    protected void FireRecoil() 
    {
        onFire?.Invoke(recoil);
    }

    public void Equip() 
    {
        onBulletCountChange?.Invoke(bulletCount);
        if (!canFire) 
        {
            StartCoroutine(FireReady());
        }
    }

    public void UnEquip() 
    {
        StopAllCoroutines();
    }
}
