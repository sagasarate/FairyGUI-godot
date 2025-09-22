using Godot;
using System;

namespace FairyGUI
{
    public interface IDisplayObject
    {
        GObject gOwner { get; set; }
        IDisplayObject parent { get; }
        Control node { get; }
        bool visible { get; set; }
        float skewX { get; set; }
        float skewY { get; set; }
        Vector2 position { get; set; }
        BlendMode blendMode { get; set; }
        event System.Action<double> onUpdate;
    }
}