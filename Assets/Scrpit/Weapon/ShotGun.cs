using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : WeaponBase
{
    public int pellet = 6;
    protected override void FireProcess()
    {
        base.FireProcess();
        for (int i = 0; i < pellet; i++) 
        {
            HitProcess();
        }
        FireRecoil();
    }
}
