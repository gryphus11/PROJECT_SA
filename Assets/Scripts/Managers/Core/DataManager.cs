using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;
using System.Reflection;
using System;
using Unity.VisualScripting.FullSerializer;
using Data;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.CreatureData> CreatureDic { get; private set; } = new Dictionary<int, Data.CreatureData>();
    public Dictionary<int, Data.DropItemData> DropDic { get; private set; } = new Dictionary<int, Data.DropItemData>();
    public Dictionary<int, List<WaveData>> WaveDic { get; private set; } = new Dictionary<int, List<WaveData>>();
    public Dictionary<int, Data.LevelUpExpData> LevelUpExpDic { get; private set; } = new Dictionary<int, Data.LevelUpExpData>();
    public Dictionary<int, Data.SkillData> SkillDic { get; private set; } = new Dictionary<int, Data.SkillData>();

    public void Init()
    {
        CreatureDic     = LoadJson<Data.CreatureDataLoader, int, Data.CreatureData>("CreatureTable").MakeDict();
        DropDic         = LoadJson<Data.DropItemDataLoader, int, Data.DropItemData>("DropItemTable").MakeDict();
        WaveDic         = LoadJson<Data.WaveDataLoader, int, List<WaveData>>("WaveTable").MakeDict();
        LevelUpExpDic   = LoadJson<Data.LevelUpExpDataLoader, int, Data.LevelUpExpData>("LevelUpExpTable").MakeDict();
        SkillDic        = LoadJson<Data.SkillDataLoader, int, Data.SkillData>("SkillTable").MakeDict();
    }

    #region Json
    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
    #endregion

    #region XML
    Item LoadSingleXml<Item>(string name)
    {
        XmlSerializer xs = new XmlSerializer(typeof(Item));
        TextAsset textAsset = Managers.Resource.Load<TextAsset>(name);
        using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(textAsset.text)))
            return (Item)xs.Deserialize(stream);
    }

    Loader LoadXml<Loader, Key, Item>(string name) where Loader : ILoader<Key, Item>, new()
    {
        XmlSerializer xs = new XmlSerializer(typeof(Loader));
        TextAsset textAsset = Managers.Resource.Load<TextAsset>(name);
        using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(textAsset.text)))
            return (Loader)xs.Deserialize(stream);
    }
    #endregion

    #region ScriptableObject
    Loader LoadScriptable<Loader, Key, Value>(string path) where Loader : ScriptableObject, ILoader<Key, Value>
    {
        return Managers.Resource.Load<Loader>($"{path}");
    }
    #endregion
}
