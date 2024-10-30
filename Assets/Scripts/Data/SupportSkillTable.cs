using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

namespace Data
{
    [Serializable]
    public class SupportSkillData
    {
        public int AcquiredLevel;
        public int DataId;
        public Define.SupportSkillType SupportSkillType;
        public Define.SupportSkillName SupportSkillName;
        public string Name;
        public string Description;
        public string IconLabel;
        public float HpRegen;
        public float HealRate; // 회복량 (최대HP%)
        public float HealBonusRate; // 회복량 증가
        public float MagneticRange; // 아이템 습득 범위
        public int SoulAmount; // 영혼획득
        public float HpRate;
        public float AtkRate;
        public float DefRate;
        public float MoveSpeedRate;
        public float CriRate;
        public float CriDmg;
        public float DamageReduction;
        public float ExpBonusRate;
        public float SoulBonusRate;
        public float ProjectileSpacing;// 발사체 사이 간격
        public float Duration; //스킬 지속시간
        public int NumProjectiles;// 회당 공격횟수
        public float AttackInterval; //공격간격
        public int NumBounce;//바운스 횟수
        public int NumPenetrations; //관통 횟수
        public float ProjectileRange; //투사체 사거리
        public float RotateSpeed; // 회전 속도
        public float ScaleMultiplier;
        public float Price;
        public bool IsLocked = false;
        public bool IsPurchased = false;
    }

    [Serializable]
    public class SupportSkillDataLoader : ILoader<int, SupportSkillData>
    {
        public List<SupportSkillData> supportSkills = new List<SupportSkillData>();

        public Dictionary<int, SupportSkillData> MakeDict()
        {
            Dictionary<int, SupportSkillData> dict = new Dictionary<int, SupportSkillData>();
            foreach (SupportSkillData skill in supportSkills)
                dict.Add(skill.DataId, skill);
            return dict;
        }
    }
}