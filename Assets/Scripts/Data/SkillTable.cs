using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static Define;

namespace Data
{
    #region SkillData - Xml
    public class SkillData
    {
        [XmlAttribute]
        public int templateID;

        [XmlAttribute]
        public string name;

        //[XmlAttribute(AttributeName = "type")]
        //public SkillType skillType;

        //[XmlAttribute]
        //public int PrevID;
        //[XmlAttribute]
        //public int NextID;

        [XmlAttribute]
        public string prefab;

        [XmlAttribute]
        public int damage;

        [XmlAttribute]
        public int speed;
    }

    [Serializable, XmlRoot("SkillDatas")]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        [XmlElement("SkillData")]
        public List<SkillData> skills = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            Dictionary<int, SkillData> playerDataDict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
                playerDataDict.Add(skill.templateID, skill);
            return playerDataDict;
        }
    }
    #endregion
}