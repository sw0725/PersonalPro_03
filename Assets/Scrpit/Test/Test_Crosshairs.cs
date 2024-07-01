using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Crosshairs : TestBase
{
    public Crosshair crosshair;
    public float recoil;

    protected override void OnLClick(InputAction.CallbackContext context)
    {
        crosshair.Expend(recoil);
    }
}
