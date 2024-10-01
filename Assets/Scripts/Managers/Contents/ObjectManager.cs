using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Define;

/// <summary>
/// 오브젝트의 스폰, 디스폰을 관리
/// </summary>
public class ObjectManager
{
    public PlayerController Player { get; private set; }
    public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
    public HashSet<GemController> Gems { get; } = new HashSet<GemController>();
    public HashSet<DropItemController> DropItems { get; } = new HashSet<DropItemController>();
    public HashSet<ProjectileController> Projectiles { get; } = new HashSet<ProjectileController>();

    public T Spawn<T>(Vector3 position, int templateID = 0, string prefabName = "") where T : BaseController
    {
        System.Type type = typeof(T);

        if (type == typeof(PlayerController))
        {
            GameObject go = Managers.Resource.Instantiate(Managers.Data.CreatureDic[templateID].prefabLabel);
            go.transform.position = position;
            PlayerController pc = go.GetOrAddComponent<PlayerController>();
            pc.SetInfo(templateID);
            Player = pc;
            Managers.Game.Player = pc;

            return pc as T;
        }
        else if (type == typeof(MonsterController))
        {
            var creatureData = Managers.Data.CreatureDic[templateID];
            GameObject go = Managers.Resource.Instantiate(creatureData.prefabLabel);
            go.transform.position = position;
            go.name = creatureData.prefabLabel;
            MonsterController mc = go.GetOrAddComponent<MonsterController>();
            mc.SetInfo(templateID);
            Monsters.Add(mc);

            return mc as T;
        }
        else if (type == typeof(GemController))
        {
            return null;
        }
        else if (type == typeof(PotionController))
        {
            return null;
        }
        else if (type == typeof(BombController))
        {
            return null;
        }
        else if (type == typeof(MagnetController))
        {
            return null;
        }
        else if (type == typeof(ProjectileController))
        {
            return null;
        }

        return null;
    }

    public void Despawn<T>(T obj) where T : BaseController
    {
        System.Type type = typeof(T);

        if (type == typeof(PlayerController))
        {
            // ?
        }

        else if (type == typeof(MonsterController))
        {
            Monsters.Remove(obj as MonsterController);
            Managers.Resource.Destroy(obj.gameObject);
        }
    }

    public void Clear()
    { }
}
