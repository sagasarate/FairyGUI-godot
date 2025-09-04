using FairyGUI;
using System;
using System.Collections.Generic;
using Godot;

public class ChatDemo : FairyGUI.Window
{
    static ChatDemo _instance;
    public static ChatDemo inst
    {
        get
        {
            if (_instance == null)
                _instance = new ChatDemo();
            return _instance;
        }
    }
    class Message
    {
        public string sender;
        public string senderIcon;
        public string msg;
        public bool fromMe;
    }
    GList _list;
    GTextInput _input1;
    GComponent _emojiSelectUI1;
    List<Message> _messages;
    Dictionary<uint, Emoji> _emojies;
    public ChatDemo()
    {
        UIPackage.AddPackage("res://Examples/Resources/Chat.res");
    }
    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("Chat", "Main").asCom;
        MakeFullScreen();

        _messages = new List<Message>();

        _list = contentPane.GetChild("list").asList;
        _list.SetVirtual();
        _list.itemProvider = GetListItemResource;
        _list.itemRenderer = RenderListItem;

        _input1 = contentPane.GetChild("input1").asTextInput;
        _input1.onKeyDown.Add(__inputKeyDown1);


        //作为demo，这里只添加了部分表情素材
        _emojies = new Dictionary<uint, Emoji>();
        for (uint i = 0x1f600; i < 0x1f637; i++)
        {
            string url = UIPackage.GetItemURL("Chat", Convert.ToString(i, 16));
            if (url != null)
                _emojies.Add(i, new Emoji(url));
        }

        contentPane.GetChild("btnSend1").onClick.Add(__clickSendBtn1);
        contentPane.GetChild("btnEmoji1").onClick.Add(__clickEmojiBtn1);
        _emojiSelectUI1 = UIPackage.CreateObject("Chat", "EmojiSelectUI").asCom;
        _emojiSelectUI1.GetChild("list").asList.onClickItem.Add(__clickEmoji1);
    }

    void AddMsg(string sender, string senderIcon, string msg, bool fromMe)
    {
        bool isScrollBottom = _list.scrollPane.isBottomMost;

        Message newMessage = new Message();
        newMessage.sender = sender;
        newMessage.senderIcon = senderIcon;
        newMessage.msg = msg;
        newMessage.fromMe = fromMe;
        _messages.Add(newMessage);

        if (newMessage.fromMe)
        {
            if (_messages.Count == 1 || RandomUtil.Range(0f, 1f) < 0.5f)
            {
                Message replyMessage = new Message();
                replyMessage.sender = "FairyGUI";
                replyMessage.senderIcon = "r1";
                replyMessage.msg = "Today is a good day. \U0001f600";
                replyMessage.fromMe = false;
                _messages.Add(replyMessage);
            }
        }

        if (_messages.Count > 100)
            _messages.RemoveRange(0, _messages.Count - 100);

        _list.numItems = _messages.Count;

        if (isScrollBottom)
            _list.scrollPane.ScrollBottom();
    }

    string GetListItemResource(int index)
    {
        Message msg = _messages[index];
        if (msg.fromMe)
            return "ui://Chat/chatRight";
        else
            return "ui://Chat/chatLeft";
    }

    void RenderListItem(int index, GObject obj)
    {
        GButton item = (GButton)obj;
        Message msg = _messages[index];
        if (!msg.fromMe)
            item.GetChild("name").text = msg.sender;
        item.icon = UIPackage.GetItemURL("Chat", msg.senderIcon);

        //Recaculate the text width
        GRichTextField tf = item.GetChild("msg").asRichTextField;
        //tf.emojies = _emojies;
        tf.text = EmojiParser.inst.Parse(msg.msg);
    }

    void __clickSendBtn1(EventContext context)
    {
        string msg = _input1.text;
        if (msg.Length == 0)
            return;

        AddMsg("Unity", "r0", msg, true);
        _input1.text = "";
    }
   
    void __clickEmojiBtn1(EventContext context)
    {
        GRoot.inst.ShowPopup(_emojiSelectUI1, (GObject)context.sender, PopupDirection.Up);
    }
    

    void __clickEmoji1(EventContext context)
    {
        GButton item = (GButton)context.data;
        _input1.ReplaceSelection("[:" + item.text + "]");
    }

    void __inputKeyDown1(EventContext context)
    {
        if (context.inputEvent.keyCode == Key.Enter)
            contentPane.GetChild("btnSend1").onClick.Call();
    }


}