using FairyGUI.Utils;
using Godot;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class NImage : Control, IDisplayObject
    {
        protected float _skewX = 0;
        protected float _skewY = 0;
        protected Rect? _scale9Grid;
        protected bool _scaleByTile;
        protected Vector2 _textureScale = Vector2.One;
        protected int _tileGridIndice = 0;
        protected FlipType _flip = FlipType.None;
        protected FillMethod _fillMethod = FillMethod.None;
        protected int _fillOrigin = 0;
        protected float _fillAmount = 1f;
        protected bool _fillClockwise = true;
        protected NTexture _texture;
        protected CanvasItemMaterial _material;
        protected ArrayMesh _mesh;
        protected SurfaceTool _surfaceTool;



        public GObject gOwner { get; set; }
        public IDisplayObject parent { get { return GetParent() as IDisplayObject; } }
        public Control node { get { return this; } }
        public bool visible { get { return Visible; } set { Visible = value; } }
        public float skewX
        {
            get { return _skewX; }
            set
            {
                if (!Mathf.IsEqualApprox(_skewX, value))
                {
                    _skewX = value;
                    QueueRedraw();
                }
            }
        }
        public float skewY
        {
            get { return _skewY; }
            set
            {
                if (!Mathf.IsEqualApprox(_skewY, value))
                {
                    _skewY = value;
                    QueueRedraw();
                }
            }
        }

        public BlendMode blendMode
        {
            get
            {
                switch (_material.BlendMode)
                {
                    case CanvasItemMaterial.BlendModeEnum.Mix:
                        return BlendMode.Normal;
                    case CanvasItemMaterial.BlendModeEnum.Add:
                        return BlendMode.Add;
                    case CanvasItemMaterial.BlendModeEnum.Mul:
                        return BlendMode.Multiply;
                    case CanvasItemMaterial.BlendModeEnum.PremultAlpha:
                        return BlendMode.Off;
                    default:
                        return BlendMode.None;
                }
            }
            set
            {
                switch (value)
                {
                    case BlendMode.Normal:
                        _material.BlendMode = CanvasItemMaterial.BlendModeEnum.Mix;
                        break;
                    case BlendMode.Add:
                        _material.BlendMode = CanvasItemMaterial.BlendModeEnum.Add;
                        break;
                    case BlendMode.Multiply:
                        _material.BlendMode = CanvasItemMaterial.BlendModeEnum.Mul;
                        break;
                    default:
                        _material.BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha;
                        break;
                }
            }
        }
        public event System.Action<double> onUpdate;
        public override void _Process(double delta)
        {
            if (onUpdate != null)
                onUpdate(delta);
        }
        public NImage(GObject owner)
        {
            gOwner = owner;
            Init();
        }
        public NImage(NTexture texture)
            : base()
        {
            Init();
            if (texture != null)
                UpdateTexture(texture);
        }

        void Init()
        {
            //Name = "Image";
            MouseFilter = MouseFilterEnum.Ignore;
            _material = new CanvasItemMaterial();
            _material.LightMode = CanvasItemMaterial.LightModeEnum.Unshaded;
            _mesh = new ArrayMesh();
            _surfaceTool = new SurfaceTool();
        }
        public NTexture texture
        {
            get { return _texture; }
            set
            {
                UpdateTexture(value);
            }
        }
        public Vector2 textureScale
        {
            get { return _textureScale; }
            set
            {
                if (!Mathf.IsEqualApprox(_textureScale.X, value.X) || !Mathf.IsEqualApprox(_textureScale.Y, value.Y))
                {
                    _textureScale = value;
                    QueueRedraw();
                }
            }
        }
        public Color color
        {
            get
            {
                return Modulate;
            }
            set
            {
                Modulate = value;
            }
        }
        public FlipType flip
        {
            get { return _flip; }
            set
            {
                if (_flip != value)
                {
                    _flip = value;
                    QueueRedraw();
                }
            }
        }
        public FillMethod fillMethod
        {
            get { return _fillMethod; }
            set
            {
                if (_fillMethod != value)
                {
                    _fillMethod = value;
                    QueueRedraw();
                }
            }
        }
        public int fillOrigin
        {
            get { return _fillOrigin; }
            set
            {
                if (_fillOrigin != value)
                {
                    _fillOrigin = value;
                    QueueRedraw();
                }
            }
        }
        public bool fillClockwise
        {
            get { return _fillClockwise; }
            set
            {
                if (_fillClockwise != value)
                {
                    _fillClockwise = value;
                    QueueRedraw();
                }
            }
        }
        public float fillAmount
        {
            get { return _fillAmount; }
            set
            {
                if (!Mathf.IsEqualApprox(_fillAmount, value))
                {
                    _fillAmount = Mathf.Clamp(value, 0f, 1f);
                    QueueRedraw();
                }
            }
        }
        public Rect? scale9Grid
        {
            get { return _scale9Grid; }
            set
            {
                if (_scale9Grid != value)
                {
                    _scale9Grid = value;
                    QueueRedraw();
                }
            }
        }
        public bool scaleByTile
        {
            get { return _scaleByTile; }
            set
            {
                if (_scaleByTile != value)
                {
                    _scaleByTile = value;
                    QueueRedraw();
                }
            }
        }
        public int tileGridIndice
        {
            get { return _tileGridIndice; }
            set
            {
                if (_tileGridIndice != value)
                {
                    _tileGridIndice = value;
                    QueueRedraw();
                }
            }
        }
        public void SetNativeSize()
        {
            if (_texture != null)
                Size = new Vector2(_texture.width, _texture.height);
            else
                Size = Vector2.Zero;
        }

        void UpdateTexture(NTexture value)
        {
            if (value == _texture)
                return;
            _texture = value;
            _textureScale = Vector2.One;
            if (Mathf.IsEqualApprox(Size.X, 0))
                SetNativeSize();
            QueueRedraw();
        }

        public void BuildMesh(SurfaceTool surfaceTool)
        {
            if (_texture == null)
            {
                return;
            }
            _mesh.ClearSurfaces();
            _surfaceTool.Clear();
            _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
            Rect vertRect = new Rect(Vector2.Zero, Size);
            vertRect = _texture.GetDrawRect(vertRect, _flip);
            Rect uvRect = _texture.uvRect;
            if (_flip != FlipType.None)
            {
                if (_flip == FlipType.Horizontal || _flip == FlipType.Both)
                {
                    float tmp = uvRect.xMin;
                    uvRect.xMin = uvRect.xMax;
                    uvRect.xMax = tmp;
                }
                if (_flip == FlipType.Vertical || _flip == FlipType.Both)
                {
                    float tmp = uvRect.yMin;
                    uvRect.yMin = uvRect.yMax;
                    uvRect.yMax = tmp;
                }
            }
            TextureRepeat = TextureRepeatEnum.Disabled;
            if (_fillMethod != FillMethod.None)
            {
                switch (_fillMethod)
                {
                    case FillMethod.Horizontal:
                        FillHorizontal(surfaceTool, vertRect, uvRect, _fillOrigin, _fillAmount);
                        break;

                    case FillMethod.Vertical:
                        FillVertical(surfaceTool, vertRect, uvRect, _fillOrigin, _fillAmount);
                        break;

                    case FillMethod.Radial90:
                        FillRadial90(surfaceTool, vertRect, uvRect, (Origin90)_fillOrigin, _fillAmount, _fillClockwise, 0);
                        break;

                    case FillMethod.Radial180:
                        FillRadial180(surfaceTool, vertRect, uvRect, (Origin180)_fillOrigin, _fillAmount, _fillClockwise, 0);
                        break;

                    case FillMethod.Radial360:
                        FillRadial360(surfaceTool, vertRect, uvRect, (Origin360)_fillOrigin, _fillAmount, _fillClockwise, 0);
                        break;
                }
            }
            else if (_scaleByTile)
            {
                if (_texture.root == _texture && _texture.nativeTexture != null)
                {
                    //独立纹理，可以直接使用tile模式
                    TextureRepeat = TextureRepeatEnum.Enabled;
                    uvRect.width *= vertRect.width / texture.width * _textureScale.X;
                    uvRect.height *= vertRect.height / texture.height * _textureScale.Y;
                    ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, 0);
                }
                else
                {
                    Rect contentRect = vertRect;
                    contentRect.width *= _textureScale.X;
                    contentRect.height *= _textureScale.Y;

                    TileFill(surfaceTool, contentRect, uvRect, texture.width, texture.height, 0);
                }
            }
            else if (_scale9Grid != null)
            {
                SliceFill(surfaceTool, vertRect, uvRect, texture.width, texture.height);
            }
            else
            {
                ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, 0);
            }

            _surfaceTool.GenerateNormals();
            if (_material != null)
                _surfaceTool.SetMaterial(_material);
            _surfaceTool.Commit(_mesh);
        }

        void FillHorizontal(SurfaceTool surfaceTool, Rect vertRect, Rect uvRect, int origin, float amount)
        {
            float a = vertRect.width * amount;
            if ((OriginHorizontal)origin == OriginHorizontal.Right || (OriginVertical)origin == OriginVertical.Bottom)
                vertRect.X += vertRect.width - a;
            vertRect.width = a;

            a = uvRect.width * amount;
            if ((OriginHorizontal)origin == OriginHorizontal.Right || (OriginVertical)origin == OriginVertical.Bottom)
                uvRect.X += uvRect.width - a;
            uvRect.width = a;

            Utils.ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, 0);
        }

        void FillVertical(SurfaceTool surfaceTool, Rect vertRect, Rect uvRect, int origin, float amount)
        {
            float a = vertRect.height * amount;
            if ((OriginHorizontal)origin == OriginHorizontal.Right || (OriginVertical)origin == OriginVertical.Bottom)
                vertRect.Y += vertRect.height - a;
            vertRect.height = a;

            a = uvRect.height * amount;
            if ((OriginHorizontal)origin == OriginHorizontal.Right || (OriginVertical)origin == OriginVertical.Bottom)
                uvRect.Y += uvRect.height - a;
            uvRect.height = a;

            Utils.ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, 0);
        }

        int FillRadial90(SurfaceTool surfaceTool, Rect vertRect, Rect uvRect, Origin90 origin, float amount, bool clockwise, int StartIndex)
        {
            bool flipX = origin == Origin90.TopRight || origin == Origin90.BottomRight;
            bool flipY = origin == Origin90.BottomLeft || origin == Origin90.BottomRight;
            if (flipX != flipY)
                clockwise = !clockwise;

            float ratio = clockwise ? amount : (1 - amount);
            float tan = Mathf.Tan(Mathf.Pi * 0.5f * ratio);
            bool thresold = false;
            if (ratio != 1)
                thresold = (vertRect.height / vertRect.width - tan) > 0;
            if (!clockwise)
                thresold = !thresold;
            float x = vertRect.X + (ratio == 0 ? float.MaxValue : (vertRect.height / tan));
            float y = vertRect.Y + (ratio == 1 ? float.MaxValue : (vertRect.width * tan));
            float x2 = x;
            float y2 = y;
            if (flipX)
                x2 = vertRect.width - x;
            if (flipY)
                y2 = vertRect.height - y;
            float xMin = flipX ? (vertRect.width - vertRect.X) : vertRect.xMin;
            float yMin = flipY ? (vertRect.height - vertRect.Y) : vertRect.yMin;
            float xMax = flipX ? -vertRect.xMin : vertRect.xMax;
            float yMax = flipY ? -vertRect.yMin : vertRect.yMax;

            ToolSet.MeshAddVertex(surfaceTool, xMin, yMin, vertRect, uvRect);
            StartIndex++;
            if (clockwise)
            {
                ToolSet.MeshAddVertex(surfaceTool, xMax, yMin, vertRect, uvRect);
                StartIndex++;
            }
            if (y > vertRect.yMax)
            {
                if (thresold)
                {
                    ToolSet.MeshAddVertex(surfaceTool, x2, yMax, vertRect, uvRect);
                    StartIndex++;
                }
                else
                {
                    ToolSet.MeshAddVertex(surfaceTool, xMax, yMax, vertRect, uvRect);
                    StartIndex++;
                }
            }
            else
            {
                Utils.ToolSet.MeshAddVertex(surfaceTool, xMax, y2, vertRect, uvRect);
                StartIndex++;
            }
            if (x > vertRect.xMax)
            {
                if (thresold)
                {
                    ToolSet.MeshAddVertex(surfaceTool, xMax, y2, vertRect, uvRect);
                    StartIndex++;
                }
                else
                {
                    ToolSet.MeshAddVertex(surfaceTool, xMax, yMax, vertRect, uvRect);
                    StartIndex++;
                }
            }
            else
            {
                ToolSet.MeshAddVertex(surfaceTool, x2, yMax, vertRect, uvRect);
                StartIndex++;
            }
            if (!clockwise)
            {
                ToolSet.MeshAddVertex(surfaceTool, xMin, yMax, vertRect, uvRect);
                StartIndex++;
            }
            if (flipX == flipY)
            {
                surfaceTool.AddIndex(0);
                surfaceTool.AddIndex(1);
                surfaceTool.AddIndex(2);

                surfaceTool.AddIndex(0);
                surfaceTool.AddIndex(2);
                surfaceTool.AddIndex(3);
            }
            else
            {
                surfaceTool.AddIndex(2);
                surfaceTool.AddIndex(1);
                surfaceTool.AddIndex(0);

                surfaceTool.AddIndex(3);
                surfaceTool.AddIndex(2);
                surfaceTool.AddIndex(0);
            }
            return StartIndex;
        }
        int FillRadial180(SurfaceTool surfaceTool, Rect vertRect, Rect uvRect, Origin180 origin, float amount, bool clockwise, int StartIndex)
        {
            switch (origin)
            {
                case Origin180.Top:
                    if (amount <= 0.5f)
                    {
                        vertRect.width /= 2;
                        uvRect.width /= 2;
                        if (clockwise)
                        {
                            vertRect.X += vertRect.width;
                            uvRect.X += uvRect.width;
                        }
                        StartIndex = FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.TopLeft : Origin90.TopRight, amount / 0.5f, clockwise, StartIndex);
                    }
                    else
                    {
                        vertRect.width /= 2;
                        uvRect.width /= 2;
                        if (!clockwise)
                        {
                            vertRect.X += vertRect.width;
                            uvRect.X += uvRect.width;
                        }
                        StartIndex = FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.TopRight : Origin90.TopLeft, (amount - 0.5f) / 0.5f, clockwise, StartIndex);
                        if (clockwise)
                        {
                            vertRect.X += vertRect.width;
                            uvRect.X += uvRect.width;
                        }
                        else
                        {
                            vertRect.X -= vertRect.width;
                            uvRect.X -= uvRect.width;
                        }
                        ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, StartIndex);
                        StartIndex += 4;
                    }
                    break;

                case Origin180.Bottom:
                    if (amount <= 0.5f)
                    {
                        vertRect.width /= 2;
                        uvRect.width /= 2;
                        if (!clockwise)
                        {
                            vertRect.X += vertRect.width;
                            uvRect.X += uvRect.width;
                        }
                        StartIndex = FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.BottomRight : Origin90.BottomLeft, amount / 0.5f, clockwise, StartIndex);
                    }
                    else
                    {
                        vertRect.width /= 2;
                        uvRect.width /= 2;
                        if (clockwise)
                        {
                            vertRect.X += vertRect.width;
                            uvRect.X += uvRect.width;
                        }
                        StartIndex = FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.BottomLeft : Origin90.BottomRight, (amount - 0.5f) / 0.5f, clockwise, StartIndex);
                        if (clockwise)
                        {
                            vertRect.X -= vertRect.width;
                            uvRect.X -= uvRect.width;
                        }
                        else
                        {
                            vertRect.X += vertRect.width;
                            uvRect.X += uvRect.width;
                        }
                        ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, StartIndex);
                        StartIndex += 4;
                    }
                    break;

                case Origin180.Left:
                    if (amount <= 0.5f)
                    {
                        vertRect.height /= 2;
                        uvRect.height /= 2;
                        if (!clockwise)
                        {
                            vertRect.Y += vertRect.height;
                            uvRect.Y += uvRect.height;
                        }
                        StartIndex = FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.BottomLeft : Origin90.TopLeft, amount / 0.5f, clockwise, StartIndex);
                    }
                    else
                    {
                        vertRect.height /= 2;
                        uvRect.height /= 2;
                        if (clockwise)
                        {
                            vertRect.Y += vertRect.height;
                            uvRect.Y += uvRect.height;
                        }
                        StartIndex = FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.TopLeft : Origin90.BottomLeft, (amount - 0.5f) / 0.5f, clockwise, StartIndex);
                        if (clockwise)
                        {
                            vertRect.Y -= vertRect.height;
                            uvRect.Y -= uvRect.height;
                        }
                        else
                        {
                            vertRect.Y += vertRect.height;
                            uvRect.Y += uvRect.height;
                        }
                        ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, StartIndex);
                        StartIndex += 4;
                    }
                    break;

                case Origin180.Right:
                    if (amount <= 0.5f)
                    {
                        vertRect.height /= 2;
                        uvRect.height /= 2;
                        if (clockwise)
                        {
                            vertRect.Y += vertRect.height;
                            uvRect.Y += uvRect.height;
                        }
                        StartIndex = FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.TopRight : Origin90.BottomRight, amount / 0.5f, clockwise, StartIndex);
                    }
                    else
                    {
                        vertRect.height /= 2;
                        uvRect.height /= 2;
                        if (!clockwise)
                        {
                            vertRect.Y += vertRect.height;
                            uvRect.Y += uvRect.height;
                        }
                        StartIndex = FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.BottomRight : Origin90.TopRight, (amount - 0.5f) / 0.5f, clockwise, StartIndex);
                        if (clockwise)
                        {
                            vertRect.Y += vertRect.height;
                            uvRect.Y += uvRect.height;
                        }
                        else
                        {
                            vertRect.Y -= vertRect.height;
                            uvRect.Y -= uvRect.height;
                        }
                        ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, StartIndex);
                        StartIndex += 4;
                    }
                    break;
            }
            return StartIndex;
        }
        int FillRadial360(SurfaceTool surfaceTool, Rect vertRect, Rect uvRect, Origin360 origin, float amount, bool clockwise, int StartIndex)
        {
            switch (origin)
            {
                case Origin360.Top:
                    if (amount < 0.5f)
                    {
                        vertRect.width /= 2;
                        uvRect.width /= 2;
                        if (clockwise)
                        {
                            vertRect.X += vertRect.width;
                            uvRect.X += uvRect.width;
                        }
                        StartIndex = FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Left : Origin180.Right, amount / 0.5f, clockwise, StartIndex);
                    }
                    else
                    {
                        vertRect.width /= 2;
                        uvRect.width /= 2;
                        if (!clockwise)
                        {
                            vertRect.X += vertRect.width;
                            uvRect.X += uvRect.width;
                        }
                        StartIndex = FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Right : Origin180.Left, (amount - 0.5f) / 0.5f, clockwise, StartIndex);
                        if (clockwise)
                        {
                            vertRect.X += vertRect.width;
                            uvRect.X += uvRect.width;
                        }
                        else
                        {
                            vertRect.X -= vertRect.width;
                            uvRect.X -= uvRect.width;
                        }
                        ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, StartIndex);
                    }
                    break;
                case Origin360.Bottom:
                    if (amount < 0.5f)
                    {
                        vertRect.width /= 2;
                        uvRect.width /= 2;
                        if (!clockwise)
                        {
                            vertRect.X += vertRect.width;
                            uvRect.X += uvRect.width;
                        }
                        StartIndex = FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Right : Origin180.Left, amount / 0.5f, clockwise, StartIndex);
                    }
                    else
                    {
                        vertRect.width /= 2;
                        uvRect.width /= 2;
                        if (clockwise)
                        {
                            vertRect.X += vertRect.width;
                            uvRect.X += uvRect.width;
                        }
                        StartIndex = FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Left : Origin180.Right, (amount - 0.5f) / 0.5f, clockwise, StartIndex);
                        if (clockwise)
                        {
                            vertRect.X -= vertRect.width;
                            uvRect.X -= uvRect.width;
                        }
                        else
                        {
                            vertRect.X += vertRect.width;
                            uvRect.X += uvRect.width;
                        }
                        ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, StartIndex);
                    }
                    break;

                case Origin360.Left:
                    if (amount < 0.5f)
                    {
                        vertRect.height /= 2;
                        uvRect.height /= 2;
                        if (!clockwise)
                        {
                            vertRect.Y += vertRect.height;
                            uvRect.Y += uvRect.height;
                        }
                        StartIndex = FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Bottom : Origin180.Top, amount / 0.5f, clockwise, StartIndex);
                    }
                    else
                    {
                        vertRect.height /= 2;
                        uvRect.height /= 2;
                        if (clockwise)
                        {
                            vertRect.Y += vertRect.height;
                            uvRect.Y += uvRect.height;
                        }
                        StartIndex = FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Top : Origin180.Bottom, (amount - 0.5f) / 0.5f, clockwise, StartIndex);

                        if (clockwise)
                        {
                            vertRect.Y -= vertRect.height;
                            uvRect.Y -= uvRect.height;
                        }
                        else
                        {
                            vertRect.Y += vertRect.height;
                            uvRect.Y += uvRect.height;
                        }
                        ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, StartIndex);
                    }
                    break;

                case Origin360.Right:
                    if (amount < 0.5f)
                    {
                        vertRect.height /= 2;
                        uvRect.height /= 2;
                        if (clockwise)
                        {
                            vertRect.Y += vertRect.height;
                            uvRect.Y += uvRect.height;
                        }
                        StartIndex = FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Top : Origin180.Bottom, amount / 0.5f, clockwise, StartIndex);
                    }
                    else
                    {
                        vertRect.height /= 2;
                        uvRect.height /= 2;
                        if (!clockwise)
                        {
                            vertRect.Y += vertRect.height;
                            uvRect.Y += uvRect.height;
                        }

                        StartIndex = FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Bottom : Origin180.Top, (amount - 0.5f) / 0.5f, clockwise, StartIndex);

                        if (clockwise)
                        {
                            vertRect.Y += vertRect.height;
                            uvRect.Y += uvRect.height;
                        }
                        else
                        {
                            vertRect.Y -= vertRect.height;
                            uvRect.Y -= uvRect.height;
                        }
                        ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, StartIndex);
                    }
                    break;
            }
            return StartIndex;
        }


        static int[] TRIANGLES_9_GRID = new int[] {
            4,0,1,1,5,4,
            5,1,2,2,6,5,
            6,2,3,3,7,6,
            8,4,5,5,9,8,
            9,5,6,6,10,9,
            10,6,7,7,11,10,
            12,8,9,9,13,12,
            13,9,10,10,14,13,
            14,10,11,
            11,15,14
        };

        static int[] gridTileIndice = new int[] { -1, 0, -1, 2, 4, 3, -1, 1, -1 };
        float[] gridX = new float[4];
        float[] gridY = new float[4];
        float[] gridTexX = new float[4];
        float[] gridTexY = new float[4];

        void SliceFill(SurfaceTool surfaceTool, Rect contentRect, Rect uvRect, float sourceW, float sourceH)
        {
            Rect gridRect = (Rect)_scale9Grid;
            contentRect.width *= _textureScale.X;
            contentRect.height *= _textureScale.Y;

            if (_flip != FlipType.None)
            {
                if (_flip == FlipType.Horizontal || _flip == FlipType.Both)
                {
                    gridRect.X = sourceW - gridRect.xMax;
                    gridRect.xMax = gridRect.X + gridRect.width;
                }

                if (_flip == FlipType.Vertical || _flip == FlipType.Both)
                {
                    gridRect.Y = sourceH - gridRect.yMax;
                    gridRect.yMax = gridRect.Y + gridRect.height;
                }
            }

            float sx = uvRect.width / sourceW;
            float sy = uvRect.height / sourceH;
            float xMax = uvRect.xMax;
            float yMax = uvRect.yMax;
            float xMax2 = gridRect.xMax;
            float yMax2 = gridRect.yMax;

            gridTexX[0] = uvRect.X;
            gridTexX[1] = uvRect.X + gridRect.X * sx;
            gridTexX[2] = uvRect.X + xMax2 * sx;
            gridTexX[3] = xMax;
            gridTexY[0] = uvRect.Y;
            gridTexY[1] = uvRect.Y + gridRect.Y * sy;
            gridTexY[2] = uvRect.Y + yMax2 * sy;
            gridTexY[3] = yMax;


            if (contentRect.width >= (sourceW - gridRect.width))
            {
                gridX[1] = gridRect.X;
                gridX[2] = contentRect.width - (sourceW - xMax2);
                gridX[3] = contentRect.width;
            }
            else
            {
                float tmp = gridRect.X / (sourceW - xMax2);
                tmp = contentRect.width * tmp / (1 + tmp);
                gridX[1] = tmp;
                gridX[2] = tmp;
                gridX[3] = contentRect.width;
            }

            if (contentRect.height >= (sourceH - gridRect.height))
            {
                gridY[1] = gridRect.Y;
                gridY[2] = contentRect.height - (sourceH - yMax2);
                gridY[3] = contentRect.height;
            }
            else
            {
                float tmp = gridRect.Y / (sourceH - yMax2);
                tmp = contentRect.height * tmp / (1 + tmp);
                gridY[1] = tmp;
                gridY[2] = tmp;
                gridY[3] = contentRect.height;
            }

            if (_tileGridIndice == 0)
            {
                for (int cy = 0; cy < 4; cy++)
                {
                    for (int cx = 0; cx < 4; cx++)
                    {
                        surfaceTool.SetUV(new Vector2(gridTexX[cx], gridTexY[cy]));
                        surfaceTool.AddVertex(new Vector3(gridX[cx] / _textureScale.X, gridY[cy] / _textureScale.Y, 0));
                    }
                }
                for (int i = 0; i < TRIANGLES_9_GRID.Length; i++)
                {
                    surfaceTool.AddIndex(TRIANGLES_9_GRID[i]);
                }
            }
            else
            {
                Rect drawRect;
                Rect texRect;
                int row, col;
                int part;
                int StartIndex = 0;

                for (int pi = 0; pi < 9; pi++)
                {
                    col = pi % 3;
                    row = pi / 3;
                    part = gridTileIndice[pi];
                    drawRect = Rect.MinMaxRect(gridX[col], gridY[row], gridX[col + 1], gridY[row + 1]);
                    texRect = Rect.MinMaxRect(gridTexX[col], gridTexY[row], gridTexX[col + 1], gridTexY[row + 1]);

                    if (part != -1 && (_tileGridIndice & (1 << part)) != 0)
                    {
                        StartIndex = TileFill(surfaceTool, drawRect, texRect,
                            (part == 0 || part == 1 || part == 4) ? gridRect.width : drawRect.width,
                            (part == 2 || part == 3 || part == 4) ? gridRect.height : drawRect.height, StartIndex);
                    }
                    else
                    {
                        drawRect.X /= _textureScale.X;
                        drawRect.Y /= _textureScale.Y;
                        drawRect.width /= _textureScale.X;
                        drawRect.height /= _textureScale.Y;
                        ToolSet.MeshAddRect(surfaceTool, drawRect, texRect, StartIndex);
                        StartIndex += 4;
                    }
                }
            }
        }

        int TileFill(SurfaceTool surfaceTool, Rect contentRect, Rect uvRect, float sourceW, float sourceH, int StartIndex)
        {
            int hc = Mathf.CeilToInt(contentRect.width / sourceW);
            int vc = Mathf.CeilToInt(contentRect.height / sourceH);
            float tailWidth = contentRect.width - (hc - 1) * sourceW;
            float tailHeight = contentRect.height - (vc - 1) * sourceH;
            float xMax = uvRect.xMax;
            float yMax = uvRect.yMax;
            for (int i = 0; i < hc; i++)
            {
                for (int j = 0; j < vc; j++)
                {
                    Rect uvTmp = uvRect;
                    if (i == hc - 1)
                        uvTmp.xMax = Mathf.Lerp(uvRect.X, xMax, tailWidth / sourceW);
                    if (j == vc - 1)
                        uvTmp.yMax = Mathf.Lerp(uvRect.Y, yMax, tailHeight / sourceH);

                    Rect drawRect = new Rect(contentRect.X + i * sourceW, contentRect.Y + j * sourceH,
                            i == (hc - 1) ? tailWidth : sourceW, j == (vc - 1) ? tailHeight : sourceH);

                    drawRect.X /= _textureScale.X;
                    drawRect.Y /= _textureScale.Y;
                    drawRect.width /= _textureScale.X;
                    drawRect.height /= _textureScale.Y;

                    ToolSet.MeshAddRect(surfaceTool, drawRect, uvTmp, StartIndex);
                    StartIndex += 4;
                }
            }
            return StartIndex;
        }

        public override void _Draw()
        {
            BuildMesh(_surfaceTool);
            if (!Mathf.IsEqualApprox(_skewX, 0) || !Mathf.IsEqualApprox(_skewY, 0))
            {
                float shx = Mathf.DegToRad(-_skewX);
                float shy = Mathf.DegToRad(_skewY);
                Transform2D skewTransform;
                if (PivotOffset.IsEqualApprox(Vector2.Zero))
                {
                    skewTransform = new Transform2D(
                                        Mathf.Cos(shy), Mathf.Sin(shy),
                                        Mathf.Sin(shx), Mathf.Cos(shx),
                                        0, 0
                                    );
                }
                else
                {
                    skewTransform = Transform2D.Identity.Translated(PivotOffset) *
                            new Transform2D(
                                                Mathf.Cos(shy), Mathf.Sin(shy),
                                                Mathf.Sin(shx), Mathf.Cos(shx),
                                                0, 0
                                            );
                    skewTransform = skewTransform * Transform2D.Identity.Translated(-PivotOffset);
                }
                DrawSetTransformMatrix(skewTransform);
            }
            DrawMesh(_mesh, _texture?.nativeTexture);
        }
    }
}
