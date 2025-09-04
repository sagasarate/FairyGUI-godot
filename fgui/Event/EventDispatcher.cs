using System;
using System.Collections.Generic;
using Godot;

namespace FairyGUI
{
    public delegate void EventCallback0();
    public delegate void EventCallback1(EventContext context);

    /// <summary>
    /// 
    /// </summary>
    public class EventDispatcher : IEventDispatcher
    {
        protected Dictionary<string, EventBridge> _bridegDic;

        public EventDispatcher()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="callback"></param>
        public void AddEventListener(string strType, EventCallback1 callback)
        {
            GetBridge(strType).Add(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="callback"></param>
        public void AddEventListener(string strType, EventCallback0 callback)
        {
            GetBridge(strType).Add(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="callback"></param>
        public void RemoveEventListener(string strType, EventCallback1 callback)
        {
            if (_bridegDic == null)
                return;

            EventBridge bridge = null;
            if (_bridegDic.TryGetValue(strType, out bridge))
                bridge.Remove(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="callback"></param>
        public void RemoveEventListener(string strType, EventCallback0 callback)
        {
            if (_bridegDic == null)
                return;

            EventBridge bridge = null;
            if (_bridegDic.TryGetValue(strType, out bridge))
                bridge.Remove(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="callback"></param>
        public void AddCapture(string strType, EventCallback1 callback)
        {
            GetBridge(strType).AddCapture(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="callback"></param>
        public void RemoveCapture(string strType, EventCallback1 callback)
        {
            if (_bridegDic == null)
                return;

            EventBridge bridge = null;
            if (_bridegDic.TryGetValue(strType, out bridge))
                bridge.RemoveCapture(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveEventListeners()
        {
            RemoveEventListeners(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        public void RemoveEventListeners(string strType)
        {
            if (_bridegDic == null)
                return;

            if (strType != null)
            {
                EventBridge bridge;
                if (_bridegDic.TryGetValue(strType, out bridge))
                    bridge.Clear();
            }
            else
            {
                foreach (KeyValuePair<string, EventBridge> kv in _bridegDic)
                    kv.Value.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <returns></returns>
        public bool hasEventListeners(string strType)
        {
            EventBridge bridge = TryGetEventBridge(strType);
            if (bridge == null)
                return false;

            return !bridge.isEmpty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <returns></returns>
        public bool isDispatching(string strType)
        {
            EventBridge bridge = TryGetEventBridge(strType);
            if (bridge == null)
                return false;

            return bridge._dispatching;
        }

        internal EventBridge TryGetEventBridge(string strType)
        {
            if (_bridegDic == null)
                return null;

            EventBridge bridge = null;
            _bridegDic.TryGetValue(strType, out bridge);
            return bridge;
        }

        internal EventBridge GetEventBridge(string strType)
        {
            if (_bridegDic == null)
                _bridegDic = new Dictionary<string, EventBridge>();

            EventBridge bridge = null;
            if (!_bridegDic.TryGetValue(strType, out bridge))
            {
                bridge = new EventBridge(this);
                _bridegDic[strType] = bridge;
            }
            return bridge;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <returns></returns>
        public bool DispatchEvent(string strType)
        {
            return DispatchEvent(strType, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool DispatchEvent(string strType, object data)
        {
            return InternalDispatchEvent(strType, null, data, null);
        }

        public bool DispatchEvent(string strType, object data, object initiator)
        {
            return InternalDispatchEvent(strType, null, data, initiator);
        }

        static NInputEvent sCurrentInputEvent;

        internal bool InternalDispatchEvent(string strType, EventBridge bridge, object data, object initiator)
        {
            if (bridge == null)
                bridge = TryGetEventBridge(strType);
            if (bridge != null && !bridge.isEmpty)
            {
                EventContext context = EventContext.Get();
                context.initiator = initiator != null ? initiator : this;
                context.type = strType;
                context.data = data;
                if (data is NInputEvent)
                    sCurrentInputEvent = (NInputEvent)data;
                context.inputEvent = sCurrentInputEvent;

                bridge.CallCaptureInternal(context);
                bridge.CallInternal(context);

                EventContext.Return(context);
                context.initiator = null;
                context.sender = null;
                context.data = null;

                return context._defaultPrevented;
            }
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool DispatchEvent(EventContext context)
        {
            EventBridge bridge = TryGetEventBridge(context.type);

            EventDispatcher savedSender = context.sender;

            if (bridge != null && !bridge.isEmpty)
            {
                bridge.CallCaptureInternal(context);
                bridge.CallInternal(context);
            }
            context.sender = savedSender;
            return context._defaultPrevented;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="data"></param>
        /// <param name="addChain"></param>
        /// <returns></returns>
        internal bool BubbleEvent(string strType, object data, List<EventBridge> addChain)
        {
            EventContext context = EventContext.Get();
            context.initiator = this;

            context.type = strType;
            context.data = data;
            if (data is NInputEvent)
                sCurrentInputEvent = (NInputEvent)data;
            context.inputEvent = sCurrentInputEvent;
            List<EventBridge> bubbleChain = context.callChain;
            bubbleChain.Clear();

            GetChainBridges(strType, bubbleChain, true);

            int length = bubbleChain.Count;
            for (int i = length - 1; i >= 0; i--)
            {
                bubbleChain[i].CallCaptureInternal(context);
                if (context._touchCapture)
                {
                    context._touchCapture = false;
                    if (strType == "onTouchBegin")
                        Stage.inst.AddTouchMonitor(context.inputEvent.touchId, bubbleChain[i].owner);
                }
            }

            if (!context._stopsPropagation)
            {
                for (int i = 0; i < length; ++i)
                {
                    bubbleChain[i].CallInternal(context);

                    if (context._touchCapture)
                    {
                        context._touchCapture = false;
                        if (strType == "onTouchBegin")
                            Stage.inst.AddTouchMonitor(context.inputEvent.touchId, bubbleChain[i].owner);
                    }

                    if (context._stopsPropagation)
                        break;
                }

                if (addChain != null)
                {
                    length = addChain.Count;
                    for (int i = 0; i < length; ++i)
                    {
                        EventBridge bridge = addChain[i];
                        if (bubbleChain.IndexOf(bridge) == -1)
                        {
                            bridge.CallCaptureInternal(context);
                            bridge.CallInternal(context);
                        }
                    }
                }
            }

            EventContext.Return(context);
            context.initiator = null;
            context.sender = null;
            context.data = null;
            return context._defaultPrevented;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool BubbleEvent(string strType, object data)
        {
            return BubbleEvent(strType, data, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool BroadcastEvent(string strType, object data)
        {
            EventContext context = EventContext.Get();
            context.initiator = this;
            context.type = strType;
            context.data = data;
            if (data is NInputEvent)
                sCurrentInputEvent = (NInputEvent)data;
            context.inputEvent = sCurrentInputEvent;
            List<EventBridge> bubbleChain = context.callChain;
            bubbleChain.Clear();

            if (this is GObject)
                GetChildEventBridges(strType, (GObject)this, bubbleChain);

            int length = bubbleChain.Count;
            for (int i = 0; i < length; ++i)
                bubbleChain[i].CallInternal(context);

            EventContext.Return(context);
            context.initiator = null;
            context.sender = null;
            context.data = null;
            return context._defaultPrevented;
        }

        EventBridge GetBridge(string strType)
        {
            if (strType == null)
                throw new Exception("event type cant be null");

            if (_bridegDic == null)
                _bridegDic = new Dictionary<string, EventBridge>();

            EventBridge bridge = null;
            if (!_bridegDic.TryGetValue(strType, out bridge))
            {
                bridge = new EventBridge(this);
                _bridegDic[strType] = bridge;
            }

            return bridge;
        }

        // static void GetChildEventBridges(string strType, Control node, List<EventBridge> bridges)
        // {
        //     int count = node.GetChildCount();
        //     for (int i = 0; i < count; i++)
        //     {
        //         var child = node.GetChild(i);
        //         if ((child is IDisplayObject obj) && obj.gOwner != null)
        //             GetChildEventBridges(strType, obj.gOwner, bridges);
        //     }
        // }

        static void GetChildEventBridges(string strType, GObject gObject, List<EventBridge> bridges)
        {
            EventBridge bridge = gObject.TryGetEventBridge(strType);
            if (bridge != null && bridges.IndexOf(bridge) < 0)
                bridges.Add(bridge);

            // GetChildEventBridges(strType, gObject.displayObject.node, bridges);

            if (gObject is GComponent gComponent)
            {
                // if (gComponent.container != gComponent.displayObject)
                //     GetChildEventBridges(strType, gComponent.container, bridges);
                int count = gComponent.numChildren;
                for (int i = 0; i < count; ++i)
                {
                    GObject obj = gComponent.GetChildAt(i);
                    if (obj is GComponent)
                        GetChildEventBridges(strType, (GComponent)obj, bridges);
                    else
                        GetChildEventBridges(strType, obj, bridges);
                }
                count = gComponent.numInternalChildren;
                for (int i = 0; i < count; ++i)
                {
                    GObject obj = gComponent.GetInternalChildAt(i);
                    if (obj is GComponent)
                        GetChildEventBridges(strType, (GComponent)obj, bridges);
                    else
                        GetChildEventBridges(strType, obj, bridges);
                }
            }
        }

        internal void GetChainBridges(string strType, List<EventBridge> chain, bool bubble)
        {
            EventBridge bridge = TryGetEventBridge(strType);
            if (bridge != null && !bridge.isEmpty)
                chain.Add(bridge);

            if (!bubble)
                return;

            if (this is GObject)
            {
                GObject element = (GObject)this;
                while ((element = element.parent) != null)
                {
                    bridge = element.TryGetEventBridge(strType);
                    if (bridge != null && !bridge.isEmpty)
                        chain.Add(bridge);
                }
            }
        }
    }
}
