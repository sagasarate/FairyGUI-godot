using FairyGUI;
using System;
using System.Collections.Generic;
using Godot;

public class BagDemo : FairyGUI.Window
{
    static BagDemo _instance;
    public static BagDemo inst
    {
        get
        {
            if (_instance == null)
                _instance = new BagDemo();
            return _instance;
        }
    }
    BagWindow _bagWindow;
    public BagDemo()
    {
        UIPackage.AddPackage("res://Examples/Resources/Bag.res");
        UIObjectFactory.SetLoaderExtension(typeof(MyGLoader));
    }
    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("Bag", "Main").asCom;
        MakeFullScreen();

        _bagWindow = new BagWindow();
        contentPane.GetChild("bagBtn").onClick.Add(() => { _bagWindow.Show(); });
    }
    
}