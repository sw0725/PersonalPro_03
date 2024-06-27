using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : WeaponBase
{
    protected override void FireProcess()
    {
        base.FireProcess();
        HitProcess();
        FireRecoil();
    }
}
