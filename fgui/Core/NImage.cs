using FairyGUI.Utils;
using Godot;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class NImage : Control, IDisplayObject
    {
        public enum ReverseType
        {
            None,
            All,
            OnlyColor,
        }
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
        protected Texture2D _reverseTexture;
        protected CanvasItemMaterial _material;
        protected ArrayMesh _mesh;
        protected ArrayMesh _outBoundMesh;
        protected SurfaceTool _surfaceTool;
        internal NContainer maskOwner;
        internal bool reverseMask = false;

        static Color outColor = Colors.White;

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
        public Vector2 position
        {
            get { return Position; }
            set
            {
                Position = value;
                if (maskOwner != null)
                    QueueRedraw();
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
            Material = _material;
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
        public Texture2D drawTexture
        {
            get
            {
                if (reverseMask)
                    return _reverseTexture;
                else
                    return _texture?.nativeTexture;
            }
        }
        public ArrayMesh mesh
        {
            get { return _mesh; }
        }
        public ArrayMesh outBoundMesh
        {
            get { return _outBoundMesh; }
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

        public void UpdateMesh()
        {
            if (_texture == null)
            {
                return;
            }
            _mesh.ClearSurfaces();
            _surfaceTool.Clear();
            _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

            bool reserveDraw = maskOwner != null && reverseMask;
            int vertexCount = 0;

            Rect vertRect = new Rect(Vector2.Zero, Size);
            Rect uvRect = _texture.uvRect;
            TextureRepeat = TextureRepeatEnum.Disabled;

            if (reserveDraw && _texture != null)
            {
                Rect textRect = _texture.region;
                _reverseTexture = ToolSet.ExtractAndInvertAlpha(_texture.nativeTexture, new Rect2I((int)textRect.xMin, (int)textRect.yMin, (int)textRect.width, (int)textRect.height));
                uvRect.xMin = 0;
                uvRect.yMin = 0;
                uvRect.width = 1;
                uvRect.height = 1;
            }
            else
            {
                vertRect = _texture.GetDrawRect(vertRect, _flip);
            }

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
                        vertexCount += FillHorizontal(_surfaceTool, vertRect, uvRect, _fillOrigin, _fillAmount, vertexCount);
                        break;

                    case FillMethod.Vertical:
                        vertexCount += FillVertical(_surfaceTool, vertRect, uvRect, _fillOrigin, _fillAmount, vertexCount);
                        break;

                    case FillMethod.Radial90:
                        vertexCount += FillRadial90(_surfaceTool, vertRect, uvRect, (Origin90)_fillOrigin, _fillAmount, _fillClockwise, vertexCount);
                        break;

                    case FillMethod.Radial180:
                        vertexCount += FillRadial180(_surfaceTool, vertRect, uvRect, (Origin180)_fillOrigin, _fillAmount, _fillClockwise, vertexCount);
                        break;

                    case FillMethod.Radial360:
                        vertexCount += FillRadial360(_surfaceTool, vertRect, uvRect, (Origin360)_fillOrigin, _fillAmount, _fillClockwise, vertexCount);
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
                    ToolSet.MeshAddRect(_surfaceTool, vertRect, uvRect, 0);
                    vertexCount += 4;
                }
                else
                {
                    Rect contentRect = vertRect;
                    contentRect.width *= _textureScale.X;
                    contentRect.height *= _textureScale.Y;

                    vertexCount += TileFill(_surfaceTool, contentRect, uvRect, texture.width, texture.height, 0);
                }
            }
            else if (_scale9Grid != null)
            {
                vertexCount += SliceFill(_surfaceTool, vertRect, uvRect, texture.width, texture.height);
            }
            else
            {
                ToolSet.MeshAddRect(_surfaceTool, vertRect, uvRect, 0);
                vertexCount += 4;
            }

            _surfaceTool.GenerateNormals();
            // if (_material != null)
            //     _surfaceTool.SetMaterial(_material);
            _surfaceTool.Commit(_mesh);

            if (reserveDraw)
            {
                if (_outBoundMesh == null)
                    _outBoundMesh = new ArrayMesh();
                else
                    _outBoundMesh.ClearSurfaces();
                _surfaceTool.Clear();
                _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
                vertexCount = 0;
                if (_fillMethod != FillMethod.None)
                {
                    switch (_fillMethod)
                    {
                        case FillMethod.Horizontal:
                            vertexCount += FillHorizontal(_surfaceTool, vertRect, uvRect, _fillOrigin, _fillAmount, vertexCount, ReverseType.All);
                            break;

                        case FillMethod.Vertical:
                            vertexCount += FillVertical(_surfaceTool, vertRect, uvRect, _fillOrigin, _fillAmount, vertexCount, ReverseType.All);
                            break;

                        case FillMethod.Radial90:
                            vertexCount += FillRadial90(_surfaceTool, vertRect, uvRect, (Origin90)_fillOrigin, _fillAmount, _fillClockwise, vertexCount, ReverseType.All);
                            break;

                        case FillMethod.Radial180:
                            vertexCount += FillRadial180(_surfaceTool, vertRect, uvRect, (Origin180)_fillOrigin, _fillAmount, _fillClockwise, vertexCount, ReverseType.All);
                            break;

                        case FillMethod.Radial360:
                            vertexCount += FillRadial360(_surfaceTool, vertRect, uvRect, (Origin360)_fillOrigin, _fillAmount, _fillClockwise, vertexCount, ReverseType.All);
                            break;
                    }
                }
                DrawOutBound(vertexCount);
                _surfaceTool.GenerateNormals();
                _surfaceTool.Commit(_outBoundMesh);
            }
        }

        Rect MakeOutRect(float offsetX = 0, float offsetY = 0)
        {
            if (maskOwner == null)
                return new Rect(-offsetX, -offsetY, Size.X + offsetX * 2, Size.Y + offsetY * 2);
            Transform2D Trans = GetTransform();
            Vector2 min = Trans * new Vector2(-offsetX, -offsetY);
            Vector2 max = Trans * (Size + new Vector2(offsetX, offsetY));
            min.X = Mathf.Min(min.X, 0);
            min.Y = Mathf.Min(min.Y, 0);
            max.X = Mathf.Max(max.X, maskOwner.Size.X);
            max.Y = Mathf.Max(max.Y, maskOwner.Size.Y);
            Trans = GetTransform().AffineInverse();
            min = Trans * min;
            max = Trans * max;
            return Rect.MinMaxRect(
                Mathf.Min(min.X, max.X), Mathf.Min(min.Y, max.Y),
                Mathf.Max(min.X, max.X), Mathf.Max(min.Y, max.Y));
        }
        void DrawOutBound(int vertexStart)
        {
            Rect rect = new Rect(Vector2.Zero, Size);
            Rect outRect = MakeOutRect();
            ToolSet.MeshAddVertex(_surfaceTool, rect.xMin, rect.yMin, outColor);
            ToolSet.MeshAddVertex(_surfaceTool, rect.xMax, rect.yMin, outColor);
            ToolSet.MeshAddVertex(_surfaceTool, rect.xMax, rect.yMax, outColor);
            ToolSet.MeshAddVertex(_surfaceTool, rect.xMin, rect.yMax, outColor);
            ToolSet.MeshAddVertex(_surfaceTool, outRect.xMin, outRect.yMin, outColor);
            ToolSet.MeshAddVertex(_surfaceTool, outRect.xMax, outRect.yMin, outColor);
            ToolSet.MeshAddVertex(_surfaceTool, outRect.xMax, outRect.yMax, outColor);
            ToolSet.MeshAddVertex(_surfaceTool, outRect.xMin, outRect.yMax, outColor);
            ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexStart, vertexStart + 4, vertexStart + 5);
            ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexStart, vertexStart + 5, vertexStart + 1);
            ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexStart + 1, vertexStart + 5, vertexStart + 6);
            ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexStart + 1, vertexStart + 6, vertexStart + 2);
            ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexStart + 2, vertexStart + 6, vertexStart + 7);
            ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexStart + 2, vertexStart + 7, vertexStart + 3);
            ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexStart + 3, vertexStart + 7, vertexStart + 4);
            ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexStart + 3, vertexStart + 4, vertexStart + 0);
        }

        int FillHorizontal(SurfaceTool surfaceTool, Rect vertRect, Rect uvRect, int origin, float amount, int startIndex, ReverseType reverse = ReverseType.None)
        {
            if (reverse == ReverseType.All)
            {
                if ((OriginHorizontal)origin == OriginHorizontal.Right)
                    origin = (int)OriginHorizontal.Left;
                else
                    origin = (int)OriginHorizontal.Right;
                amount = 1.0f - amount;
            }
            if (amount <= 0)
                return 0;
            float a = vertRect.width * amount;
            if ((OriginHorizontal)origin == OriginHorizontal.Right || (OriginVertical)origin == OriginVertical.Bottom)
                vertRect.X += vertRect.width - a;
            vertRect.width = a;

            a = uvRect.width * amount;
            if ((OriginHorizontal)origin == OriginHorizontal.Right || (OriginVertical)origin == OriginVertical.Bottom)
                uvRect.X += uvRect.width - a;
            uvRect.width = a;

            if (reverse != ReverseType.None)
                ToolSet.MeshAddRect(surfaceTool, vertRect, outColor, startIndex, null, 0);
            else
                ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, startIndex);
            return 4;
        }

        int FillVertical(SurfaceTool surfaceTool, Rect vertRect, Rect uvRect, int origin, float amount, int startIndex, ReverseType reverse = ReverseType.None)
        {
            if (reverse == ReverseType.All)
            {
                if ((OriginVertical)origin == OriginVertical.Bottom)
                    origin = (int)OriginVertical.Top;
                else
                    origin = (int)OriginVertical.Bottom;
                amount = 1.0f - amount;
            }
            if (amount <= 0)
                return 0;
            float a = vertRect.height * amount;
            if ((OriginHorizontal)origin == OriginHorizontal.Right || (OriginVertical)origin == OriginVertical.Bottom)
                vertRect.Y += vertRect.height - a;
            vertRect.height = a;

            a = uvRect.height * amount;
            if ((OriginHorizontal)origin == OriginHorizontal.Right || (OriginVertical)origin == OriginVertical.Bottom)
                uvRect.Y += uvRect.height - a;
            uvRect.height = a;

            if (reverse != ReverseType.None)
                ToolSet.MeshAddRect(surfaceTool, vertRect, outColor, startIndex, null, 0);
            else
                ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, startIndex);
            return 4;
        }

        int FillRadial90(SurfaceTool surfaceTool, Rect vertRect, Rect uvRect, Origin90 origin, float amount, bool clockwise, int startIndex, ReverseType reverse = ReverseType.None)
        {
            if (reverse == ReverseType.All)
            {
                clockwise = !clockwise;
                amount = 1.0f - amount;
            }
            if (amount <= 0)
                return 0;
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


            ToolSet.MeshAddVertex(surfaceTool, xMin, yMin, vertRect, uvRect, reverse != ReverseType.None ? outColor : null);
            if (clockwise)
            {
                ToolSet.MeshAddVertex(surfaceTool, xMax, yMin, vertRect, uvRect, reverse != ReverseType.None ? outColor : null);
            }
            if (y > vertRect.yMax)
            {
                if (thresold)
                {
                    ToolSet.MeshAddVertex(surfaceTool, x2, yMax, vertRect, uvRect, reverse != ReverseType.None ? outColor : null);
                }
                else
                {
                    ToolSet.MeshAddVertex(surfaceTool, xMax, yMax, vertRect, uvRect, reverse != ReverseType.None ? outColor : null);
                }
            }
            else
            {
                ToolSet.MeshAddVertex(surfaceTool, xMax, y2, vertRect, uvRect, reverse != ReverseType.None ? outColor : null);
            }
            if (x > vertRect.xMax)
            {
                if (thresold)
                {
                    ToolSet.MeshAddVertex(surfaceTool, xMax, y2, vertRect, uvRect, reverse != ReverseType.None ? outColor : null);
                }
                else
                {
                    ToolSet.MeshAddVertex(surfaceTool, xMax, yMax, vertRect, uvRect, reverse != ReverseType.None ? outColor : null);
                }
            }
            else
            {
                ToolSet.MeshAddVertex(surfaceTool, x2, yMax, vertRect, uvRect, reverse != ReverseType.None ? outColor : null);
            }
            if (!clockwise)
            {
                ToolSet.MeshAddVertex(surfaceTool, xMin, yMax, vertRect, uvRect, reverse != ReverseType.None ? outColor : null);
            }
            if (flipX == flipY)
            {
                surfaceTool.AddIndex(startIndex);
                surfaceTool.AddIndex(startIndex + 1);
                surfaceTool.AddIndex(startIndex + 2);

                surfaceTool.AddIndex(startIndex);
                surfaceTool.AddIndex(startIndex + 2);
                surfaceTool.AddIndex(startIndex + 3);
            }
            else
            {
                surfaceTool.AddIndex(startIndex + 2);
                surfaceTool.AddIndex(startIndex + 1);
                surfaceTool.AddIndex(startIndex);

                surfaceTool.AddIndex(startIndex + 3);
                surfaceTool.AddIndex(startIndex + 2);
                surfaceTool.AddIndex(startIndex);
            }
            return 4;
        }
        int FillRadial180(SurfaceTool surfaceTool, Rect vertRect, Rect uvRect, Origin180 origin, float amount, bool clockwise, int StartIndex, ReverseType reverse = ReverseType.None)
        {
            if (reverse == ReverseType.All)
            {
                clockwise = !clockwise;
                amount = 1.0f - amount;
                reverse = ReverseType.OnlyColor;
            }
            if (amount <= 0)
                return 0;
            int firstIndex = StartIndex;
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
                        StartIndex += FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.TopLeft : Origin90.TopRight, amount / 0.5f, clockwise, StartIndex, reverse);
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
                        StartIndex += FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.TopRight : Origin90.TopLeft, (amount - 0.5f) / 0.5f, clockwise, StartIndex, reverse);
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
                        if (reverse != ReverseType.None)
                            ToolSet.MeshAddRect(surfaceTool, vertRect, outColor, StartIndex, null, 0);
                        else
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
                        StartIndex += FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.BottomRight : Origin90.BottomLeft, amount / 0.5f, clockwise, StartIndex, reverse);
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
                        StartIndex += FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.BottomLeft : Origin90.BottomRight, (amount - 0.5f) / 0.5f, clockwise, StartIndex, reverse);
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
                        if (reverse != ReverseType.None)
                            ToolSet.MeshAddRect(surfaceTool, vertRect, outColor, StartIndex, null, 0);
                        else
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
                        StartIndex += FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.BottomLeft : Origin90.TopLeft, amount / 0.5f, clockwise, StartIndex, reverse);
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
                        StartIndex += FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.TopLeft : Origin90.BottomLeft, (amount - 0.5f) / 0.5f, clockwise, StartIndex, reverse);
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
                        if (reverse != ReverseType.None)
                            ToolSet.MeshAddRect(surfaceTool, vertRect, outColor, StartIndex, null, 0);
                        else
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
                        StartIndex += FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.TopRight : Origin90.BottomRight, amount / 0.5f, clockwise, StartIndex, reverse);
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
                        StartIndex += FillRadial90(surfaceTool, vertRect, uvRect, clockwise ? Origin90.BottomRight : Origin90.TopRight, (amount - 0.5f) / 0.5f, clockwise, StartIndex, reverse);
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
                        if (reverse != ReverseType.None)
                            ToolSet.MeshAddRect(surfaceTool, vertRect, outColor, StartIndex, null, 0);
                        else
                            ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, StartIndex);
                        StartIndex += 4;
                    }
                    break;
            }
            return StartIndex - firstIndex;
        }
        int FillRadial360(SurfaceTool surfaceTool, Rect vertRect, Rect uvRect, Origin360 origin, float amount, bool clockwise, int StartIndex, ReverseType reverse = ReverseType.None)
        {
            if (reverse == ReverseType.All)
            {
                clockwise = !clockwise;
                amount = 1.0f - amount;
                reverse = ReverseType.OnlyColor;
            }
            if (amount <= 0)
                return 0;
            int firstIndex = StartIndex;
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
                        StartIndex += FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Left : Origin180.Right, amount / 0.5f, clockwise, StartIndex, reverse);
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
                        StartIndex += FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Right : Origin180.Left, (amount - 0.5f) / 0.5f, clockwise, StartIndex, reverse);
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
                        if (reverse != ReverseType.None)
                            ToolSet.MeshAddRect(surfaceTool, vertRect, outColor, StartIndex, null, 0);
                        else
                            ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, StartIndex);
                        StartIndex += 4;
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
                        StartIndex += FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Right : Origin180.Left, amount / 0.5f, clockwise, StartIndex, reverse);
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
                        StartIndex += FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Left : Origin180.Right, (amount - 0.5f) / 0.5f, clockwise, StartIndex, reverse);
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
                        if (reverse != ReverseType.None)
                            ToolSet.MeshAddRect(surfaceTool, vertRect, outColor, StartIndex, null, 0);
                        else
                            ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, StartIndex);
                        StartIndex += 4;
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
                        StartIndex += FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Bottom : Origin180.Top, amount / 0.5f, clockwise, StartIndex, reverse);
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
                        StartIndex += FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Top : Origin180.Bottom, (amount - 0.5f) / 0.5f, clockwise, StartIndex, reverse);

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
                        if (reverse != ReverseType.None)
                            ToolSet.MeshAddRect(surfaceTool, vertRect, outColor, StartIndex, null, 0);
                        else
                            ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, StartIndex);
                        StartIndex += 4;
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
                        StartIndex += FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Top : Origin180.Bottom, amount / 0.5f, clockwise, StartIndex, reverse);
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

                        StartIndex += FillRadial180(surfaceTool, vertRect, uvRect, clockwise ? Origin180.Bottom : Origin180.Top, (amount - 0.5f) / 0.5f, clockwise, StartIndex, reverse);

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
                        if (reverse != ReverseType.None)
                            ToolSet.MeshAddRect(surfaceTool, vertRect, outColor, StartIndex, null, 0);
                        else
                            ToolSet.MeshAddRect(surfaceTool, vertRect, uvRect, StartIndex);
                        StartIndex += 4;
                    }
                    break;
            }
            return StartIndex - firstIndex;
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

        int SliceFill(SurfaceTool surfaceTool, Rect contentRect, Rect uvRect, float sourceW, float sourceH)
        {
            Rect gridRect = (Rect)_scale9Grid;
            contentRect.width *= _textureScale.X;
            contentRect.height *= _textureScale.Y;
            int vertexCount = 0;

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
                        vertexCount++;
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

                for (int pi = 0; pi < 9; pi++)
                {
                    col = pi % 3;
                    row = pi / 3;
                    part = gridTileIndice[pi];
                    drawRect = Rect.MinMaxRect(gridX[col], gridY[row], gridX[col + 1], gridY[row + 1]);
                    texRect = Rect.MinMaxRect(gridTexX[col], gridTexY[row], gridTexX[col + 1], gridTexY[row + 1]);

                    if (part != -1 && (_tileGridIndice & (1 << part)) != 0)
                    {
                        vertexCount += TileFill(surfaceTool, drawRect, texRect,
                            (part == 0 || part == 1 || part == 4) ? gridRect.width : drawRect.width,
                            (part == 2 || part == 3 || part == 4) ? gridRect.height : drawRect.height, vertexCount);
                    }
                    else
                    {
                        drawRect.X /= _textureScale.X;
                        drawRect.Y /= _textureScale.Y;
                        drawRect.width /= _textureScale.X;
                        drawRect.height /= _textureScale.Y;
                        ToolSet.MeshAddRect(surfaceTool, drawRect, texRect, vertexCount);
                        vertexCount += 4;
                    }
                }
            }
            return vertexCount;
        }

        int TileFill(SurfaceTool surfaceTool, Rect contentRect, Rect uvRect, float sourceW, float sourceH, int StartIndex)
        {
            int hc = Mathf.CeilToInt(contentRect.width / sourceW);
            int vc = Mathf.CeilToInt(contentRect.height / sourceH);
            float tailWidth = contentRect.width - (hc - 1) * sourceW;
            float tailHeight = contentRect.height - (vc - 1) * sourceH;
            float xMax = uvRect.xMax;
            float yMax = uvRect.yMax;
            int firstIndex = StartIndex;
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
            return StartIndex - firstIndex;
        }

        public override void _Draw()
        {
            UpdateMesh();
            if (maskOwner != null)
            {
                maskOwner.QueueRedraw();
                return;
            }
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
