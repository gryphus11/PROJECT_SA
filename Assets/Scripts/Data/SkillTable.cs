using System.Collections.Generic;
using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class SkillData
    {
        public int DataId;
        public string Name;
        public string Description;
        public string PrefabLabel; //������ ���
        public string IconLabel;//������ ���
        public string SoundLabel;// �ߵ����� ���
        public string Category;//��ų ī�װ�
        public float CoolTime; // ��Ÿ��
        public float DamageMultiplier; //��ų������ (���ϱ�)
        public float ProjectileSpacing;// �߻�ü ���� ����
        public float Duration; //��ų ���ӽð�
        public float RecognitionRange;//�νĹ���
        public int NumProjectiles;// ȸ�� ����Ƚ��
        public string CastingSound; // ��������
        public float AngleBetweenProj;// �߻�ü ���� ����
        public float AttackInterval; //���ݰ���
        public int NumBounce;//�ٿ Ƚ��
        public float BounceSpeed;// �ٿ �ӵ�
        public float BounceDist;//�ٿ �Ÿ�
        public int NumPenerations; //���� Ƚ��
        public string HitSoundLabel; // ��Ʈ����
        public float ProjRange; //����ü ��Ÿ�
        public float MinCoverage; //�ּ� ȿ�� ���� ����
        public float MaxCoverage; // �ִ� ȿ�� ���� ����
        public float RoatateSpeed; // ȸ�� �ӵ�
        public float ProjSpeed; //�߻�ü �ӵ�
        public float ScaleMultiplier;
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