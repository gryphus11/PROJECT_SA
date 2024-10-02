using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GameScene : BaseScene
{
    protected override bool Init()
    {
        if(base.Init() == false)
            return false;

        sceneType = Define.SceneType.Game;

        InitAsync().Forget();
        return true;
    }

    private async UniTask InitAsync()
    {
        await Managers.Resource.LoadAsyncLabelTask<Object>("Preload",
                (key, count, total) =>
                {
                    Debug.Log($"{key} : {count} / {total}");
                });

        StartLoad();

        foreach (var data in Managers.Data.CreatureDic)
        {
            Debug.Log($"{data.Key} / {data.Value.maxHp}");
        }
    }

    private void StartLoad()
    {
        Managers.Data.Init();

        Managers.UI.ShowSceneUI<UI_Joystick>();
        Managers.Resource.Instantiate("Map01");
        var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, 100002);

        for (int i = 0; i < 5; ++i)
        {
            var randomPos = player.transform.position + (Vector3)Random.insideUnitCircle * 5.0f;
            Managers.Object.Spawn<MonsterController>(randomPos, 200001);
        }
        
        var cameraController = FindObjectOfType<CameraController>();
        cameraController.Target = player.transform;
    }
}
