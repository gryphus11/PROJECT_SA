using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ProjectileController : SkillBase
{
    public SkillBase Skill { get; set; }
    Vector2 _spawnPos;
    Vector3 _dir = Vector3.zero;
    Vector3 _target = Vector3.zero;
    Define.SkillType _skillType;
    Rigidbody2D _rigid;
    int _numPenetrations;
    public int _bounceCount = 1;

    List<CreatureController> _enteredColliderList = new List<CreatureController>();

    public void SetInfo(CreatureController owner, Vector2 position, Vector2 dir, Vector2 target, SkillBase skill)
    {
        Owner = owner;

        _spawnPos = position;
        _dir = dir;
        Skill = skill;
        _rigid = GetComponent<Rigidbody2D>();
        _target = target;
        transform.localScale = Vector3.one * Skill.SkillData.ScaleMultiplier;
        _numPenetrations = skill.SkillData.NumPenetrations;
        _bounceCount = skill.SkillData.NumBounce;


        // ����ü�� Ư���� ���� ������ �ʿ��ϴٸ� ���⿡ ����
        switch (skill.SkillType)
        {
            // �⺻�� Ư�� �������� �߻�
            default:
                transform.rotation = Quaternion.FromToRotation(Vector3.up, _dir);
                _numPenetrations = Skill.SkillData.NumPenetrations;
                _rigid.velocity = _dir * Skill.SkillData.ProjectileSpeed;
                break;
        }

        if (gameObject.activeInHierarchy)
            DestroyTask().Forget();

    }

    private async UniTask DestroyTask()
    {
        CancellationTokenSource src = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSource.Token, destroyCancellationToken);

        while (true)
        {
            await UniTask.Delay(5000, cancellationToken: src.Token);
            DestroyProjectile();
        }
    }

    public void DestroyProjectile()
    {
        Managers.Object.Despawn(this);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        CreatureController creature = collision.transform.GetComponent<CreatureController>();
        if (creature.IsValid() == false)
            return;
        if (this.IsValid() == false)
            return;

        switch (Skill.SkillType)
        {
            case Define.SkillType.IcicleArrow:
                _numPenetrations--;
                if (_numPenetrations < 0)
                {
                    _rigid.velocity = Vector3.zero;
                    DestroyProjectile();
                }
                break;
            default:
                _enteredColliderList.Add(creature);
                break;
        }
        creature.OnDamaged(Owner, Skill);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // Ʈ���� �浹�� ���� ������ �ۿ��� �ؾ��ϴ� ��ų�� ����

        CreatureController target = collision.transform.GetComponent<CreatureController>();
        if (target.IsValid() == false)
            return;

        if (this.IsValid() == false)
            return;

        _enteredColliderList.Remove(target);
    }
}
