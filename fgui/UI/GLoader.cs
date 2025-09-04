using System;
using Godot;
using FairyGUI.Utils;
using System.Threading.Tasks;

namespace FairyGUI
{
    /// <summary>
    /// GLoader class
    /// </summary>
    public class GLoader : GObject, IAnimationGear, IColorGear
    {
        /// <summary>
        /// Display an error sign if the loader fails to load the content.
        /// UIConfig.loaderErrorSign muse be set.
        /// </summary>
        public bool showErrorSign;

        string _url;
        AlignType _align;
        VertAlignType _verticalAlign;
        bool _autoSize;
        FillType _fill;
        bool _shrinkOnly;
        bool _useResize;
        bool _updatingLayout;
        PackageItem _contentItem;
        Action<NTexture> _reloadDelegate;

        MovieClip _content;
        GObject _errorSign;
        GComponent _content2;





        public GLoader()
        {
            _url = string.Empty;
            _align = AlignType.Left;
            _verticalAlign = VertAlignType.Top;
            showErrorSign = true;
            _reloadDelegate = OnExternalReload;
        }

        override protected void CreateDisplayObject()
        {
            _content = new MovieClip(this);
            displayObject = AddParentContainer(_content);
        }

        override public void Dispose()
        {
            if (_disposed) return;

            if (_content.texture != null)
            {
                if (_contentItem == null)
                {
                    _content.texture.onSizeChanged -= _reloadDelegate;
                    try
                    {
                        FreeExternal(_content.texture);
                    }
                    catch (Exception err)
                    {
                        GD.PushWarning(err);
                    }
                }
            }
            if (_errorSign != null)
                _errorSign.Dispose();
            if (_content2 != null)
                _content2.Dispose();
            _content.Dispose();
            (displayObject as NContainer)?.Dispose();
            displayObject = null;

            base.Dispose();
        }

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
        public bool useResize
        {
            get { return _useResize; }
            set
            {
                if (_useResize != value)
                {
                    _useResize = value;
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

        /// <summary>
        /// 
        /// </summary>
        public bool playing
        {
            get { return _content.playing; }
            set
            {
                _content.playing = value;
                UpdateGear(5);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int frame
        {
            get { return _content.frame; }
            set
            {
                _content.frame = value;
                UpdateGear(5);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float timeScale
        {
            get { return _content.timeScale; }
            set { _content.timeScale = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ignoreEngineTimeScale
        {
            get { return _content.ignoreEngineTimeScale; }
            set { _content.ignoreEngineTimeScale = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        public void Advance(float time)
        {
            _content.Advance(time);
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
            get { return _content.color; }
            set
            {
                if (_content.color != value)
                {
                    _content.color = value;
                    UpdateGear(4);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FillMethod fillMethod
        {
            get { return _content.fillMethod; }
            set { _content.fillMethod = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int fillOrigin
        {
            get { return _content.fillOrigin; }
            set { _content.fillOrigin = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool fillClockwise
        {
            get { return _content.fillClockwise; }
            set { _content.fillClockwise = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float fillAmount
        {
            get { return _content.fillAmount; }
            set { _content.fillAmount = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public NImage image
        {
            get { return _content; }
        }

        /// <summary>
        /// 
        /// </summary>
        public MovieClip movieClip
        {
            get { return _content; }
        }

        /// <summary>
        /// 
        /// </summary>
        public GComponent component
        {
            get { return _content2; }
        }

        /// <summary>
        /// 
        /// </summary>
        public NTexture texture
        {
            get
            {
                return _content.texture;
            }

            set
            {
                this.url = null;

                _content.texture = value;
                if (value != null)
                {
                    sourceWidth = value.width;
                    sourceHeight = value.height;
                }
                else
                {
                    sourceWidth = sourceHeight = 0;
                }

                UpdateLayout();
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

            if (_url.StartsWith(UIPackage.URL_PREFIX))
                LoadFromPackage(_url);
            else if (_url.StartsWith(UIPackage.RES_PREFIX))
                LoadFromResource(_url);
            else
                LoadExternal();
        }

        protected void LoadFromPackage(string itemURL)
        {
            _contentItem = UIPackage.GetItemByURL(itemURL);

            if (_contentItem != null)
            {
                _contentItem = _contentItem.getBranch();
                sourceWidth = _contentItem.width;
                sourceHeight = _contentItem.height;
                _contentItem = _contentItem.getHighResolution();
                _contentItem.Load();

                if (_contentItem.type == PackageItemType.Image)
                {
                    _content.texture = _contentItem.texture;
                    _content.textureScale = new Vector2(_contentItem.width / (float)sourceWidth, _contentItem.height / (float)sourceHeight);
                    _content.scale9Grid = _contentItem.scale9Grid;
                    _content.scaleByTile = _contentItem.scaleByTile;
                    _content.tileGridIndice = _contentItem.tileGridIndice;

                    UpdateLayout();
                }
                else if (_contentItem.type == PackageItemType.MovieClip)
                {
                    _content.interval = _contentItem.interval;
                    _content.swing = _contentItem.swing;
                    _content.repeatDelay = _contentItem.repeatDelay;
                    _content.frames = _contentItem.frames;

                    UpdateLayout();
                }
                else if (_contentItem.type == PackageItemType.Component)
                {
                    GObject obj = UIPackage.CreateObjectFromURL(itemURL);
                    if (obj == null)
                        SetErrorState();
                    else if (!(obj is GComponent))
                    {
                        obj.Dispose();
                        SetErrorState();
                    }
                    else
                    {
                        _content2 = (GComponent)obj;
                        displayObject.node.AddChild(_content2.displayObject.node);
                        UpdateLayout();
                    }
                }
                else
                {
                    if (_autoSize)
                        this.SetSize(_contentItem.width, _contentItem.height);

                    SetErrorState();

                    GD.PushWarning("Unsupported type of GLoader: " + _contentItem.type);
                }
            }
            else
                SetErrorState();
        }

        protected void LoadFromResource(string itemResPath)
        {
            Texture2D tex = ResourceLoader.Load<Texture2D>(itemResPath);
            if (tex != null)
            {
                var ntex = new NTexture(tex);
                _content.texture = ntex;
                sourceWidth = ntex.width;
                sourceHeight = ntex.height;
                _content.scale9Grid = null;
                _content.scaleByTile = false;
                ntex.onSizeChanged += _reloadDelegate;
                UpdateLayout();
            }
        }

        virtual protected void LoadExternal()
        {
            LoadByHTTP().ContinueWith(task =>
                        {
                            if (task.Status == TaskStatus.Faulted)
                            {
                                GD.PushError($"An error occurred in LoadExternal: {task.Exception?.InnerException?.Message}");
                                onExternalLoadFailed();
                            }
                        }, TaskContinuationOptions.OnlyOnFaulted);
        }
        protected async Task LoadByHTTP()
        {
            if (displayObject == null || displayObject.node == null)
            {
                onExternalLoadFailed();
                return;
            }
            using var client = new HttpClient();
            try
            {
                var uri = new Uri(url);
                // 连接到服务器
                Error connectError = client.ConnectToHost(uri.Host);
                if (connectError != Error.Ok)
                {
                    GD.PrintErr("连接失败: " + connectError);
                    onExternalLoadFailed();
                }

                // 等待连接完成
                while (client.GetStatus() == HttpClient.Status.Connecting)
                {
                    await displayObject.node.ToSignal(displayObject.node.GetTree(), SceneTree.SignalName.ProcessFrame);
                }

                if (client.GetStatus() != HttpClient.Status.Connected)
                {
                    GD.PrintErr("连接失败，状态: " + client.GetStatus());
                    onExternalLoadFailed();
                }

                // 发送请求
                string path = uri.PathAndQuery;
                string[] headers = [$"Content-Type: application/x-www-form-urlencoded"];
                Error requestError = client.Request(HttpClient.Method.Get, path, headers);
                if (requestError != Error.Ok)
                {
                    GD.PrintErr("请求失败: " + requestError);
                    onExternalLoadFailed();
                }

                // 等待响应
                while (client.GetStatus() == HttpClient.Status.Requesting)
                {
                    await displayObject.node.ToSignal(displayObject.node.GetTree(), SceneTree.SignalName.ProcessFrame);
                }

                // 读取响应体
                byte[] body = client.ReadResponseBodyChunk();
                if (body.Length == 0)
                {
                    GD.PrintErr("响应体为空");
                    onExternalLoadFailed();
                }

                // 创建图片
                Image image = new Image();
                Error loadError = image.LoadPngFromBuffer(body);

                if (loadError != Error.Ok)
                {
                    loadError = image.LoadJpgFromBuffer(body);
                }

                if (loadError == Error.Ok)
                {
                    onExternalLoadSuccess(new NTexture(ImageTexture.CreateFromImage(image)));
                }
                else
                {
                    GD.PrintErr("图片加载失败: " + loadError);
                    onExternalLoadFailed();
                }
            }
            finally
            {
                client.Close();
            }
        }

        virtual protected void FreeExternal(NTexture texture)
        {

        }

        public void onExternalLoadSuccess(NTexture texture)
        {
            _content.texture = texture;
            sourceWidth = texture.width;
            sourceHeight = texture.height;
            _content.scale9Grid = null;
            _content.scaleByTile = false;
            texture.onSizeChanged += _reloadDelegate;
            UpdateLayout();
        }

        public void onExternalLoadFailed()
        {
            SetErrorState();
        }

        void OnExternalReload(NTexture texture)
        {
            sourceWidth = texture.width;
            sourceHeight = texture.height;
            UpdateLayout();
        }

        private void SetErrorState()
        {
            if (!showErrorSign)
                return;

            if (_errorSign == null)
            {
                if (UIConfig.loaderErrorSign != null)
                    _errorSign = UIPackage.CreateObjectFromURL(UIConfig.loaderErrorSign);
                else
                    return;
            }

            if (_errorSign != null)
            {
                _errorSign.SetSize(this.width, this.height);
                displayObject.node.AddChild(_errorSign.displayObject.node);
            }
        }

        protected void ClearErrorState()
        {
            if (_errorSign != null && _errorSign.displayObject.parent != null)
                displayObject.node.RemoveChild(_errorSign.displayObject.node);
        }

        protected void UpdateLayout()
        {
            if (_content2 == null && _content.texture == null && _content.frames == null)
            {
                if (_autoSize)
                {
                    _updatingLayout = true;
                    SetSize(50, 30);
                    _updatingLayout = false;
                }
                return;
            }

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
                    if (_content2 != null)
                    {
                        _content2.SetXY(0, 0);
                        _content2.SetScale(1, 1);
                        if (_useResize)
                            _content2.SetSize(contentWidth, contentHeight, true);
                    }
                    else
                    {
                        _content.Position = Vector2.Zero;
                        _content.Size = new Vector2(contentWidth, contentHeight);
                    }

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

            if (_content2 != null)
            {
                if (_useResize)
                {
                    _content2.SetScale(1, 1);
                    _content2.SetSize(contentWidth, contentHeight, true);
                }
                else
                    _content2.SetScale(sx, sy);
            }
            else
                _content.Size = new Vector2(contentWidth, contentHeight);

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
            if (_content2 != null)
                _content2.SetXY(nx, ny);
            else
                _content.Position = new Vector2(nx, ny);

        }

        private void ClearContent()
        {
            ClearErrorState();

            if (_content.texture != null)
            {
                if (_contentItem == null)
                {
                    _content.texture.onSizeChanged -= _reloadDelegate;
                    FreeExternal(_content.texture);
                }
                _content.texture = null;
            }
            _content.frames = null;

            if (_content2 != null)
            {
                _content2.Dispose();
                _content2 = null;
            }
            _contentItem = null;
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
            showErrorSign = buffer.ReadBool();
            _content.playing = buffer.ReadBool();
            _content.frame = buffer.ReadInt();

            if (buffer.ReadBool())
                _content.color = buffer.ReadColor();
            _content.fillMethod = (FillMethod)buffer.ReadByte();
            if (_content.fillMethod != FillMethod.None)
            {
                _content.fillOrigin = buffer.ReadByte();
                _content.fillClockwise = buffer.ReadBool();
                _content.fillAmount = buffer.ReadFloat();
            }
            if (buffer.version >= 7)
                _useResize = buffer.ReadBool();

            if (!string.IsNullOrEmpty(_url))
                LoadContent();
        }
    }
}
