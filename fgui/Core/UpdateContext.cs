using System;
using System.Collections.Generic;
using Godot;
using FairyGUI.Utils;

namespace FairyGUI
{
    /// <summary>
    /// UpdateContext is for internal use.
    /// </summary>
    public class UpdateContext
    {
        public struct ClipInfo
        {
            public Rect rect;
            public Vector4 clipBox;
            public bool soft;
            public Vector4 softness;//left-top-right-bottom
            public uint clipId;
            public int rectMaskDepth;
            public int referenceValue;
            public bool reversed;
        }

        Stack<ClipInfo> _clipStack;

        public bool clipped;
        public ClipInfo clipInfo;

        public int renderingOrder;
        public int batchingDepth;
        public int rectMaskDepth;
        public int stencilReferenceValue;
        public int stencilCompareValue;

        public float alpha;
        public bool grayed;

        public static UpdateContext current;
        public static bool working;

        public static event Action OnBegin;
        public static event Action OnEnd;

        static Action _tmpBegin;

        public UpdateContext()
        {
            _clipStack = new Stack<ClipInfo>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Begin()
        {
            current = this;

            renderingOrder = 0;
            batchingDepth = 0;
            rectMaskDepth = 0;
            stencilReferenceValue = 0;
            alpha = 1;
            grayed = false;

            clipped = false;
            _clipStack.Clear();

            Stats.ObjectCount = 0;
            Stats.GraphicsCount = 0;

            _tmpBegin = OnBegin;
            OnBegin = null;

            //允许OnBegin里再次Add，这里没有做死锁检查
            while (_tmpBegin != null)
            {
                _tmpBegin.Invoke();
                _tmpBegin = OnBegin;
                OnBegin = null;
            }

            working = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void End()
        {
            working = false;

            if (OnEnd != null)
                OnEnd.Invoke();

            OnEnd = null;
        }
    }
}
