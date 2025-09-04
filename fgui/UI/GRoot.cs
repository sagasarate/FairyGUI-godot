using System;
using System.Collections.Generic;
using Godot;

namespace FairyGUI
{
    /// <summary>
    /// GRoot is a topmost component of UI display list.You dont need to create GRoot. It is created automatically.
    /// </summary>
    public class GRoot : GComponent
    {
        GGraph _modalLayer;
        GObject _modalWaitPane;
        List<GObject> _popupStack;
        List<GObject> _justClosedPopups;
        HashSet<GObject> _specialPopups;
        GObject _tooltipWin;
        GObject _defaultTooltipWin;

        EventListener _onStageResized;        

        internal static GRoot _inst;
        public static GRoot inst
        {
            get
            {
                if (_inst == null)
                    Stage.Instantiate();

                return _inst;
            }
        }

        public static float contentScaleFactor
        {
            get { return Stage.contentScaleFactor; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static int contentScaleLevel
        {
            get { return Stage.contentScaleLevel; }
        }


        public GRoot()
        {
            this.opaque = true;

            _popupStack = new List<GObject>();
            _justClosedPopups = new List<GObject>();
            _specialPopups = new HashSet<GObject>();

            // 设置锚点铺满整个父容器
            container.SetDeferred(Control.PropertyName.AnchorLeft, 0.0f);
            container.SetDeferred(Control.PropertyName.AnchorTop, 0.0f);
            container.SetDeferred(Control.PropertyName.AnchorRight, 1.0f);
            container.SetDeferred(Control.PropertyName.AnchorBottom, 1.0f);

            container.Resized += OnContainerSizeChanged;

            OnContainerSizeChanged();

            onTouchBegin.AddCapture(__stageTouchBegin);
            onTouchEnd.AddCapture(__stageTouchEnd);

        }


        void OnContainerSizeChanged()
        {
            SetSize(container.Size.X, container.Size.Y, false, true);
        }

        override public void Dispose()
        {
            onTouchBegin.RemoveCapture(__stageTouchBegin);
            onTouchEnd.RemoveCapture(__stageTouchEnd);

            if (_modalLayer != null)
                _modalLayer.Dispose();
            if (_modalWaitPane != null)
                _modalWaitPane.Dispose();
            if (_defaultTooltipWin != null)
                _defaultTooltipWin.Dispose();

            base.Dispose();
        }
        public static void Release()
        {
            if (_inst != null)
            {
                _inst.Dispose();
                _inst = null;
            }
        }

        public EventListener onStageResized
        {
            get { return _onStageResized ?? (_onStageResized = new EventListener(this, "onStageResized")); }
        }



        /// <summary>
        /// Display a window.
        /// </summary>
        /// <param name="win"></param>
        public void ShowWindow(Window win)
        {
            AddChild(win);
            AdjustModalLayer();
        }

        /// <summary>
        /// Call window.Hide
        /// 关闭一个窗口。将调用Window.Hide方法。
        /// </summary>
        /// <param name="win"></param>
        public void HideWindow(Window win)
        {
            win.Hide();
        }

        /// <summary>
        /// Remove a window from stage immediatelly. window.Hide/window.OnHide will never be called.
        ///立刻关闭一个窗口。不会调用Window.Hide方法，Window.OnHide也不会被调用。
        /// </summary>
        /// <param name="win"></param>
        public void HideWindowImmediately(Window win)
        {
            HideWindowImmediately(win, false);
        }

        /// <summary>
        /// Remove a window from stage immediatelly. window.Hide/window.OnHide will never be called.
        /// 立刻关闭一个窗口。不会调用Window.Hide方法，Window.OnHide也不会被调用。
        /// </summary>
        /// <param name="win"></param>
        /// <param name="dispose">True to dispose the window.</param>
        public void HideWindowImmediately(Window win, bool dispose)
        {
            if (win.parent == this)
                RemoveChild(win, dispose);
            else if (dispose)
                win.Dispose();

            AdjustModalLayer();
        }

        /// <summary>
        /// 将一个窗口提到所有窗口的最前面
        /// </summary>
        /// <param name="win"></param>
        public void BringToFront(Window win)
        {
            int cnt = this.numChildren;
            int i;
            if (_modalLayer != null && _modalLayer.parent != null && !win.modal)
                i = GetChildIndex(_modalLayer) - 1;
            else
                i = cnt - 1;

            for (; i >= 0; i--)
            {
                GObject g = GetChildAt(i);
                if (g == win)
                    return;
                if (g is Window)
                    break;
            }

            if (i >= 0)
                SetChildIndex(win, i);
        }

        /// <summary>
        /// Display a modal layer and a waiting sign in the front.
        /// 显示一个半透明层和一个等待标志在最前面。半透明层的颜色可以通过UIConfig.modalLayerColor设定。
        /// 等待标志的资源可以通过UIConfig.globalModalWaiting。等待标志组件会设置为屏幕大小，请内部做好关联。
        /// </summary>
        public void ShowModalWait()
        {
            if (UIConfig.globalModalWaiting != null)
            {
                if (_modalWaitPane == null || _modalWaitPane.isDisposed)
                {
                    _modalWaitPane = UIPackage.CreateObjectFromURL(UIConfig.globalModalWaiting);
                }
                _modalWaitPane.SetSize(this.width, this.height);
                _modalWaitPane.AddRelation(this, RelationType.Size);

                AddChild(_modalWaitPane);
            }
        }

        /// <summary>
        /// Hide modal layer and waiting sign.
        /// </summary>
        public void CloseModalWait()
        {
            if (_modalWaitPane != null && _modalWaitPane.parent != null)
                RemoveChild(_modalWaitPane);
        }

        /// <summary>
        /// Close all windows except modal windows.
        /// </summary>
        public void CloseAllExceptModals()
        {
            GObject[] arr = _children.ToArray();
            foreach (GObject g in arr)
            {
                if ((g is Window) && !(g as Window).modal)
                    HideWindowImmediately(g as Window);
            }
        }

        /// <summary>
        /// Close all windows.
        /// </summary>
        public void CloseAllWindows()
        {
            GObject[] arr = _children.ToArray();
            foreach (GObject g in arr)
            {
                if (g is Window)
                    HideWindowImmediately(g as Window);
            }
        }

        /// <summary>
        /// Get window on top.
        /// </summary>
        /// <returns></returns>
        public Window GetTopWindow()
        {
            int cnt = this.numChildren;
            for (int i = cnt - 1; i >= 0; i--)
            {
                GObject g = this.GetChildAt(i);
                if (g is Window)
                {
                    return (Window)(g);
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public GGraph modalLayer
        {
            get
            {
                if (_modalLayer == null || _modalLayer.isDisposed)
                    CreateModalLayer();

                return _modalLayer;
            }
        }

        void CreateModalLayer()
        {
            _modalLayer = new GGraph();
            _modalLayer.DrawRect(this.width, this.height, 0, Colors.White, UIConfig.modalLayerColor);
            _modalLayer.AddRelation(this, RelationType.Size);
            _modalLayer.name = _modalLayer.name = "ModalLayer";
        }

        /// <summary>
        /// Return true if a modal window is on stage.
        /// </summary>
        public bool hasModalWindow
        {
            get { return _modalLayer != null && _modalLayer.parent != null; }
        }

        /// <summary>
        /// Return true if modal waiting layer is on stage.
        /// </summary>
        public bool modalWaiting
        {
            get
            {
                return (_modalWaitPane != null) && _modalWaitPane.onStage;
            }
        }

        /// <summary>
        /// Get current touch target. (including hover)
        /// </summary>
        public GObject touchTarget
        {
            get
            {
                return Stage.inst.touchTarget;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public GObject DisplayObjectToGObject(IDisplayObject obj)
        {
            while (obj != null)
            {
                if (obj.gOwner != null)
                    return obj.gOwner;

                obj = obj.parent;
            }
            return null;
        }

        private void AdjustModalLayer()
        {
            if (_modalLayer == null || _modalLayer.isDisposed)
                CreateModalLayer();

            int cnt = this.numChildren;

            if (_modalWaitPane != null && _modalWaitPane.parent != null)
                SetChildIndex(_modalWaitPane, cnt - 1);

            for (int i = cnt - 1; i >= 0; i--)
            {
                GObject g = this.GetChildAt(i);
                if ((g is Window) && (g as Window).modal)
                {
                    if (_modalLayer.parent == null)
                        AddChildAt(_modalLayer, i);
                    else
                        SetChildIndexBefore(_modalLayer, i);
                    return;
                }
            }

            if (_modalLayer.parent != null)
                RemoveChild(_modalLayer);
        }

        /// <summary>
        /// Show a  popup object.
        /// 显示一个popup。
        /// popup的特点是点击popup对象外的区域，popup对象将自动消失。
        /// </summary>
        /// <param name="popup"></param>
        public void ShowPopup(GObject popup)
        {
            ShowPopup(popup, null, PopupDirection.Auto, false);
        }

        /// <summary>
        /// Show a popup object along with the specific target object.
        /// 显示一个popup。将popup显示在指定对象的上边或者下边。
        /// popup的特点是点击popup对象外的区域，popup对象将自动消失。
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="target"></param>
        public void ShowPopup(GObject popup, GObject target)
        {
            ShowPopup(popup, target, PopupDirection.Auto, false);
        }

        [Obsolete]
        public void ShowPopup(GObject popup, GObject target, object downward)
        {
            ShowPopup(popup, target,
                downward == null ? PopupDirection.Auto : ((bool)downward == true ? PopupDirection.Down : PopupDirection.Up),
                false);
        }

        /// <summary>
        /// Show a popup object along with the specific target object.
        /// 显示一个popup。将popup显示在指定对象的上方或者下方。
        /// popup的特点是点击popup对象外的区域，popup对象将自动消失。
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="target"></param>
        /// <param name="dir"></param>
        public void ShowPopup(GObject popup, GObject target, PopupDirection dir)
        {
            ShowPopup(popup, target, dir, false);
        }

        /// <summary>
        /// Show a popup object along with the specific target object.
        /// 显示一个popup。将popup显示在指定对象的上方或者下方。
        /// popup的特点是点击popup对象外的区域，popup对象将自动消失。
        /// 默认情况下，popup在touchEnd事件中关闭；特别设置closeUntilUpEvent=true则可使该popup在touchEnd中才关闭。
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="target"></param>
        /// <param name="dir"></param>
        /// <param name="closeUntilUpEvent"></param>
        public void ShowPopup(GObject popup, GObject target, PopupDirection dir, bool closeUntilUpEvent)
        {
            if (_popupStack.Count > 0)
            {
                int k = _popupStack.IndexOf(popup);
                if (k != -1)
                {
                    for (int i = _popupStack.Count - 1; i >= k; i--)
                    {
                        int last = _popupStack.Count - 1;
                        GObject obj = _popupStack[last];
                        ClosePopup(obj);
                        _popupStack.RemoveAt(last);
                        _specialPopups.Remove(obj);
                    }
                }
            }
            _popupStack.Add(popup);
            if (closeUntilUpEvent)
                _specialPopups.Add(popup);

            if (target != null)
            {
                GObject p = target;
                while (p != null)
                {
                    if (p.parent == this)
                    {
                        if (popup.sortingOrder < p.sortingOrder)
                        {
                            popup.sortingOrder = p.sortingOrder;
                        }
                        break;
                    }
                    p = p.parent;
                }
            }

            AddChild(popup);
            AdjustModalLayer();

            if ((popup is Window) && target == null && dir == PopupDirection.Auto)
                return;

            Vector2 pos = GetPoupPosition(popup, target, dir);
            popup.xy = pos;
        }

        [Obsolete]
        public Vector2 GetPoupPosition(GObject popup, GObject target, object downward)
        {
            return GetPoupPosition(popup, target,
                downward == null ? PopupDirection.Auto : ((bool)downward == true ? PopupDirection.Down : PopupDirection.Up));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="target"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public Vector2 GetPoupPosition(GObject popup, GObject target, PopupDirection dir)
        {
            Vector2 pos;
            Vector2 size = Vector2.Zero;
            if (target != null)
            {
                pos = target.LocalToRoot(Vector2.Zero);
                size = target.LocalToRoot(target.size) - pos;
            }
            else
            {
                pos = this.GlobalToLocal(Stage.inst.touchPosition);
            }
            float xx, yy;
            xx = pos.X;
            if (xx + popup.width > this.width)
                xx = xx + size.X - popup.width;
            yy = pos.Y + size.Y;
            if ((dir == PopupDirection.Auto && yy + popup.height > this.height)
                || dir == PopupDirection.Up)
            {
                yy = pos.Y - popup.height - 1;
                if (yy < 0)
                {
                    yy = 0;
                    xx += size.X / 2;
                }
            }

            return new Vector2(Mathf.RoundToInt(xx), Mathf.RoundToInt(yy));
        }

        /// <summary>
        /// If a popup is showing, then close it; otherwise, open it.
        /// </summary>
        /// <param name="popup"></param>
        public void TogglePopup(GObject popup)
        {
            TogglePopup(popup, null, PopupDirection.Auto, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="target"></param>
        public void TogglePopup(GObject popup, GObject target)
        {
            TogglePopup(popup, target, PopupDirection.Auto, false);
        }

        [Obsolete]
        public void TogglePopup(GObject popup, GObject target, object downward)
        {
            TogglePopup(popup, target,
                downward == null ? PopupDirection.Auto : ((bool)downward == true ? PopupDirection.Down : PopupDirection.Up),
                false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="target"></param>
        /// <param name="dir"></param>
        public void TogglePopup(GObject popup, GObject target, PopupDirection dir)
        {
            TogglePopup(popup, target, dir, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="target"></param>
        /// <param name="dir"></param>
        /// <param name="closeUntilUpEvent"></param>
        public void TogglePopup(GObject popup, GObject target, PopupDirection dir, bool closeUntilUpEvent)
        {
            if (_justClosedPopups.IndexOf(popup) != -1)
                return;

            ShowPopup(popup, target, dir, closeUntilUpEvent);
        }

        /// <summary>
        /// Close all popups.
        /// </summary>
        public void HidePopup()
        {
            HidePopup(null);
        }

        /// <summary>
        /// Close a popup.
        /// </summary>
        /// <param name="popup"></param>
        public void HidePopup(GObject popup)
        {
            if (popup != null)
            {
                int k = _popupStack.IndexOf(popup);
                if (k != -1)
                {
                    for (int i = _popupStack.Count - 1; i >= k; i--)
                    {
                        int last = _popupStack.Count - 1;
                        GObject obj = _popupStack[last];
                        ClosePopup(obj);
                        _popupStack.RemoveAt(last);
                        _specialPopups.Remove(obj);
                    }
                }
            }
            else
            {
                foreach (GObject obj in _popupStack)
                    ClosePopup(obj);
                _popupStack.Clear();
                _specialPopups.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool hasAnyPopup
        {
            get { return _popupStack.Count > 0; }
        }

        void ClosePopup(GObject target)
        {
            if (target.parent != null)
            {
                if (target is Window)
                    ((Window)target).Hide();
                else
                    RemoveChild(target);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void ShowTooltips(string msg)
        {
            ShowTooltips(msg, 0.1f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="delay"></param>
        public void ShowTooltips(string msg, float delay)
        {
            if (_defaultTooltipWin == null || _defaultTooltipWin.isDisposed)
            {
                string resourceURL = UIConfig.tooltipsWin;
                if (string.IsNullOrEmpty(resourceURL))
                {
                    GD.PushWarning("FairyGUI: UIConfig.tooltipsWin not defined");
                    return;
                }

                _defaultTooltipWin = UIPackage.CreateObjectFromURL(resourceURL);
                _defaultTooltipWin.touchable = false;
            }

            _defaultTooltipWin.text = msg;
            ShowTooltipsWin(_defaultTooltipWin, delay);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tooltipWin"></param>
        public void ShowTooltipsWin(GObject tooltipWin)
        {
            ShowTooltipsWin(tooltipWin, 0.1f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tooltipWin"></param>
        /// <param name="delay"></param>
        public void ShowTooltipsWin(GObject tooltipWin, float delay)
        {
            HideTooltips();

            _tooltipWin = tooltipWin;
            Timers.inst.Add(delay, 1, __showTooltipsWin);
        }

        void __showTooltipsWin(object param)
        {
            if (_tooltipWin == null)
                return;

            float xx = Stage.inst.touchPosition.X + 10;
            float yy = Stage.inst.touchPosition.Y + 20;

            Vector2 pt = this.GlobalToLocal(new Vector2(xx, yy));
            xx = pt.X;
            yy = pt.Y;

            if (xx + _tooltipWin.width > this.width)
                xx = xx - _tooltipWin.width;
            if (yy + _tooltipWin.height > this.height)
            {
                yy = yy - _tooltipWin.height - 1;
                if (yy < 0)
                    yy = 0;
            }

            _tooltipWin.x = Mathf.RoundToInt(xx);
            _tooltipWin.y = Mathf.RoundToInt(yy);
            AddChild(_tooltipWin);
        }

        /// <summary>
        /// 
        /// </summary>
        public void HideTooltips()
        {
            if (_tooltipWin != null)
            {
                if (_tooltipWin.parent != null)
                    RemoveChild(_tooltipWin);
                _tooltipWin = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public GObject focus
        {
            get
            {
                Control focused = displayObject.node.GetViewport().GuiGetFocusOwner();
                if (focused != null && focused is IDisplayObject obj)
                {
                    if (obj.gOwner != null)
                    {
                        if (!IsAncestorOf(obj.gOwner))
                            return obj.gOwner;
                    }
                }
                return null;
            }

            set
            {
                if (value == null)
                {
                    Control focused = displayObject.node.GetViewport().GuiGetFocusOwner();
                    if (focused != null)
                        focused.ReleaseFocus();
                }
                else
                    value.displayObject.node.GrabFocus();
            }
        }

        void __stageTouchBegin(EventContext context)
        {
            if (_tooltipWin != null)
                HideTooltips();

            CheckPopups(true);
        }

        void __stageTouchEnd(EventContext context)
        {
            CheckPopups(false);
        }

        void CheckPopups(bool touchBegin)
        {
            if (touchBegin)
                _justClosedPopups.Clear();

            if (_popupStack.Count > 0)
            {
                GObject mc = Stage.inst.touchTarget;
                bool handled = false;
                while (mc != this && mc != null)
                {
                    int k = _popupStack.IndexOf(mc);
                    if (k != -1)
                    {
                        for (int i = _popupStack.Count - 1; i > k; i--)
                        {
                            int last = _popupStack.Count - 1;
                            GObject popup = _popupStack[last];
                            if (touchBegin == _specialPopups.Contains(popup))
                                continue;

                            ClosePopup(popup);
                            _justClosedPopups.Add(popup);
                            _popupStack.RemoveAt(last);
                            _specialPopups.Remove(popup);
                        }
                        handled = true;
                        break;
                    }
                    mc = mc.parent;
                }

                if (!handled)
                {
                    for (int i = _popupStack.Count - 1; i >= 0; i--)
                    {
                        GObject popup = _popupStack[i];
                        if (touchBegin == _specialPopups.Contains(popup))
                            continue;

                        ClosePopup(popup);
                        _justClosedPopups.Add(popup);
                        _popupStack.RemoveAt(i);
                        _specialPopups.Remove(popup);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void EnableSound()
        {
            Stage.inst.EnableSound();
        }

        /// <summary>
        /// 
        /// </summary>
        public void DisableSound()
        {
            Stage.inst.DisableSound();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="volumeScale"></param>
        public void PlayOneShotSound(AudioStream clip, float volumeScale)
        {
            Stage.inst.PlayOneShotSound(clip, volumeScale);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clip"></param>
        public void PlayOneShotSound(AudioStream clip)
        {
            Stage.inst.PlayOneShotSound(clip);
        }

        /// <summary>
        /// 
        /// </summary>
        public float soundVolume
        {
            get { return Stage.inst.soundVolume; }
            set { Stage.inst.soundVolume = value; }
        }
    }
}
