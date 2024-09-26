using UnityEngine;
using TMPro;
using DG.Tweening; // DoTween ���ӽ����̽�
using Cysharp.Threading.Tasks; // UniTask ���ӽ����̽�

[RequireComponent(typeof(TMP_Text))]
public class TextBrightnessController : MonoBehaviour
{
    private TMP_Text _tmpText; // TMP_Text ������Ʈ ĳ��
    public float duration = 0.5f;
    private bool _isRunning = false; // ���� ���� ������ ���θ� ��Ÿ���� �÷���

    private void Awake()
    {
        // TMP_Text ������Ʈ ĳ��
        _tmpText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        _isRunning = true;
        // ������Ʈ�� Ȱ��ȭ�� �� ��� ���� ����
        ChangeTextBrightnessLoop().Forget();
    }

    private void OnDisable()
    {
        // ������Ʈ�� ��Ȱ��ȭ�Ǹ� ���� ����
        _isRunning = false;
        _tmpText.DOKill(); // DoTween���� ���� ���� Ʈ���� ����
    }

    private async UniTaskVoid ChangeTextBrightnessLoop()
    {
        while (_isRunning)
        {
            // ��� �ø��� (���İ� 1)
            await _tmpText.DOFade(1f, duration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();

            if (!_isRunning) break; // ������ �����Ǿ��� �� Ż��

            // ��� ���߱� (���İ� 0.5)
            await _tmpText.DOFade(0.5f, duration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();

            if (!_isRunning) break; // ������ �����Ǿ��� �� Ż��
        }
    }
}
