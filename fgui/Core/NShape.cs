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
        protected float _skewX = 0;
        protected float _skewY = 0;
        protected CanvasItemMaterial _material;
        protected ArrayMesh _mesh;
        protected SurfaceTool _surfaceTool;
        protected Color _lineColor = Colors.Black;
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
        public NShape(GObject owner)
        {
            gOwner = owner;
            MouseFilter = MouseFilterEnum.Ignore;
            //Name = "Shape";
            _material = new CanvasItemMaterial();
            _material.LightMode = CanvasItemMaterial.LightModeEnum.Unshaded;
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
                    if (!_rectRadius.IsZeroApprox())
                        BuildRoundRectMesh();
                    else
                        BuildRectMesh();
                    DrawMesh(_mesh, null);
                    break;
                case ShapeType.Ellipse:
                    BuildEllipseMesh();
                    DrawMesh(_mesh, null);
                    break;
                case ShapeType.Polygon:
                    BuildPolygonMesh();
                    DrawMesh(_mesh, _texture?.nativeTexture);
                    break;
                case ShapeType.RegularPolygon:
                    BuildRegularPolygonMesh();
                    DrawMesh(_mesh, null);
                    break;
            }
        }
        void BuildRectMesh()
        {
            _mesh.ClearSurfaces();
            _surfaceTool.Clear();
            _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

            Rect rect = new Rect(Vector2.Zero, Size);
            if (_lineWidth == 0)
            {
                if (!Mathf.IsEqualApprox(_fillColor.A, 0))
                    ToolSet.MeshAddRect(_surfaceTool, rect, _fillColor, 0, _colors, 0);
            }
            else
            {
                Rect part;

                //left,right
                int StartIndex = 0;
                part = new Rect(rect.X, rect.Y, _lineWidth, rect.height);
                ToolSet.MeshAddRect(_surfaceTool, part, _lineColor, StartIndex, _colors, StartIndex);
                StartIndex += 4;
                part = new Rect(rect.xMax - _lineWidth, rect.Y, _lineWidth, rect.height);
                ToolSet.MeshAddRect(_surfaceTool, part, _lineColor, StartIndex, _colors, StartIndex);
                StartIndex += 4;

                //top, bottom
                part = new Rect(rect.X + _lineWidth, rect.Y, rect.width - _lineWidth * 2, _lineWidth);
                ToolSet.MeshAddRect(_surfaceTool, part, _lineColor, StartIndex, _colors, StartIndex);
                StartIndex += 4;
                part = new Rect(rect.X + _lineWidth, rect.yMax - _lineWidth, rect.width - _lineWidth * 2, _lineWidth);
                ToolSet.MeshAddRect(_surfaceTool, part, _lineColor, StartIndex, _colors, StartIndex);
                StartIndex += 4;

                //middle
                if (!Mathf.IsEqualApprox(_fillColor.A, 0))//optimized
                {
                    part = Rect.MinMaxRect(rect.X + _lineWidth, rect.Y + _lineWidth, rect.xMax - _lineWidth, rect.yMax - _lineWidth);
                    if (part.width > 0 && part.height > 0)
                        ToolSet.MeshAddRect(_surfaceTool, part, _fillColor, StartIndex, _colors, StartIndex);
                }
            }
            _surfaceTool.GenerateNormals();
            if (_material != null)
                _surfaceTool.SetMaterial(_material);
            _surfaceTool.Commit(_mesh);
        }
        void BuildRoundRectMesh()
        {
            _mesh.ClearSurfaces();
            _surfaceTool.Clear();
            _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

            Rect rect = new Rect(Vector2.Zero, Size);

            float radiusX = rect.width / 2;
            float radiusY = rect.height / 2;
            float cornerMaxRadius = Mathf.Min(radiusX, radiusY);
            float centerX = radiusX + rect.X;
            float centerY = radiusY + rect.Y;

            ToolSet.MeshAddVertex(_surfaceTool, centerX, centerY, _fillColor);

            int cnt = 0;
            for (int i = 0; i < 4; i++)
            {
                float radius = 0;
                switch (i)
                {
                    case 0:
                        radius = _rectRadius.W;
                        break;

                    case 1:
                        radius = _rectRadius.Z;
                        break;

                    case 2:
                        radius = _rectRadius.X;
                        break;

                    case 3:
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
                        Vector3 v1 = new Vector3(offsetX + Mathf.Cos(angle) * (radius - _lineWidth) + radius,
                            offsetY + Mathf.Sin(angle) * (radius - _lineWidth) + radius, 0);
                        ToolSet.MeshAddVertex(_surfaceTool, v1.X, v1.Y, _fillColor);
                        cnt++;
                        if (_lineWidth != 0)
                        {
                            ToolSet.MeshAddVertex(_surfaceTool, v1.X, v1.Y, _lineColor);
                            ToolSet.MeshAddVertex(_surfaceTool, offsetX + Mathf.Cos(angle) * radius + radius, offsetY + Mathf.Sin(angle) * radius + radius, _lineColor);
                            cnt += 2;
                        }
                        angle += angleDelta;
                    }
                }
                else
                {
                    Vector3 v1 = new Vector3(offsetX, offsetY, 0);
                    if (_lineWidth != 0)
                    {
                        if (i == 0 || i == 3)
                            offsetX -= _lineWidth;
                        else
                            offsetX += _lineWidth;
                        if (i == 0 || i == 1)
                            offsetY -= _lineWidth;
                        else
                            offsetY += _lineWidth;
                        Vector3 v2 = new Vector3(offsetX, offsetY, 0);
                        ToolSet.MeshAddVertex(_surfaceTool, v2.X, v2.Y, _fillColor);
                        ToolSet.MeshAddVertex(_surfaceTool, v2.X, v2.Y, _lineColor);
                        ToolSet.MeshAddVertex(_surfaceTool, v1.X, v1.Y, _lineColor);
                        cnt += 3;
                    }
                    else
                    {
                        ToolSet.MeshAddVertex(_surfaceTool, v1.X, v1.Y, _fillColor);
                        cnt++;
                    }
                }
            }

            if (_lineWidth > 0)
            {
                for (int i = 0; i < cnt; i += 3)
                {
                    if (i != cnt - 3)
                    {
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, i + 1, i + 4);
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, i + 5, i + 2, i + 3);
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, i + 3, i + 6, i + 5);
                    }
                    else
                    {
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, i + 1, 1);
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, 2, i + 2, i + 3);
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, i + 3, 3, 2);
                    }
                }
            }
            else
            {
                for (int i = 0; i < cnt; i++)
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, i + 1, (i == cnt - 1) ? 1 : i + 2);
            }

            _surfaceTool.GenerateNormals();
            if (_material != null)
                _surfaceTool.SetMaterial(_material);
            _surfaceTool.Commit(_mesh);
        }
        struct VertexWithColor
        {
            public float x;
            public float y;
            public Color color;
            public VertexWithColor(float _x, float _y, Color _color)
            {
                x = _x;
                y = _y;
                color = _color;
            }
        };
        static int[] SECTOR_CENTER_TRIANGLES = new int[] {
                0, 4, 1,
                0, 3, 4,
                0, 2, 3,
                0, 8, 5,
                0, 7, 8,
                0, 6, 7,
                6, 5, 2,
                2, 1, 6
            };
        void BuildEllipseMesh()
        {
            _mesh.ClearSurfaces();
            _surfaceTool.Clear();
            _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

            Rect rect = new Rect(Vector2.Zero, Size);

            float sectionStart = Mathf.Clamp(_startDegree, 0, 360);
            float sectionEnd = Mathf.Clamp(_endDegree, 0, 360);
            bool clipped = sectionStart > 0 || sectionEnd < 360;
            sectionStart = Mathf.DegToRad(sectionStart);
            sectionEnd = Mathf.DegToRad(sectionEnd);
            Color centerColor2 = _centerColor == null ? _fillColor : (Color)_centerColor;

            float radiusX = rect.width / 2;
            float radiusY = rect.height / 2;
            int sides = Mathf.CeilToInt(Mathf.Pi * (radiusX + radiusY) / 4);
            sides = Mathf.Clamp(sides, 40, 800);
            float angleDelta = 2 * Mathf.Pi / sides;
            float angle = 0;
            float lineAngle = 0;

            if (_lineWidth > 0 && clipped)
            {
                lineAngle = _lineWidth / Mathf.Max(radiusX, radiusY);
                sectionStart += lineAngle;
                sectionEnd -= lineAngle;
            }

            float centerX = rect.X + radiusX;
            float centerY = rect.Y + radiusY;
            List<VertexWithColor> sideVertList = new List<VertexWithColor>(sides * 3 + 1);
            sideVertList.Add(new VertexWithColor(centerX, centerY, centerColor2));
            for (int i = 0; i < sides; i++)
            {
                if (angle < sectionStart)
                    angle = sectionStart;
                else if (angle > sectionEnd)
                    angle = sectionEnd;
                Vector3 vec = new Vector3(Mathf.Cos(angle) * (radiusX - _lineWidth) + centerX, Mathf.Sin(angle) * (radiusY - _lineWidth) + centerY, 0);
                sideVertList.Add(new VertexWithColor(vec.X, vec.Y, _fillColor));
                if (_lineWidth > 0)
                {
                    sideVertList.Add(new VertexWithColor(vec.X, vec.Y, _lineColor));
                    sideVertList.Add(new VertexWithColor(Mathf.Cos(angle) * radiusX + centerX, Mathf.Sin(angle) * radiusY + centerY, _lineColor));
                }
                angle += angleDelta;
            }

            foreach (var vert in sideVertList)
                ToolSet.MeshAddVertex(_surfaceTool, vert.x, vert.y, vert.color);

            if (_lineWidth > 0)
            {
                int cnt = sides * 3;
                for (int i = 0; i < cnt; i += 3)
                {
                    if (i != cnt - 3)
                    {
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, i + 1, i + 4);
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, i + 5, i + 2, i + 3);
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, i + 3, i + 6, i + 5);
                    }
                    else if (!clipped)
                    {
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, i + 1, 1);
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, 2, i + 2, i + 3);
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, i + 3, 3, 2);
                    }
                    else
                    {
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, i + 1, i + 1);
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, i + 2, i + 2, i + 3);
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, i + 3, i + 3, i + 2);
                    }
                }
            }
            else
            {
                for (int i = 0; i < sides; i++)
                {
                    if (i != sides - 1)
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, i + 1, i + 2);
                    else if (!clipped)
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, i + 1, 1);
                    else
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, i + 1, i + 1);
                }
            }

            if (_lineWidth > 0 && clipped)
            {
                //扇形内边缘的线条

                ToolSet.MeshAddVertex(_surfaceTool, radiusX, radiusY, _lineColor);
                float centerRadius = _lineWidth * 0.5f;

                sectionStart -= lineAngle;
                angle = sectionStart + lineAngle * 0.5f + Mathf.Pi * 0.5f;
                ToolSet.MeshAddVertex(_surfaceTool, Mathf.Cos(angle) * centerRadius + radiusX, Mathf.Sin(angle) * centerRadius + radiusY, _lineColor);
                angle -= Mathf.Pi;
                ToolSet.MeshAddVertex(_surfaceTool, Mathf.Cos(angle) * centerRadius + radiusX, Mathf.Sin(angle) * centerRadius + radiusY, _lineColor);
                ToolSet.MeshAddVertex(_surfaceTool, Mathf.Cos(sectionStart) * radiusX + radiusX, Mathf.Sin(sectionStart) * radiusY + radiusY, _lineColor);
                VertexWithColor vert = sideVertList[3];
                ToolSet.MeshAddVertex(_surfaceTool, vert.x, vert.y, _lineColor);

                sectionEnd += lineAngle;
                angle = sectionEnd - lineAngle * 0.5f + Mathf.Pi * 0.5f;
                ToolSet.MeshAddVertex(_surfaceTool, Mathf.Cos(angle) * centerRadius + radiusX, Mathf.Sin(angle) * centerRadius + radiusY, _lineColor);
                angle -= Mathf.Pi;
                ToolSet.MeshAddVertex(_surfaceTool, Mathf.Cos(angle) * centerRadius + radiusX, Mathf.Sin(angle) * centerRadius + radiusY, _lineColor);
                vert = sideVertList[sides * 3];
                ToolSet.MeshAddVertex(_surfaceTool, vert.x, vert.y, _lineColor);
                ToolSet.MeshAddVertex(_surfaceTool, Mathf.Cos(sectionEnd) * radiusX + radiusX, Mathf.Sin(sectionEnd) * radiusY + radiusY, _lineColor);

                for (int i = 0; i < SECTOR_CENTER_TRIANGLES.Length; i++)
                    _surfaceTool.AddIndex(SECTOR_CENTER_TRIANGLES[i] + sideVertList.Count);
            }

            _surfaceTool.GenerateNormals();
            if (_material != null)
                _surfaceTool.SetMaterial(_material);
            _surfaceTool.Commit(_mesh);
        }
        static List<int> sRestIndices = new List<int>();
        void BuildPolygonMesh()
        {
            _mesh.ClearSurfaces();
            _surfaceTool.Clear();
            _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

            int numVertices = _polygonPoints.Count;
            if (numVertices < 3)
                return;

            int restIndexPos, numRestIndices;

            float w = Size.X;
            float h = Size.Y;
            bool useTexcoords = _polygonUVs.Count >= numVertices;
            Rect uvRect = _texture != null ? _texture.uvRect : Rect.zero;
            for (int i = 0; i < numVertices; i++)
            {
                Vector3 vec = new Vector3(_polygonPoints[i].X, _polygonPoints[i].Y, 0);
                if (_usePercentPositions)
                {
                    vec.X *= w;
                    vec.Y *= h;
                }
                if (useTexcoords)
                {
                    Vector2 uv = _polygonUVs[i];
                    uv.X = Mathf.Lerp(uvRect.X, uvRect.xMax, uv.X);
                    uv.Y = Mathf.Lerp(uvRect.Y, uvRect.yMax, uv.Y);
                    ToolSet.MeshAddVertex(_surfaceTool, vec, uv, _colors == null || _colors.Length <= i ? _fillColor : _colors[i]);
                }
                else
                    ToolSet.MeshAddVertex(_surfaceTool, vec, _colors == null || _colors.Length <= i ? _fillColor : _colors[i]);
            }

            // Algorithm "Ear clipping method" described here:
            // -> https://en.wikipedia.org/wiki/Polygon_triangulation
            //
            // Implementation inspired by:
            // -> http://polyk.ivank.net
            // -> Starling

            sRestIndices.Clear();
            for (int i = 0; i < numVertices; ++i)
                sRestIndices.Add(i);

            restIndexPos = 0;
            numRestIndices = numVertices;

            Vector2 a, b, c, p;
            int otherIndex;
            bool earFound;
            int i0, i1, i2;

            while (numRestIndices > 3)
            {
                earFound = false;
                i0 = sRestIndices[restIndexPos % numRestIndices];
                i1 = sRestIndices[(restIndexPos + 1) % numRestIndices];
                i2 = sRestIndices[(restIndexPos + 2) % numRestIndices];

                a = _polygonPoints[i0];
                b = _polygonPoints[i1];
                c = _polygonPoints[i2];

                if ((a.Y - b.Y) * (c.X - b.X) + (b.X - a.X) * (c.Y - b.Y) >= 0)
                {
                    earFound = true;
                    for (int i = 3; i < numRestIndices; ++i)
                    {
                        otherIndex = sRestIndices[(restIndexPos + i) % numRestIndices];
                        p = _polygonPoints[otherIndex];

                        if (IsPointInTriangle(ref p, ref a, ref b, ref c))
                        {
                            earFound = false;
                            break;
                        }
                    }
                }

                if (earFound)
                {
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, i0, i1, i2);
                    sRestIndices.RemoveAt((restIndexPos + 1) % numRestIndices);

                    numRestIndices--;
                    restIndexPos = 0;
                }
                else
                {
                    restIndexPos++;
                    if (restIndexPos == numRestIndices) break; // no more ears
                }
            }
            ToolSet.MeshAddTriangleIndecies(_surfaceTool, sRestIndices[0], sRestIndices[1], sRestIndices[2]);

            if (_lineWidth > 0)
                DrawOutline(numVertices);

            _surfaceTool.GenerateNormals();
            if (_material != null)
                _surfaceTool.SetMaterial(_material);
            _surfaceTool.Commit(_mesh);
        }
        bool IsPointInTriangle(ref Vector2 p, ref Vector2 a, ref Vector2 b, ref Vector2 c)
        {
            // From Starling
            // This algorithm is described well in this article:
            // http://www.blackpawn.com/texts/pointinpoly/default.html

            float v0x = c.X - a.X;
            float v0y = c.Y - a.Y;
            float v1x = b.X - a.X;
            float v1y = b.Y - a.Y;
            float v2x = p.X - a.X;
            float v2y = p.Y - a.Y;

            float dot00 = v0x * v0x + v0y * v0y;
            float dot01 = v0x * v1x + v0y * v1y;
            float dot02 = v0x * v2x + v0y * v2y;
            float dot11 = v1x * v1x + v1y * v1y;
            float dot12 = v1x * v2x + v1y * v2y;

            float invDen = 1.0f / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDen;
            float v = (dot00 * dot12 - dot01 * dot02) * invDen;

            return (u >= 0) && (v >= 0) && (u + v < 1);
        }
        void DrawOutline(int StartIndex)
        {
            int k = StartIndex;
            for (int i = 0; i < _polygonPoints.Count; i++)
            {
                Vector3 p0 = new Vector3(_polygonPoints[i].X, _polygonPoints[i].Y, 0);
                // p0.Y = -p0.Y;
                Vector3 p1;
                if (i < _polygonPoints.Count - 1)
                    p1 = new Vector3(_polygonPoints[i + 1].X, _polygonPoints[i + 1].Y, 0);
                else
                    p1 = new Vector3(_polygonPoints[0].X, _polygonPoints[0].Y, 0);
                // p1.Y = -p1.Y;

                Vector3 lineVector = p1 - p0;
                Vector3 widthVector = lineVector.Cross(new Vector3(0, 0, 1));
                widthVector = widthVector.Normalized();

                ToolSet.MeshAddVertex(_surfaceTool, p0 - widthVector * _lineWidth * 0.5f, _lineColor);
                ToolSet.MeshAddVertex(_surfaceTool, p0 + widthVector * _lineWidth * 0.5f, _lineColor);
                ToolSet.MeshAddVertex(_surfaceTool, p1 - widthVector * _lineWidth * 0.5f, _lineColor);
                ToolSet.MeshAddVertex(_surfaceTool, p1 + widthVector * _lineWidth * 0.5f, _lineColor);

                k += 4;
                ToolSet.MeshAddTriangleIndecies(_surfaceTool, k - 4, k - 3, k - 1);
                ToolSet.MeshAddTriangleIndecies(_surfaceTool, k - 4, k - 1, k - 2);

                //joint
                if (i != 0)
                {
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, k - 6, k - 5, k - 3);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, k - 6, k - 3, k - 4);
                }
                if (i == _polygonPoints.Count - 1)
                {
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, k - 2, k - 1, StartIndex + 1);
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, k - 2, StartIndex + 1, StartIndex);
                }
            }
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

            Rect rect = new Rect(Vector2.Zero, Size);

            float angleDelta = 2 * Mathf.Pi / _polygonSides;
            float angle = Mathf.DegToRad(_polygonRotation);
            float radius = Mathf.Min(rect.width / 2, rect.height / 2);

            float centerX = radius + rect.X;
            float centerY = radius + rect.Y;
            ToolSet.MeshAddVertex(_surfaceTool, centerX, centerY, _centerColor == null ? _fillColor : (Color)_centerColor);
            for (int i = 0; i < _polygonSides; i++)
            {
                float r = radius;
                if (_polygonDistances != null)
                    r *= _polygonDistances[i];
                float xv = Mathf.Cos(angle) * (r - _lineWidth);
                float yv = Mathf.Sin(angle) * (r - _lineWidth);
                Vector3 vec = new Vector3(xv + centerX, yv + centerY, 0);
                ToolSet.MeshAddVertex(_surfaceTool, vec, _fillColor);
                if (_lineWidth > 0)
                {
                    ToolSet.MeshAddVertex(_surfaceTool, vec, _lineColor);

                    xv = Mathf.Cos(angle) * r + centerX;
                    yv = Mathf.Sin(angle) * r + centerY;
                    ToolSet.MeshAddVertex(_surfaceTool, xv, yv, _lineColor);
                }
                angle += angleDelta;
            }

            if (_lineWidth > 0)
            {
                int tmp = _polygonSides * 3;
                for (int i = 0; i < tmp; i += 3)
                {
                    if (i != tmp - 3)
                    {
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, i + 1, i + 4);
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, i + 5, i + 2, i + 3);
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, i + 3, i + 6, i + 5);
                    }
                    else
                    {
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, i + 1, 1);
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, 2, i + 2, i + 3);
                        ToolSet.MeshAddTriangleIndecies(_surfaceTool, i + 3, 3, 2);
                    }
                }
            }
            else
            {
                for (int i = 0; i < _polygonSides; i++)
                    ToolSet.MeshAddTriangleIndecies(_surfaceTool, 0, i + 1, (i == _polygonSides - 1) ? 1 : i + 2);
            }

            _surfaceTool.GenerateNormals();
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
