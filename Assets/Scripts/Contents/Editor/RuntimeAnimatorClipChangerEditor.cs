using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.Linq;

[CustomEditor(typeof(RuntimeAnimatorClipChanger))]
public class RuntimeAnimatorClipChangerEditor : Editor
{
    private UnityEditorInternal.ReorderableList _reorderableList;
    private RuntimeAnimatorClipChanger _clipChanger;
    private string[] _stateNames;  // 상태 이름 배열
    private Animator _previousAnimator;
    private RuntimeAnimatorController _previousController;

    private void OnEnable()
    {
        _clipChanger = (RuntimeAnimatorClipChanger)target;
        RefreshStateNames();  // Animator 상태와 AnimatorController 상태를 확인하여 stateNames 업데이트
        _previousAnimator = _clipChanger.GetComponent<Animator>();  // 이전 Animator 저장
        _previousController = _previousAnimator?.runtimeAnimatorController;  // 이전 AnimatorController 저장
    }

    // 상태 이름을 업데이트하는 메서드
    private void RefreshStateNames()
    {
        var animator = _clipChanger.GetComponent<Animator>();

        if (animator == null)
        {
            _stateNames = new string[] { "No Animator" };
        }
        else if (animator.runtimeAnimatorController == null)
        {
            _stateNames = new string[] { "No Animator Controller" };
        }
        else
        {
            var animatorController = animator.runtimeAnimatorController as AnimatorController;
            if (animatorController != null)
            {
                // AnimatorController에서 모든 상태(State) 이름을 가져옴
                _stateNames = animatorController.layers
                    .SelectMany(layer => layer.stateMachine.states)
                    .Select(state => state.state.name)
                    .ToArray();

                // 상태가 없는 경우 처리
                if (_stateNames.Length == 0)
                {
                    _stateNames = new string[] { "No States Available" };
                }
            }
            else
            {
                _stateNames = new string[] { "Invalid Animator Controller" };
            }
        }

        // ReorderableList 초기화
        InitializeReorderableList();
    }

    // ReorderableList 초기화 메서드
    private void InitializeReorderableList()
    {
        _reorderableList = new UnityEditorInternal.ReorderableList(serializedObject,
            serializedObject.FindProperty("animationClips"),
            true, true, true, true);

        _reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            // stateName 드롭다운
            var stateNameProperty = element.FindPropertyRelative("stateName");
            int selectedIndex = ArrayUtility.IndexOf(_stateNames, stateNameProperty.stringValue);

            if (selectedIndex == -1) selectedIndex = 0;

            selectedIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), selectedIndex, _stateNames);
            stateNameProperty.stringValue = _stateNames[selectedIndex];

            // AnimationClip 필드
            var clipProperty = element.FindPropertyRelative("clip");
            EditorGUI.PropertyField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight), clipProperty, GUIContent.none);
        };

        _reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Animation Clip Pairs");
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var currentAnimator = _clipChanger.GetComponent<Animator>();
        var currentController = currentAnimator?.runtimeAnimatorController;

        // Animator나 Controller의 변경 여부를 감지
        if (currentAnimator != _previousAnimator || currentController != _previousController)
        {
            RefreshStateNames();  // 상태 목록 갱신
            _previousAnimator = currentAnimator;  // 이전 Animator 상태 저장
            _previousController = currentController;  // 이전 Controller 상태 저장
        }

        // 기본 인스펙터와 ReorderableList 표시
        _reorderableList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
