using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.Data.Init();
        sceneType = Define.SceneType.Lobby;

        Managers.UI.ShowSceneUI<UI_LobbyScene>();

        return true;

    }
}
