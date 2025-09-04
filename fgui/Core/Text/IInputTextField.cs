using FairyGUI;
using Godot;
using System.Collections.Generic;

namespace FairyGUI
{
    public enum VirtualKeyboardType
    {
        Default,
        Character,
        NumberAndInterpunction,
        Url,
        Number,
        Phone,
        EmailAddress

    }
    public interface IInputTextField : ITextField
    {
        public bool editable { get; set; }
        public int maxLength { get; set; }
        public string restrict { get; set; }
        public bool displayAsPassword { get; set; }
        public int caretPosition { get; set; }
        public string promptText { get; set; }
        public bool keyboardInput { get; set; }
        public FairyGUI.VirtualKeyboardType keyboardType { get; set; }
        public bool disableIME { get; set; }
        public Dictionary<uint, Emoji> emojies { get; set; }
        public void SetSelection(int start, int length);
        public void ReplaceSelection(string value);
        public void CloneSetting(IInputTextField target);
    }
}