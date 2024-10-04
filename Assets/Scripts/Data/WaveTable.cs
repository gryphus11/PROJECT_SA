using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class WaveData
    {
        public int StageIndex = 1;
        public int WaveIndex = 1;
        public float SpawnInterval = 0.5f;
        public int OnceSpawnCount;
        public List<int> MonsterId;
        public List<int> EleteId;
        public List<int> BossId;
        public float RemainsTime;
        public float FirstMonsterSpawnRate;
        public float HpIncreaseRate;
        public float nonDropRate;
        public float SmallGemDropRate;
        public float GreenGemDropRate;
        public float BlueGemDropRate;
        public float YellowGemDropRate;
        public List<int> EliteDropItemId = new List<int>();
    }

    [Serializable]
    public class WaveDataLoader : ILoader<int, List<WaveData>>
    {
        public List<WaveData> waves = new List<WaveData>();

        public Dictionary<int, List<WaveData>> MakeDict()
        {
            Dictionary<int, List<WaveData>> dict = new Dictionary<int, List<WaveData>>();
            
            foreach (WaveData wave in waves)
            {
                if (dict.ContainsKey(wave.StageIndex))
                {
                    dict[wave.StageIndex].Add(wave);
                }
                else
                {
                    dict.Add(wave.WaveIndex, new List<WaveData>());
                }
            }

            return dict;
        }
    }
}