using Godot;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public class NInputEvent
    {
        /// <summary>
        /// x position in stage coordinates.
        /// </summary>
        public float x { get; internal set; }

        /// <summary>
        /// y position in stage coordinates.
        /// </summary>
        public float y { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public Key keyCode { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public long character { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public KeyModifierMask keyModifiers { get; internal set; }
        public MouseButtonMask mouseModifiers { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public float mouseWheelDelta { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public int touchId { get; internal set; }

        /// <summary>
        /// -1-none,0-left,1-right,2-middle
        /// </summary>
        public MouseButton button { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int clickCount { get; internal set; }

        /// <summary>
        /// Duraion of holding the button. You can read this in touchEnd or click event.
        /// </summary>
        /// <value></value>
        public float holdTime { get; internal set; }

        public NInputEvent()
        {
            Clear();
        }
        public void Clear()
        {
            x = 0;
            y = 0;
            keyCode = Key.None;
            character = '\0';
            keyModifiers = 0;
            mouseModifiers = 0;
            touchId = -1;
            button = MouseButton.None;
            clickCount = 0;
            holdTime = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 position
        {
            get { return new Vector2(x, y); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isDoubleClick
        {
            get { return clickCount > 1 && button == MouseButton.Left; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ctrlOrCmd
        {
            get
            {
                return ctrl || command;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ctrl
        {
            get
            {
                return Input.IsKeyPressed(Key.Ctrl);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool shift
        {
            get
            {
                return Input.IsKeyPressed(Key.Shift);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool alt
        {
            get
            {
                return Input.IsKeyPressed(Key.Alt);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool command
        {
            get
            {
                return Input.IsKeyPressed(Key.Meta);
            }
        }
    }
}
