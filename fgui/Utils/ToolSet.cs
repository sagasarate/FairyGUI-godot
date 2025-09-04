using System.Runtime.Intrinsics;
using Godot;

namespace FairyGUI.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class ToolSet
    {
        public static Color ConvertFromHtmlColor(string str)
        {
            if (str.Length < 7 || str[0] != '#')
                return Colors.Black;

            if (str.Length == 9)
            {
                //optimize:avoid using Convert.ToByte and Substring
                //return new Color32(Convert.ToByte(str.Substring(3, 2), 16), Convert.ToByte(str.Substring(5, 2), 16),
                //  Convert.ToByte(str.Substring(7, 2), 16), Convert.ToByte(str.Substring(1, 2), 16));

                return Color.Color8((byte)(CharToHex(str[3]) * 16 + CharToHex(str[4])),
                    (byte)(CharToHex(str[5]) * 16 + CharToHex(str[6])),
                    (byte)(CharToHex(str[7]) * 16 + CharToHex(str[8])),
                    (byte)(CharToHex(str[1]) * 16 + CharToHex(str[2])));
            }
            else
            {
                //return new Color32(Convert.ToByte(str.Substring(1, 2), 16), Convert.ToByte(str.Substring(3, 2), 16),
                //Convert.ToByte(str.Substring(5, 2), 16), 255);

                return Color.Color8((byte)(CharToHex(str[1]) * 16 + CharToHex(str[2])),
                    (byte)(CharToHex(str[3]) * 16 + CharToHex(str[4])),
                    (byte)(CharToHex(str[5]) * 16 + CharToHex(str[6])),
                    255);
            }
        }

        public static Color ColorFromRGB(int value)
        {
            return new Color(((value >> 16) & 0xFF) / 255f, ((value >> 8) & 0xFF) / 255f, (value & 0xFF) / 255f, 1);
        }

        public static Color ColorFromRGBA(uint value)
        {
            return new Color(((value >> 16) & 0xFF) / 255f, ((value >> 8) & 0xFF) / 255f, (value & 0xFF) / 255f, ((value >> 24) & 0xFF) / 255f);
        }

        public static int CharToHex(char c)
        {
            if (c >= '0' && c <= '9')
                return (int)c - 48;
            if (c >= 'A' && c <= 'F')
                return 10 + (int)c - 65;
            else if (c >= 'a' && c <= 'f')
                return 10 + (int)c - 97;
            else
                return 0;
        }

        public static Rect Intersection(ref Rect rect1, ref Rect rect2)
        {
            if (rect1.width == 0 || rect1.height == 0 || rect2.width == 0 || rect2.height == 0)
                return new Rect(0, 0, 0, 0);

            float left = rect1.xMin > rect2.xMin ? rect1.xMin : rect2.xMin;
            float right = rect1.xMax < rect2.xMax ? rect1.xMax : rect2.xMax;
            float top = rect1.yMin > rect2.yMin ? rect1.yMin : rect2.yMin;
            float bottom = rect1.yMax < rect2.yMax ? rect1.yMax : rect2.yMax;

            if (left > right || top > bottom)
                return new Rect(0, 0, 0, 0);
            else
                return Rect.MinMaxRect(left, top, right, bottom);
        }

        public static Rect Union(ref Rect rect1, ref Rect rect2)
        {
            if (rect2.width == 0 || rect2.height == 0)
                return rect1;

            if (rect1.width == 0 || rect1.height == 0)
                return rect2;

            float x = Mathf.Min(rect1.X, rect2.X);
            float y = Mathf.Min(rect1.Y, rect2.Y);
            return new Rect(x, y, Mathf.Max(rect1.xMax, rect2.xMax) - x, Mathf.Max(rect1.yMax, rect2.yMax) - y);
        }

        // public static void SkewMatrix(ref Matrix4x4 matrix, float skewX, float skewY)
        // {
        //     skewX = -skewX * Mathf.Deg2Rad;
        //     skewY = -skewY * Mathf.Deg2Rad;
        //     float sinX = Mathf.Sin(skewX);
        //     float cosX = Mathf.Cos(skewX);
        //     float sinY = Mathf.Sin(skewY);
        //     float cosY = Mathf.Cos(skewY);

        //     float m00 = matrix.m00 * cosY - matrix.m10 * sinX;
        //     float m10 = matrix.m00 * sinY + matrix.m10 * cosX;
        //     float m01 = matrix.m01 * cosY - matrix.m11 * sinX;
        //     float m11 = matrix.m01 * sinY + matrix.m11 * cosX;
        //     float m02 = matrix.m02 * cosY - matrix.m12 * sinX;
        //     float m12 = matrix.m02 * sinY + matrix.m12 * cosX;

        //     matrix.m00 = m00;
        //     matrix.m10 = m10;
        //     matrix.m01 = m01;
        //     matrix.m11 = m11;
        //     matrix.m02 = m02;
        //     matrix.m12 = m12;
        // }
        public static void SkewTransform3D(ref Transform3D transform, float skewX, float skewY)
        {
            // 转成弧度并取反
            skewX = -skewX * Mathf.DegToRad(1);
            skewY = -skewY * Mathf.DegToRad(1);

            float sinX = Mathf.Sin(skewX);
            float cosX = Mathf.Cos(skewX);
            float sinY = Mathf.Sin(skewY);
            float cosY = Mathf.Cos(skewY);

            // 在 Godot 里 basis 存储旋转/缩放部分，相当于 3x3 矩阵
            Basis b = transform.Basis;

            float m00 = b.X.X * cosY - b.Y.X * sinX;
            float m10 = b.X.X * sinY + b.Y.X * cosX;
            float m01 = b.X.Y * cosY - b.Y.Y * sinX;
            float m11 = b.X.Y * sinY + b.Y.Y * cosX;
            float m02 = b.X.Z * cosY - b.Y.Z * sinX;
            float m12 = b.X.Z * sinY + b.Y.Z * cosX;

            b.X = new Vector3(m00, m01, m02);
            b.Y = new Vector3(m10, m11, m12);

            transform.Basis = b;
        }
        public static void SkewTransform2D(ref Transform2D transform, float skewX, float skewY)
        {
            // 转成弧度并取反
            skewX = -skewX * Mathf.DegToRad(1);
            skewY = -skewY * Mathf.DegToRad(1);

            float sinX = Mathf.Sin(skewX);
            float cosX = Mathf.Cos(skewX);
            float sinY = Mathf.Sin(skewY);
            float cosY = Mathf.Cos(skewY);

            // Transform2D 的 basis 列向量
            Vector2 x = transform.X; // 对应原来的 m00, m01
            Vector2 y = transform.Y; // 对应原来的 m10, m11

            float m00 = x.X * cosY - y.X * sinX;
            float m10 = x.X * sinY + y.X * cosX;
            float m01 = x.Y * cosY - y.Y * sinX;
            float m11 = x.Y * sinY + y.Y * cosX;

            transform.X = new Vector2(m00, m01);
            transform.Y = new Vector2(m10, m11);
        }

        public static void RotateUV(Vector2[] uv, ref Rect baseUVRect)
        {
            int vertCount = uv.Length;
            float xMin = Mathf.Min(baseUVRect.xMin, baseUVRect.xMax);
            float yMin = baseUVRect.yMin;
            float yMax = baseUVRect.yMax;
            if (yMin > yMax)
            {
                yMin = yMax;
                yMax = baseUVRect.yMin;
            }

            float tmp;
            for (int i = 0; i < vertCount; i++)
            {
                Vector2 m = uv[i];
                tmp = m.Y;
                m.Y = yMin + m.X - xMin;
                m.X = xMin + yMax - tmp;
                uv[i] = m;
            }
        }
        public static void MeshAddRect(SurfaceTool surfaceTool, Rect vertRect, Rect uvRect, int startIndex)
        {
            surfaceTool.SetUV(uvRect.leftTop);
            surfaceTool.AddVertex(new Vector3(vertRect.xMin, vertRect.yMin, 0));

            surfaceTool.SetUV(uvRect.rightTop);
            surfaceTool.AddVertex(new Vector3(vertRect.xMax, vertRect.yMin, 0));

            surfaceTool.SetUV(uvRect.leftBottom);
            surfaceTool.AddVertex(new Vector3(vertRect.xMin, vertRect.yMax, 0));

            surfaceTool.SetUV(uvRect.rightBottom);
            surfaceTool.AddVertex(new Vector3(vertRect.xMax, vertRect.yMax, 0));

            surfaceTool.AddIndex(startIndex + 0);
            surfaceTool.AddIndex(startIndex + 3);
            surfaceTool.AddIndex(startIndex + 2);

            surfaceTool.AddIndex(startIndex + 0);
            surfaceTool.AddIndex(startIndex + 1);
            surfaceTool.AddIndex(startIndex + 3);
        }
        public static void MeshAddRect(SurfaceTool surfaceTool, Rect vertRect, Color vertColor, int startIndex, Color[] vertColors, int startColorIndex)
        {
            surfaceTool.SetColor(vertColors == null || (vertColors.Length <= startColorIndex) ? vertColor : vertColors[startColorIndex]);
            startColorIndex++;
            surfaceTool.AddVertex(new Vector3(vertRect.xMin, vertRect.yMin, 0));
            surfaceTool.SetColor(vertColors == null || (vertColors.Length <= startColorIndex) ? vertColor : vertColors[startColorIndex]);
            startColorIndex++;
            surfaceTool.AddVertex(new Vector3(vertRect.xMax, vertRect.yMin, 0));
            surfaceTool.SetColor(vertColors == null || (vertColors.Length <= startColorIndex) ? vertColor : vertColors[startColorIndex]);
            startColorIndex++;
            surfaceTool.AddVertex(new Vector3(vertRect.xMin, vertRect.yMax, 0));
            surfaceTool.SetColor(vertColors == null || (vertColors.Length <= startColorIndex) ? vertColor : vertColors[startColorIndex]);
            startColorIndex++;
            surfaceTool.AddVertex(new Vector3(vertRect.xMax, vertRect.yMax, 0));

            surfaceTool.AddIndex(startIndex + 0);
            surfaceTool.AddIndex(startIndex + 3);
            surfaceTool.AddIndex(startIndex + 2);

            surfaceTool.AddIndex(startIndex + 0);
            surfaceTool.AddIndex(startIndex + 1);
            surfaceTool.AddIndex(startIndex + 3);
        }

        public static void MeshAddVertex(SurfaceTool surfaceTool, float X, float Y, Rect vertRect, Rect uvRect)
        {
            Vector3 Vertex = new Vector3(X, Y, 0);
            Vector2 UV = new Vector2(
                Mathf.Lerp(uvRect.xMin, uvRect.xMax, (X - vertRect.xMin) / vertRect.width),
                Mathf.Lerp(uvRect.yMin, uvRect.yMax, (Y - vertRect.yMin) / vertRect.height));
            surfaceTool.SetUV(UV);
            surfaceTool.AddVertex(Vertex);
        }

        public static void MeshAddVertex(SurfaceTool surfaceTool, float X, float Y, Color vertColor)
        {
            Vector3 Vertex = new Vector3(X, Y, 0);
            surfaceTool.SetColor(vertColor);
            surfaceTool.AddVertex(Vertex);
        }
        public static void MeshAddVertex(SurfaceTool surfaceTool, Vector3 vert, Color vertColor)
        {
            surfaceTool.SetColor(vertColor);
            surfaceTool.AddVertex(vert);
        }
        public static void MeshAddVertex(SurfaceTool surfaceTool, Vector3 vert, Vector2 uv, Color vertColor)
        {
            surfaceTool.SetColor(vertColor);
            surfaceTool.SetUV(uv);
            surfaceTool.AddVertex(vert);
        }
        public static void MeshAddTriangleIndecies(SurfaceTool surfaceTool, int v1, int v2, int v3)
        {
            surfaceTool.AddIndex(v1);
            surfaceTool.AddIndex(v2);
            surfaceTool.AddIndex(v3);
        }
    }
}
