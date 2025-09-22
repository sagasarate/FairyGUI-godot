using Godot;
using FairyGUI.Utils;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class GLoader3D : GObject, IAnimationGear, IColorGear
    {
        string _url;
        AlignType _align;
        VertAlignType _verticalAlign;
        bool _autoSize;
        FillType _fill;
        bool _shrinkOnly;
        string _animationName;
        string _skinName;
        bool _playing;
        int _frame;
        bool _loop;
        bool _updatingLayout;
        Color _color;

        protected PackageItem _contentItem;
        protected NContainer3D _content;

        public GLoader3D()
        {
            _url = string.Empty;
            _align = AlignType.Left;
            _verticalAlign = VertAlignType.Top;
            _playing = true;
            _color = Colors.White;
        }

        override protected void CreateDisplayObject()
        {
            _content = new NContainer3D(this);
            _content.onUpdate += OnUpdateContent;
            displayObject = _content;
        }

        override public void Dispose()
        {
            _content.QueueFree();
            base.Dispose();
        }

        public NContainer3D content { get { return _content; } }

        /// <summary>
        /// 
        /// </summary>
        public string url
        {
            get { return _url; }
            set
            {
                if (_url == value)
                    return;

                ClearContent();
                _url = value;
                LoadContent();
                UpdateGear(7);
            }
        }

        override public string icon
        {
            get { return _url; }
            set { this.url = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public AlignType align
        {
            get { return _align; }
            set
            {
                if (_align != value)
                {
                    _align = value;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public VertAlignType verticalAlign
        {
            get { return _verticalAlign; }
            set
            {
                if (_verticalAlign != value)
                {
                    _verticalAlign = value;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FillType fill
        {
            get { return _fill; }
            set
            {
                if (_fill != value)
                {
                    _fill = value;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool shrinkOnly
        {
            get { return _shrinkOnly; }
            set
            {
                if (_shrinkOnly != value)
                {
                    _shrinkOnly = value;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool autoSize
        {
            get { return _autoSize; }
            set
            {
                if (_autoSize != value)
                {
                    _autoSize = value;
                    UpdateLayout();
                }
            }
        }

        public bool playing
        {
            get { return _playing; }
            set
            {
                if (_playing != value)
                {
                    _playing = value;
                    OnChange("playing");
                    UpdateGear(5);
                }
            }
        }

        public int frame
        {
            get { return _frame; }
            set
            {

                if (_frame != value)
                {
                    _frame = value;
                    OnChange("frame");
                    UpdateGear(5);
                }
            }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public float timeScale
        {
            get;
            set;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public bool ignoreEngineTimeScale
        {
            get;
            set;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="time"></param>
        public void Advance(float time)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public bool loop
        {
            get { return _loop; }
            set
            {
                if (_loop != value)
                {
                    _loop = value;
                    OnChange("loop");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string animationName
        {
            get { return _animationName; }
            set
            {
                _animationName = value;
                OnChange("animationName");
                UpdateGear(5);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string skinName
        {
            get { return _skinName; }
            set
            {
                _skinName = value;
                OnChange("skinName");
                UpdateGear(5);
            }
        }

        // public Material material
        // {
        //     get { return _content.material; }
        //     set { _content.material = value; }
        // }
        // public string shader
        // {
        //     get { return _content.shader; }
        //     set { _content.shader = value; }
        // }

        /// <summary>
        /// 
        /// </summary>
        public Color color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    UpdateGear(4);

                    OnChange("color");
                }
            }
        }

        // override public IFilter filter
        // {
        //     get { return _content.filter; }
        //     set { _content.filter = value; }
        // }

        override public BlendMode blendMode
        {
            get { return _content.blendMode; }
            set { _content.blendMode = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void LoadContent()
        {
            ClearContent();

            if (string.IsNullOrEmpty(_url))
                return;

            _contentItem = UIPackage.GetItemByURL(_url);

            if (_contentItem != null)
            {
                _contentItem.skeletonLoaders.Add(this);
                _contentItem = _contentItem.getBranch();
                _contentItem = _contentItem.getHighResolution();
                _contentItem.Load();

                if (_contentItem.type == PackageItemType.Spine)
                {

                }
                else if (_contentItem.type == PackageItemType.DragoneBones)
                {

                }
            }
            else
                LoadExternal();
        }

        virtual protected void OnChange(string propertyName)
        {
            if (_contentItem == null)
                return;


            if (_contentItem.type == PackageItemType.Spine)
            {

            }
            else if (_contentItem.type == PackageItemType.DragoneBones)
            {

            }
        }

        virtual protected void LoadExternal()
        {
        }

        virtual protected void FreeExternal()
        {

        }

        protected void UpdateLayout()
        {
            if (sourceWidth == 0 || sourceHeight == 0)
                return;

            float contentWidth = sourceWidth;
            float contentHeight = sourceHeight;

            if (_autoSize)
            {
                _updatingLayout = true;
                if (contentWidth == 0)
                    contentWidth = 50;
                if (contentHeight == 0)
                    contentHeight = 30;
                SetSize(contentWidth, contentHeight);

                _updatingLayout = false;

                if (_width == contentWidth && _height == contentHeight)
                {
                    _content.position = Vector2.Zero;
                    _content.Scale = Vector2.One;
                    return;
                }
                //如果不相等，可能是由于大小限制造成的，要后续处理
            }

            float sx = 1, sy = 1;
            if (_fill != FillType.None)
            {
                sx = this.width / sourceWidth;
                sy = this.height / sourceHeight;

                if (sx != 1 || sy != 1)
                {
                    if (_fill == FillType.ScaleMatchHeight)
                        sx = sy;
                    else if (_fill == FillType.ScaleMatchWidth)
                        sy = sx;
                    else if (_fill == FillType.Scale)
                    {
                        if (sx > sy)
                            sx = sy;
                        else
                            sy = sx;
                    }
                    else if (_fill == FillType.ScaleNoBorder)
                    {
                        if (sx > sy)
                            sy = sx;
                        else
                            sx = sy;
                    }

                    if (_shrinkOnly)
                    {
                        if (sx > 1)
                            sx = 1;
                        if (sy > 1)
                            sy = 1;
                    }

                    contentWidth = sourceWidth * sx;
                    contentHeight = sourceHeight * sy;
                }
            }

            _content.Scale = new Vector2(sx, sy);

            float nx;
            float ny;
            if (_align == AlignType.Center)
                nx = (this.width - contentWidth) / 2;
            else if (_align == AlignType.Right)
                nx = this.width - contentWidth;
            else
                nx = 0;
            if (_verticalAlign == VertAlignType.Middle)
                ny = (this.height - contentHeight) / 2;
            else if (_verticalAlign == VertAlignType.Bottom)
                ny = this.height - contentHeight;
            else
                ny = 0;
            _content.position = new Vector2(nx, ny);
        }

        protected void ClearContent()
        {
            if (_content != null)
            {
                if (_contentItem != null)
                {
                    if (_contentItem.type == PackageItemType.Spine)
                    {

                    }
                    else if (_contentItem.type == PackageItemType.DragoneBones)
                    {

                    }
                }
                else
                    FreeExternal();
            }
            _contentItem = null;
        }

        protected void OnUpdateContent(double delta)
        {
            if (_contentItem == null)
                return;


            if (_contentItem.type == PackageItemType.Spine)
            {

            }
            else if (_contentItem.type == PackageItemType.DragoneBones)
            {

            }
        }

        override protected void HandleSizeChanged(bool fromNode)
        {
            base.HandleSizeChanged(fromNode);

            if (!_updatingLayout)
                UpdateLayout();
        }

        override public void Setup_BeforeAdd(ByteBuffer buffer, int beginPos)
        {
            base.Setup_BeforeAdd(buffer, beginPos);

            buffer.Seek(beginPos, 5);

            _url = buffer.ReadS();
            _align = (AlignType)buffer.ReadByte();
            _verticalAlign = (VertAlignType)buffer.ReadByte();
            _fill = (FillType)buffer.ReadByte();
            _shrinkOnly = buffer.ReadBool();
            _autoSize = buffer.ReadBool();
            _animationName = buffer.ReadS();
            _skinName = buffer.ReadS();
            _playing = buffer.ReadBool();
            _frame = buffer.ReadInt();
            _loop = buffer.ReadBool();

            if (buffer.ReadBool())
                this.color = buffer.ReadColor(); //color

            if (!string.IsNullOrEmpty(_url))
                LoadContent();
        }
    }
}