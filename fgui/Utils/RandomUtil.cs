using Godot;
using System;

namespace FairyGUI
{
    public static class RandomUtil
    {
        private static readonly Random _rand = new Random();

        /// <summary>
        /// 返回半径为1的单位球内的随机点（均匀分布）
        /// </summary>
        public static Vector3 InsideUnitSphere()
        {
            while (true)
            {
                // 在立方体 [-1,1]³ 内生成随机点
                float x = (float)(_rand.NextDouble() * 2 - 1);
                float y = (float)(_rand.NextDouble() * 2 - 1);
                float z = (float)(_rand.NextDouble() * 2 - 1);

                Vector3 point = new Vector3(x, y, z);

                // 如果在单位球内，返回
                if (point.LengthSquared() <= 1)
                    return point;
            }
        }
        public static int Range(int min, int max)
        {
            if (min == max) return min;
            int actualMin = Math.Min(min, max);
            int actualMax = Math.Max(min, max);
            return actualMin + _rand.Next(actualMax - actualMin + 1);
        }
        public static float Range(float min, float max)
        {
            if (min == max) return min;
            float actualMin = Math.Min(min, max);
            float actualMax = Math.Max(min, max);
            return actualMin + _rand.NextSingle() * (actualMax - actualMin);
        }
    }
}
