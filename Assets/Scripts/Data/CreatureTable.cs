using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Data
{
    [System.Serializable]
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
        public string iconLabel;
        public List<string> animationLabels;
        public List<int> learnableSkill;
        public int defaultSkill;

        public static List<CreatureData> GetPlayerCreatureData() 
        {
            List<CreatureData> playerCharacters = new List<CreatureData>();

            var creatures = Managers.Data.CreatureDic;

            foreach ( var creature in creatures ) 
            {
                if (creature.Key < 200000)
                {
                    playerCharacters.Add(creature.Value);
                }
            }

            return playerCharacters;
        }
    }

    [System.Serializable]
    public class CreatureDataLoader : ILoader<int, CreatureData>
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