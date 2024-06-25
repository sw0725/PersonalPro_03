using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCode : byte
{
    SafeWater = 0,
    DangerWater,
    PollutWater,
    SafeFood,
    DangerFood,
    PollutFood,
    PistolBullet,
    ShotGunBullet,
    AssaultRifleBullet,
    Pill
}

public class Inventory : Singleton<Inventory>
{
    Dictionary<ItemCode, int> items;

    protected override void OnInitialize()
    {
        //items = new Dictionary<ItemCode, int>(ItemCode.);
    }
    //ResidentalMap,
    //IndustrialMap,
    //DowntownMap,
    //MainStreetMap,
    //SewerAgeMap,
    //TowerMap,
    public void AddItem(ItemCode code) 
    {

    }
}
