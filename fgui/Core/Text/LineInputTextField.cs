using System;
using System.Collections.Generic;
using System.Text;
using Godot;
using FairyGUI.Utils;
using System.Text.RegularExpressions;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class LineInputTextField : LineEdit, IDisplayObject, IInputTextField
    {
        AutoSizeType _autoSize = AutoSizeType.None;
        Vector2 _shadowOffset = Vector2.Zero;
        int _maxWidth;
        string _restrict;
        Regex _invalidRegex;
        string _oldText = string.Empty;

        public GObject gOwner { get; set; }
        public IDisplayObject parent { get { return GetParent() as IDisplayObject; } }
        public Control node { get { return this; } }
        public bool visible { get { return Visible; } set { Visible = value; } }
        public float skewX { get; set; }
        public float skewY { get; set; }
        public BlendMode blendMode { get; set; }
        public event System.Action<double> onUpdate;
        public LineInputTextField(GObject owner)
        {
            gOwner = owner;
            StyleBoxEmpty empty = new StyleBoxEmpty();
            AddThemeStyleboxOverride("normal", empty);
            AddThemeStyleboxOverride("focus", empty);
            AddThemeStyleboxOverride("read_only", empty);
            ExpandToTextLength = false;
            CaretBlink = true;
            Resized += OnSizeChanged;
            TextChanged += OnTextChanged;
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
            get { return GetThemeColor("font_color"); }
            set
            {
                AddThemeColorOverride("font_color", value);
                AddThemeColorOverride("caret_color", value);
            }
        }
        public float lineSpacing
        {
            get { return 0; }
            set { }
        }

        public AlignType align
        {
            get
            {
                switch (Alignment)
                {
                    case HorizontalAlignment.Left:
                        return AlignType.Left;
                    case HorizontalAlignment.Center:
                        return AlignType.Center;
                    case HorizontalAlignment.Right:
                        return AlignType.Right;
                    default:
                        return AlignType.Left;
                }
            }
            set
            {
                switch (value)
                {
                    case AlignType.Left:
                        Alignment = HorizontalAlignment.Left;
                        break;
                    case AlignType.Center:
                        Alignment = HorizontalAlignment.Center;
                        break;
                    case AlignType.Right:
                        Alignment = HorizontalAlignment.Right;
                        break;
                    default:
                        Alignment = HorizontalAlignment.Left;
                        break;
                }
            }
        }
        public VertAlignType verticalAlign { get; set; }

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
                            ExpandToTextLength = false;
                            break;
                        case AutoSizeType.Both:
                            ExpandToTextLength = true;
                            break;
                        case AutoSizeType.Height:
                            ExpandToTextLength = true;
                            break;
                        case AutoSizeType.Ellipsis:
                            ExpandToTextLength = false;
                            break;
                    }

                }
            }
        }
        public bool singleLine { get { return true; } set { } }

        public int stroke
        {
            get
            {
                return GetThemeConstant("outline_size");
            }
            set
            {
                AddThemeConstantOverride("outline_size", value);
            }
        }
        public Color strokeColor
        {
            get
            {
                return GetThemeColor("font_outline_color");
            }
            set
            {
                AddThemeColorOverride("font_outline_color", value);
            }
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
            set { Text = value; }
        }

        public bool ubbEnabled { get; set; }
        public bool editable
        {
            get { return Editable; }
            set { Editable = value; }
        }
        public int maxLength
        {
            get { return MaxLength; }
            set { MaxLength = value; }
        }
        public string restrict
        {
            get { return _restrict; }
            set
            {
                if (_restrict != value)
                {
                    _restrict = value;
                    if (!string.IsNullOrEmpty(_restrict))
                        _invalidRegex = new Regex(_restrict);
                    else
                        _invalidRegex = null;
                    _oldText = string.Empty;
                    text = string.Empty;
                }
            }
        }
        public bool displayAsPassword
        {
            get { return Secret; }
            set { Secret = value; }
        }
        public int caretPosition
        {
            get { return CaretColumn; }
            set { CaretColumn = value; }
        }
        public string promptText
        {
            get { return PlaceholderText; }
            set { PlaceholderText = value; }
        }
        public bool keyboardInput
        {
            get { return VirtualKeyboardEnabled; }
            set { VirtualKeyboardEnabled = value; }
        }
        public FairyGUI.VirtualKeyboardType keyboardType
        {
            get
            {
                switch (VirtualKeyboardType)
                {
                    case VirtualKeyboardTypeEnum.Number:
                        return FairyGUI.VirtualKeyboardType.Number;
                    case VirtualKeyboardTypeEnum.NumberDecimal:
                        return FairyGUI.VirtualKeyboardType.NumberAndInterpunction;
                    case VirtualKeyboardTypeEnum.Phone:
                        return FairyGUI.VirtualKeyboardType.Phone;
                    case VirtualKeyboardTypeEnum.EmailAddress:
                        return FairyGUI.VirtualKeyboardType.EmailAddress;
                    case VirtualKeyboardTypeEnum.Password:
                        return FairyGUI.VirtualKeyboardType.Number;
                    default:
                        return FairyGUI.VirtualKeyboardType.Default;
                }
            }
            set
            {
                switch (value)
                {
                    case FairyGUI.VirtualKeyboardType.NumberAndInterpunction:
                        VirtualKeyboardType = VirtualKeyboardTypeEnum.NumberDecimal;
                        break;
                    case FairyGUI.VirtualKeyboardType.Url:
                        VirtualKeyboardType = VirtualKeyboardTypeEnum.Url;
                        break;
                    case FairyGUI.VirtualKeyboardType.Number:
                        VirtualKeyboardType = VirtualKeyboardTypeEnum.Number;
                        break;
                    case FairyGUI.VirtualKeyboardType.Phone:
                        VirtualKeyboardType = VirtualKeyboardTypeEnum.Phone;
                        break;
                    case FairyGUI.VirtualKeyboardType.EmailAddress:
                        VirtualKeyboardType = VirtualKeyboardTypeEnum.EmailAddress;
                        break;
                    default:
                        VirtualKeyboardType = VirtualKeyboardTypeEnum.Default;
                        break;
                }
            }
        }
        public bool disableIME { get; set; }
        public Dictionary<uint, Emoji> emojies { get; set; }
        public void SetSelection(int start, int length)
        {
            Select(start, start + length);
        }
        public void ReplaceSelection(string value)
        {
            if (HasSelection())
                DeleteText(GetSelectionFromColumn(), GetSelectionToColumn());
            InsertTextAtCaret(value);
        }
        public void CloneSetting(IInputTextField target)
        {
            editable = target.editable;
            maxLength = target.maxLength;
            restrict = target.restrict;
            displayAsPassword = target.displayAsPassword;
            promptText = target.promptText;
            keyboardInput = target.keyboardInput;
            keyboardType = target.keyboardType;
            disableIME = target.disableIME;
            emojies = target.emojies;
            if (target is ITextField textField)
            {
                font = target.font;
                fontSize = target.fontSize;
                lineSpacing = target.lineSpacing;
                fontColor = target.fontColor;
                align = target.align;
                verticalAlign = target.verticalAlign;
                autoSize = target.autoSize;
                stroke = target.stroke;
                strokeColor = target.strokeColor;
                shadowOffset = target.shadowOffset;
                shadowColor = target.shadowColor;
                maxWidth = target.maxWidth;
                text = target.text;
                ubbEnabled = target.ubbEnabled;
            }
            if (target is Control control)
            {
                Size = control.Size;
                Scale = control.Scale;
                Rotation = control.Rotation;
                Position = control.Position;
            }
        }
        protected void OnSizeChanged()
        {
            if (_maxWidth > 0 && Size.X > _maxWidth)
            {
                Size = new Vector2(_maxWidth, Size.Y);
            }
        }
        protected void OnTextChanged(string newText)
        {
            if (_invalidRegex != null)
            {
                if (!_invalidRegex.IsMatch(newText))
                {
                    Text = _oldText;
                    return;
                }
            }
            _oldText = Text;
        }
    }
}

