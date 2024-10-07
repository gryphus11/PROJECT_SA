using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LobbyScene : UI_Scene
{
    enum VerticalLayouts
    {
        CharacterSelectContent,
    }

    Transform _selectCharacterParent;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        var playerCharacters = CreatureData.GetPlayerCreatureData();

        BindObject(typeof(VerticalLayouts));
        _selectCharacterParent = GetObject((int)VerticalLayouts.CharacterSelectContent).transform;

        foreach (var character in playerCharacters)
        {
            var item = Managers.UI.MakeSubItem<UI_CharacterSelectItem>(_selectCharacterParent);
            item.Init();
            item.SetInfo(character);    
        }

        return true;
    }
}
