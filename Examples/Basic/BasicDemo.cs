using FairyGUI;
using System;
using System.Collections.Generic;
using Godot;

public class BasicDemo : FairyGUI.Window
{
    static BasicDemo _instance;
    public static BasicDemo inst
    {
        get
        {
            if (_instance == null)
                _instance = new BasicDemo();
            return _instance;
        }

    }


    GObject _backBtn;
    GComponent _demoContainer;
    Controller _controller;
    Dictionary<string, GComponent> _demoObjects = new Dictionary<string, GComponent>();
    public BasicDemo()
    {
        UIPackage.AddPackage("res://Examples/Resources/Basics.res");

        UIConfig.verticalScrollBar = "ui://Basics/ScrollBar_VT";
        UIConfig.horizontalScrollBar = "ui://Basics/ScrollBar_HZ";
        UIConfig.popupMenu = "ui://Basics/PopupMenu";
        UIConfig.buttonSound = UIPackage.GetItemAsset("Basics", "click") as NAudioClip;
    }

    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("Basics", "Main").asCom;
        MakeFullScreen();


        _backBtn = contentPane.GetChild("btn_Back");
        _backBtn.visible = false;
        _backBtn.onClick.Add(OnClickBack);

        _demoContainer = contentPane.GetChild("container").asCom;
        _controller = contentPane.GetController("c1");

        var cnt = contentPane.numChildren;
        for (int i = 0; i < cnt; i++)
        {
            var obj = contentPane.GetChildAt(i);
            if (obj.group != null && obj.group.name == "btns")
                obj.onClick.Add(RunDemo);
        }
    }
    void OnClickBack()
    {
        _controller.selectedIndex = 0;
        _backBtn.visible = false;
    }

    private void RunDemo(EventContext context)
    {
        string type = ((GObject)(context.sender)).name.Substring(4);
        GComponent obj;
        if (!_demoObjects.TryGetValue(type, out obj))
        {
            obj = UIPackage.CreateObject("Basics", "Demo_" + type).asCom;
            _demoObjects[type] = obj;
        }

        _demoContainer.RemoveChildren();
        _demoContainer.AddChild(obj);
        _controller.selectedIndex = 1;
        _backBtn.visible = true;

        switch (type)
        {
            case "Graph":
                PlayGraph();
                break;

            case "Button":
                PlayButton();
                break;

            case "Text":
                PlayText();
                break;

            case "Grid":
                PlayGrid();
                break;

            case "Transition":
                PlayTransition();
                break;

            case "Window":
                PlayWindow();
                break;

            case "Popup":
                PlayPopup();
                break;

            case "Drag&Drop":
                PlayDragDrop();
                break;

            case "Depth":
                PlayDepth();
                break;

            case "ProgressBar":
                PlayProgressBar();
                break;
        }
    }

    //-----------------------------
    private void PlayGraph()
    {
        GComponent obj = _demoObjects["Graph"];

        NShape shape;

        shape = obj.GetChild("pie").asGraph.shape;
        shape.startDegree = 30;
        shape.endDegree = 300;

        shape = obj.GetChild("trapezoid").asGraph.shape;
        shape.usePercentPositions = true;
        shape.polygonPoints = new List<Vector2> { new Vector2(0f, 1f), new Vector2(0.3f, 0), new Vector2(0.7f, 0), new Vector2(1f, 1f) };
        shape.texture = UIPackage.GetItemAsset("Basics", "change") as NTexture;
    }

    //-----------------------------
    private void PlayButton()
    {
        GComponent obj = _demoObjects["Button"];
        obj.GetChild("n34").onClick.Add(() => { GD.Print("click button"); });
    }

    //------------------------------
    private void PlayText()
    {
        GComponent obj = _demoObjects["Text"];
        obj.GetChild("n12").asRichTextField.onClickLink.Add((EventContext context) =>
        {
            GRichTextField t = context.sender as GRichTextField;
            t.text = "[img]ui://Basics/pet[/img][color=#FF0000]You click the link[/color]：" + context.data;
        });
        obj.GetChild("n25").onClick.Add(() =>
        {
            obj.GetChild("n24").text = obj.GetChild("n22").text;
        });
    }

    //------------------------------
    private void PlayGrid()
    {
        GComponent obj = _demoObjects["Grid"];
        GList list1 = obj.GetChild("list1").asList;
        list1.RemoveChildrenToPool();
        string[] testNames = System.Enum.GetNames(typeof(Error));
        Color[] testColor = new Color[] { Colors.Yellow, Colors.Red, Colors.White, Colors.Cyan };
        int cnt = testNames.Length;
        for (int i = 0; i < cnt; i++)
        {
            GButton item = list1.AddItemFromPool().asButton;
            item.GetChild("t0").text = "" + (i + 1);
            item.GetChild("t1").text = testNames[i];
            item.GetChild("t2").asTextField.color = testColor[RandomUtil.Range(0, 3)];
            item.GetChild("star").asProgress.value = (int)((float)RandomUtil.Range(1, 3) / 3f * 100);
        }

        GList list2 = obj.GetChild("list2").asList;
        list2.RemoveChildrenToPool();
        for (int i = 0; i < cnt; i++)
        {
            GButton item = list2.AddItemFromPool().asButton;
            item.GetChild("cb").asButton.selected = false;
            item.GetChild("t1").text = testNames[i];
            item.GetChild("mc").asMovieClip.playing = i % 2 == 0;
            item.GetChild("t3").text = "" + RandomUtil.Range(0, 9999);
        }
    }

    //------------------------------
    private void PlayTransition()
    {
        GComponent obj = _demoObjects["Transition"];
        obj.GetChild("n2").asCom.GetTransition("t0").Play(int.MaxValue, 0, null);
        obj.GetChild("n3").asCom.GetTransition("peng").Play(int.MaxValue, 0, null);

        obj.onAddedToStage.Add(() =>
        {
            obj.GetChild("n2").asCom.GetTransition("t0").Stop();
            obj.GetChild("n3").asCom.GetTransition("peng").Stop();
        });
    }

    //------------------------------
    private FairyGUI.Window _winA;
    private FairyGUI.Window _winB;
    private void PlayWindow()
    {
        GComponent obj = _demoObjects["Window"];
        obj.GetChild("n0").onClick.Add(() =>
        {
            if (_winA == null)
                _winA = new Window1();
            _winA.Show();
        });

        obj.GetChild("n1").onClick.Add(() =>
        {
            if (_winB == null)
                _winB = new Window2();
            _winB.Show();
        });
    }

    //------------------------------
    private FairyGUI.PopupMenu _pm;
    private GComponent _popupCom;
    private void PlayPopup()
    {
        if (_pm == null)
        {
            _pm = new FairyGUI.PopupMenu();
            _pm.AddItem("Item 1", __clickMenu);
            _pm.AddItem("Item 2", __clickMenu);
            _pm.AddItem("Item 3", __clickMenu);
            _pm.AddItem("Item 4", __clickMenu);
        }

        if (_popupCom == null)
        {
            _popupCom = UIPackage.CreateObject("Basics", "Component12").asCom;
            _popupCom.Center();
        }
        GComponent obj = _demoObjects["Popup"];
        obj.GetChild("n0").onClick.Add((EventContext context) =>
        {
            _pm.Show((GObject)context.sender, PopupDirection.Down);
        });

        obj.GetChild("n1").onClick.Add(() =>
        {
            GRoot.inst.ShowPopup(_popupCom);
        });


        obj.onRightClick.Add(() =>
        {
            _pm.Show();
        });
    }

    private void __clickMenu(EventContext context)
    {
        GObject itemObject = (GObject)context.data;
        //UnityEngine.Debug.Log("click " + itemObject.text);
    }

    //------------------------------
    Vector2 startPos;
    private void PlayDepth()
    {
        GComponent obj = _demoObjects["Depth"];
        GComponent testContainer = obj.GetChild("n22").asCom;
        GObject fixedObj = testContainer.GetChild("n0");
        fixedObj.sortingOrder = 100;
        fixedObj.draggable = true;

        int numChildren = testContainer.numChildren;
        int i = 0;
        while (i < numChildren)
        {
            GObject child = testContainer.GetChildAt(i);
            if (child != fixedObj)
            {
                testContainer.RemoveChildAt(i);
                numChildren--;
            }
            else
                i++;
        }
        startPos = new Vector2(fixedObj.x, fixedObj.y);

        obj.GetChild("btn0").onClick.Add(() =>
        {
            GGraph graph = new GGraph();
            startPos.X += 10;
            startPos.Y += 10;
            graph.xy = startPos;
            graph.DrawRect(150, 150, 1, Colors.Black, Colors.Red);
            obj.GetChild("n22").asCom.AddChild(graph);
        });

        obj.GetChild("btn1").onClick.Add(() =>
        {
            GGraph graph = new GGraph();
            startPos.X += 10;
            startPos.Y += 10;
            graph.xy = startPos;
            graph.DrawRect(150, 150, 1, Colors.Black, Colors.Green);
            graph.sortingOrder = 200;
            obj.GetChild("n22").asCom.AddChild(graph);
        });
    }

    //------------------------------
    private void PlayDragDrop()
    {
        GComponent obj = _demoObjects["Drag&Drop"];
        obj.GetChild("a").draggable = true;

        GButton b = obj.GetChild("b").asButton;
        b.draggable = true;
        b.onDragStart.Add((EventContext context) =>
        {
            //Cancel the original dragging, and start a new one with a agent.
            context.PreventDefault();

            DragDropManager.inst.StartDrag(b, b.icon, b.icon, (int)context.data);
        });

        GButton c = obj.GetChild("c").asButton;
        c.icon = null;
        c.onDrop.Add((EventContext context) =>
        {
            c.icon = (string)context.data;
        });

        GObject bounds = obj.GetChild("bounds");       

        Rect rect = bounds.LocalToViewport(new Rect(0, 0, bounds.width, bounds.height));

        //---!!Because at this time the container is on the right side of the stage and beginning to move to left(transition), so we need to caculate the final position
        rect.X -= obj.parent.x;
        //----

        GButton d = obj.GetChild("d").asButton;
        d.draggable = true;
        d.dragBounds = rect;
    }

    //------------------------------
    private void PlayProgressBar()
    {
        GComponent obj = _demoObjects["ProgressBar"];
        Timers.inst.Add(0.001f, 0, __playProgress);
        obj.onRemovedFromStage.Add(() => { Timers.inst.Remove(__playProgress); });
    }

    void __playProgress(object param)
    {
        GComponent obj = _demoObjects["ProgressBar"];
        int cnt = obj.numChildren;
        for (int i = 0; i < cnt; i++)
        {
            GProgressBar child = obj.GetChildAt(i) as GProgressBar;
            if (child != null)
            {

                child.value += 1;
                if (child.value > child.max)
                    child.value = 0;
            }
        }
    }
}