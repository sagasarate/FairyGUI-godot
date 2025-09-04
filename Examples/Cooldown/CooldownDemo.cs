using FairyGUI;
using System;
using System.Collections.Generic;
using Godot;

public class CooldownDemo : FairyGUI.Window
{
    static CooldownDemo _instance;
    public static CooldownDemo inst
    {
        get
        {
            if (_instance == null)
                _instance = new CooldownDemo();
            return _instance;
        }
    }
    GProgressBar _btn0;
    GButton _btn1;
    GImage _mask;
    float _time;
    public CooldownDemo()
    {
        UIPackage.AddPackage("res://Examples/Resources/Cooldown.res");
    }
    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("Cooldown", "Main").asCom;
        MakeFullScreen();

        _btn0 = contentPane.GetChild("b0").asProgress;
        _btn0.GetChild("icon").icon = "res://Examples/resources/icons/k0.png";

        _btn1 = contentPane.GetChild("b1").asButton;
        _btn1.icon = "res://Examples/resources/icons/k1.png";
        _mask = _btn1.GetChild("bar").asImage;
        _time = 10;

        GTween.To(0, 100, 5).SetTarget(this._btn0, TweenPropType.Progress).SetRepeat(-1);
    }

    protected override void OnUpdate(double delta)
    {
        base.OnUpdate(delta);
        _time -= (float)delta;
        if (_time < 0)
            _time = 10;
        _btn1.text = string.Empty + Mathf.RoundToInt(_time);
        _mask.fillAmount = 1 - (10 - _time) / 10f;
    }
}