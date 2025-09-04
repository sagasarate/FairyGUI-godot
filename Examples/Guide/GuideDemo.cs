using FairyGUI;
using System;
using System.Collections.Generic;
using Godot;

public class GuideDemo : FairyGUI.Window
{
    static GuideDemo _instance;
    public static GuideDemo inst
    {
        get
        {
            if (_instance == null)
                _instance = new GuideDemo();
            return _instance;
        }
    }
    GComponent _guideLayer;
    public GuideDemo()
    {
        UIPackage.AddPackage("res://Examples/Resources/Guide.res");
    }
    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("Guide", "Main").asCom;
        MakeFullScreen();

        _guideLayer = UIPackage.CreateObject("Guide", "GuideLayer").asCom;
        _guideLayer.SetSize(GRoot.inst.width, GRoot.inst.height);
        _guideLayer.AddRelation(GRoot.inst, RelationType.Size);

        GObject bagBtn = contentPane.GetChild("bagBtn");
        bagBtn.onClick.Add(() =>
        {
            _guideLayer.RemoveFromParent();
        });

        contentPane.GetChild("n2").onClick.Add(() =>
        {
            GRoot.inst.AddChild(_guideLayer); //!!Before using TransformRect(or GlobalToLocal), the object must be added first
            Rect rect = bagBtn.TransformRect(new Rect(0, 0, bagBtn.width, bagBtn.height), _guideLayer);

            GObject window = _guideLayer.GetChild("window");
            window.size = new Vector2((int)rect.size.X, (int)rect.size.Y);
            window.TweenMove(new Vector2((int)rect.position.X, (int)rect.position.Y), 0.5f);
        });
    }
}