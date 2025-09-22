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
    public partial class TextField : Label, IDisplayObject, ITextField
    {
        AutoSizeType _autoSize = AutoSizeType.Both;
        int _maxWidth;
        bool _singleLine = false;

        public GObject gOwner { get; set; }
        public IDisplayObject parent { get { return GetParent() as IDisplayObject; } }
        public Control node { get { return this; } }
        public bool visible { get { return Visible; } set { Visible = value; } }
        public float skewX { get; set; }
        public float skewY { get; set; }
        public Vector2 position { get { return Position; } set { Position = value; } }
        public BlendMode blendMode { get; set; }
        public event System.Action<double> onUpdate;
        public TextField(GObject owner)
        {
            gOwner = owner;
            MouseFilter = MouseFilterEnum.Ignore;
            ClipText = false;
            AutowrapMode = TextServer.AutowrapMode.Off;
            this.LabelSettings = new LabelSettings();
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
                if (LabelSettings.Font != null)
                    return LabelSettings.Font.GetFontName();
                else
                    return string.Empty;
            }
            set
            {
                LabelSettings.Font = FontManager.GetFont(value);
            }
        }
        public int fontSize
        {
            get { return LabelSettings.FontSize; }
            set { LabelSettings.FontSize = value; }
        }
        public float lineSpacing
        {
            get { return LabelSettings.LineSpacing; }
            set { LabelSettings.LineSpacing = value; }
        }
        public Color fontColor
        {
            get { return LabelSettings.FontColor; }
            set { LabelSettings.FontColor = value; }
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
                            ClipText = true;
                            AutowrapMode = TextServer.AutowrapMode.WordSmart;
                            TextOverrunBehavior = TextServer.OverrunBehavior.NoTrimming;
                            break;
                        case AutoSizeType.Both:
                            ClipText = false;
                            AutowrapMode = TextServer.AutowrapMode.Off;
                            break;
                        case AutoSizeType.Height:
                            ClipText = false;
                            AutowrapMode = TextServer.AutowrapMode.WordSmart;
                            TextOverrunBehavior = TextServer.OverrunBehavior.NoTrimming;
                            break;
                        case AutoSizeType.Ellipsis:
                            ClipText = true;
                            AutowrapMode = TextServer.AutowrapMode.WordSmart;
                            TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
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
            get
            {
                return LabelSettings.OutlineSize;
            }
            set
            {
                if (LabelSettings.OutlineSize != value)
                {
                    LabelSettings.OutlineSize = value;
                }
            }
        }
        public Color strokeColor
        {
            get
            {
                return LabelSettings.OutlineColor;
            }
            set
            {
                if (LabelSettings.OutlineColor != value)
                {
                    LabelSettings.OutlineColor = value;
                }
            }
        }
        public Vector2 shadowOffset
        {
            get
            {
                return LabelSettings.ShadowOffset;
            }
            set
            {
                LabelSettings.ShadowOffset = value;
                LabelSettings.ShadowSize = Mathf.RoundToInt(value.Length());
            }
        }
        public Color shadowColor
        {
            get
            {
                return LabelSettings.ShadowColor;
            }
            set
            {
                if (LabelSettings.ShadowColor != value)
                {
                    LabelSettings.ShadowColor = value;
                }
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
        public bool ubbEnabled { get; set; }

        protected void OnSizeChanged()
        {
            if (_maxWidth > 0 && Size.X > _maxWidth)
            {
                Size = new Vector2(_maxWidth, Size.Y);
            }
        }
    }
}
