using Godot;
using System;
using System.Collections.Generic;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public enum DestroyMethod
    {
        Destroy,
        Unload,
        None,
        ReleaseTemp,
        Custom
    }

    /// <summary>
    /// 
    /// </summary>
    public class NTexture
    {
        /// <summary>
        /// This event will trigger when a texture is destroying if its destroyMethod is Custom
        /// </summary>
        public static event Action<Texture2D> CustomDestroyMethod;

        /// <summary>
        /// 
        /// </summary>
        public Rect uvRect;

        /// <summary>
        /// 
        /// </summary>
        public bool rotated;

        /// <summary>
        /// 
        /// </summary>
        public int refCount;

        /// <summary>
        /// 
        /// </summary>
        public float lastActive;

        /// <summary>
        /// 
        /// </summary>
        public DestroyMethod destroyMethod;

        /// <summary>
        /// This event will trigger when texture reloaded and size changed.
        /// </summary>
        public event Action<NTexture> onSizeChanged;

        /// <summary>
        /// This event will trigger when ref count is zero.
        /// </summary>
        public event Action<NTexture> onRelease;

        Texture2D _nativeTexture;
        Texture2D _alphaTexture;

        Rect _region;
        Vector2 _offset;
        Vector2 _originalSize;

        NTexture _root;

        internal static Texture2D CreateEmptyTexture()
        {
            Image img = Image.CreateEmpty(1, 1, false, Image.Format.Rgb8);
            img.Fill(new Color(1f, 1f, 1f));
            ImageTexture tex = ImageTexture.CreateFromImage(img);
            tex.ResourceName = "White Texture";
            return tex;
        }

        static NTexture _empty;

        /// <summary>
        /// 
        /// </summary>
        public static NTexture Empty
        {
            get
            {
                if (_empty == null)
                    _empty = new NTexture(CreateEmptyTexture());

                return _empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void DisposeEmpty()
        {
            if (_empty != null)
            {
                NTexture tmp = _empty;
                _empty = null;
                tmp.Dispose();
            }
        }

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitializeOnLoad()
        {
            DisposeEmpty();
            CustomDestroyMethod = null;
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        public NTexture(Texture2D texture) : this(texture, null, 1, 1)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        public NTexture(Texture2D texture, Texture2D alphaTexture, float xScale, float yScale)
        {
            _root = this;
            _nativeTexture = texture;
            _alphaTexture = alphaTexture;
            uvRect = new Rect(0, 0, xScale, yScale);
            if (yScale < 0)
            {
                uvRect.Y = -yScale;
                uvRect.yMax = 0;
            }
            if (xScale < 0)
            {
                uvRect.X = -xScale;
                uvRect.xMax = 0;
            }
            if (_nativeTexture != null)
                _originalSize = new Vector2(_nativeTexture.GetWidth(), _nativeTexture.GetHeight());
            _region = new Rect(0, 0, _originalSize.X, _originalSize.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="region"></param>
        public NTexture(Texture2D texture, Rect region)
        {
            _root = this;
            _nativeTexture = texture;
            _region = region;
            _originalSize = new Vector2(_region.width, _region.height);
            if (_nativeTexture != null)
                uvRect = new Rect(region.X / _nativeTexture.GetWidth(), region.Y / _nativeTexture.GetHeight(),
                    region.width / _nativeTexture.GetWidth(), region.height / _nativeTexture.GetHeight());
            else
                uvRect = new Rect(0, 0, 1, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="region"></param>
        /// <param name="rotated"></param>
        public NTexture(NTexture root, Rect region, bool rotated)
        {
            _root = root;
            this.rotated = rotated;
            region.X += root._region.X;
            region.Y += root._region.Y;
            uvRect = new Rect(region.X * root.uvRect.width / root.width, region.Y * root.uvRect.height / root.height,
                region.width * root.uvRect.width / root.width, region.height * root.uvRect.height / root.height);
            if (rotated)
            {
                float tmp = region.width;
                region.width = region.height;
                region.height = tmp;

                tmp = uvRect.width;
                uvRect.width = uvRect.height;
                uvRect.height = tmp;
            }
            _region = region;
            _originalSize = _region.size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="region"></param>
        /// <param name="rotated"></param>
        /// <param name="originalSize"></param>
        /// <param name="offset"></param>
        public NTexture(NTexture root, Rect region, bool rotated, Vector2 originalSize, Vector2 offset)
            : this(root, region, rotated)
        {
            _originalSize = originalSize;
            _offset = offset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprite"></param>
        // public NTexture(Sprite sprite)
        // {
        //     Rect rect = sprite.textureRect;
        //     rect.Y = sprite.texture.height - rect.yMax;

        //     _root = this;
        //     _nativeTexture = sprite.texture;
        //     _region = rect;
        //     _originalSize = new Vector2(_region.width, _region.height);
        //     uvRect = new Rect(_region.X / _nativeTexture.width, 1 - _region.yMax / _nativeTexture.height,
        //         _region.width / _nativeTexture.width, _region.height / _nativeTexture.height);
        // }

        public Rect region
        {
            get { return _region; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int width
        {
            get { return (int)_region.width; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int height
        {
            get { return (int)_region.height; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 originalSize
        {
            get { return _originalSize; }
            set { _originalSize = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawRect"></param>
        /// <returns></returns>
        public Rect GetDrawRect(Rect drawRect)
        {
            return GetDrawRect(drawRect, FlipType.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawRect"></param>
        /// <param name="flip"></param>
        /// <returns></returns>
        public Rect GetDrawRect(Rect drawRect, FlipType flip)
        {
            if (_originalSize.X == _region.width && _originalSize.Y == _region.height)
                return drawRect;

            float sx = drawRect.width / _originalSize.X;
            float sy = drawRect.height / _originalSize.Y;
            Rect rect = new Rect(_offset.X * sx, _offset.Y * sy, _region.width * sx, _region.height * sy);

            if (flip != FlipType.None)
            {
                if (flip == FlipType.Horizontal || flip == FlipType.Both)
                {
                    rect.X = drawRect.width - rect.xMax;
                }
                if (flip == FlipType.Vertical || flip == FlipType.Both)
                {
                    rect.Y = drawRect.height - rect.yMax;
                }
            }

            return rect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uv"></param>
        public void GetUV(Vector2[] uv)
        {
            uv[0] = uvRect.position;
            uv[1] = new Vector2(uvRect.xMin, uvRect.yMax);
            uv[2] = new Vector2(uvRect.xMax, uvRect.yMax);
            uv[3] = new Vector2(uvRect.xMax, uvRect.yMin);
            if (rotated)
            {
                float xMin = uvRect.xMin;
                float yMin = uvRect.yMin;
                float yMax = uvRect.yMax;

                float tmp;
                for (int i = 0; i < 4; i++)
                {
                    Vector2 m = uv[i];
                    tmp = m.Y;
                    m.Y = yMin + m.X - xMin;
                    m.X = xMin + yMax - tmp;
                    uv[i] = m;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public NTexture root
        {
            get { return _root; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool disposed
        {
            get { return _root == null; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Texture2D nativeTexture
        {
            get { return _root != null ? _root._nativeTexture : null; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Texture2D alphaTexture
        {
            get { return _root != null ? _root._alphaTexture : null; }
        }

        /// <summary>
        /// 
        /// </summary>


        /// <summary>
        /// 
        /// </summary>
        public void Unload()
        {
            Unload(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Unload(bool destroyMaterials)
        {
            if (this == _empty)
                return;

            if (_root != this)
                throw new Exception("Unload is not allow to call on none root NTexture.");

            if (_nativeTexture != null)
            {
                DestroyTexture();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nativeTexture"></param>
        /// <param name="alphaTexture"></param>
        public void Reload(Texture2D nativeTexture, Texture2D alphaTexture)
        {
            if (_root != this)
                throw new System.Exception("Reload is not allow to call on none root NTexture.");

            if (_nativeTexture != null && _nativeTexture != nativeTexture)
                DestroyTexture();

            _nativeTexture = nativeTexture;
            _alphaTexture = alphaTexture;

            Vector2 lastSize = _originalSize;
            if (_nativeTexture != null)
                _originalSize = new Vector2(_nativeTexture.GetWidth(), _nativeTexture.GetHeight());
            else
                _originalSize = Vector2.Zero;
            _region = new Rect(0, 0, _originalSize.X, _originalSize.Y);


            if (onSizeChanged != null && lastSize != _originalSize)
                onSizeChanged(this);
        }

        void DestroyTexture()
        {
            switch (destroyMethod)
            {
                case DestroyMethod.Destroy:
                    _nativeTexture.Free();
                    _nativeTexture = null;
                    if (_alphaTexture != null)
                        _alphaTexture.Free();
                    _alphaTexture = null;
                    break;
                case DestroyMethod.Unload:
                    _nativeTexture = null;
                    _alphaTexture = null;
                    break;
                case DestroyMethod.ReleaseTemp:
                    _nativeTexture = null;
                    _alphaTexture = null;
                    break;
                case DestroyMethod.Custom:
                    if (CustomDestroyMethod == null)
                        GD.Print("NTexture.CustomDestroyMethod must be set to handle DestroyMethod.Custom");
                    else
                    {
                        CustomDestroyMethod(_nativeTexture);
                        if (_alphaTexture != null)
                            CustomDestroyMethod(_alphaTexture);
                    }
                    break;
            }

            _nativeTexture = null;
            _alphaTexture = null;
        }



        public void AddRef()
        {
            if (_root == null) //disposed
                return;

            if (_root != this && refCount == 0)
                _root.AddRef();

            refCount++;
        }

        public void ReleaseRef()
        {
            if (_root == null) //disposed
                return;

            refCount--;

            if (refCount == 0)
            {
                if (_root != this)
                    _root.ReleaseRef();

                if (onRelease != null)
                    onRelease(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (this == _empty)
                return;

            if (_root == this)
                Unload(true);
            _root = null;
            onSizeChanged = null;
            onRelease = null;
        }
    }
}
