using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RuntimeAnimatorClipChanger : MonoBehaviour
{
    [System.Serializable]
    public class AnimationClipPair
    {
        public string stateName;
        public AnimationClip clip;
    }

    public List<AnimationClipPair> animationClips = new List<AnimationClipPair>();

    private Animator _animator;
    private AnimatorOverrideController _overrideController;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (_animator == null || _animator.runtimeAnimatorController == null) return;


        // Animator의 기본 Runtime Animator Controller를 가져옴
        RuntimeAnimatorController originalController = _animator.runtimeAnimatorController;

        // AnimatorOverrideController 생성
        _overrideController = new AnimatorOverrideController(originalController);

        // Animator에 새로운 Override Controller 할당
        _animator.runtimeAnimatorController = _overrideController;

        // 런타임에 애니메이션 클립을 설정
        foreach (var clip in animationClips)
        {
            OverrideAnimation(clip.stateName, clip.clip);
        }
    }

    public void OverrideAnimation(string stateName, AnimationClip newClip)
    {
        if (_overrideController == null || newClip == null) return;

        var oldClip = _overrideController[stateName];
        // 기존 애니메이션 클립을 찾아서 새로운 클립으로 대체
        _overrideController[stateName] = newClip;

        Debug.Log($"애니메이션 {oldClip.name}이(가) {newClip.name}으로 교체되었습니다.");
    }
}
