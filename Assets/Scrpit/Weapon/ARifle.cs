using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARifle : WeaponBase
{
    protected override void FireProcess()
    {
        base.FireProcess();
        HitProcess();
        FireRecoil();
    }
}
