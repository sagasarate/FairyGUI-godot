using FairyGUI;
using System;
using System.Collections.Generic;
using Godot;

public class ListEffectDemo : FairyGUI.Window
{
    static ListEffectDemo _instance;
    public static ListEffectDemo inst
    {
        get
        {
            if (_instance == null)
                _instance = new ListEffectDemo();
            return _instance;
        }
    }
    GList _list;
    public ListEffectDemo()
    {
        UIPackage.AddPackage("res://Examples/Resources/ListEffect.res");
        UIObjectFactory.SetPackageItemExtension("ui://ListEffect/mailItem", typeof(MailItem));
    }
    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("ListEffect", "Main").asCom;
        MakeFullScreen();

        _list = contentPane.GetChild("mailList").asList;
        for (int i = 0; i < 10; i++)
        {
            MailItem item = (MailItem)_list.AddItemFromPool();
            item.setFetched(i % 3 == 0);
            item.setRead(i % 2 == 0);
            item.setTime("5 Nov 2015 16:24:33");
            item.title = "Mail title here";
        }

        _list.EnsureBoundsCorrect();
        float delay = 0f;
        for (int i = 0; i < 10; i++)
        {
            MailItem item = (MailItem)_list.GetChildAt(i);
            if (_list.IsChildInView(item))
            {
                item.PlayEffect(delay);
                delay += 0.2f;
            }
            else
                break;
        }
    }
}