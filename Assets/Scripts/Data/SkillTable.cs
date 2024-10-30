using System.Collections.Generic;
using System;
using UnityEngine;
using static Define;

namespace Data
{
    [Serializable]
    public class SkillData
    {
        public int DataId;
        public string Name;
        public string Description;
        public string PrefabLabel; //프리팹 경로
        public string IconLabel;//아이콘 경로
        public string SoundLabel;// 발동사운드 경로
        public string Category;//스킬 카테고리
        public float CoolTime; // 쿨타임
        public float DamageMultiplier; //스킬데미지 (곱하기)
        public float ProjectileSpacing;// 발사체 사이 간격
        public float Duration; //스킬 지속시간
        public float RecognitionRange;//인식범위
        public int NumProjectiles;// 회당 공격횟수
        public string CastingSound; // 시전사운드
        public float AngleBetweenProject;// 발사체 사이 각도
        public float AttackInterval; //공격간격
        public int NumBounce;//바운스 횟수
        public float BounceSpeed;// 바운스 속도
        public float BounceDistance;//바운스 거리
        public int NumPenetrations; //관통 횟수
        public string HitSoundLabel; // 히트사운드
        public float ProjectileRange; //투사체 사거리
        public float MinCoverage; //최소 효과 적용 범위
        public float MaxCoverage; // 최대 효과 적용 범위
        public float RotateSpeed; // 회전 속도
        public float ProjectileSpeed; //발사체 속도
        public float ScaleMultiplier;

        public static SkillType GetSkillTypeFromInt(int value)
        {
            foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
            {
                int minValue = (int)skillType;
                int maxValue = minValue + 5; // 100501~ 100506 사이 값이면 100501값 리턴

                if (value >= minValue && value <= maxValue)
                {
                    return skillType;
                }
            }

            Debug.LogError($" Get skill Type Failed : {value}");
            return SkillType.None;
        }
    }


    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
                dict.Add(skill.DataId, skill);
            return dict;
        }
    }
}