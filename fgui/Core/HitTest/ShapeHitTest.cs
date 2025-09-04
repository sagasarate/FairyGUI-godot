using Godot;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public class ShapeHitTest : IHitTest
    {
        /// <summary>
        /// 
        /// </summary>
        public IDisplayObject shape;

        public ShapeHitTest(IDisplayObject obj)
        {
            shape = obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentRect"></param>
        /// <param name="localPoint"></param>
        /// <returns></returns>
        public bool HitTest(Rect contentRect, Vector2 localPoint)
        {
            IHitTest ht = shape as IHitTest;
            if (ht == null)
                return false;
            return ht.HitTest(contentRect, localPoint);
        }
    }
}
