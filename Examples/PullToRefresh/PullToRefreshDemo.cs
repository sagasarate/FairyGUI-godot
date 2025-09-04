using FairyGUI;
using System;
using System.Collections.Generic;
using Godot;

public class PullToRefreshDemo : FairyGUI.Window
{
    static PullToRefreshDemo _instance;
    public static PullToRefreshDemo inst
    {
        get
        {
            if (_instance == null)
                _instance = new PullToRefreshDemo();
            return _instance;
        }
    }
    GList _list1;
    GList _list2;
    public PullToRefreshDemo()
    {
        UIPackage.AddPackage("res://Examples/Resources/PullToRefresh.res");
        UIObjectFactory.SetPackageItemExtension("ui://PullToRefresh/Header", typeof(ScrollPaneHeader));
    }
    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("PullToRefresh", "Main").asCom;
        MakeFullScreen();

        _list1 = contentPane.GetChild("list1").asList;
        _list1.itemRenderer = RenderListItem1;
        _list1.SetVirtual();
        _list1.numItems = 1;
        _list1.scrollPane.onPullDownRelease.Add(OnPullDownToRefresh);

        _list2 = contentPane.GetChild("list2").asList;
        _list2.itemRenderer = RenderListItem2;
        _list2.SetVirtual();
        _list2.numItems = 1;
        _list2.scrollPane.onPullUpRelease.Add(OnPullUpToRefresh);
    }

    void RenderListItem1(int index, GObject obj)
    {
        GButton item = obj.asButton;
        item.title = "Item " + (_list1.numItems - index - 1);
    }

    void RenderListItem2(int index, GObject obj)
    {
        GButton item = obj.asButton;
        item.title = "Item " + index;
    }

    void OnPullDownToRefresh()
    {
        ScrollPaneHeader header = (ScrollPaneHeader)_list1.scrollPane.header;
        if (header.ReadyToRefresh)
        {
            header.SetRefreshStatus(2);
            _list1.scrollPane.LockHeader(header.sourceHeight);

            //Simulate a async resquest
            Timers.inst.Add(2, 1, (object param) =>
            {
                _list1.numItems += 5;

                //Refresh completed
                header.SetRefreshStatus(3);
                _list1.scrollPane.LockHeader(35);

                Timers.inst.Add(2, 1, (object param2) =>
                {
                    header.SetRefreshStatus(0);
                    _list1.scrollPane.LockHeader(0);
                });
            });
        }
    }

    void OnPullUpToRefresh()
    {
        GComponent footer = (GComponent)_list2.scrollPane.footer;

        footer.GetController("c1").selectedIndex = 1;
        _list2.scrollPane.LockFooter(footer.sourceHeight);

        //Simulate a async resquest
        Timers.inst.Add(2, 1, (object param) =>
        {
            _list2.numItems += 5;

            //Refresh completed
            footer.GetController("c1").selectedIndex = 0;
            _list2.scrollPane.LockFooter(0);
        });
    }
}