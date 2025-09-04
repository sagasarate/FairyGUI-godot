using Godot;
namespace FairyGUI
{
    public interface ITextField
    {
        public string font { get; set; }
        public int fontSize { get; set; }
        public float lineSpacing { get; set; }
        public Color fontColor { get; set; }
        public AlignType align { get; set; }
        public VertAlignType verticalAlign { get; set; }
        public AutoSizeType autoSize { get; set; }
        public bool singleLine { get; set; }
        public int stroke { get; set; }
        public Color strokeColor { get; set; }
        public Vector2 shadowOffset { get; set; }
        public Color shadowColor { get; set; }
        public Vector2 size { get; set; }
        public float width { get; set; }
        public float height { get; set; }
        public int maxWidth { get; set; }
        public string text { get; set; }
        public bool ubbEnabled { get; set; }

    }
}