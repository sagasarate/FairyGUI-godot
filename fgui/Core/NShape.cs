using System;
using System.Collections.Generic;
using FairyGUI.Utils;
using Godot;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class NShape : Control, IDisplayObject, IHitTest
    {
        public enum ShapeType
        {
            Empty,
            Rect,
            Ellipse,
            Polygon,
            RegularPolygon
        }
        enum PolygonType
        {
            Convex,
            Concave,
            Unknow
        }
        protected float _skewX = 0;
        protected float _skewY = 0;
        protected CanvasItemMaterial _material;
        protected ArrayMesh _mesh;
        protected SurfaceTool _surfaceTool;
        protected Color _lineColor = Colors.Black;
        protected Color? _lineColorOuter;
        protected Color _fillColor = Colors.White;
        protected Color[] _colors = null;
        protected Color? _centerColor;
        protected float _lineWidth = 1;
        protected ShapeType _shapeType = ShapeType.Empty;
        protected Vector4 _rectRadius = Vector4.Zero;
        protected float _startDegree = 0;
        protected float _endDegree = 360;
        protected List<Vector2> _polygonPoints = new List<Vector2>();
        protected List<Vector2> _polygonUVs = new List<Vector2>();
        protected bool _usePercentPositions = false;
        protected int _polygonSides = 3;
        protected float _polygonRotation = 0;
        protected float[] _polygonDistances = new float[] { 1.0f, 1.0f, 1.0f };
        protected NTexture _texture;
        internal NContainer maskOwner;
        internal bool reverseMask = false;

        static Color outColor = Colors.White;
        static List<Vector2> sVertexBuffer1 = new List<Vector2>();

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
        public NShape(GObject owner)
        {
            gOwner = owner;
            MouseFilter = MouseFilterEnum.Ignore;
            //Name = "Shape";
            _material = new CanvasItemMaterial();
            _material.LightMode = CanvasItemMaterial.LightModeEnum.Unshaded;
            Material = _material;
            _mesh = new ArrayMesh();
            _surfaceTool = new SurfaceTool();
        }       
        public override void _Process(double delta)
        {
            if (onUpdate != null)
                onUpdate(delta);
        }
        public Color color
        {
            get { return fillColor; }
            set { fillColor = value; }
        }

        public Color fillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                if (_fillColor != value)
                {
                    _fillColor = value;
                    QueueRedraw();
                }

            }
        }

        public Color lineColor
        {
            get
            {
                return _lineColor;
            }
            set
            {
                if (_lineColor != value)
                {
                    _lineColor = value;
                    QueueRedraw();
                }

            }
        }
        public Color lineColorOuter
        {
            get
            {
                return _lineColorOuter == null ? _lineColor : (Color)_lineColorOuter;
            }
            set
            {
                if (_lineColorOuter != value)
                {
                    _lineColorOuter = value;
                    QueueRedraw();
                }

            }
        }
        public Color[] colors
        {
            get { return _colors; }
            set
            {
                if (_colors != value)
                {
                    _colors = value;
                    QueueRedraw();
                }

            }
        }
        public Color centerColor
        {
            get { return _centerColor == null ? _fillColor : (Color)_centerColor; }
            set
            {
                if (_centerColor != value)
                {
                    _centerColor = value;
                    QueueRedraw();
                }
            }
        }
        public float lineWidth
        {
            get { return _lineWidth; }
            set
            {
                if (!Mathf.IsEqualApprox(_lineWidth, value))
                {
                    _lineWidth = value;
                    QueueRedraw();
                }
            }
        }
        public ShapeType shapeType
        {
            get { return _shapeType; }
        }
        public Vector4 rectRadius
        {
            get { return _rectRadius; }
            set
            {
                if (!_rectRadius.IsEqualApprox(value))
                {
                    _rectRadius = value;
                    QueueRedraw();
                }
            }
        }
        public float startDegree
        {
            get { return _startDegree; }
            set
            {
                if (!Mathf.IsEqualApprox(_startDegree, value))
                {
                    _startDegree = value;
                    QueueRedraw();
                }
            }
        }
        public float endDegree
        {
            get { return _endDegree; }
            set
            {
                if (!Mathf.IsEqualApprox(_endDegree, value))
                {
                    _endDegree = value;
                    QueueRedraw();
                }
            }
        }
        public List<Vector2> polygonPoints
        {
            get { return _polygonPoints; }
            set
            {
                _polygonPoints = value;
                QueueRedraw();
            }
        }
        public bool usePercentPositions
        {
            get { return _usePercentPositions; }
            set
            {
                if (_usePercentPositions != value)
                {
                    _usePercentPositions = value;
                    QueueRedraw();
                }
            }
        }
        public int polygonSides
        {
            get { return _polygonSides; }
            set
            {
                if (_polygonSides != value)
                {
                    _polygonSides = value;
                    QueueRedraw();
                }
            }
        }
        public float polygonRotation
        {
            get { return _polygonRotation; }
            set
            {
                if (!Mathf.IsEqualApprox(_polygonRotation, value))
                {
                    _polygonRotation = value;
                    QueueRedraw();
                }
            }
        }
        public float[] polygonDistances
        {
            get { return _polygonDistances; }
            set
            {
                _polygonDistances = value;
                QueueRedraw();
            }
        }

        public NTexture texture
        {
            get { return _texture; }
            set
            {
                if (_texture != value)
                {
                    _texture = value;
                    QueueRedraw();
                }
            }
        }

        public ArrayMesh mesh
        {
            get { return _mesh; }
        }

        public void DrawRect(float lineSize, Color lineColor, Color fillColor)
        {
            _shapeType = ShapeType.Rect;
            _lineWidth = lineSize;
            _lineColor = lineColor;
            _colors = null;
            _fillColor = fillColor;
            _rectRadius = Vector4.Zero;
            QueueRedraw();
        }

        public void DrawRect(float lineSize, Color[] colors)
        {
            _shapeType = ShapeType.Rect;
            _colors = colors;
            _rectRadius = Vector4.Zero;
            QueueRedraw();
        }


        public void DrawRoundRect(float lineSize, Color lineColor, Color fillColor,
            float topLeftRadius, float topRightRadius, float bottomLeftRadius, float bottomRightRadius)
        {
            _shapeType = ShapeType.Rect;
            _lineWidth = lineSize;
            _lineColor = lineColor;
            _colors = null;
            _fillColor = fillColor;
            _rectRadius.X = topLeftRadius;
            _rectRadius.Y = topRightRadius;
            _rectRadius.Z = bottomLeftRadius;
            _rectRadius.W = bottomRightRadius;
            QueueRedraw();
        }

        public void DrawEllipse(Color fillColor)
        {
            _shapeType = ShapeType.Ellipse;
            _lineWidth = 0;
            _startDegree = 0;
            _endDegree = 360;
            _fillColor = fillColor;
            _centerColor = null;
            QueueRedraw();
        }

        public void DrawEllipse(float lineSize, Color centerColor, Color lineColor, Color fillColor, float startDegree, float endDegree)
        {
            _shapeType = ShapeType.Ellipse;
            _lineWidth = lineSize;
            if (centerColor.Equals(fillColor))
                _centerColor = null;
            else
                _centerColor = centerColor;
            _lineColor = lineColor;
            _fillColor = fillColor;
            _startDegree = startDegree;
            _endDegree = endDegree;
            QueueRedraw();
        }

        public void DrawPolygon(IList<Vector2> points, Color fillColor)
        {
            _shapeType = ShapeType.Polygon;
            _polygonPoints.Clear();
            _polygonPoints.AddRange(points);
            _fillColor = fillColor;
            _lineWidth = 0;
            QueueRedraw();
        }

        public void DrawPolygon(IList<Vector2> points, float lineSize, Color[] colors)
        {
            _shapeType = ShapeType.Polygon;
            _polygonPoints.Clear();
            _polygonPoints.AddRange(points);
            _lineWidth = lineSize;
            _colors = colors;
            QueueRedraw();
        }

        public void DrawPolygon(IList<Vector2> points, Color fillColor, float lineSize, Color lineColor)
        {
            _shapeType = ShapeType.Polygon;
            _polygonPoints.Clear();
            _polygonPoints.AddRange(points);
            _fillColor = fillColor;
            _lineWidth = lineSize;
            _lineColor = lineColor;
            _colors = null;
            QueueRedraw();
        }

        public void DrawRegularPolygon(int sides, float lineSize, Color? centerColor, Color lineColor, Color fillColor, float rotation, float[] distances)
        {
            _shapeType = ShapeType.RegularPolygon;
            _polygonSides = sides;
            _lineWidth = lineSize;
            _centerColor = centerColor;
            _lineColor = lineColor;
            _fillColor = fillColor;
            _polygonRotation = rotation;
            _polygonDistances = distances;
            QueueRedraw();
        }

        public void Clear()
        {
            _shapeType = ShapeType.Empty;
            QueueRedraw();
        }

        public bool isEmpty
        {
            get { return _shapeType == ShapeType.Empty; }
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
            switch (_shapeType)
            {
                case ShapeType.Rect:
                    DrawMesh(_mesh, null);
                    break;
                case ShapeType.Ellipse:
                    DrawMesh(_mesh, null);
                    break;
                case ShapeType.Polygon:
                    DrawMesh(_mesh, _texture?.nativeTexture);
                    break;
                case ShapeType.RegularPolygon:
                    DrawMesh(_mesh, null);
                    break;
            }
        }
        internal void UpdateMesh()
        {
            switch (_shapeType)
            {
                case ShapeType.Rect:
                    if (!_rectRadius.IsZeroApprox())
                        BuildRoundRectMesh();
                    else
                        BuildRectMesh();
                    break;
                case ShapeType.Ellipse:
                    BuildEllipseMesh();
                    break;
                case ShapeType.Polygon:
                    BuildPolygonMesh();
                    break;
                case ShapeType.RegularPolygon:
                    BuildRegularPolygonMesh();
                    break;
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
        void BuildRectMesh()
        {
            _mesh.ClearSurfaces();
            _surfaceTool.Clear();
            _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

            Color lineColor_ = _lineColor;
            Color lineColorOuter_ = _lineColorOuter == null ? _lineColor : (Color)_lineColorOuter;
            Color fillColor_ = _fillColor;
            Color[] colors_ = _colors;

            bool reserveDraw = maskOwner != null && reverseMask;
            if (reserveDraw)
            {
                lineColor_.A = 1.0f - lineColor_.A;
                lineColorOuter_.A = 1.0f - lineColorOuter_.A;
                fillColor_.A = 1.0f - fillColor_.A;
                if (colors_ != null)
                {
                    colors_ = colors_.Clone() as Color[];
                    for (int i = 0; i < colors_.Length; i++)
                        colors_[i].A = 1.0f - colors_[i].A;
                }
            }

            float lineWidth_ = Mathf.Min(_lineWidth, Mathf.Min(Size.X / 2, Size.Y / 2));

            Rect rect = new Rect(Vector2.Zero, Size);
            int vertexCount = 0;
            if (lineWidth_ > 0)
            {
                Rect centerRect = Rect.MinMaxRect(rect.X + lineWidth_, rect.Y + lineWidth_, rect.xMax - lineWidth_, rect.yMax - lineWidth_);
                int colorIndex = 0;
                //middle
                if (!Mathf.IsEqualApprox(fillColor_.A, 0))//optimized
                {
                    if (centerRect.width > 0 && centerRect.height > 0)
                    {
                        ToolSet.MeshAddRect(_surfaceTool, centerRect, fillColor_, vertexCount, colors_, 0);
                        vertexCount += 4;
                        colorIndex += 4;
                    }
                }
                ToolSet.MeshAddVertex(_surfaceTool, centerRect.xMax, centerRect.yMax, colors_ != null && colorIndex < colors_.Length ? colors_[colorIndex++] : lineColor_);
                ToolSet.MeshAddVertex(_surfaceTool, centerRect.xMin, centerRect.yMax, colors_ != null && colorIndex < colors_.Length ? colors_[colorIndex++] : lineColor_);
                ToolSet.MeshAddVertex(_surfaceTool, centerRect.xMin, centerRect.yMin, colors_ != null && colorIndex < colors_.Length ? colors_[colorIndex++] : lineColor_);
                ToolSet.MeshAddVertex(_surfaceTool, centerRect.xMax, centerRect.yMin, colors_ != null && colorIndex < colors_.Length ? colors_[colorIndex++] : lineColor_);

                ToolSet.MeshAddVertex(_surfaceTool, rect.xMax, rect.yMax, colors_ != null && colorIndex < colors_.Length ? colors_[colorIndex++] : lineColorOuter_);
                ToolSet.MeshAddVertex(_surfaceTool, rect.xMin, rect.yMax, colors_ != null && colorIndex < colors_.Length ? colors_[colorIndex++] : lineColorOuter_);
                ToolSet.MeshAddVertex(_surfaceTool, rect.xMin, rect.yMin, colors_ != null && colorIndex < colors_.Length ? colors_[colorIndex++] : lineColorOuter_);
                ToolSet.MeshAddVertex(_surfaceTool, rect.xMax, rect.yMin, colors_ != null && colorIndex < colors_.Length ? colors_[colorIndex++] : lineColorOuter_);

                ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount, vertexCount + 4, vertexCount + 5);
                ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount, vertexCount + 5, vertexCount + 1);
                ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount + 1, vertexCount + 5, vertexCount + 6);
                ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount + 1, vertexCount + 6, vertexCount + 2);
                ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount + 2, vertexCount + 6, vertexCount + 7);
                ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount + 2, vertexCount + 7, vertexCount + 3);
                ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount + 3, vertexCount + 7, vertexCount + 4);
                ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount + 3, vertexCount + 4, vertexCount + 0);

                vertexCount += 8;
            }
            else
            {
                if (!Mathf.IsEqualApprox(fillColor_.A, 0))
                {
                    ToolSet.MeshAddRect(_surfaceTool, rect, fillColor_, 0, colors_, 0);
                    vertexCount += 4;
                }
            }
            if (reserveDraw)
            {
                Rect outRect = MakeOutRect();
                if (!outRect.IsEqualApprox(rect))
                {
                    ToolSet.MeshAddVertex(_surfaceTool, rect.xMax, rect.yMax, outColor);
                    ToolSet.MeshAddVertex(_surfaceTool, rect.xMin, rect.yMax, outColor);
                    ToolSet.MeshAddVertex(_surfaceTool, rect.xMin, rect.yMin, outColor);
                    ToolSet.MeshAddVertex(_surfaceTool, rect.xMax, rect.yMin, outColor);

                    ToolSet.MeshAddVertex(_surfaceTool, outRect.xMax, outRect.yMax, outColor);
                    ToolSet.MeshAddVertex(_surfaceTool, outRect.xMin, outRect.yMax, outColor);
                    ToolSet.MeshAddVertex(_surfaceTool, outRect.xMin, outRect.yMin, outColor);
                    ToolSet.MeshAddVertex(_surfaceTool, outRect.xMax, outRect.yMin, outColor);

                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount, vertexCount + 4, vertexCount + 5);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount, vertexCount + 5, vertexCount + 1);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount + 1, vertexCount + 5, vertexCount + 6);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount + 1, vertexCount + 6, vertexCount + 2);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount + 2, vertexCount + 6, vertexCount + 7);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount + 2, vertexCount + 7, vertexCount + 3);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount + 3, vertexCount + 7, vertexCount + 4);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, vertexCount + 3, vertexCount + 4, vertexCount + 0);
                }
            }
            // _surfaceTool.GenerateNormals();
            if (_material != null)
                _surfaceTool.SetMaterial(_material);
            _surfaceTool.Commit(_mesh);
        }
        void BuildRoundRectMesh()
        {
            _mesh.ClearSurfaces();
            _surfaceTool.Clear();
            _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

            Color lineColor_ = _lineColor;
            Color lineColorOuter_ = _lineColorOuter == null ? _lineColor : (Color)_lineColorOuter;
            Color fillColor_ = _fillColor;
            // Color[] colors_ = _colors;

            bool reserveDraw = maskOwner != null && reverseMask;
            if (reserveDraw)
            {
                lineColor_.A = 1.0f - lineColor_.A;
                lineColorOuter_.A = 1.0f - lineColorOuter_.A;
                fillColor_.A = 1.0f - fillColor_.A;
                // if (colors_ != null)
                // {
                //     colors_ = colors_.Clone() as Color[];
                //     for (int i = 0; i < colors_.Length; i++)
                //         colors_[i].A = 1.0f - colors_[i].A;
                // }
            }

            bool drawRect = !Mathf.IsZeroApprox(fillColor_.A);
            bool drawBorder = !Mathf.IsZeroApprox(lineColor_.A) || !Mathf.IsZeroApprox(lineColorOuter_.A);

            float lineWidth_ = Mathf.Min(_lineWidth, Mathf.Min(Size.X / 2, Size.Y / 2));

            Rect rect = new Rect(Vector2.Zero, Size);

            float radiusX = rect.width / 2;
            float radiusY = rect.height / 2;
            float cornerMaxRadius = Mathf.Min(radiusX, radiusY);
            float centerX = radiusX + rect.X;
            float centerY = radiusY + rect.Y;

            ToolSet.MeshAddVertex(_surfaceTool, centerX, centerY, fillColor_);

            int cnt = 0;
            for (int i = 0; i < 4; i++)
            {
                float radius = 0;
                switch (i)
                {
                    case 0://RightBottom
                        radius = _rectRadius.W;
                        break;

                    case 1://LeftBottom
                        radius = _rectRadius.Z;
                        break;

                    case 2://LeftTop
                        radius = _rectRadius.X;
                        break;

                    case 3://LeftBottom
                        radius = _rectRadius.Y;
                        break;
                }
                radius = Mathf.Min(cornerMaxRadius, radius);

                float offsetX = rect.X;
                float offsetY = rect.Y;

                if (i == 0 || i == 3)
                    offsetX = rect.xMax - radius * 2;
                if (i == 0 || i == 1)
                    offsetY = rect.yMax - radius * 2;

                if (radius != 0)
                {
                    int partNumSides = Mathf.Max(1, Mathf.CeilToInt(Mathf.Pi * radius / 8)) + 1;
                    float angleDelta = Mathf.Pi / 2 / partNumSides;
                    float angle = Mathf.Pi / 2 * i;
                    float startAngle = angle;

                    for (int j = 1; j <= partNumSides; j++)
                    {
                        if (j == partNumSides) //消除精度误差带来的不对齐
                            angle = startAngle + Mathf.Pi / 2;
                        Vector2 v1 = new Vector2(offsetX + Mathf.Cos(angle) * (radius - lineWidth_) + radius,
                            offsetY + Mathf.Sin(angle) * (radius - lineWidth_) + radius);
                        if (drawRect)
                        {
                            ToolSet.MeshAddVertex(_surfaceTool, v1.X, v1.Y, fillColor_);
                            cnt++;
                        }
                        if (lineWidth_ != 0)
                        {
                            Vector2 v2 = new Vector2(offsetX + Mathf.Cos(angle) * radius + radius, offsetY + Mathf.Sin(angle) * radius + radius);
                            if (drawBorder)
                            {
                                ToolSet.MeshAddVertex(_surfaceTool, v1.X, v1.Y, lineColor_);
                                ToolSet.MeshAddVertex(_surfaceTool, v2.X, v2.Y, lineColorOuter_);
                                cnt += 2;
                            }
                            if (reserveDraw)
                            {
                                ToolSet.MeshAddVertex(_surfaceTool, v2.X, v2.Y, outColor);
                                cnt++;
                            }
                        }
                        else if (reserveDraw)
                        {
                            ToolSet.MeshAddVertex(_surfaceTool, v1.X, v1.Y, outColor);
                            cnt++;
                        }
                        angle += angleDelta;
                    }
                }
                else
                {
                    Vector2 v1 = new Vector2(offsetX, offsetY);
                    if (lineWidth_ != 0)
                    {
                        if (i == 0 || i == 3)
                            offsetX -= lineWidth_;
                        else
                            offsetX += lineWidth_;
                        if (i == 0 || i == 1)
                            offsetY -= lineWidth_;
                        else
                            offsetY += lineWidth_;
                        Vector2 v2 = new Vector2(offsetX, offsetY);
                        if (drawRect)
                        {
                            ToolSet.MeshAddVertex(_surfaceTool, v2.X, v2.Y, fillColor_);
                            cnt++;
                        }
                        if (drawBorder)
                        {
                            ToolSet.MeshAddVertex(_surfaceTool, v2.X, v2.Y, lineColor_);
                            ToolSet.MeshAddVertex(_surfaceTool, v1.X, v1.Y, lineColorOuter_);
                            cnt += 2;
                        }
                        if (reserveDraw)
                        {
                            ToolSet.MeshAddVertex(_surfaceTool, v1.X, v1.Y, outColor);
                            cnt++;
                        }
                    }
                    else
                    {
                        if (drawRect)
                        {
                            ToolSet.MeshAddVertex(_surfaceTool, v1.X, v1.Y, fillColor_);
                            cnt++;
                        }
                        if (reserveDraw)
                        {
                            ToolSet.MeshAddVertex(_surfaceTool, v1.X, v1.Y, outColor);
                            cnt++;
                        }
                    }
                }
            }
            int sideVertexCount = 0;
            if (drawRect)
                sideVertexCount++;
            if (reserveDraw)
                sideVertexCount++;
            if (lineWidth_ > 0)
            {
                if (drawBorder)
                    sideVertexCount += 2;
                if (drawRect || drawBorder)
                {
                    for (int i = 0; i < cnt; i += sideVertexCount)
                    {
                        if (i != cnt - sideVertexCount)
                        {
                            int start = i + 1;
                            int end = i + sideVertexCount + 1;
                            if (drawRect)
                            {
                                ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, start, end);
                                start++;
                                end++;
                            }
                            if (drawBorder)
                            {
                                ToolSet.MeshAddTriangleIndecies(_surfaceTool, start + 1, end, start);
                                ToolSet.MeshAddTriangleIndecies(_surfaceTool, start + 1, end + 1, end);
                            }
                        }
                        else
                        {
                            if (drawRect)
                            {
                                ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, i + 1, 1);
                                if (drawBorder)
                                {
                                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 2, i + 2, i + 3);
                                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, i + 3, 3, 2);
                                }

                            }
                            else if (drawBorder)
                            {
                                ToolSet.MeshAddTriangleIndecies(_surfaceTool, 1, i + 1, i + 2);
                                ToolSet.MeshAddTriangleIndecies(_surfaceTool, i + 2, 2, 1);
                            }
                        }
                    }
                }
            }
            else if (drawRect)
            {
                for (int i = 0; i < cnt; i += sideVertexCount)
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, i + 1, (i == cnt - sideVertexCount) ? 1 : i + sideVertexCount + 1);
            }

            if (reserveDraw)
            {
                Rect outRect = MakeOutRect();
                Vector2[] rectPoints = new Vector2[]{
                        new Vector2(outRect.xMax, outRect.yMax),
                        new Vector2(outRect.xMin, outRect.yMax),
                        new Vector2(outRect.xMin, outRect.yMin),
                        new Vector2(outRect.xMax, outRect.yMin)
                        };
                ToolSet.MeshAddVertex(_surfaceTool, rectPoints[0].X, rectPoints[0].Y, outColor);
                ToolSet.MeshAddVertex(_surfaceTool, rectPoints[1].X, rectPoints[1].Y, outColor);
                ToolSet.MeshAddVertex(_surfaceTool, rectPoints[2].X, rectPoints[2].Y, outColor);
                ToolSet.MeshAddVertex(_surfaceTool, rectPoints[3].X, rectPoints[3].Y, outColor);
                int bound = -1;
                int firstBound = -1;
                int boundVertexStart = cnt + 1;
                cnt = sideVertexCount;
                Vector2 prevVertex = Vector2.Inf;
                for (int i = 0; i < 4; i++)
                {
                    float radius = 0;
                    switch (i)
                    {
                        case 0://RightBottom
                            radius = _rectRadius.W;
                            break;

                        case 1://LeftBottom
                            radius = _rectRadius.Z;
                            break;

                        case 2://LeftTop
                            radius = _rectRadius.X;
                            break;

                        case 3://LeftBottom
                            radius = _rectRadius.Y;
                            break;
                    }
                    radius = Mathf.Min(cornerMaxRadius, radius);

                    float offsetX = rect.X;
                    float offsetY = rect.Y;

                    if (i == 0 || i == 3)
                        offsetX = rect.xMax - radius * 2;
                    if (i == 0 || i == 1)
                        offsetY = rect.yMax - radius * 2;

                    Vector2 CurVertex = Vector2.Zero;
                    if (radius != 0)
                    {
                        int partNumSides = Mathf.Max(1, Mathf.CeilToInt(Mathf.Pi * radius / 8)) + 1;
                        float angleDelta = Mathf.Pi / 2 / partNumSides;
                        float angle = Mathf.Pi / 2 * i;
                        float startAngle = angle;

                        for (int j = 1; j <= partNumSides; j++)
                        {
                            if (j == partNumSides) //消除精度误差带来的不对齐
                                angle = startAngle + Mathf.Pi / 2;
                            if (lineWidth_ != 0)
                            {
                                CurVertex = new Vector2(offsetX + Mathf.Cos(angle) * radius + radius, offsetY + Mathf.Sin(angle) * radius + radius);
                            }
                            else if (reserveDraw)
                            {
                                CurVertex = new Vector2(offsetX + Mathf.Cos(angle) * (radius - lineWidth_) + radius, offsetY + Mathf.Sin(angle) * (radius - lineWidth_) + radius);
                            }
                            angle += angleDelta;
                            if (prevVertex.IsFinite())
                                bound = DrawRectOuter(rectPoints, prevVertex, cnt - sideVertexCount, CurVertex, cnt, bound, boundVertexStart);
                            prevVertex = CurVertex;
                            cnt += sideVertexCount;
                            if (firstBound <= 0)
                                firstBound = bound;
                        }
                    }
                    else
                    {
                        if (lineWidth_ != 0)
                        {
                            if (i == 0 || i == 3)
                                offsetX -= lineWidth_;
                            else
                                offsetX += lineWidth_;
                            if (i == 0 || i == 1)
                                offsetY -= lineWidth_;
                            else
                                offsetY += lineWidth_;
                            CurVertex = new Vector2(offsetX, offsetY);
                        }
                        else
                        {
                            CurVertex = new Vector2(offsetX, offsetY);
                        }
                        if (prevVertex.IsFinite())
                            bound = DrawRectOuter(rectPoints, prevVertex, cnt - sideVertexCount, CurVertex, cnt, bound, boundVertexStart);
                        prevVertex = CurVertex;
                        cnt += sideVertexCount;
                        if (firstBound <= 0)
                            firstBound = bound;
                    }
                }
                ToolSet.MeshAddTriangleIndecies(_surfaceTool, firstBound, sideVertexCount, cnt - sideVertexCount);
                if (bound != firstBound)
                {
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, cnt - sideVertexCount, bound, firstBound);
                }
            }

            // _surfaceTool.GenerateNormals();
            if (_material != null)
                _surfaceTool.SetMaterial(_material);
            _surfaceTool.Commit(_mesh);
        }
        int DrawRectOuter(Vector2[] rectPoints, Vector2 startVertex, int startIndex, Vector2 endVertex, int endIndex, int bound, int boundVertexStart)
        {
            int newBound = SelectBoundPoint(rectPoints, startVertex, endVertex) + boundVertexStart;
            if (bound >= 0 && newBound != bound)
            {
                if (Math.Abs(newBound - bound) > 1)
                {
                    int midBound = ChangeBound(bound - boundVertexStart, 1) + boundVertexStart;
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, newBound, startIndex, midBound);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, midBound, startIndex, bound);
                }
                else
                {
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, bound, startIndex, newBound);
                }
            }
            bound = newBound;
            ToolSet.MeshAddTriangleIndecies(_surfaceTool, startIndex, bound, endIndex);
            return bound;
        }
        void BuildEllipseMesh()
        {
            _mesh.ClearSurfaces();
            _surfaceTool.Clear();
            _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

            Rect rect = new Rect(Vector2.Zero, Size);

            float sectionStart = Mathf.Clamp(_startDegree, 0, 360);
            float sectionEnd = Mathf.Clamp(_endDegree, 0, 360);
            if (sectionStart > sectionEnd)
            {
                float temp = sectionStart;
                sectionStart = sectionEnd;
                sectionEnd = temp;
            }
            bool clipped = sectionStart > 0 || sectionEnd < 360;
            sectionStart = Mathf.DegToRad(sectionStart);
            sectionEnd = Mathf.DegToRad(sectionEnd);
            Color fillColor_ = _fillColor;
            Color lineColor_ = _lineColor;
            Color lineColorOuter_ = _lineColorOuter == null ? _lineColor : (Color)_lineColorOuter;
            Color centerColor_ = _centerColor == null ? _fillColor : (Color)_centerColor;

            bool reserveDraw = maskOwner != null && reverseMask;
            if (reserveDraw)
            {
                fillColor_.A = 1.0f - fillColor_.A;
                lineColor_.A = 1.0f - lineColor_.A;
                lineColorOuter_.A = 1.0f - lineColorOuter_.A;
                centerColor_.A = 1.0f - centerColor_.A;
            }


            float radiusX = rect.width / 2;
            float radiusY = rect.height / 2;
            float lineAngle = _lineWidth / Mathf.Max(radiusX, radiusY);
            if (sectionEnd - sectionStart < lineAngle * 2)
                sectionEnd = sectionStart + lineAngle * 2;
            PolygonType polygonType = (sectionEnd - sectionStart) > Mathf.Pi ? PolygonType.Concave : PolygonType.Convex;
            int sides = Mathf.CeilToInt(Mathf.Pi * (radiusX + radiusY) / 4);
            sides = Mathf.Clamp(sides, 40, 800);
            float angleDelta = Mathf.Pi * 2 / sides;
            float angle;
            int vertexCount = 0;

            float centerX = rect.X + radiusX;
            float centerY = rect.Y + radiusY;
            int centerCount = 0;

            bool skipDrawLine = Mathf.IsZeroApprox(lineColor_.A) && Mathf.IsZeroApprox(lineColorOuter_.A);
            bool skipDrawEllipse = Mathf.IsZeroApprox(fillColor_.A) && Mathf.IsZeroApprox(centerColor_.A);

            sVertexBuffer1.Clear();
            if (_lineWidth > 0 && clipped)
            {
                Vector2 pc = new Vector2(centerX, centerY);
                Vector2 p3 = new Vector2(Mathf.Cos(sectionStart) * radiusX + centerX, Mathf.Sin(sectionStart) * radiusY + centerY);
                Vector2 q3 = new Vector2(Mathf.Cos(sectionEnd) * radiusX + centerX, Mathf.Sin(sectionEnd) * radiusY + centerY);
                sectionStart += lineAngle;
                sectionEnd -= lineAngle;
                Vector2 p1 = new Vector2(Mathf.Cos(sectionStart) * radiusX + centerX, Mathf.Sin(sectionStart) * radiusY + centerY);
                Vector2 p4 = new Vector2(Mathf.Cos(sectionStart) * (radiusX - _lineWidth) + centerX, Mathf.Sin(sectionStart) * (radiusY - _lineWidth) + centerY);
                Vector2 q1 = new Vector2(Mathf.Cos(sectionEnd) * radiusX + centerX, Mathf.Sin(sectionEnd) * radiusY + centerY);
                Vector2 q4 = new Vector2(Mathf.Cos(sectionEnd) * (radiusX - _lineWidth) + centerX, Mathf.Sin(sectionEnd) * (radiusY - _lineWidth) + centerY);
                Vector2 p2 = p1 + (pc - p3);
                Vector2 q2 = q1 + (pc - q3);
                Vector2 cross;
                if (!ToolSet.LineIntersection(p1, p2, q1, q2, out cross))
                    cross = p2;

                ToolSet.MeshAddVertex(_surfaceTool, centerX, centerY, lineColorOuter_);//外圈中心点
                vertexCount++;
                if ((sectionEnd - sectionStart) > Mathf.Pi && cross.DistanceTo(pc) > _lineWidth * 2)
                {
                    //有三个内圈中心点
                    angle = (sectionStart + sectionEnd) / 2;
                    cross = new Vector2(Mathf.Cos(angle) * _lineWidth + centerX, Mathf.Sin(angle) * _lineWidth + centerY);
                    ToolSet.MeshAddVertex(_surfaceTool, p1.X, p1.Y, lineColorOuter_);
                    ToolSet.MeshAddVertex(_surfaceTool, p2.X, p2.Y, lineColor_);
                    ToolSet.MeshAddVertex(_surfaceTool, p3.X, p3.Y, lineColorOuter_);
                    ToolSet.MeshAddVertex(_surfaceTool, q1.X, q1.Y, lineColorOuter_);
                    ToolSet.MeshAddVertex(_surfaceTool, q2.X, q2.Y, lineColor_);
                    ToolSet.MeshAddVertex(_surfaceTool, q3.X, q3.Y, lineColorOuter_);
                    ToolSet.MeshAddVertex(_surfaceTool, cross.X, cross.Y, lineColor_);
                    ToolSet.MeshAddVertex(_surfaceTool, p4.X, p4.Y, lineColor_);
                    ToolSet.MeshAddVertex(_surfaceTool, q4.X, q4.Y, lineColor_);
                    vertexCount += 9;

                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 8, 0, 3);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 1, 8, 3);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 8, 2, 0);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 2, 7, 0);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 6, 0, 9);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 6, 9, 4);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, 5, 9);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, 7, 5);

                    if (reserveDraw)
                    {
                        sVertexBuffer1.Add(q3);
                        sVertexBuffer1.Add(pc);
                        sVertexBuffer1.Add(p3);
                    }
                    if (!skipDrawEllipse)
                    {
                        ToolSet.MeshAddVertex(_surfaceTool, p2.X, p2.Y, centerColor_);//内圈中心点1
                        ToolSet.MeshAddVertex(_surfaceTool, cross.X, cross.Y, centerColor_);//内圈中心点2
                        ToolSet.MeshAddVertex(_surfaceTool, q2.X, q2.Y, centerColor_);//内圈中心点3
                        vertexCount += 3;
                        centerCount = 3;
                    }
                }
                else
                {
                    //有一个内圈中心点                    
                    ToolSet.MeshAddVertex(_surfaceTool, p1.X, p1.Y, lineColorOuter_);
                    ToolSet.MeshAddVertex(_surfaceTool, p3.X, p3.Y, lineColorOuter_);
                    ToolSet.MeshAddVertex(_surfaceTool, q1.X, q1.Y, lineColorOuter_);
                    ToolSet.MeshAddVertex(_surfaceTool, q3.X, q3.Y, lineColorOuter_);
                    ToolSet.MeshAddVertex(_surfaceTool, cross.X, cross.Y, lineColor_);
                    ToolSet.MeshAddVertex(_surfaceTool, p4.X, p4.Y, lineColor_);
                    ToolSet.MeshAddVertex(_surfaceTool, q4.X, q4.Y, lineColor_);
                    vertexCount += 7;

                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 2, 6, 0);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 1, 6, 2);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 6, 5, 0);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, 7, 4);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 4, 7, 3);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, 5, 7);

                    if (reserveDraw)
                    {
                        sVertexBuffer1.Add(q3);
                        sVertexBuffer1.Add(pc);
                        sVertexBuffer1.Add(p3);
                    }
                    if (!skipDrawEllipse)
                    {
                        ToolSet.MeshAddVertex(_surfaceTool, cross.X, cross.Y, centerColor_);//内圈中心点
                        vertexCount++;
                        centerCount = 1;
                    }
                }

            }
            else
            {
                if (!skipDrawEllipse)
                {
                    //完整圆或者没有边线，就只有一个中心点
                    ToolSet.MeshAddVertex(_surfaceTool, centerX, centerY, centerColor_);
                    vertexCount++;
                    centerCount = 1;
                }
                if (reserveDraw && clipped)
                {
                    sVertexBuffer1.Add(new Vector2(centerX, centerY));
                }
            }

            int sideVertexStart = vertexCount;
            int vertexPerSide = 0;
            if (!skipDrawEllipse)
                vertexPerSide++;
            if (_lineWidth > 0 && !skipDrawLine)
                vertexPerSide += 2;
            sides = 0;
            angle = sectionStart;
            while (true)
            {
                if (angle > sectionEnd)
                    angle = sectionEnd;
                Vector2 vec = new Vector2(Mathf.Cos(angle) * (radiusX - _lineWidth) + centerX, Mathf.Sin(angle) * (radiusY - _lineWidth) + centerY);
                if (!skipDrawEllipse)
                {
                    ToolSet.MeshAddVertex(_surfaceTool, vec.X, vec.Y, fillColor_);
                    vertexCount++;
                }
                if (_lineWidth > 0)
                {
                    Vector2 vec2 = new Vector2(Mathf.Cos(angle) * radiusX + centerX, Mathf.Sin(angle) * radiusY + centerY);
                    if (!skipDrawLine)
                    {
                        ToolSet.MeshAddVertex(_surfaceTool, vec.X, vec.Y, lineColor_);
                        ToolSet.MeshAddVertex(_surfaceTool, vec2.X, vec2.Y, lineColorOuter_);
                        vertexCount += 2;
                    }
                    if (reserveDraw)
                    {
                        sVertexBuffer1.Add(vec2);
                    }

                }
                else if (reserveDraw)
                {
                    sVertexBuffer1.Add(vec);
                }
                sides++;
                if (angle >= sectionEnd)
                    break;
                angle += angleDelta;
            }

            if (_lineWidth > 0)
            {
                int cnt = sides * vertexPerSide;
                int center = sideVertexStart - centerCount;
                angle = sectionStart;
                for (int i = 0; i < cnt; i += vertexPerSide)
                {
                    int start = i + sideVertexStart;
                    int next = start + vertexPerSide;
                    if (centerCount > 1 && !skipDrawEllipse)
                    {
                        int newcenter = sideVertexStart - centerCount + Mathf.Clamp(Mathf.FloorToInt((angle - sectionStart) * 3 / (sectionEnd - sectionStart)), 0, centerCount - 1);
                        if (newcenter > center)
                        {
                            ToolSet.MeshAddTriangleIndecies(_surfaceTool, newcenter, start, center);
                        }
                        center = newcenter;
                    }
                    if (i != cnt - vertexPerSide)
                    {
                        if (!skipDrawEllipse)
                        {
                            ToolSet.MeshAddTriangleIndecies(_surfaceTool, center, start, next);
                            start++;
                            next++;
                        }
                        if (!skipDrawLine)
                        {
                            ToolSet.MeshAddTriangleIndecies(_surfaceTool, next, start, start + 1);
                            ToolSet.MeshAddTriangleIndecies(_surfaceTool, start + 1, next + 1, next);
                        }
                    }
                    else if (!clipped)
                    {
                        int first = sideVertexStart;
                        if (!skipDrawEllipse)
                        {
                            ToolSet.MeshAddTriangleIndecies(_surfaceTool, center, start, sideVertexStart);
                            start++;
                            next++;
                            first++;
                        }
                        if (!skipDrawLine)
                        {
                            ToolSet.MeshAddTriangleIndecies(_surfaceTool, first, start, start + 1);
                            ToolSet.MeshAddTriangleIndecies(_surfaceTool, start + 1, first + 1, first);
                        }
                    }
                    angle += angleDelta;
                    if (angle > sectionEnd)
                        angle = sectionEnd;
                }
            }
            else if (!skipDrawEllipse)
            {
                int cnt = sides * vertexPerSide;
                int center = sideVertexStart - 1;
                for (int i = 0; i < cnt; i += vertexPerSide)
                {
                    int start = i + sideVertexStart;
                    if (i != cnt - vertexPerSide)
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, center, start, start + vertexPerSide);
                    else if (!clipped)
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, center, start, sideVertexStart);
                }
            }
            if (reserveDraw)
            {
                sVertexBuffer1.Reverse();
                DrawOutBound(_surfaceTool, sVertexBuffer1, vertexCount, polygonType);
            }

            // _surfaceTool.GenerateNormals();
            if (_material != null)
                _surfaceTool.SetMaterial(_material);
            _surfaceTool.Commit(_mesh);
        }
        static int ChangeBound(int bound, int change)
        {
            bound += change;
            if (bound >= 4)
                bound -= 4;
            if (bound < 0)
                bound += 4;
            return bound;
        }
        static int GetBoundGap(int startBound, int endBound, int boundCount)
        {
            if (endBound > startBound)
                return endBound - startBound;
            else
                return boundCount - startBound + endBound;
        }
        static int SelectBoundPoint(Vector2[] boundPoints, Vector2 start, Vector2 end)
        {
            int select = 0;
            float selectLen = -1;
            for (int i = 0; i < boundPoints.Length; i++)
            {
                Vector2 r = end - boundPoints[i];
                Vector2 s = start - boundPoints[i];
                float rxs = r.Cross(s);
                float len = r.Length() + s.Length();
                if ((rxs >= 0) && (selectLen < 0 || selectLen > len))
                {
                    select = i;
                    selectLen = len;
                }
            }
            return select;
        }

        static int SelectNextBoundPoint(Vector2[] boundPoints, Vector2 start, Vector2 end, int bound)
        {
            for (int i = 0; i < boundPoints.Length; i++)
            {
                Vector2 r = end - boundPoints[bound];
                Vector2 s = start - boundPoints[bound];
                float rxs = r.Cross(s);
                if (rxs >= 0)
                {
                    return bound;
                }
                bound++;
                if (bound >= boundPoints.Length)
                    bound = 0;
            }
            return -1;
        }
        static void DrawBoundGap(SurfaceTool surfaceTool, int startBound, int endBound, int boundVertexStart, int boundCount, int vertexIndex)
        {
            int gap = GetBoundGap(startBound - boundVertexStart, endBound - boundVertexStart, boundCount);
            if (gap > 0)
            {
                int bound1 = startBound;
                if (gap > 1)
                {
                    while (gap > 1)
                    {
                        int bound2 = ChangeBound(bound1 - boundVertexStart, 1) + boundVertexStart;
                        ToolSet.MeshAddTriangleIndecies(surfaceTool, bound1, bound2, vertexIndex);
                        gap--;
                        bound1 = bound2;
                    }
                }
                ToolSet.MeshAddTriangleIndecies(surfaceTool, bound1, endBound, vertexIndex);
            }
        }

        //填充多边型外围，需保证顶点为逆时针排列
        void DrawOutBound(SurfaceTool surfaceTool, List<Vector2> polygonVertices, int vertexStart, PolygonType polygonType)
        {
            bool IsConvex;
            switch (polygonType)
            {
                case PolygonType.Convex:
                    IsConvex = true;
                    break;
                case PolygonType.Concave:
                    IsConvex = false;
                    break;
                default:
                    IsConvex = ToolSet.IsConvex(polygonVertices);
                    break;
            }
            if (IsConvex)
            {
                //凸多边形，采用简化算法
                int count = polygonVertices.Count;
                for (int i = 0; i < count; i++)
                    ToolSet.MeshAddVertex(surfaceTool, polygonVertices[i].X, polygonVertices[i].Y, outColor);
                int boundVertexStart = vertexStart + count;
                Rect outRect = MakeOutRect();
                Vector2[] rectPoints = new Vector2[]{
                        new Vector2(outRect.xMax, outRect.yMax),
                        new Vector2(outRect.xMin, outRect.yMax),
                        new Vector2(outRect.xMin, outRect.yMin),
                        new Vector2(outRect.xMax, outRect.yMin)
                        };
                ToolSet.MeshAddVertex(surfaceTool, rectPoints[0].X, rectPoints[0].Y, outColor);
                ToolSet.MeshAddVertex(surfaceTool, rectPoints[1].X, rectPoints[1].Y, outColor);
                ToolSet.MeshAddVertex(surfaceTool, rectPoints[2].X, rectPoints[2].Y, outColor);
                ToolSet.MeshAddVertex(surfaceTool, rectPoints[3].X, rectPoints[3].Y, outColor);
                int bound = -1;
                int firstBound = -1;
                int lastBound = -1;
                int start;
                int end;
                Vector2 startVertex;
                Vector2 endVertex;
                for (int i = count - 1; i >= 0; i--)
                {
                    start = i;
                    end = (i > 0) ? i - 1 : count - 1;
                    startVertex = polygonVertices[start];
                    endVertex = polygonVertices[end];
                    start += vertexStart;
                    end += vertexStart;
                    int newBound;
                    if (bound < 0)
                        newBound = SelectBoundPoint(rectPoints, startVertex, endVertex) + boundVertexStart;
                    else
                        newBound = SelectNextBoundPoint(rectPoints, startVertex, endVertex, bound - boundVertexStart) + boundVertexStart;
                    if (bound >= 0 && newBound != bound)
                        DrawBoundGap(surfaceTool, bound, newBound, boundVertexStart, 4, start);
                    bound = newBound;
                    ToolSet.MeshAddTriangleIndecies(surfaceTool, start, bound, end);
                    if (firstBound <= 0)
                        firstBound = bound;
                    lastBound = bound;
                }
                if (firstBound != lastBound)
                    DrawBoundGap(surfaceTool, lastBound, firstBound, boundVertexStart, 4, vertexStart + count - 1);
            }
            else
            {
                //凹多边形，构建环形条带后，使用切耳三角化
                Rect outRect = MakeOutRect(_lineWidth, _lineWidth);
                Vector2[] rectPoints = new Vector2[]{
                        new Vector2(outRect.xMax, outRect.yMax),
                        new Vector2(outRect.xMin, outRect.yMax),
                        new Vector2(outRect.xMin, outRect.yMin),
                        new Vector2(outRect.xMax, outRect.yMin)
                        };
                int start = -1;
                int testIndex = 0;
                for (; testIndex < polygonVertices.Count; testIndex++)
                {
                    Vector2 a = polygonVertices[testIndex];
                    float minDis = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 b = rectPoints[i];
                        if (!ToolSet.SegmentIntersectsPolygon(a, b, polygonVertices))
                        {
                            float dis = a.DistanceTo(b);
                            if (start < 0 || dis < minDis)
                            {
                                start = i;
                                minDis = dis;
                            }
                        }
                    }
                    if (start >= 0)
                        break;
                }
                if (testIndex > 0)
                {
                    for (int i = 0; i < testIndex; i++)
                        polygonVertices.Add(polygonVertices[i]);
                    polygonVertices.RemoveRange(0, testIndex);
                }
                if (start >= 0)
                {
                    polygonVertices.Add(polygonVertices[0]);
                    for (int i = 0; i < 4; i++)
                    {
                        polygonVertices.Add(rectPoints[start]);
                        start++;
                        if (start >= 4)
                            start = 0;
                    }
                    polygonVertices.Add(rectPoints[start]);
                }
                for (int i = 0; i < polygonVertices.Count; i++)
                    ToolSet.MeshAddVertex(surfaceTool, polygonVertices[i].X, polygonVertices[i].Y, outColor);
                ToolSet.AddPolygonIndecies(surfaceTool, polygonVertices, vertexStart);
            }
        }

        void BuildPolygonMesh()
        {
            _mesh.ClearSurfaces();
            _surfaceTool.Clear();
            _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

            int numVertices = _polygonPoints.Count;
            if (numVertices < 3)
                return;

            Color lineColor_ = _lineColor;
            Color lineColorOuter_ = _lineColorOuter == null ? _lineColor : (Color)_lineColorOuter;
            Color fillColor_ = _fillColor;
            Color[] colors_ = _colors;

            bool reserveDraw = maskOwner != null && reverseMask;
            if (reserveDraw)
            {
                lineColor_.A = 1.0f - lineColor_.A;
                lineColorOuter_.A = 1.0f - lineColorOuter_.A;
                fillColor_.A = 1.0f - fillColor_.A;
                if (colors_ != null)
                {
                    colors_ = colors_.Clone() as Color[];
                    for (int i = 0; i < colors_.Length; i++)
                        colors_[i].A = 1.0f - colors_[i].A;
                }
            }

            bool drawPolygon = !Mathf.IsZeroApprox(fillColor_.A);
            if (colors_ != null && colors_.Length > 0)
            {
                drawPolygon = true;
                for (int i = 0; i < colors_.Length; i++)
                {
                    if (!Mathf.IsZeroApprox(colors_[i].A))
                    {
                        drawPolygon = true;
                        break;
                    }
                }
            }
            if (drawPolygon)
            {
                float w = Size.X;
                float h = Size.Y;
                Rect uvRect = new Rect(0, 0, 1, 1);
                for (int i = 0; i < numVertices; i++)
                {
                    Vector3 vec = new Vector3(_polygonPoints[i].X, _polygonPoints[i].Y, 0);
                    if (_usePercentPositions)
                    {
                        vec.X *= w;
                        vec.Y *= h;
                    }
                    if (_texture != null)
                    {
                        Vector2 uv;
                        if (_polygonUVs.Count >= numVertices)
                        {
                            uv = _polygonUVs[i];
                            uv.X = Mathf.Lerp(uvRect.X, uvRect.xMax, uv.X);
                            uv.Y = Mathf.Lerp(uvRect.Y, uvRect.yMax, uv.Y);
                        }
                        else
                        {
                            uv = Vector2.Zero;
                            uv.X = Mathf.Lerp(uvRect.X, uvRect.xMax, vec.X / Size.X);
                            uv.Y = Mathf.Lerp(uvRect.Y, uvRect.yMax, vec.Y / Size.Y);
                        }
                        ToolSet.MeshAddVertex(_surfaceTool, vec, uv, _colors == null || _colors.Length <= i ? _fillColor : _colors[i]);
                    }
                    else
                        ToolSet.MeshAddVertex(_surfaceTool, vec, _colors == null || _colors.Length <= i ? _fillColor : _colors[i]);
                }
                ToolSet.AddPolygonIndecies(_surfaceTool, _polygonPoints, 0);
            }
            else
            {
                numVertices = 0;
            }

            if (_lineWidth > 0 && (!Mathf.IsZeroApprox(lineColor_.A) || !Mathf.IsZeroApprox(lineColorOuter_.A)))
                numVertices += DrawPolygonOutline(numVertices, lineColor_, lineColorOuter_);

            if (reserveDraw)
                DrawPolygonOutBound(numVertices);

            // _surfaceTool.GenerateNormals();
            if (_material != null)
                _surfaceTool.SetMaterial(_material);
            _surfaceTool.Commit(_mesh);
        }

        int DrawPolygonOutline(int StartIndex, Color lineColor_, Color lineColorOuter_)
        {
            sVertexBuffer1.Clear();
            Vector2 vec = _polygonPoints[0];
            ToolSet.MeshAddVertex(_surfaceTool, vec.X, vec.Y, lineColor_);
            sVertexBuffer1.Add(vec);
            for (int i = _polygonPoints.Count - 1; i >= 0; i--)
            {
                vec = _polygonPoints[i];
                ToolSet.MeshAddVertex(_surfaceTool, vec.X, vec.Y, lineColor_);
                sVertexBuffer1.Add(vec);
            }
            Vector2 first = Vector2.Inf;
            for (int i = 0; i < _polygonPoints.Count; i++)
            {
                Vector2 p1;
                if (i == 0)
                    p1 = _polygonPoints[_polygonPoints.Count - 1];
                else
                    p1 = _polygonPoints[i - 1];
                Vector2 p2 = _polygonPoints[i];
                Vector2 p3;
                if (i + 1 < _polygonPoints.Count)
                    p3 = _polygonPoints[i + 1];
                else
                    p3 = _polygonPoints[0];

                Vector2 dir = p1 - p2;
                Vector2 perp = new Vector2(-dir.Y, dir.X);
                perp = perp.Normalized();
                Vector2 q1 = p1 + perp * _lineWidth;
                Vector2 q2 = p2 + perp * _lineWidth;

                dir = p2 - p3;
                perp = new Vector2(-dir.Y, dir.X);
                perp = perp.Normalized();
                Vector2 q3 = p2 + perp * _lineWidth;
                Vector2 q4 = p3 + perp * _lineWidth;


                Vector2 cross;
                ToolSet.LineIntersection(q1, q2, q3, q4, out cross);
                if ((q2.Y - cross.Y) * (q3.X - cross.X) + (cross.X - q2.X) * (q3.Y - cross.Y) >= 0)
                {
                    if (cross.DistanceTo(p2) > _lineWidth)
                        cross = p2 + (cross - p2).Normalized() * _lineWidth;
                    ToolSet.MeshAddVertex(_surfaceTool, q2.X, q2.Y, lineColorOuter_);
                    ToolSet.MeshAddVertex(_surfaceTool, cross.X, cross.Y, lineColorOuter_);
                    ToolSet.MeshAddVertex(_surfaceTool, q3.X, q3.Y, lineColorOuter_);
                    sVertexBuffer1.Add(q2);
                    sVertexBuffer1.Add(cross);
                    sVertexBuffer1.Add(q3);
                    if (!first.IsFinite())
                        first = q2;
                }
                else
                {
                    ToolSet.MeshAddVertex(_surfaceTool, cross.X, cross.Y, lineColorOuter_);
                    sVertexBuffer1.Add(cross);
                    if (!first.IsFinite())
                        first = cross;
                }

            }
            ToolSet.MeshAddVertex(_surfaceTool, first.X, first.Y, lineColorOuter_);
            sVertexBuffer1.Add(first);
            ToolSet.AddRibbonIndecies(_surfaceTool, sVertexBuffer1, StartIndex);
            return sVertexBuffer1.Count;
        }

        void DrawPolygonOutBound(int StartIndex)
        {
            sVertexBuffer1.Clear();
            if (_lineWidth > 0)
            {
                for (int i = _polygonPoints.Count - 1; i >= 0; i--)
                {
                    Vector2 p1;
                    if (i == _polygonPoints.Count - 1)
                        p1 = _polygonPoints[0];
                    else
                        p1 = _polygonPoints[i + 1];
                    Vector2 p2 = _polygonPoints[i];
                    Vector2 p3;
                    if (i == 0)
                        p3 = _polygonPoints[_polygonPoints.Count - 1];
                    else
                        p3 = _polygonPoints[i - 1];

                    Vector2 dir = p1 - p2;
                    Vector2 perp = new Vector2(-dir.Y, dir.X);
                    perp = perp.Normalized();
                    Vector2 q1 = p1 - perp * _lineWidth;
                    Vector2 q2 = p2 - perp * _lineWidth;

                    dir = p2 - p3;
                    perp = new Vector2(-dir.Y, dir.X);
                    perp = perp.Normalized();
                    Vector2 q3 = p2 - perp * _lineWidth;
                    Vector2 q4 = p3 - perp * _lineWidth;


                    Vector2 cross;
                    ToolSet.LineIntersection(q1, q2, q3, q4, out cross);
                    if ((q2.Y - cross.Y) * (q3.X - cross.X) + (cross.X - q2.X) * (q3.Y - cross.Y) < 0)
                    {
                        if (cross.DistanceTo(p2) > _lineWidth)
                            cross = p2 + (cross - p2).Normalized() * _lineWidth;
                        sVertexBuffer1.Add(q2);
                        sVertexBuffer1.Add(cross);
                        sVertexBuffer1.Add(q3);
                    }
                    else
                    {
                        sVertexBuffer1.Add(cross);
                    }
                }
            }
            else
            {
                for (int i = _polygonPoints.Count - 1; i >= 0; i--)
                {
                    sVertexBuffer1.Add(_polygonPoints[i]);
                }
            }
            DrawOutBound(_surfaceTool, sVertexBuffer1, StartIndex, PolygonType.Unknow);
        }

        void BuildRegularPolygonMesh()
        {
            _mesh.ClearSurfaces();
            _surfaceTool.Clear();
            _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

            if (_polygonDistances != null && _polygonDistances.Length < _polygonSides)
            {
                GD.PushError("distances.Length<sides");
                return;
            }

            Color fillColor_ = _fillColor;
            Color lineColor_ = _lineColor;
            Color lineColorOuter_ = _lineColorOuter == null ? _lineColor : (Color)_lineColorOuter;
            Color centerColor_ = _centerColor == null ? _fillColor : (Color)_centerColor;

            bool reserveDraw = maskOwner != null && reverseMask;
            if (reserveDraw)
            {
                fillColor_.A = 1.0f - fillColor_.A;
                lineColor_.A = 1.0f - lineColor_.A;
                lineColorOuter_.A = 1.0f - lineColorOuter_.A;
                centerColor_.A = 1.0f - centerColor_.A;
            }

            Rect rect = new Rect(Vector2.Zero, Size);

            float angleDelta = 2 * Mathf.Pi / _polygonSides;
            float angle = Mathf.DegToRad(_polygonRotation);
            float radius = Mathf.Min(rect.width / 2, rect.height / 2);


            float centerX = radius + rect.X;
            float centerY = radius + rect.Y;
            ToolSet.MeshAddVertex(_surfaceTool, centerX, centerY, centerColor_);
            int vertexCount = 1;
            int sideVertexCount = 1;
            if (_lineWidth > 0)
                sideVertexCount += 2;
            sVertexBuffer1.Clear();
            for (int i = 0; i < _polygonSides; i++)
            {
                float r = radius;
                if (_polygonDistances != null)
                    r *= _polygonDistances[i];
                float xv = Mathf.Cos(angle) * (r - _lineWidth) + centerX;
                float yv = Mathf.Sin(angle) * (r - _lineWidth) + centerY;
                ToolSet.MeshAddVertex(_surfaceTool, xv, yv, fillColor_);
                vertexCount++;
                if (_lineWidth > 0)
                {
                    ToolSet.MeshAddVertex(_surfaceTool, xv, yv, lineColor_);
                    xv = Mathf.Cos(angle) * r + centerX;
                    yv = Mathf.Sin(angle) * r + centerY;
                    ToolSet.MeshAddVertex(_surfaceTool, xv, yv, lineColorOuter_);
                    vertexCount += 2;
                }
                if (reserveDraw)
                {
                    sVertexBuffer1.Add(new Vector2(xv, yv));
                }
                angle += angleDelta;
            }

            int tmp = _polygonSides * sideVertexCount;
            for (int i = 0; i < tmp; i += sideVertexCount)
            {
                int start = i + 1;
                int end = (i != tmp - sideVertexCount) ? i + sideVertexCount + 1 : 1;
                ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, start, end);
                if (_lineWidth > 0)
                {
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, start + 1, start + 2, end + 2);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, start + 1, end + 2, end + 1);
                }
            }

            if (reserveDraw)
            {
                sVertexBuffer1.Reverse();
                DrawOutBound(_surfaceTool, sVertexBuffer1, vertexCount, PolygonType.Unknow);
            }

            // _surfaceTool.GenerateNormals();
            if (_material != null)
                _surfaceTool.SetMaterial(_material);
            _surfaceTool.Commit(_mesh);
        }

        public bool HitTest(Rect contentRect, Vector2 localPoint)
        {
            switch (_shapeType)
            {
                case ShapeType.Ellipse:
                    {
                        if (!contentRect.Contains(localPoint))
                            return false;
                        float radiusX = contentRect.width * 0.5f;
                        float raduisY = contentRect.height * 0.5f;
                        float xx = localPoint.X - radiusX - contentRect.X;
                        float yy = localPoint.Y - raduisY - contentRect.Y;
                        if (Mathf.Pow(xx / radiusX, 2) + Mathf.Pow(yy / raduisY, 2) < 1)
                        {
                            if (_startDegree != 0 || _endDegree != 360)
                            {
                                float deg = Mathf.RadToDeg(Mathf.Atan2(yy, xx));
                                if (deg < 0)
                                    deg += 360;
                                return deg >= _startDegree && deg <= _endDegree;
                            }
                            else
                                return true;
                        }
                        return false;
                    }
                case ShapeType.Polygon:
                    {
                        if (!contentRect.Contains(localPoint))
                            return false;
                        // Algorithm & implementation thankfully taken from:
                        // -> http://alienryderflex.com/polygon/
                        // inspired by Starling
                        int len = _polygonPoints.Count;
                        int i;
                        int j = len - 1;
                        bool oddNodes = false;
                        float w = contentRect.width;
                        float h = contentRect.height;
                        for (i = 0; i < len; ++i)
                        {
                            float ix = _polygonPoints[i].X;
                            float iy = _polygonPoints[i].Y;
                            float jx = _polygonPoints[j].X;
                            float jy = _polygonPoints[j].Y;
                            if (_usePercentPositions)
                            {
                                ix *= w;
                                iy *= h;
                                ix *= w;
                                iy *= h;
                            }
                            if ((iy < localPoint.Y && jy >= localPoint.Y || jy < localPoint.Y && iy >= localPoint.Y) && (ix <= localPoint.X || jx <= localPoint.X))
                            {
                                if (ix + (localPoint.Y - iy) / (jy - iy) * (jx - ix) < localPoint.X)
                                    oddNodes = !oddNodes;
                            }
                            j = i;
                        }
                        return oddNodes;
                    }
                default:
                    return contentRect.Contains(localPoint);
            }
        }
    }
}
