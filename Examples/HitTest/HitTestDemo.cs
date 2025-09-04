using FairyGUI;
using System;
using System.Collections.Generic;
using Godot;

public class HitTestDemo : FairyGUI.Window
{
    static HitTestDemo _instance;
    public static HitTestDemo inst
    {
        get
        {
            if (_instance == null)
                _instance = new HitTestDemo();
            return _instance;
        }
    }
    public HitTestDemo()
    {
        UIPackage.AddPackage("res://Examples/Resources/HitTest.res");
    }
    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("HitTest", "Main").asCom;
        MakeFullScreen();
    }
}