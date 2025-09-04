using System.Collections.Generic;
using System.Text;
using Godot;
using FairyGUI.Utils;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public class GTextField : GObject, ITextColorGear
    {
        protected ITextField _textField;
        protected string _text;
        protected bool _updatingSize;
        protected Dictionary<string, string> _templateVars;

        public GTextField()
            : base()
        {
            _textField.font = UIConfig.defaultFont;
            _textField.fontSize = 12;
            _textField.fontColor = Colors.Black;
            _textField.lineSpacing = 3;
            _text = string.Empty;
            _textField.autoSize = AutoSizeType.Both;
            touchable = false;
        }

        override protected void CreateDisplayObject()
        {
            var obj = new TextField(this);
            obj.Resized += OnTextFieldSizeChanged;
            displayObject = obj;
            _textField = obj;
        }

        /// <summary>
        /// 
        /// </summary>
        override public string text
        {
            get
            {
                if (this is GTextInput)
                    _text = ((GTextInput)this)._textField.text;
                return _text;
            }
            set
            {
                if (value == null)
                    value = string.Empty;
                _text = value;
                SetTextFieldText();
                UpdateGear(6);
            }
        }

        virtual protected void SetTextFieldText()
        {
            string str = _text;
            if (_templateVars != null)
                str = ParseTemplate(str);

            _textField.maxWidth = maxWidth;
            _textField.text = str;
        }

        public Dictionary<string, string> templateVars
        {
            get { return _templateVars; }
            set
            {
                if (_templateVars == null && value == null)
                    return;

                _templateVars = value;

                FlushVars();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public GTextField SetVar(string name, string value)
        {
            if (_templateVars == null)
                _templateVars = new Dictionary<string, string>();
            _templateVars[name] = value;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public void FlushVars()
        {
            SetTextFieldText();
        }


        protected string ParseTemplate(string template)
        {
            int pos1 = 0, pos2 = 0;
            int pos3;
            string tag;
            string value;
            StringBuilder buffer = new StringBuilder();

            while ((pos2 = template.IndexOf('{', pos1)) != -1)
            {
                if (pos2 > 0 && template[pos2 - 1] == '\\')
                {
                    buffer.Append(template, pos1, pos2 - pos1 - 1);
                    buffer.Append('{');
                    pos1 = pos2 + 1;
                    continue;
                }

                buffer.Append(template, pos1, pos2 - pos1);
                pos1 = pos2;
                pos2 = template.IndexOf('}', pos1);
                if (pos2 == -1)
                    break;

                if (pos2 == pos1 + 1)
                {
                    buffer.Append(template, pos1, 2);
                    pos1 = pos2 + 1;
                    continue;
                }

                tag = template.Substring(pos1 + 1, pos2 - pos1 - 1);
                pos3 = tag.IndexOf('=');
                if (pos3 != -1)
                {
                    if (!_templateVars.TryGetValue(tag.Substring(0, pos3), out value))
                        value = tag.Substring(pos3 + 1);
                }
                else
                {
                    if (!_templateVars.TryGetValue(tag, out value))
                        value = "";
                }
                buffer.Append(value);
                pos1 = pos2 + 1;
            }
            if (pos1 < template.Length)
                buffer.Append(template, pos1, template.Length - pos1);

            return buffer.ToString();
        }

        public ITextField textField { get { return _textField; } }

        public string font
        {
            get { return _textField.font; }
            set { _textField.font = value; }
        }

        public int fontSize
        {
            get { return _textField.fontSize; }
            set { _textField.fontSize = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Color color
        {
            get
            {
                return _textField.fontColor;
            }
            set
            {
                if (_textField.fontColor != value)
                {
                    _textField.fontColor = value;
                    UpdateGear(4);
                }
            }
        }

        public AlignType align
        {
            get { return _textField.align; }
            set { _textField.align = value; }
        }

        public VertAlignType verticalAlign
        {
            get { return _textField.verticalAlign; }
            set { _textField.verticalAlign = value; }
        }

        public virtual bool singleLine
        {
            get { return _textField.singleLine; }
            set { _textField.singleLine = value; }
        }

        public int stroke
        {
            get { return _textField.stroke; }
            set { _textField.stroke = value; }
        }

        public Color strokeColor
        {
            get { return _textField.strokeColor; }
            set
            {
                _textField.strokeColor = value;
                UpdateGear(4);
            }
        }

        public Vector2 shadowOffset
        {
            get { return _textField.shadowOffset; }
            set { _textField.shadowOffset = value; }
        }

        public AutoSizeType autoSize
        {
            get { return _textField.autoSize; }
            set
            {
                _textField.autoSize = value;

            }
        }

        public float textWidth
        {
            get { return _textField.width; }
        }

        public float textHeight
        {
            get { return _textField.height; }
        }

        public bool ubbEnabled
        {
            get { return _textField.ubbEnabled; }
            set { _textField.ubbEnabled = value; }
        }

        protected void OnTextFieldSizeChanged()
        {
            _updatingSize = true;
            size = _textField.size;
            _updatingSize = false;
        }

        override protected void HandleSizeChanged(bool fromNode)
        {
            base.HandleSizeChanged(fromNode);
            if (_updatingSize)
                return;

            if (underConstruct)
                _textField.size = size;
            else if (_textField.autoSize != AutoSizeType.Both)
            {
                if (_textField.autoSize == AutoSizeType.Height)
                {
                    _textField.width = this.width;
                }
                else
                    _textField.size = size;
            }
        }

        override public void Setup_BeforeAdd(ByteBuffer buffer, int beginPos)
        {
            base.Setup_BeforeAdd(buffer, beginPos);

            buffer.Seek(beginPos, 5);

            var @font = buffer.ReadS();
            var @fontSize = buffer.ReadShort();
            var @fontColor = buffer.ReadColor();
            var @align = (AlignType)buffer.ReadByte();
            var @verticalAlign = (VertAlignType)buffer.ReadByte();
            var @lineSpacing = buffer.ReadShort();
            //tf.letterSpacing = buffer.ReadShort();            
            buffer.Skip(2);
            var @ubbEnabled = buffer.ReadBool();
            var @autoSize = (AutoSizeType)buffer.ReadByte();
            // tf.underline = buffer.ReadBool();
            // tf.italic = buffer.ReadBool();
            // tf.bold = buffer.ReadBool();
            buffer.Skip(3);
            this.singleLine = buffer.ReadBool();
            _textField.font = @font;
            _textField.fontSize = fontSize;
            _textField.fontColor = @fontColor;
            this.align = (AlignType)@align;
            this.verticalAlign = (VertAlignType)@verticalAlign;
            _textField.lineSpacing = @lineSpacing;

            _textField.ubbEnabled = @ubbEnabled;
            this.autoSize = (AutoSizeType)@autoSize;

            if (buffer.ReadBool())
            {
                _textField.strokeColor = buffer.ReadColor();
                _textField.stroke = (int)buffer.ReadFloat();
            }

            if (buffer.ReadBool())
            {
                _textField.shadowColor = buffer.ReadColor();
                float f1 = buffer.ReadFloat();
                float f2 = buffer.ReadFloat();
                _textField.shadowOffset = new Vector2(f1, f2);
            }

            if (buffer.ReadBool())
                _templateVars = new Dictionary<string, string>();

            if (buffer.version >= 3)
            {
                // tf.strikethrough = buffer.ReadBool();
                // #if FAIRYGUI_TMPRO
                //                 tf.faceDilate = buffer.ReadFloat();
                //                 tf.outlineSoftness = buffer.ReadFloat();
                //                 tf.underlaySoftness = buffer.ReadFloat();
                // #else
                buffer.Skip(13);
                // #endif
            }

        }

        override public void Setup_AfterAdd(ByteBuffer buffer, int beginPos)
        {
            base.Setup_AfterAdd(buffer, beginPos);

            buffer.Seek(beginPos, 6);

            string str = buffer.ReadS();
            if (str != null)
                this.text = str;
        }
    }
}
