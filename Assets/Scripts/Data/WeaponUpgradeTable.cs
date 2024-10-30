using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

namespace Data
{
    [Serializable]
    public class WeaponUpgradeData
    {
        public int DataId;
        public List<int> RequireWeaponsID;
        public List<int> RequireSupportsID;
        public int ResultWeaponID;
    }

    [Serializable]
    public class WeaponUpgradeDataLoader : ILoader<int, WeaponUpgradeData>
    {
        public List<WeaponUpgradeData> upgrades = new List<WeaponUpgradeData>();

        public Dictionary<int, WeaponUpgradeData> MakeDict()
        {
            Dictionary<int, WeaponUpgradeData> dict = new Dictionary<int, WeaponUpgradeData>();
            foreach (WeaponUpgradeData skill in upgrades)
                dict.Add(skill.DataId, skill);
            return dict;
        }
    }
}