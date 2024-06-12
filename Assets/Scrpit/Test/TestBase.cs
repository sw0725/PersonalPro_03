using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestBase : MonoBehaviour
{
    TestInputAction action;
    public int seed = -1;
    const int allRandom = -1;

    private void Awake()
    {
        action = new TestInputAction();

        if (seed != allRandom) 
        {
            UnityEngine.Random.InitState(seed);
        }
    }

    private void OnEnable()
    {
        action.Test.Enable();
        action.Test.Test1.performed += OnTest1;
        action.Test.Test2.performed += OnTest2;
        action.Test.Test3.performed += OnTest3;
        action.Test.Test4.performed += OnTest4;
        action.Test.Test5.performed += OnTest5;
        action.Test.LClick.performed += OnLClick;
        action.Test.RClick.performed += OnRClick;
    }

    private void OnDisable()
    {
        action.Test.Test1.performed -= OnTest1;
        action.Test.Test2.performed -= OnTest2;
        action.Test.Test3.performed -= OnTest3;
        action.Test.Test4.performed -= OnTest4;
        action.Test.Test5.performed -= OnTest5;
        action.Test.LClick.performed -= OnLClick;
        action.Test.RClick.performed -= OnRClick;
        action.Test.Disable();
    }

    protected virtual void OnRClick(InputAction.CallbackContext context)
    {
    }

    protected virtual void OnLClick(InputAction.CallbackContext context)
    {
    }

    protected virtual void OnTest5(InputAction.CallbackContext context)
    {
    }

    protected virtual void OnTest4(InputAction.CallbackContext context)
    {
    }

    protected virtual void OnTest3(InputAction.CallbackContext context)
    {
    }

    protected virtual void OnTest2(InputAction.CallbackContext context)
    {
    }

    protected virtual void OnTest1(InputAction.CallbackContext context)
    {
    }

}
