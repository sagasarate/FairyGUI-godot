using FairyGUI;
using System;
using System.Collections.Generic;
using Godot;

public class VirtualListDemo : FairyGUI.Window
{
    static VirtualListDemo _instance;
    public static VirtualListDemo inst
    {
        get
        {
            if (_instance == null)
                _instance = new VirtualListDemo();
            return _instance;
        }

    }
    GList _list;
    public VirtualListDemo()
    {
        UIPackage.AddPackage("res://Examples/Resources/VirtualList.res");
        UIObjectFactory.SetPackageItemExtension("ui://VirtualList/mailItem", typeof(MailItem));
    }
    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("VirtualList", "Main").asCom;
        MakeFullScreen();

        contentPane.GetChild("n6").onClick.Add(() => { _list.AddSelection(500, true); });
        contentPane.GetChild("n7").onClick.Add(() => { _list.scrollPane.ScrollTop(); });
        contentPane.GetChild("n8").onClick.Add(() => { _list.scrollPane.ScrollBottom(); });

        _list = contentPane.GetChild("mailList").asList;
        _list.SetVirtual();

        _list.itemRenderer = RenderListItem;
        _list.numItems = 1000;
    }

    void RenderListItem(int index, GObject obj)
    {
        MailItem item = (MailItem)obj;
        item.setFetched(index % 3 == 0);
        item.setRead(index % 2 == 0);
        item.setTime("5 Nov 2015 16:24:33");
        item.title = index + " Mail title here";
    }
}