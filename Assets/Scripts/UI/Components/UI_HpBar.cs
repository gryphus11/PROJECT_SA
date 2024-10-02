using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HpBar : UI_Base
{
    enum Sliders
    {
        HpBar,
    }

    enum Images
    {
        Fill,
    }

    Slider _hpBar;
    Image _hp;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindSlider(typeof(Sliders));
        BindImage(typeof(Images));

        _hpBar = GetSlider((int)Sliders.HpBar);
        _hp = GetImage((int)Images.Fill);

        return true;
    }

    private void Update()
    {
        if (_hpBar == null)
            return;

        if (Managers.Object.Player == null)
            return;

        transform.rotation = Camera.main.transform.rotation;

        _hpBar.value = Managers.Object.Player.Hp / (float)Managers.Object.Player.MaxHp;
        _hp.color = Color.Lerp(Color.red, Color.green, _hpBar.value);
    }
}
