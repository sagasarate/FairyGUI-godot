using FairyGUI;
using System;
using System.Collections.Generic;
using Godot;
using System.Threading.Tasks;

public class ModalWaitingDemo : FairyGUI.Window
{
    static ModalWaitingDemo _instance;
    public static ModalWaitingDemo inst
    {
        get
        {
            if (_instance == null)
                _instance = new ModalWaitingDemo();
            return _instance;
        }
    }

    Window4 _testWin;
    public ModalWaitingDemo()
    {
        UIPackage.AddPackage("res://Examples/Resources/ModalWaiting.res");
        UIConfig.globalModalWaiting = "ui://ModalWaiting/GlobalModalWaiting";
        UIConfig.windowModalWaiting = "ui://ModalWaiting/WindowModalWaiting";
    }
    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("ModalWaiting", "Main").asCom;
        MakeFullScreen();

        _testWin = new Window4();

        contentPane.GetChild("n0").onClick.Add(() => { _testWin.Show(); });

        _ = WaitSomeTime();
    }

    async Task WaitSomeTime()
    {
        GRoot.inst.ShowModalWait();

        await container.ToSignal(container.GetTree().CreateTimer(3.0f), "timeout");

        GRoot.inst.CloseModalWait();
    }
}