using UnityEngine;
using TMPro;
using DG.Tweening; // DoTween 네임스페이스
using Cysharp.Threading.Tasks; // UniTask 네임스페이스

[RequireComponent(typeof(TMP_Text))]
public class TextBrightnessController : MonoBehaviour
{
    private TMP_Text _tmpText; // TMP_Text 컴포넌트 캐싱
    public float duration = 0.5f;
    private bool _isRunning = false; // 현재 실행 중인지 여부를 나타내는 플래그

    private void Awake()
    {
        // TMP_Text 컴포넌트 캐싱
        _tmpText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        _isRunning = true;
        // 오브젝트가 활성화될 때 밝기 조정 시작
        ChangeTextBrightnessLoop().Forget();
    }

    private void OnDisable()
    {
        // 오브젝트가 비활성화되면 동작 중지
        _isRunning = false;
        _tmpText.DOKill(); // DoTween으로 실행 중인 트윈을 중지
    }

    private async UniTaskVoid ChangeTextBrightnessLoop()
    {
        while (_isRunning)
        {
            // 밝기 올리기 (알파값 1)
            await _tmpText.DOFade(1f, duration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();

            if (!_isRunning) break; // 동작이 중지되었을 때 탈출

            // 밝기 낮추기 (알파값 0.5)
            await _tmpText.DOFade(0.5f, duration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();

            if (!_isRunning) break; // 동작이 중지되었을 때 탈출
        }
    }
}
