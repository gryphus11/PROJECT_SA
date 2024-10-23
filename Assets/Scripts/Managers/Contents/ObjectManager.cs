using Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            GameObject go = Managers.Resource.Instantiate(creatureData.prefabLabel, pooling: true);
            go.transform.position = position;
            go.name = creatureData.prefabLabel;
            MonsterController mc = go.GetOrAddComponent<MonsterController>();
            mc.SetInfo(templateID);
            Monsters.Add(mc);

            return mc as T;
        }
        else if (type == typeof(EliteController))
        {
            var creatureData = Managers.Data.CreatureDic[templateID];
            GameObject go = Managers.Resource.Instantiate($"{creatureData.prefabLabel}", pooling: true);
            EliteController mc = go.GetOrAddComponent<EliteController>();
            go.transform.position = position;
            mc.SetInfo(templateID);
            go.name = creatureData.prefabLabel;
            Monsters.Add(mc);

            return mc as T;
        }
        else if (type == typeof(GemController))
        {
            GameObject go = Managers.Resource.Instantiate("Gem", pooling: true);
            GemController gc = go.GetOrAddComponent<GemController>();
            go.transform.position = position;
            Gems.Add(gc);
            Managers.Game.CurrentMap.Grid.Add(gc);

            return gc as T;
        }
        else if (type == typeof(MagnetController))
        {
            GameObject go = Managers.Resource.Instantiate("Magnet", pooling: true);
            MagnetController mc = go.GetOrAddComponent<MagnetController>();
            go.transform.position = position;
            DropItems.Add(mc);
            Managers.Game.CurrentMap.Grid.Add(mc);

            return mc as T;
        }
        else if (type == typeof(ProjectileController))
        {
            GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
            ProjectileController pc = go.GetOrAddComponent<ProjectileController>();
            go.transform.position = position;
            Projectiles.Add(pc);

            return pc as T;
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
        else if (type == typeof(EliteController))
        {
            Monsters.Remove(obj as EliteController);
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (type == typeof(GemController))
        {
            Gems.Remove(obj as GemController);
            Managers.Resource.Destroy(obj.gameObject);
            Managers.Game.CurrentMap.Grid.Remove(obj as GemController); 
        }
        else if (type == typeof(MagnetController))
        {
            Managers.Resource.Destroy(obj.gameObject);
            Managers.Game.CurrentMap.Grid.Remove(obj as MagnetController);
        }
        else if (type == typeof(ProjectileController))
        {
            Projectiles.Remove(obj as ProjectileController);
            Managers.Resource.Destroy(obj.gameObject);
        }
    }

    public List<MonsterController> GetMonsterWithinCamera(int count = 1)
    {
        List<MonsterController> monsterList = Monsters.ToList().Where(monster => IsWithInCamera(Camera.main.WorldToViewportPoint(monster.CenterPosition)) == true).ToList();
        monsterList.Shuffle();

        int min = Mathf.Min(count, monsterList.Count);

        List<MonsterController> monsters = monsterList.Take(min).ToList();

        if (monsters.Count == 0) return null;

        while (monsters.Count < count)
        {
            monsters.Add(monsters.Last());
        }

        return monsterList.Take(count).ToList();
    }

    bool IsWithInCamera(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1)
            return true;
        return false;
    }

    public void ShowDamageFont(Vector2 pos, float damage, float healAmount, Transform parent, bool isCritical = false)
    {
        GameObject go = Managers.Resource.Instantiate("DamageFont", pooling: true);
        DamageFont damageText = go.GetOrAddComponent<DamageFont>();
        damageText.SetInfo(pos, damage, healAmount, parent, isCritical);
    }

    public void CollectAllItems()
    {
        foreach (GemController gem in Gems.ToList())
        {
            gem.GetItem();
        }
    }

    public List<Transform> GetFindMonstersInFanShape(Vector3 origin, Vector3 forward, float radius = 2, float angleRange = 80)
    {
        List<Transform> listMonster = new List<Transform>();
        LayerMask targetLayer = LayerMask.GetMask("Monster", "Boss");
        RaycastHit2D[] _targets = Physics2D.CircleCastAll(origin, radius, Vector2.zero, 0, targetLayer);

        // 타겟중에 부채꼴 안에 있는것만 리스트에 넣는다.
        foreach (RaycastHit2D target in _targets)
        {
            // '타겟-origin 벡터'와 '내 정면 벡터'를 내적
            float dot = Vector3.Dot((target.transform.position - origin).normalized, forward);
            // 두 벡터 모두 단위 벡터이므로 내적 결과에 cos의 역을 취해서 theta를 구함
            float theta = Mathf.Acos(dot);
            // angleRange와 비교하기 위해 degree로 변환
            float degree = Mathf.Rad2Deg * theta;
            // 시야각 판별
            if (degree <= angleRange / 2f)
                listMonster.Add(target.transform);
        }

        return listMonster;
    }

    public void Clear()
    { }
}
