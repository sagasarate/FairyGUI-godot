using FairyGUI;
using System;
using System.Collections.Generic;
using Godot;

public class TreeViewDemo : FairyGUI.Window
{
    static TreeViewDemo _instance;
    public static TreeViewDemo inst
    {
        get
        {
            if (_instance == null)
                _instance = new TreeViewDemo();
            return _instance;
        }
    }
    GTree _tree1;
    GTree _tree2;
    string _fileURL;
    public TreeViewDemo()
    {
        UIPackage.AddPackage("res://Examples/Resources/TreeView.res");
    }
    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("TreeView", "Main").asCom;
        MakeFullScreen();

        _fileURL = "ui://TreeView/file";

        _tree1 = contentPane.GetChild("tree").asTree;
        _tree1.onClickItem.Add(__clickNode);
        _tree2 = contentPane.GetChild("tree2").asTree;
        _tree2.onClickItem.Add(__clickNode);
        _tree2.treeNodeRender = RenderTreeNode;

        GTreeNode topNode = new GTreeNode(true);
        topNode.data = "I'm a top node";
        _tree2.rootNode.AddChild(topNode);
        for (int i = 0; i < 5; i++)
        {
            GTreeNode node = new GTreeNode(false);
            node.data = "Hello " + i;
            topNode.AddChild(node);
        }

        GTreeNode aFolderNode = new GTreeNode(true);
        aFolderNode.data = "A folder node";
        topNode.AddChild(aFolderNode);
        for (int i = 0; i < 5; i++)
        {
            GTreeNode node = new GTreeNode(false);
            node.data = "Good " + i;
            aFolderNode.AddChild(node);
        }

        for (int i = 0; i < 3; i++)
        {
            GTreeNode node = new GTreeNode(false);
            node.data = "World " + i;
            topNode.AddChild(node);
        }

        GTreeNode anotherTopNode = new GTreeNode(false);
        anotherTopNode.data = new string[] { "I'm a top node too", "ui://TreeView/heart" };
        _tree2.rootNode.AddChild(anotherTopNode);
    }
    void RenderTreeNode(GTreeNode node, GComponent obj)
    {
        if (node.isFolder)
        {
            obj.text = (string)node.data;
        }
        else if (node.data is string[])
        {
            obj.icon = ((string[])node.data)[1];
            obj.text = ((string[])node.data)[0];
        }
        else
        {
            obj.icon = _fileURL;
            obj.text = (string)node.data;
        }
    }

    void __clickNode(EventContext context)
    {
        GTreeNode node = ((GObject)context.data).treeNode;
        GD.Print(node.text);
    }

   
}