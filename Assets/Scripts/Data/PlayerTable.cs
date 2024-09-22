using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Data
{
    #region PlayerData - Json
    //[Serializable]
    //public class PlayerData
    //{
    //    public int level;

    //    public int maxHp;

    //    public int attack;

    //    public int totalExp;
    //}

    //[Serializable]
    //public class PlayerDataLoader : ILoader<int, PlayerData>
    //{
    //    // Json인 경우 변수명은 Json의 리스트 이름과 맞춰줄 것
    //    public List<PlayerData> stats = new List<PlayerData>();

    //    public Dictionary<int, PlayerData> MakeDict()
    //    {
    //        Dictionary<int, PlayerData> playerDataDict = new Dictionary<int, PlayerData>();

    //        foreach (var playerData in stats)
    //            playerDataDict.Add(playerData.level, playerData);

    //        return playerDataDict;
    //    }
    //}
    #endregion

    #region PlayerData - Xml
    public class PlayerData
    {
        [XmlAttribute]
        public int level;
        [XmlAttribute]
        public int maxHp;
        [XmlAttribute]
        public int attack;
        [XmlAttribute]
        public int totalExp;
    }

    [Serializable, XmlRoot("PlayerDatas")]
    public class PlayerDataLoader : ILoader<int, PlayerData>
    {
        [XmlElement("PlayerData")]
        public List<PlayerData> stats = new List<PlayerData>();

        public Dictionary<int, PlayerData> MakeDict()
        {
            Dictionary<int, PlayerData> playerDataDict = new Dictionary<int, PlayerData>();
            foreach (PlayerData stat in stats)
                playerDataDict.Add(stat.level, stat);
            return playerDataDict;
        }
    }
    #endregion
}