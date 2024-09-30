using System.Collections.Generic;
using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class CreatureData
    {
        public int dataId;
        public string descriptionTextID;
        public string prefabLabel;

        public float maxHp = 100.0f;
        public float upMaxHp = 0.0f;
        
        public float atk = 1.0f;
        public float upAtk = 1.0f; 

        public float def = 50.0f;
        public float moveSpeed = 3.0f;

        public float totalExp = 3.0f;

        public float hpRate;
        public float atkRate;
        public float defRate;
        public float moveSpeedRate ;
        public string animationLabels;
        public string iconLabel;
        public int skill;
    }

    [Serializable]
    [CreateAssetMenu(fileName = "CreatureTable", menuName = "Make Table/CreatureTable")]
    public class CreatureDataLoader : ScriptableObject, ILoader<int, CreatureData>
    {
        public List<CreatureData> creatures = new List<CreatureData>();
        public Dictionary<int, CreatureData> MakeDict()
        {
            Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
            foreach (CreatureData creature in creatures)
                dict.Add(creature.dataId, creature);
            return dict;
        }
    }
}