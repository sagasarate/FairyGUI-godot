using System.Collections.Generic;
using System.Drawing;
using FairyGUI.Utils;
using Godot;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public class GTextInput : GTextField
    {
        /// <summary>
        /// 
        /// </summary>
        IInputTextField _inputTextField;

        EventListener _onChanged;
        EventListener _onSubmit;

        public GTextInput()
        {
            _inputTextField.autoSize = AutoSizeType.None;
            touchable = true;
            tabStop = true;
            focusable = true;
        }

        override protected void CreateDisplayObject()
        {
            CreateLineInputTextField();
        }

        void CreateLineInputTextField()
        {
            var obj = new LineInputTextField(this);
            obj.Resized += OnTextFieldSizeChanged;
            obj.Size = size;
            if (_inputTextField is Control control)
            {
                obj.CloneSetting(_inputTextField);
                if (control.GetParent() != null)
                    control.AddSibling(obj);
                control.QueueFree();
            }
            displayObject = obj;
            _inputTextField = obj;
            _textField = _inputTextField;
        }
        void CreateInputTextField()
        {
            var obj = new InputTextField(this);
            obj.Resized += OnTextFieldSizeChanged;
            obj.Size = size;
            if (_inputTextField is Control control)
            {
                obj.CloneSetting(_inputTextField);
                if (control.GetParent() != null)
                    control.AddSibling(obj);
                control.QueueFree();
            }
            displayObject = obj;
            _inputTextField = obj;
            _textField = _inputTextField;
        }

        /// <summary>
        /// 
        /// </summary>
        public EventListener onChanged
        {
            get { return _onChanged ?? (_onChanged = new EventListener(this, "onChanged")); }
        }

        /// <summary>
        /// 
        /// </summary>
        public EventListener onSubmit
        {
            get { return _onSubmit ?? (_onSubmit = new EventListener(this, "onSubmit")); }
        }

        public override bool singleLine
        {
            get { return displayObject is LineInputTextField; }
            set
            {
                if (value)
                {
                    if (!(displayObject is LineInputTextField))
                    {
                        CreateLineInputTextField();
                    }
                }
                else
                {
                    if (!(displayObject is InputTextField))
                    {
                        CreateInputTextField();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool editable
        {
            get { return _inputTextField.editable; }
            set { _inputTextField.editable = value; }
        }



        /// <summary>
        /// 
        /// </summary>
        public int maxLength
        {
            get { return _inputTextField.maxLength; }
            set { _inputTextField.maxLength = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string restrict
        {
            get { return _inputTextField.restrict; }
            set { _inputTextField.restrict = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool displayAsPassword
        {
            get { return _inputTextField.displayAsPassword; }
            set { _inputTextField.displayAsPassword = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int caretPosition
        {
            get { return _inputTextField.caretPosition; }
            set { _inputTextField.caretPosition = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string promptText
        {
            get { return _inputTextField.promptText; }
            set { _inputTextField.promptText = value; }
        }

        /// <summary>
        /// 在移动设备上是否使用键盘输入。如果false，则文本在获得焦点后不会弹出键盘。
        /// </summary>
        public bool keyboardInput
        {
            get { return _inputTextField.keyboardInput; }
            set { _inputTextField.keyboardInput = value; }
        }

        /// <summary>
        /// <see cref="UnityEngine.TouchScreenKeyboardType"/>
        /// </summary>
        public FairyGUI.VirtualKeyboardType keyboardType
        {
            get { return _inputTextField.keyboardType; }
            set { _inputTextField.keyboardType = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool disableIME
        {
            get { return _inputTextField.disableIME; }
            set { _inputTextField.disableIME = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<uint, Emoji> emojies
        {
            get { return _inputTextField.emojies; }
            set { _inputTextField.emojies = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        public void SetSelection(int start, int length)
        {
            _inputTextField.SetSelection(start, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void ReplaceSelection(string value)
        {
            _inputTextField.ReplaceSelection(value);
        }

        override protected void SetTextFieldText()
        {
            _inputTextField.text = _text;
        }

        public override void Setup_BeforeAdd(ByteBuffer buffer, int beginPos)
        {
            base.Setup_BeforeAdd(buffer, beginPos);

            buffer.Seek(beginPos, 4);

            string str = buffer.ReadS();
            if (str != null)
                _inputTextField.promptText = str;

            str = buffer.ReadS();
            if (str != null)
                _inputTextField.restrict = str;

            int iv = buffer.ReadInt();
            if (iv != 0)
                _inputTextField.maxLength = iv;
            iv = buffer.ReadInt();
            if (iv != 0)
                _inputTextField.keyboardType = (FairyGUI.VirtualKeyboardType)iv;
            if (buffer.ReadBool())
                _inputTextField.displayAsPassword = true;
        }
    }
}