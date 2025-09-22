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
    public partial class InputTextField : TextEdit, IDisplayObject, IInputTextField
    {
        AutoSizeType _autoSize = AutoSizeType.None;
        Vector2 _shadowOffset = Vector2.Zero;
        int _maxWidth;
        string _restrict;
        Regex _invalidRegex;
        string _oldText = string.Empty;
        int _maxLength = 0;

        public GObject gOwner { get; set; }
        public IDisplayObject parent { get { return GetParent() as IDisplayObject; } }
        public Control node { get { return this; } }
        public bool visible { get { return Visible; } set { Visible = value; } }
        public float skewX { get; set; }
        public float skewY { get; set; }
        public Vector2 position { get { return Position; } set { Position = value; } }
        public BlendMode blendMode { get; set; }
        public event System.Action<double> onUpdate;
        public InputTextField(GObject owner)
        {
            gOwner = owner;
            StyleBoxEmpty empty = new StyleBoxEmpty();
            AddThemeStyleboxOverride("normal", empty);
            AddThemeStyleboxOverride("focus", empty);
            AddThemeStyleboxOverride("read_only", empty);
            ScrollFitContentWidth = false;
            ScrollFitContentHeight = false;
            ClipContents = true;
            WrapMode = LineWrappingMode.None;
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
            get { return GetThemeConstant("line_spacing"); }
            set { AddThemeConstantOverride("line_spacing", (int)value); }
        }

        public AlignType align { get; set; }
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
                            ScrollFitContentWidth = false;
                            ScrollFitContentHeight = false;
                            ClipContents = true;
                            WrapMode = LineWrappingMode.None;
                            break;
                        case AutoSizeType.Both:
                            ScrollFitContentWidth = true;
                            ScrollFitContentHeight = true;
                            ClipContents = false;
                            WrapMode = LineWrappingMode.None;
                            break;
                        case AutoSizeType.Height:
                            ScrollFitContentWidth = false;
                            ScrollFitContentHeight = true;
                            ClipContents = false;
                            WrapMode = LineWrappingMode.Boundary;
                            AutowrapMode = TextServer.AutowrapMode.WordSmart;
                            break;
                        case AutoSizeType.Ellipsis:
                            ScrollFitContentWidth = false;
                            ScrollFitContentHeight = false;
                            ClipContents = true;
                            WrapMode = LineWrappingMode.Boundary;
                            AutowrapMode = TextServer.AutowrapMode.WordSmart;
                            break;
                    }

                }
            }
        }
        public bool singleLine
        {
            get { return false; }
            set { }
        }

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
            get { return _maxLength; }
            set { _maxLength = value; }
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
        public bool displayAsPassword { get; set; }
        public int caretPosition
        {
            get { return GetCaretIndex(); }
            set { SetCaretIndex(value); }
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
            get { return FairyGUI.VirtualKeyboardType.Default; }
            set { }
        }
        public bool disableIME { get; set; }
        public Dictionary<uint, Emoji> emojies { get; set; }
        public void SetSelection(int start, int length)
        {
            SelectByGlobalIndex(start, start + length);
        }
        void SelectByGlobalIndex(int startIndex, int endIndex)
        {
            int cumulative = 0;
            int startLine = 0, startCol = 0;
            int endLine = -1, endCol = -1;

            if (startIndex > endIndex)
            {
                int tmp = startIndex;
                startIndex = endIndex;
                endIndex = tmp;
            }
            for (int i = 0; i < GetLineCount(); i++)
            {
                int lineLen = GetLine(i).Length + 1; // +1 换行符
                if (cumulative <= startIndex && startIndex < cumulative + lineLen)
                {
                    startLine = i;
                    startCol = startIndex - cumulative;
                }
                if (cumulative <= endIndex && endIndex < cumulative + lineLen)
                {
                    endLine = i;
                    endCol = endIndex - cumulative;
                    break;
                }
                cumulative += lineLen;
            }
            if (endLine == -1) // 如果 endIndex 是文本末尾
            {
                endLine = GetLineCount() - 1;
                endCol = GetLine(endLine).Length;
            }
            Select(startLine, startCol, endLine, endCol);
        }
        int GetCaretIndex()
        {
            int CaretIndex = 0;
            int CaretLine = GetCaretLine();
            int CaretColumn = GetCaretColumn();
            for (int i = 0; i < GetLineCount(); i++)
            {
                if (CaretLine <= i)
                {
                    CaretIndex += CaretColumn;
                    break;
                }
                CaretIndex += GetLine(i).Length + 1;
            }
            return CaretIndex;
        }
        void SetCaretIndex(int caretIndex)
        {
            for (int i = 0; i < GetLineCount(); i++)
            {
                int lineLen = GetLine(i).Length + 1; // +1 换行符
                if (caretIndex <= lineLen)
                {
                    SetCaretLine(i);
                    SetCaretColumn(caretIndex);
                }
                caretIndex -= lineLen;
            }
        }
        public void ReplaceSelection(string value)
        {
            if (HasSelection())
                DeleteSelection();
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
        protected void OnTextChanged()
        {
            if (_maxLength > 0)
            {
                if (Text.Length > _maxLength)
                {
                    Text = Text.Substring(0, _maxLength);
                    SetCaretColumn(Text.Length);
                }
            }
            if (_invalidRegex != null)
            {
                if (!_invalidRegex.IsMatch(Text))
                {
                    Text = _oldText;
                    return;
                }
            }
            _oldText = Text;
        }
    }
}

