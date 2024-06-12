using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolObjectType
{
    Slime= 0,
}

public class Factory : Singleton<Factory>
{
    //SlimePool slimePool;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        //slimePool = GetComponentInChildren<SlimePool>();
        //if (slimePool != null) slimePool.Initialize();
    }

    public GameObject GetObject(PoolObjectType type, Vector3? position = null, Vector3? euler = null)
    {
        GameObject result = null;
        switch (type)
        {
            case PoolObjectType.Slime:
                //result = slimePool.GetObject(position, euler).gameObject;
                break;
        }

        return result;
    }

    //public Slime GetSlime()
    //{
    //    return slimePool.GetObject();
    //}

    //public Slime GetSlime(Vector3 position, float angle = 0.0f)
    //{
    //    return slimePool.GetObject(position, angle * Vector3.forward);
    //}
}