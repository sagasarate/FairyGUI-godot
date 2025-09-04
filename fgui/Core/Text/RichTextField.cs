using System;
using System.Collections.Generic;
using System.Text;
using Godot;
using FairyGUI.Utils;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RichTextField : RichTextLabel, IDisplayObject, ITextField
    {
        AutoSizeType _autoSize = AutoSizeType.Both;
        Vector2 _shadowOffset = Vector2.Zero;
        int _maxWidth;
        bool _singleLine = false;

        public GObject gOwner { get; set; }
        public IDisplayObject parent { get { return GetParent() as IDisplayObject; } }
        public Control node { get { return this; } }
        public bool visible { get { return Visible; } set { Visible = value; } }
        public float skewX { get; set; }
        public float skewY { get; set; }
        public BlendMode blendMode { get; set; }
        public event System.Action<double> onUpdate;
        public RichTextField(GObject owner)
        {
            gOwner = owner;
            MouseFilter = MouseFilterEnum.Ignore;
            FitContent = true;
            ClipContents = false;
            AutowrapMode = TextServer.AutowrapMode.Off;
            ScrollActive = false;
            Resized += OnSizeChanged;
        }
        public override void _Process(double delta)
        {
            if (onUpdate != null)
                onUpdate(delta);
        }        
        public string font
        {
            get
            {
                var f = GetThemeFont("font");
                if (f != null)
                    return f.GetFontName();
                else
                    return string.Empty;
            }
            set
            {
                AddThemeFontOverride("font", FontManager.GetFont(value));
            }
        }
        public int fontSize
        {
            get { return GetThemeFontSize("font_size"); }
            set { AddThemeFontSizeOverride("font_size", value); }
        }
        public Color fontColor
        {
            get { return GetThemeColor("default_color"); }
            set { AddThemeColorOverride("default_color", value); }
        }
        public float lineSpacing
        {
            get { return GetThemeConstant("line_separation"); }
            set { AddThemeConstantOverride("line_separation", (int)value); }
        }

        public AlignType align
        {
            get
            {
                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        return AlignType.Left;
                    case HorizontalAlignment.Center:
                        return AlignType.Center;
                    case HorizontalAlignment.Right:
                        return AlignType.Right;
                    default:
                        return AlignType.Center;
                }
            }
            set
            {
                switch (value)
                {
                    case AlignType.Left:
                        HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case AlignType.Center:
                        HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case AlignType.Right:
                        HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    default:
                        HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                }
            }
        }
        public VertAlignType verticalAlign
        {
            get
            {
                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        return VertAlignType.Top;
                    case VerticalAlignment.Center:
                        return VertAlignType.Middle;
                    case VerticalAlignment.Bottom:
                        return VertAlignType.Bottom;
                    default:
                        return VertAlignType.Middle;
                }
            }
            set
            {
                switch (value)
                {
                    case VertAlignType.Top:
                        VerticalAlignment = VerticalAlignment.Top;
                        break;
                    case VertAlignType.Middle:
                        VerticalAlignment = VerticalAlignment.Center;
                        break;
                    case VertAlignType.Bottom:
                        VerticalAlignment = VerticalAlignment.Bottom;
                        break;
                    default:
                        VerticalAlignment = VerticalAlignment.Top;
                        break;
                }
            }
        }

        public AutoSizeType autoSize
        {
            get { return _autoSize; }
            set
            {
                if (_autoSize != value)
                {
                    _autoSize = value;
                    switch (_autoSize)
                    {
                        case AutoSizeType.None:
                        case AutoSizeType.Shrink:
                            FitContent = false;
                            ClipContents = true;
                            AutowrapMode = TextServer.AutowrapMode.WordSmart;
                            break;
                        case AutoSizeType.Both:
                            FitContent = true;
                            ClipContents = false;
                            AutowrapMode = TextServer.AutowrapMode.Off;
                            break;
                        case AutoSizeType.Height:
                            FitContent = true;
                            ClipContents = false;
                            AutowrapMode = TextServer.AutowrapMode.WordSmart;
                            break;
                        case AutoSizeType.Ellipsis:
                            FitContent = false;
                            ClipContents = true;
                            AutowrapMode = TextServer.AutowrapMode.WordSmart;
                            break;
                    }

                }
            }
        }
        public bool singleLine
        {
            get { return _singleLine; }
            set
            {
                if (_singleLine != value)
                {
                    _singleLine = value;
                    if (_singleLine)
                    {
                        AutowrapMode = TextServer.AutowrapMode.Off;
                        Text = Text.Replace("\n", "");
                    }
                    else
                    {
                        autoSize = _autoSize;
                    }
                }

            }
        }

        public int stroke
        {
            get { return GetThemeConstant("outline_size"); }
            set { AddThemeConstantOverride("outline_size", value); }
        }
        public Color strokeColor
        {
            get { return GetThemeColor("font_outline_color"); }
            set { AddThemeColorOverride("font_outline_color", value); }
        }
        public Vector2 shadowOffset
        {
            get
            {
                return _shadowOffset;
            }
            set
            {
                if (!Mathf.IsEqualApprox(_shadowOffset.X, value.X) || !Mathf.IsEqualApprox(_shadowOffset.Y, value.Y))
                {
                    _shadowOffset = value;
                    AddThemeConstantOverride("shadow_offset_x", Mathf.RoundToInt(_shadowOffset.X));
                    AddThemeConstantOverride("shadow_offset_y", Mathf.RoundToInt(_shadowOffset.Y));
                    AddThemeConstantOverride("shadow_outline_size", Mathf.RoundToInt(_shadowOffset.Length()));
                }
            }
        }
        public Color shadowColor
        {
            get
            {
                return GetThemeColor("font_shadow_color");
            }
            set
            {
                AddThemeColorOverride("font_shadow_color", value);
            }
        }
        public Vector2 size
        {
            get { return Size; }
            set { Size = value; }
        }
        public float width
        {
            get { return Size.X; }
            set { Size = new Vector2(value, Size.Y); }
        }
        public float height
        {
            get { return Size.Y; }
            set { Size = new Vector2(Size.X, value); }
        }
        public int maxWidth
        {
            get { return _maxWidth; }
            set
            {
                if (_maxWidth != value)
                {
                    _maxWidth = value;
                    if (Size.X > _maxWidth)
                        Size = new Vector2(_maxWidth, Size.Y);
                }
            }
        }
        public string text
        {
            get { return Text; }
            set
            {
                if (_singleLine)
                    Text = value.Replace("\n", "");
                else
                    Text = value;
            }
        }

        public bool ubbEnabled
        {
            get { return BbcodeEnabled; }
            set { BbcodeEnabled = value; }
        }

        protected void OnSizeChanged()
        {
            if (_maxWidth > 0 && Size.X > _maxWidth)
            {
                Size = new Vector2(_maxWidth, Size.Y);
            }
        }
    }
}
