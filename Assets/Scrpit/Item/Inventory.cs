using System;
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

public enum Maps : byte
{
    ResidentalMap = 0,
    IndustrialMap,
    DowntownMap,
    MainStreetMap,
    SewerAgeMap,
    TowerMap,
}

public enum SpecialItem : byte 
{
    Hint = 0,
    BoltCutter,
    Bomb,
    PoliceKey,
    Light,
    Pass,
    CleanerKey,
    FamilyPhoto,
    SnorkelMask,
    CardKey
}

public class Inventory : Singleton<Inventory>
{
    Dictionary<ItemCode, int> items;
    Dictionary<Maps, bool> maps;
    Dictionary<SpecialItem, bool> specials;
    Dictionary<WeaponType, bool> weapons;

    public Action<Maps> GetMap;
    public Action<WeaponType> GetWeapon;

    protected override void OnInitialize()
    {
        items = new Dictionary<ItemCode, int>
        {
            { ItemCode.SafeWater, 0 },
            { ItemCode.DangerWater, 0 },
            { ItemCode.PollutWater, 0 },
            { ItemCode.SafeFood, 0 },
            { ItemCode.DangerFood, 0 },
            { ItemCode.PollutFood, 0 },
            { ItemCode.PistolBullet, 0 },
            { ItemCode.ShotGunBullet, 0 },
            { ItemCode.AssaultRifleBullet, 0 },
            { ItemCode.Pill, 0 }
        };

        maps = new Dictionary<Maps, bool>
        {
            { Maps.ResidentalMap, false},
            { Maps.IndustrialMap, false},
            { Maps.DowntownMap, false},
            { Maps.MainStreetMap, false},
            { Maps.SewerAgeMap, false},
            { Maps.TowerMap, false},
        };

        specials = new Dictionary<SpecialItem, bool>
        {
            { SpecialItem.Hint, false},
            { SpecialItem.BoltCutter, false},
            { SpecialItem.Bomb, false},
            { SpecialItem.PoliceKey, false},
            { SpecialItem.Light, false},
            { SpecialItem.Pass, false},
            { SpecialItem.CleanerKey, false},
            { SpecialItem.FamilyPhoto, false},
            { SpecialItem.SnorkelMask, false},
            { SpecialItem.CardKey, false},
        };

        weapons = new Dictionary<WeaponType, bool>
        {
            { WeaponType.Bat, true},
            { WeaponType.Pistol, false},
            { WeaponType.ShotGun, false},
            { WeaponType.AssaultRifle, false},
        };
    }
    
    public void AddItem(ItemCode code) 
    {
        if (items.ContainsKey(code)) 
        {
            items[code]++;
        }
    }

    public void UseItem(ItemCode code, int count = 1) 
    {
        if (items.ContainsKey(code))
        {
            if (items[code] > 0)
            {
                items[code] -= count;
            }
        }
    }

    public int GetItemCount(ItemCode code) 
    {
        if (items.ContainsKey(code))
        {
            return items[code];
        }
        return 0;
    }

    public void AddMap(Maps map) 
    {
        if (maps.ContainsKey(map))
        {
            maps[map] = true;
            GetMap?.Invoke(map);
        }
    }

    public bool HasMap(Maps map) 
    {
        if (maps.ContainsKey(map))
        {
            return maps[map];
        }
        return false;
    }

    public void AddSpecial(SpecialItem special)
    {
        specials[special] = true;
    }

    public bool HasSpecial(SpecialItem special)
    {
        return specials[special];
    }

    public void AddWeapon(WeaponType weapon) 
    {
        weapons[weapon] = true;
        GetWeapon?.Invoke(weapon);
    }

    public bool HasWeapon(WeaponType weapon)
    {
        return weapons[weapon];
    }
}
