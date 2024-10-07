using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageFont : MonoBehaviour
{
    TextMeshPro _damageText;

    public void SetInfo(Vector2 pos, float damage = 0, float healAmount = 0, Transform parent = null, bool isCritical = false)
    {
        if(_damageText == null)
            _damageText = GetComponent<TextMeshPro>();

        if (_damageText == null)
            return;

        transform.position = pos;

        if (healAmount > 0)
        {
            _damageText.text = $"{Mathf.RoundToInt(healAmount)}";
            _damageText.color = Utils.HexToColor("4EEE6F");
        }
        else if (isCritical)
        {
            _damageText.text = $"{Mathf.RoundToInt(damage)}";
            _damageText.color = Utils.HexToColor("EFAD00");
        }
        else
        {
            _damageText.text = $"{Mathf.RoundToInt(damage)}";
            _damageText.color = Color.white;
        }
        
        _damageText.alpha = 1;
        
        if (parent != null)
        {
            GetComponent<MeshRenderer>().sortingOrder = 321;
        }

        DoAnimation();
    }

    private void OnEnable()
    {
    }

    private void DoAnimation()
    {
        DOTween.Kill(transform, true);  // 현재 transform의 모든 DOTween 애니메이션 종료
        Sequence seq = DOTween.Sequence();

        // 크기가 커졌다 작아지며 사라진다.
        transform.localScale = new Vector3(0, 0, 0);

        seq.Append(transform.DOScale(1.3f, 0.3f).SetEase(Ease.InOutBounce))
            .Join(transform.DOMove(transform.position + Vector3.up, 0.3f).SetEase(Ease.Linear))
            .Append(transform.DOScale(1.0f, 0.3f).SetEase(Ease.InOutBounce))
            .Join(transform.GetComponent<TMP_Text>().DOFade(0, 0.3f).SetEase(Ease.InQuint))
            //.Append(GetComponent<TextMeshPro>().DOFade(0, 1f).SetEase(Ease.InBounce))
            .OnComplete(() =>
            {
                Managers.Resource.Destroy(gameObject);
            });

    }

    private void OnDisable()
    {
        DOTween.Kill(transform);  // 오브젝트가 비활성화될 때 애니메이션 종료
    }
}
