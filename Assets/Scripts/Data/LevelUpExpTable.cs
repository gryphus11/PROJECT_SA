using System.Collections.Generic;
using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class LevelUpExpData
    {
        public int Level;
        public int TotalExp;
    }

    [Serializable]
    public class LevelUpExpDataLoader : ILoader<int, LevelUpExpData>
    {
        public List<LevelUpExpData> levels = new List<LevelUpExpData>();
        public Dictionary<int, LevelUpExpData> MakeDict()
        {
            Dictionary<int, LevelUpExpData> dict = new Dictionary<int, LevelUpExpData>();
            foreach (LevelUpExpData levelData in levels)
                dict.Add(levelData.Level, levelData);
            return dict;
        }
    }
}