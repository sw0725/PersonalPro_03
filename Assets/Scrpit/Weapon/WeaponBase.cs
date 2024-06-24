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
    public float recoil;        //화면 흔들림 정도

    public Action<int> onBulletCountChange;
    public Action NoAmmo;
    public Action<float> onFire;

    protected Transform firePos;    //레이 기준점 웨폰스 위치
    Vector2 targetDir;              //커서 위치
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

    protected Vector2 GetFireDirection()   //플레이어에서 전달 커서의 위치
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
