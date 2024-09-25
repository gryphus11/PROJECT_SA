using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.Linq;

[CustomEditor(typeof(RuntimeAnimatorClipChanger))]
public class RuntimeAnimatorClipChangerEditor : Editor
{
    private UnityEditorInternal.ReorderableList reorderableList;
    private RuntimeAnimatorClipChanger clipChanger;
    private string[] stateNames;  // 상태 이름 배열
    private Animator previousAnimator;
    private RuntimeAnimatorController previousController;

    private void OnEnable()
    {
        clipChanger = (RuntimeAnimatorClipChanger)target;
        RefreshStateNames();  // Animator 상태와 AnimatorController 상태를 확인하여 stateNames 업데이트
        previousAnimator = clipChanger.GetComponent<Animator>();  // 이전 Animator 저장
        previousController = previousAnimator?.runtimeAnimatorController;  // 이전 AnimatorController 저장
    }

    // 상태 이름을 업데이트하는 메서드
    private void RefreshStateNames()
    {
        var animator = clipChanger.GetComponent<Animator>();

        if (animator == null)
        {
            stateNames = new string[] { "No Animator" };
        }
        else if (animator.runtimeAnimatorController == null)
        {
            stateNames = new string[] { "No Animator Controller" };
        }
        else
        {
            var animatorController = animator.runtimeAnimatorController as AnimatorController;
            if (animatorController != null)
            {
                // AnimatorController에서 모든 상태(State) 이름을 가져옴
                stateNames = animatorController.layers
                    .SelectMany(layer => layer.stateMachine.states)
                    .Select(state => state.state.name)
                    .ToArray();

                // 상태가 없는 경우 처리
                if (stateNames.Length == 0)
                {
                    stateNames = new string[] { "No States Available" };
                }
            }
            else
            {
                stateNames = new string[] { "Invalid Animator Controller" };
            }
        }

        // ReorderableList 초기화
        InitializeReorderableList();
    }

    // ReorderableList 초기화 메서드
    private void InitializeReorderableList()
    {
        reorderableList = new UnityEditorInternal.ReorderableList(serializedObject,
            serializedObject.FindProperty("animationClips"),
            true, true, true, true);

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            // stateName 드롭다운
            var stateNameProperty = element.FindPropertyRelative("stateName");
            int selectedIndex = ArrayUtility.IndexOf(stateNames, stateNameProperty.stringValue);

            if (selectedIndex == -1) selectedIndex = 0;

            selectedIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), selectedIndex, stateNames);
            stateNameProperty.stringValue = stateNames[selectedIndex];

            // AnimationClip 필드
            var clipProperty = element.FindPropertyRelative("clip");
            EditorGUI.PropertyField(new Rect(rect.x + rect.width / 2 + 10, rect.y, rect.width / 2 - 10, EditorGUIUtility.singleLineHeight), clipProperty, GUIContent.none);
        };

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Animation Clip Pairs");
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var currentAnimator = clipChanger.GetComponent<Animator>();
        var currentController = currentAnimator?.runtimeAnimatorController;

        // Animator나 Controller의 변경 여부를 감지
        if (currentAnimator != previousAnimator || currentController != previousController)
        {
            RefreshStateNames();  // 상태 목록 갱신
            previousAnimator = currentAnimator;  // 이전 Animator 상태 저장
            previousController = currentController;  // 이전 Controller 상태 저장
        }

        // 기본 인스펙터와 ReorderableList 표시
        reorderableList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
