using Data;
using UnityEngine;

public class UI_CharacterSelectItem : UI_Base
{
    enum TMPros
    {
        CharacterDescText
    }

    enum Images
    {
        CharacterImage,
    }

    CreatureData _data;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindTMP(typeof(TMPros));

        gameObject.BindEvent(OnClickCharacter);
        return true;
    }

    public void SetInfo(CreatureData data)
    {
        _data = data;
        var image = GetImage((int)Images.CharacterImage);
        var sprite = Managers.Resource.Load<Sprite>(data.iconLabel);
        image.sprite = sprite;
        GetTMP((int)TMPros.CharacterDescText).text = data.descriptionTextID;
    }

    private void OnClickCharacter()
    {
        Managers.Game.SelectedPlayerID = _data.dataId;
        Managers.Scene.ChangeScene(Define.SceneType.Game);
    }
}
