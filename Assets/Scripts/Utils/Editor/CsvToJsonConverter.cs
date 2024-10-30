using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Data;
using System.ComponentModel;
using static Define;

public static class CsvToJsonConverter
{
    private const string CsvFolderPath = "/@Resources/Data/Csv/";
    private const string JsonFolderPath = "/@Resources/Data/Json/";

    private const string CsvName = "Table.csv";
    private const string JsonName = "Table.json";

#if UNITY_EDITOR
    [MenuItem("Tools/ParseExcel %#K")]
    public static void ParseExcel()
    {
        ParseSkillData("Skill");
        ParseWaveData("Wave");
        ParseCreatureData("Creature");
        ParseLevelData("LevelUpExp");
        ParseDropItemData("DropItem");
        ParseSupportSkillData("SupportSkill");
        ParseWeaponUpgradeData("UpgradeWeapon");

        Debug.Log("Complete DataTransformer");
    }

    private static void ParseWeaponUpgradeData(string filename)
    {
        WeaponUpgradeDataLoader loader = new WeaponUpgradeDataLoader();

        #region ExcelData
        string[] lines = File.ReadAllText($"{Application.dataPath}{CsvFolderPath}{filename}{CsvName}").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;
            
            WeaponUpgradeData upgradeData = new WeaponUpgradeData();
            upgradeData.DataId = ConvertValue<int>(row[i++]);
            upgradeData.RequireWeaponsID = ConvertList<int>(row[i++]);
            upgradeData.RequireSupportsID = ConvertList<int>(row[i++]);
            upgradeData.ResultWeaponID = ConvertValue<int>(row[i++]);

            loader.upgrades.Add(upgradeData);
        }
        #endregion

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}{JsonFolderPath}{filename}{JsonName}", jsonStr);
        AssetDatabase.Refresh();
    }

    private static void ParseSupportSkillData(string filename)
    {
        SupportSkillDataLoader loader = new SupportSkillDataLoader();

        #region ExcelData
        string[] lines = File.ReadAllText($"{Application.dataPath}{CsvFolderPath}{filename}{CsvName}").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;
            SupportSkillData skillData = new SupportSkillData();
            skillData.DataId = ConvertValue<int>(row[i++]);
            skillData.SupportSkillType = ConvertValue<SupportSkillType>(row[i++]);
            skillData.SupportSkillName = ConvertValue<SupportSkillName>(row[i++]);
            skillData.Name = ConvertValue<string>(row[i++]);
            skillData.Description = ConvertValue<string>(row[i++]);
            skillData.IconLabel = ConvertValue<string>(row[i++]);
            skillData.HpRegen = ConvertValue<float>(row[i++]);
            skillData.HealRate = ConvertValue<float>(row[i++]);
            skillData.HealBonusRate = ConvertValue<float>(row[i++]);
            skillData.MagneticRange = ConvertValue<float>(row[i++]);
            skillData.SoulAmount = ConvertValue<int>(row[i++]);
            skillData.HpRate = ConvertValue<float>(row[i++]);
            skillData.AtkRate = ConvertValue<float>(row[i++]);
            skillData.DefRate = ConvertValue<float>(row[i++]);
            skillData.MoveSpeedRate = ConvertValue<float>(row[i++]);
            skillData.CriRate = ConvertValue<float>(row[i++]);
            skillData.CriDmg = ConvertValue<float>(row[i++]);
            skillData.DamageReduction = ConvertValue<float>(row[i++]);
            skillData.ExpBonusRate = ConvertValue<float>(row[i++]);
            skillData.SoulBonusRate = ConvertValue<float>(row[i++]);
            skillData.ProjectileSpacing = ConvertValue<float>(row[i++]);
            skillData.Duration = ConvertValue<float>(row[i++]);
            skillData.NumProjectiles = ConvertValue<int>(row[i++]);
            skillData.AttackInterval = ConvertValue<float>(row[i++]);
            skillData.NumBounce = ConvertValue<int>(row[i++]);
            skillData.NumPenetrations = ConvertValue<int>(row[i++]);
            skillData.ProjectileRange = ConvertValue<float>(row[i++]);
            skillData.RotateSpeed = ConvertValue<float>(row[i++]);
            skillData.ScaleMultiplier = ConvertValue<float>(row[i++]);
            skillData.Price = ConvertValue<float>(row[i++]);
            loader.supportSkills.Add(skillData);
        }
        #endregion

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}{JsonFolderPath}{filename}{JsonName}", jsonStr);
        AssetDatabase.Refresh();
    }

    static void ParseSkillData(string filename)
    {
        SkillDataLoader loader = new SkillDataLoader();

        #region ExcelData
        string[] lines = File.ReadAllText($"{Application.dataPath}{CsvFolderPath}{filename}{CsvName}").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;
            SkillData skillData = new SkillData();
            skillData.DataId = ConvertValue<int>(row[i++]);
            skillData.Name = ConvertValue<string>(row[i++]);
            skillData.Description = ConvertValue<string>(row[i++]);
            skillData.PrefabLabel = ConvertValue<string>(row[i++]);
            skillData.IconLabel = ConvertValue<string>(row[i++]);
            skillData.SoundLabel = ConvertValue<string>(row[i++]);
            skillData.Category = ConvertValue<string>(row[i++]);
            skillData.CoolTime = ConvertValue<float>(row[i++]);
            skillData.DamageMultiplier = ConvertValue<float>(row[i++]);
            skillData.ProjectileSpacing = ConvertValue<float>(row[i++]);
            skillData.Duration = ConvertValue<float>(row[i++]);
            skillData.RecognitionRange = ConvertValue<float>(row[i++]);
            skillData.NumProjectiles = ConvertValue<int>(row[i++]);
            skillData.CastingSound = ConvertValue<string>(row[i++]);
            skillData.AngleBetweenProject = ConvertValue<float>(row[i++]);
            skillData.AttackInterval = ConvertValue<float>(row[i++]);
            skillData.NumBounce = ConvertValue<int>(row[i++]);
            skillData.BounceSpeed = ConvertValue<float>(row[i++]);
            skillData.BounceDistance = ConvertValue<float>(row[i++]);
            skillData.NumPenetrations = ConvertValue<int>(row[i++]);
            skillData.HitSoundLabel = ConvertValue<string>(row[i++]);
            skillData.ProjectileRange = ConvertValue<float>(row[i++]);
            skillData.MinCoverage = ConvertValue<float>(row[i++]);
            skillData.MaxCoverage = ConvertValue<float>(row[i++]);
            skillData.RotateSpeed = ConvertValue<float>(row[i++]);
            skillData.ProjectileSpeed = ConvertValue<float>(row[i++]);
            skillData.ScaleMultiplier = ConvertValue<float>(row[i++]);
            loader.skills.Add(skillData);
        }
        #endregion

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}{JsonFolderPath}{filename}{JsonName}", jsonStr);
        AssetDatabase.Refresh();
    }

    static void ParseWaveData(string filename)
    {
        WaveDataLoader loader = new WaveDataLoader();

        #region ExcelData
        string[] lines = File.ReadAllText($"{Application.dataPath}{CsvFolderPath}{filename}{CsvName}").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;

            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;
            
            WaveData waveData = new WaveData();
            waveData.StageIndex = ConvertValue<int>(row[i++]);
            waveData.WaveIndex = ConvertValue<int>(row[i++]);
            waveData.SpawnInterval = ConvertValue<float>(row[i++]);
            waveData.OnceSpawnCount = ConvertValue<int>(row[i++]);
            waveData.MonsterId = ConvertList<int>(row[i++]);
            waveData.EliteId = ConvertList<int>(row[i++]);
            waveData.RemainsTime = ConvertValue<float>(row[i++]);
            waveData.FirstMonsterSpawnRate = ConvertValue<float>(row[i++]);
            waveData.HpIncreaseRate = ConvertValue<float>(row[i++]);
            waveData.nonDropRate = ConvertValue<float>(row[i++]);
            waveData.SmallGemDropRate = ConvertValue<float>(row[i++]);
            waveData.GreenGemDropRate = ConvertValue<float>(row[i++]);
            waveData.BlueGemDropRate = ConvertValue<float>(row[i++]);
            waveData.YellowGemDropRate = ConvertValue<float>(row[i++]);
            waveData.EliteDropItemId = ConvertList<int>(row[i++]);

            loader.waves.Add(waveData);
        }
        #endregion

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}{JsonFolderPath}{filename}{JsonName}", jsonStr);
        AssetDatabase.Refresh();
    }
    
    static void ParseCreatureData(string filename)
    {
        CreatureDataLoader loader = new CreatureDataLoader();

        #region ExcelData
        string[] lines = File.ReadAllText($"{Application.dataPath}{CsvFolderPath}{filename}{CsvName}").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');

            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;
            CreatureData cd = new CreatureData();
            cd.dataId = ConvertValue<int>(row[i++]);
            cd.descriptionTextID = ConvertValue<string>(row[i++]);
            cd.prefabLabel = ConvertValue<string>(row[i++]);
            
            cd.maxHp = ConvertValue<float>(row[i++]);
            cd.upMaxHp = ConvertValue<float>(row[i++]);
            
            cd.atk = ConvertValue<float>(row[i++]);
            cd.upAtk = ConvertValue<float>(row[i++]);
            
            cd.def = ConvertValue<float>(row[i++]);
            cd.moveSpeed = ConvertValue<float>(row[i++]);
            cd.totalExp = ConvertValue<float>(row[i++]);
            cd.hpRate = ConvertValue<float>(row[i++]);
            cd.atkRate = ConvertValue<float>(row[i++]);
            cd.defRate = ConvertValue<float>(row[i++]);
            cd.moveSpeedRate = ConvertValue<float>(row[i++]);

            cd.animationLabels = ConvertList<string>(row[i++]);
            cd.iconLabel = ConvertValue<string>(row[i++]);
            
            cd.learnableSkill= ConvertList<int>(row[i++]);
            cd.defaultSkill = ConvertValue<int>(row[i++]);

            loader.creatures.Add(cd);
        }

        #endregion

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}{JsonFolderPath}{filename}{JsonName}", jsonStr);
        AssetDatabase.Refresh();
    }
    
    static void ParseLevelData(string filename)
    {
        LevelUpExpDataLoader loader = new LevelUpExpDataLoader();

        #region ExcelData
        string[] lines = File.ReadAllText($"{Application.dataPath}{CsvFolderPath}{filename}{CsvName}").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');

            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;
            LevelUpExpData data = new LevelUpExpData();
            data.Level = ConvertValue<int>(row[i++]);
            data.TotalExp = ConvertValue<int>(row[i++]);
            loader.levels.Add(data);
        }

        #endregion

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}{JsonFolderPath}{filename}{JsonName}", jsonStr);
        AssetDatabase.Refresh();
    }

    static void ParseDropItemData(string filename)
    {
        DropItemDataLoader loader = new DropItemDataLoader();

        #region ExcelData
        string[] lines = File.ReadAllText($"{Application.dataPath}{CsvFolderPath}{filename}{CsvName}").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;

            DropItemData dropItem = new DropItemData();
            dropItem.DataId = ConvertValue<int>(row[i++]);
            dropItem.DropItemType = ConvertValue<Define.DropItemType>(row[i++]);
            dropItem.NameTextID = ConvertValue<string>(row[i++]);
            dropItem.DescriptionTextID = ConvertValue<string>(row[i++]);
            dropItem.SpriteName = ConvertValue<string>(row[i++]);

            loader.DropItems.Add(dropItem);
        }
        #endregion

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}{JsonFolderPath}{filename}{JsonName}", jsonStr);
        AssetDatabase.Refresh();
    }

    public static T ConvertValue<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
            return default(T);

        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        return (T)converter.ConvertFromString(value);
    }

    public static List<T> ConvertList<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
            return new List<T>();

        return value.Split('|').Select(x => ConvertValue<T>(x)).ToList();
    }
#endif
}
