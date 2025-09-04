using Godot;
using FairyGUI.Utils;

namespace FairyGUI
{
    /// <summary>
    /// GImage class.
    /// </summary>
    public class GImage : GObject, IColorGear
    {
        NImage _content;

        public GImage()
        {
            touchable = false;
        }

        override protected void CreateDisplayObject()
        {
            _content = new NImage(this);
            displayObject = _content;
        }

        public Color color
        {
            get { return _content.color; }
            set
            {
                _content.color = value;
                UpdateGear(4);
            }
        }

        public FlipType flip
        {
            get { return _content.flip; }
            set { _content.flip = value; }
        }

        public FillMethod fillMethod
        {
            get { return _content.fillMethod; }
            set { _content.fillMethod = value; }
        }

        /// <summary>
        /// Fill origin.
        /// </summary>
        /// <seealso cref="OriginHorizontal"/>
        /// <seealso cref="OriginVertical"/>
        /// <seealso cref="Origin90"/>
        /// <seealso cref="Origin180"/>
        /// <seealso cref="Origin360"/>
        public int fillOrigin
        {
            get { return _content.fillOrigin; }
            set { _content.fillOrigin = value; }
        }

        public bool fillClockwise
        {
            get { return _content.fillClockwise; }
            set { _content.fillClockwise = value; }
        }

        public float fillAmount
        {
            get { return _content.fillAmount; }
            set { _content.fillAmount = value; }
        }

        public NTexture texture
        {
            get { return _content.texture; }
            set
            {
                if (value != null)
                {
                    sourceWidth = value.width;
                    sourceHeight = value.height;
                }
                else
                {
                    sourceWidth = 0;
                    sourceHeight = 0;
                }
                initWidth = sourceWidth;
                initHeight = sourceHeight;
                _content.texture = value;
            }
        }

        override public void ConstructFromResource()
        {
            //this.name = $"GImage({packageItem.name})";
            
            PackageItem contentItem = packageItem.getBranch();
            sourceWidth = contentItem.width;
            sourceHeight = contentItem.height;
            initWidth = sourceWidth;
            initHeight = sourceHeight;

            contentItem = contentItem.getHighResolution();
            contentItem.Load();
            _content.scale9Grid = contentItem.scale9Grid;
            _content.scaleByTile = contentItem.scaleByTile;
            _content.tileGridIndice = contentItem.tileGridIndice;
            _content.texture = contentItem.texture;
            _content.textureScale = new Vector2(contentItem.width / (float)sourceWidth, contentItem.height / (float)sourceHeight);

            SetSize(sourceWidth, sourceHeight);
        }

        override public void Setup_BeforeAdd(ByteBuffer buffer, int beginPos)
        {
            base.Setup_BeforeAdd(buffer, beginPos);

            buffer.Seek(beginPos, 5);

            if (buffer.ReadBool())
            {
                Color color = buffer.ReadColor();
                color.A = _content.color.A;
                _content.color = color;
            }
                
            _content.flip = (FlipType)buffer.ReadByte();
            _content.fillMethod = (FillMethod)buffer.ReadByte();
            if (_content.fillMethod != FillMethod.None)
            {
                _content.fillOrigin = buffer.ReadByte();
                _content.fillClockwise = buffer.ReadBool();
                _content.fillAmount = buffer.ReadFloat();
            }
        }
    }
}
