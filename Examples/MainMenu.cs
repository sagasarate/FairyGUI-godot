using FairyGUI;

public class MainMenu : FairyGUI.Window
{
    static MainMenu _instance;
    static GButton _closeButton;
    public static MainMenu inst
    {
        get
        {
            if (_instance == null)
                _instance = new MainMenu();
            return _instance;
        }

    }

    FairyGUI.Window _curDemo;
    public MainMenu()
    {
        UIPackage.AddPackage("res://Examples/Resources/MainMenu.res");
        _closeButton = UIPackage.CreateObject("MainMenu", "CloseButton").asButton;
        _closeButton.SetXY(GRoot.inst.width - _closeButton.width - 10, GRoot.inst.height - _closeButton.height - 10);
        _closeButton.AddRelation(GRoot.inst, RelationType.Right_Right);
        _closeButton.AddRelation(GRoot.inst, RelationType.Bottom_Bottom);
        _closeButton.sortingOrder = 100000;
        _closeButton.onClick.Add(OnDemoClosed);
        GRoot.inst.AddChild(_closeButton);
        _closeButton.visible = false;
    }
    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("MainMenu", "Main").asCom;
        MakeFullScreen();

        contentPane.GetChild("n1").onClick.Add(() =>
        {
            Hide();
            BasicDemo.inst.Show();
            _curDemo = BasicDemo.inst;
            _closeButton.visible = true;
            
        });
        contentPane.GetChild("n2").onClick.Add(() =>
        {
            Hide();
            TransitionDemo.inst.Show();
            _curDemo = TransitionDemo.inst;
            _closeButton.visible = true;
        });
        contentPane.GetChild("n4").onClick.Add(() =>
        {
            Hide();
            VirtualListDemo.inst.Show();
            _curDemo = VirtualListDemo.inst;
            _closeButton.visible = true;
        });
        contentPane.GetChild("n5").onClick.Add(() =>
        {
            Hide();
            LoopListDemo.inst.Show();
            _curDemo = LoopListDemo.inst;
            _closeButton.visible = true;
        });
        contentPane.GetChild("n6").onClick.Add(() =>
        {
            Hide();
            HitTestDemo.inst.Show();
            _curDemo = HitTestDemo.inst;
            _closeButton.visible = true;
        });
        contentPane.GetChild("n7").onClick.Add(() =>
        {
            Hide();
            PullToRefreshDemo.inst.Show();
            _curDemo = PullToRefreshDemo.inst;
            _closeButton.visible = true;
        });
        contentPane.GetChild("n8").onClick.Add(() =>
        {
            Hide();
            ModalWaitingDemo.inst.Show();
            _curDemo = ModalWaitingDemo.inst;
            _closeButton.visible = true;
        });
        contentPane.GetChild("n9").onClick.Add(() =>
        {
            Hide();
            JoystickDemo.inst.Show();
            _curDemo = JoystickDemo.inst;
            _closeButton.visible = true;
        });
        contentPane.GetChild("n10").onClick.Add(() =>
        {
            Hide();
            BagDemo.inst.Show();
            _curDemo = BagDemo.inst;
            _closeButton.visible = true;
        });
        contentPane.GetChild("n11").onClick.Add(() =>
        {
            Hide();
            ChatDemo.inst.Show();
            _curDemo = ChatDemo.inst;
            _closeButton.visible = true;
        });
        contentPane.GetChild("n12").onClick.Add(() =>
        {
            Hide();
            ListEffectDemo.inst.Show();
            _curDemo = ListEffectDemo.inst;
            _closeButton.visible = true;
        });
        contentPane.GetChild("n13").onClick.Add(() =>
        {
            Hide();
            ScrollPaneDemo.inst.Show();
            _curDemo = ScrollPaneDemo.inst;
            _closeButton.visible = true;
        });
        contentPane.GetChild("n14").onClick.Add(() =>
        {
            Hide();
            TreeViewDemo.inst.Show();
            _curDemo = TreeViewDemo.inst;
            _closeButton.visible = true;
        });
        contentPane.GetChild("n15").onClick.Add(() =>
        {
            Hide();
            GuideDemo.inst.Show();
            _curDemo = GuideDemo.inst;
            _closeButton.visible = true;
        });
        contentPane.GetChild("n16").onClick.Add(() =>
        {
            Hide();
            CooldownDemo.inst.Show();
            _curDemo = CooldownDemo.inst;
            _closeButton.visible = true;
        });
    }

    void OnDemoClosed()
    {
        if (_curDemo != null)
        {
            _curDemo.Hide();
            _curDemo = null;
        }
        this.Show();
        _closeButton.visible = false;
    }
}