using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public enum WeaponType : byte
{
    Bat = 0,
    Pistol,
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
    public float reloadDuration = 1.0f;
    public WeaponType weaponType;

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
                    Reload();
                }
            }
        }
    }
    int bulletCount;

    public Action<int> onBulletCountChange;
    public Action<float> onFire;

    protected bool canFire = true;
    bool isReloading = false;

    protected Transform shoulder;
    Vector2 targetDir;              //커서-중심 벡터

    protected virtual void Awake()
    {
        shoulder = transform.parent;
        bulletCount = clipSize;
    }

    public void Fire(Vector2 targetPos) 
    {
        targetDir = (targetPos - (Vector2)transform.position).normalized;
        Quaternion dir = Quaternion.LookRotation(targetDir, Vector2.up);
        shoulder.rotation = dir;
        if(canFire && BulletCount > 0) 
        {
            FireProcess();
        }
    }

    protected virtual void FireProcess() 
    {
        canFire = false;
        StartCoroutine(FireReady());
        if (weaponType != WeaponType.Bat) 
        {
            BulletCount--;
        }
    }

    IEnumerator FireReady() 
    {
        yield return new WaitForSeconds(fireCoolTime);
        canFire = true;
    }

    protected void HitProcess ()    //에서 벡터로 이펙트 날리기
    {
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, GetFireDirection(), range, LayerMask.GetMask("Enemy"));
        if(hit2D.collider != null) 
        {
            EnemyBase target = hit2D.collider.GetComponent<EnemyBase>();
            if(target != null) 
            {
                target.Attacked(damege);
            }
        }
    }

    protected void FireRecoil() 
    {
        onFire?.Invoke(recoil);
    }

    public void Reload()    //앵두 납치시 사용불가
    {
        if (!isReloading && weaponType != WeaponType.Bat)
        {
            StopAllCoroutines();    //FireProcess 실행시키는 코루틴으로 isFireReady가 true되는것 방지
            isReloading = true;
            canFire = false;
            StartCoroutine(Reloading());
        }
    }

    protected IEnumerator Reloading()   //총마다 찾을 총알 다름
    {
        yield return new WaitForSeconds(reloadDuration);
        canFire = true;
        switch (weaponType) 
        {
            case WeaponType.Bat: break;
            case WeaponType.Pistol:
                BulletRemove(ItemCode.PistolBullet);
                break;
            case WeaponType.ShotGun:
                BulletRemove(ItemCode.ShotGunBullet);
                break;
            case WeaponType.AssaultRifle:
                BulletRemove(ItemCode.AssaultRifleBullet);
                break;
        }
        isReloading = false;
    }

    void BulletRemove(ItemCode code) 
    {
        Inventory inven = Inventory.Instance;
        int needAmmo = clipSize - BulletCount;
        int ammoCount = inven.GetItemCount(code);
        if (ammoCount > needAmmo)
        {
            inven.UseItem(code, needAmmo);
            BulletCount = clipSize;
        }
        else 
        {
            inven.UseItem(code, ammoCount);
            BulletCount += ammoCount;
        }
    }

    protected Vector2 GetFireDirection()
    {
        Vector2 result = targetDir;
        result = Quaternion.Euler(0, 0, Random.Range(-spread, spread)) * result;
        return result;
    }

    public void Equip() 
    {
        onBulletCountChange?.Invoke(bulletCount);
        if (!canFire && BulletCount > 0) 
        {
            StartCoroutine(FireReady());
        }
    }

    public void UnEquip() 
    {
        StopAllCoroutines();
    }


#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Handles.color = Color.yellow;
        Vector2 resulta = Quaternion.Euler(0, 0, -spread) * Vector2.left;
        Vector2 resultb = Quaternion.Euler(0, 0, spread) * Vector2.left;
        Handles.DrawLine((Vector2)transform.position, (Vector2)transform.position + resulta * range);
        Handles.DrawLine((Vector2)transform.position, (Vector2)transform.position + resultb * range);
    }

#endif
}
