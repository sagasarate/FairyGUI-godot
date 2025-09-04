using FairyGUI;
using System;
using System.Collections.Generic;
using Godot;

public class JoystickDemo : FairyGUI.Window
{
    static JoystickDemo _instance;
    public static JoystickDemo inst
    {
        get
        {
            if (_instance == null)
                _instance = new JoystickDemo();
            return _instance;
        }
    }
    GTextField _text;
    JoystickModule _joystick;
    public JoystickDemo()
    {
        UIPackage.AddPackage("res://Examples/Resources/Joystick.res");
    }
    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("Joystick", "Main").asCom;
        MakeFullScreen();

        _text = contentPane.GetChild("n9").asTextField;

        _joystick = new JoystickModule(contentPane);
        _joystick.onMove.Add(__joystickMove);
        _joystick.onEnd.Add(__joystickEnd);
    }
    void __joystickMove(EventContext context)
    {
        float degree = (float)context.data;
        _text.text = "" + degree;
    }

    void __joystickEnd()
    {
        _text.text = "";
    }
}