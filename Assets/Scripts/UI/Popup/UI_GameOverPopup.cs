using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameOverPopup : UI_Popup
{
    enum Buttons
    {
        ReturnButton,
    }

    private void Awake()
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.ReturnButton).gameObject.BindEvent(OnReturnToLobby);

        return true;
    }

    private void OnReturnToLobby()
    {
        Managers.Scene.ChangeScene(Define.SceneType.Lobby);
    }

    public void SetInfo()
    {

    }
}
