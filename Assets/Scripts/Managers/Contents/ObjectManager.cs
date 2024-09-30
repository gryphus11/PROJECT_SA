using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

/// <summary>
/// 오브젝트의 스폰, 디스폰을 관리
/// </summary>
public class ObjectManager
{
    public PlayerController Player { get; private set; }

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
            return null;
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

    public void Clear()
    { }
}
