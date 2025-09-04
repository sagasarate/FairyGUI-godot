using FairyGUI;
using System;
using System.Collections.Generic;
using Godot;

public class ScrollPaneDemo : FairyGUI.Window
{
    static ScrollPaneDemo _instance;
    public static ScrollPaneDemo inst
    {
        get
        {
            if (_instance == null)
                _instance = new ScrollPaneDemo();
            return _instance;
        }
    }

    GList _list;
    public ScrollPaneDemo()
    {
        UIPackage.AddPackage("res://Examples/Resources/ScrollPane.res");
    }
    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("ScrollPane", "Main").asCom;
        MakeFullScreen();

        _list = contentPane.GetChild("list").asList;
        _list.itemRenderer = RenderListItem;
        _list.SetVirtual();
        _list.numItems = 1000;
        _list.onTouchBegin.Add(OnClickList);

        contentPane.GetChild("box").asCom.onDrop.Add(OnDrop);

        LongPressGesture gesture = new LongPressGesture(_list);
        gesture.once = true;
        gesture.trigger = 1f;
        gesture.onAction.Add(OnLongPress);
    }
    void RenderListItem(int index, GObject obj)
    {
        GButton item = obj.asButton;
        item.title = "Item " + index;
        item.scrollPane.posX = 0; //reset scroll pos

        //Be carefull, RenderListItem is calling repeatedly, dont call 'Add' here!
        //请注意，RenderListItem是重复调用的，不要使用Add增加侦听！
        item.GetChild("b0").onClick.Set(OnClickStick);
        item.GetChild("b1").onClick.Set(OnClickDelete);
    }

    void OnClickList(EventContext context)
    {
        //find out if there is an item in edit status
        //查找是否有项目处于编辑状态
        int cnt = _list.numChildren;
        for (int i = 0; i < cnt; i++)
        {
            GButton item = _list.GetChildAt(i).asButton;
            if (item.scrollPane.posX != 0)
            {
                //Check if clicked on the button
                if (item.GetChild("b0").asButton.IsAncestorOf(GRoot.inst.touchTarget)
                    || item.GetChild("b1").asButton.IsAncestorOf(GRoot.inst.touchTarget))
                {
                    return;
                }
                item.scrollPane.SetPosX(0, true);
                //avoid scroll pane default behavior
                //取消滚动面板可能发生的拉动。
                item.scrollPane.CancelDragging();
                _list.scrollPane.CancelDragging();
                break;
            }
        }
    }

    void OnLongPress(EventContext context)
    {
        //find out which item is under finger
        //逐层往上知道查到点击了那个item
        GObject obj = GRoot.inst.touchTarget;
        GObject p = obj.parent;
        while (p != null)
        {
            if (p == _list)
                break;

            p = p.parent;
        }

        if (p == null)
            return;
        GD.Print(obj.text);
        DragDropManager.inst.StartDrag(obj, obj.icon, obj.text);
    }

    void OnDrop(EventContext context)
    {
        contentPane.GetChild("txt").text = "Drop " + (string)context.data;
    }

    void OnClickStick(EventContext context)
    {
        contentPane.GetChild("txt").text = "Stick " + (((GObject)context.sender).parent).text;
    }

    void OnClickDelete(EventContext context)
    {
        contentPane.GetChild("txt").text = "Delete " + (((GObject)context.sender).parent).text;
    }
}