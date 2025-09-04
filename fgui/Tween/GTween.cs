using Godot;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public static class GTween
    {
        /// <summary>
        /// 
        /// </summary>
        public static bool catchCallbackExceptions = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static GTweener To(float startValue, float endValue, float duration)
        {
            return TweenManager.inst.CreateTween()._To(startValue, endValue, duration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static GTweener To(Vector2 startValue, Vector2 endValue, float duration)
        {
            return TweenManager.inst.CreateTween()._To(startValue, endValue, duration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static GTweener To(Vector3 startValue, Vector3 endValue, float duration)
        {
            return TweenManager.inst.CreateTween()._To(startValue, endValue, duration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static GTweener To(Vector4 startValue, Vector4 endValue, float duration)
        {
            return TweenManager.inst.CreateTween()._To(startValue, endValue, duration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static GTweener To(Color startValue, Color endValue, float duration)
        {
            return TweenManager.inst.CreateTween()._To(startValue, endValue, duration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static GTweener ToDouble(double startValue, double endValue, float duration)
        {
            return TweenManager.inst.CreateTween()._To(startValue, endValue, duration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static GTweener DelayedCall(float delay)
        {
            return TweenManager.inst.CreateTween().SetDelay(delay);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="amplitude"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static GTweener Shake(Vector3 startValue, float amplitude, float duration)
        {
            return TweenManager.inst.CreateTween()._Shake(startValue, amplitude, duration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsTweening(object target)
        {
            return TweenManager.inst.IsTweening(target, TweenPropType.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propType"></param>
        /// <returns></returns>
        public static bool IsTweening(object target, TweenPropType propType)
        {
            return TweenManager.inst.IsTweening(target, propType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public static void Kill(object target)
        {
            TweenManager.inst.KillTweens(target, TweenPropType.None, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="complete"></param>
        public static void Kill(object target, bool complete)
        {
            TweenManager.inst.KillTweens(target, TweenPropType.None, complete);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propType"></param>
        /// <param name="complete"></param>
        public static void Kill(object target, TweenPropType propType, bool complete)
        {
            TweenManager.inst.KillTweens(target, propType, complete);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static GTweener GetTween(object target)
        {
            return TweenManager.inst.GetTween(target, TweenPropType.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propType"></param>
        /// <returns></returns>
        public static GTweener GetTween(object target, TweenPropType propType)
        {
            return TweenManager.inst.GetTween(target, propType);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Clean()
        {
            TweenManager.inst.Clean();
        }
    }
}
