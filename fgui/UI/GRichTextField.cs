using System.Collections.Generic;
using FairyGUI.Utils;
using Godot;

namespace FairyGUI
{
    /// <summary>
    /// GRichTextField class.
    /// </summary>
    public class GRichTextField : GTextField
    {
        /// <summary>
        /// 
        /// </summary>
        protected RichTextField _richTextField;

        public GRichTextField()
            : base()
        {
        }

        override protected void CreateDisplayObject()
        {
            _richTextField = new RichTextField(this);
            _richTextField.Resized += OnTextFieldSizeChanged;
            displayObject = _richTextField;
            _textField = _richTextField;
        }
    }
}
