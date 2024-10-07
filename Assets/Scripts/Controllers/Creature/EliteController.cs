using UnityEngine;
using static Define;

public class EliteController : MonsterController
{
    protected override void OnEnable()
    {
        base.OnEnable();

        if (DataId != 0)
            SetInfo(DataId);
    }

    public override bool Init()
    {
        base.Init();

        _rigidBody.simulated = true;
        transform.localScale = new Vector3(2f, 2f, 2f);

        ObjectType = ObjectType.Elite;
        return true;
    }

    public void Start()
    {
        Init();
        InvokeMonsterData();
    }

    protected override void OnDead()
    {
        base.OnDead();
        DropItem();
    }

    public override void OnDamaged(BaseController attacker, SkillBase skill = null, float damage = 0)
    {
        base.OnDamaged(attacker, skill, damage);
    }

    void DropItem()
    {
        int i = 0;
        foreach (int id in Managers.Game.CurrentWaveData.EliteDropItemId)
        {
            Data.DropItemData dropItem;
            if (Managers.Data.DropDic.TryGetValue(id, out dropItem) == true)
            {
                int dropCount = Managers.Game.CurrentWaveData.EliteDropItemId.Count;
                
                float angleInterval = 360f / dropCount;
                Vector3 dropPos;

                if (dropCount < 2)
                    dropPos = transform.position;
                else
                    dropPos = CalculateDropPotion(angleInterval * i);

                switch (dropItem.DropItemType)
                {
                    case DropItemType.Magnet:
                        Managers.Object.Spawn<MagnetController>(dropPos).SetInfo(dropItem);
                        break;
                    case DropItemType.DropBox:
                    case DropItemType.Potion:
                    case DropItemType.Bomb:
                        break;
                }
                i++;
            }
        }
    }

    Vector3 CalculateDropPotion(float angle)
    {
        float dropDistance = Random.Range(1f, 2f);

        Vector3 dropPos = transform.position;

        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = Mathf.Sin(angle * Mathf.Deg2Rad);
        Vector3 offset = new Vector3(x, y, 0f) * dropDistance;
        Vector3 pos = dropPos + offset;

        return pos;
    }
}