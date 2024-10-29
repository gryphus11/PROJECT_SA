using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading;
using UnityEngine;

public class ProjectileController : SkillBase
{
    public SkillBase Skill { get; set; }
    protected Vector2 _spawnPos;
    protected Vector3 _dir = Vector3.zero;
    protected Vector3 _target = Vector3.zero;
    protected Define.SkillType _skillType;
    protected Rigidbody2D _rigid;
    protected int _numPenetrations;
    public int _bounceCount = 1;

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
        OnSetInfo(owner, position, dir, target, skill);

        if (gameObject.activeInHierarchy)
            DestroyTask().Forget();
    }

    protected async UniTask DestroyTask()
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

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        CreatureController creature = collision.transform.GetComponent<CreatureController>();
        if (creature.IsValid() == false)
            return;
        if (this.IsValid() == false)
            return;

        // ����ü�� ��� ��Ʈ���� ���� ������ ����
        DoEnterTrigger(creature);
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

        DoExitTrigger(target);
    }

    protected virtual void OnSetInfo(CreatureController owner, Vector2 position, Vector2 dir, Vector2 target, SkillBase skill)
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.up, _dir);
        _rigid.velocity = _dir * Skill.SkillData.ProjectileSpeed;
    }

    protected virtual void DoEnterTrigger(CreatureController creature)
    {
    }

    protected virtual void DoExitTrigger(CreatureController creature)
    {
    }
}
