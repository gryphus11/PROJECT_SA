using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    private Define.SceneType _curSceneType = Define.SceneType.Unknown;

    public Define.SceneType CurrentSceneType
    {
        get
        {
            if (_curSceneType != Define.SceneType.Unknown)
                return _curSceneType;
            return CurrentScene.sceneType;
        }
        set { _curSceneType = value; }
    }

    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void ChangeScene(Define.SceneType type)
    {
        Debug.Log(CurrentScene);

        Managers.Clear();

        _curSceneType = type;
        SceneManager.LoadScene(GetSceneName(type));
    }

    string GetSceneName(Define.SceneType type)
    {
        string name = System.Enum.GetName(typeof(Define.SceneType), type);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
