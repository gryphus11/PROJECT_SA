using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
    protected override bool Init()
    {
        if(base.Init() == false)
            return false;

        sceneType = Define.SceneType.Title;

        return true;
    }
}
