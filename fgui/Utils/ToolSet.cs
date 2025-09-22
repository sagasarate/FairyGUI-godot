using System;
using System.Collections.Generic;
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

        public static void MeshAddVertex(SurfaceTool surfaceTool, float X, float Y, Rect? vertRect, Rect? uvRect, Color? color = null)
        {
            Vector3 Vertex = new Vector3(X, Y, 0);
            if (color != null)
            {
                surfaceTool.SetColor((Color)color);
            }
            if (vertRect != null && uvRect != null)
            {
                Vector2 UV = new Vector2(
                Mathf.Lerp(((Rect)uvRect).xMin, ((Rect)uvRect).xMax, (X - ((Rect)vertRect).xMin) / ((Rect)vertRect).width),
                Mathf.Lerp(((Rect)uvRect).yMin, ((Rect)uvRect).yMax, (Y - ((Rect)vertRect).yMin) / ((Rect)vertRect).height));
                surfaceTool.SetUV(UV);
            }
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
        public static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c, bool include)
        {
            // From Starling
            // This algorithm is described well in this article:
            // http://www.blackpawn.com/texts/pointinpoly/default.html

            //点相等的情况不算包含，某些环状多边形头尾的点是相同的
            if (p.IsEqualApprox(a) || p.IsEqualApprox(b) || p.IsEqualApprox(c))
                return false;

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

            if (include)
                return (u >= 0) && (v >= 0) && (u + v < 1);
            else
                return (u > 1e-6) && (v > 1e-6) && (u + v < 1 - 1e-6);
        }
        static List<int> sRestIndices = new List<int>();
        public static void AddPolygonIndecies(SurfaceTool surfaceTool, List<Vector2> vertices, int startVertex)
        {
            int numVertices = vertices.Count;
            sRestIndices.Clear();
            for (int i = 0; i < numVertices; ++i)
                sRestIndices.Add(i);

            int restIndexPos = 0;
            int numRestIndices = numVertices;

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

                a = vertices[i0];
                b = vertices[i1];
                c = vertices[i2];

                if ((a.Y - b.Y) * (c.X - b.X) + (b.X - a.X) * (c.Y - b.Y) >= 0)
                {
                    earFound = true;
                    for (int i = 3; i < numRestIndices; ++i)
                    {
                        otherIndex = sRestIndices[(restIndexPos + i) % numRestIndices];
                        p = vertices[otherIndex];

                        if (IsPointInTriangle(p, a, b, c, true))
                        {
                            earFound = false;
                            break;
                        }
                    }
                }

                if (earFound)
                {
                    MeshAddTriangleIndecies(surfaceTool, i0 + startVertex, i1 + startVertex, i2 + startVertex);
                    sRestIndices.RemoveAt((restIndexPos + 1) % numRestIndices);

                    numRestIndices--;
                    restIndexPos = 0;
                }
                else
                {
                    restIndexPos++;
                    if (restIndexPos == numRestIndices)
                        break; // no more ears
                }
            }
            MeshAddTriangleIndecies(surfaceTool, sRestIndices[0] + startVertex, sRestIndices[1] + startVertex, sRestIndices[2] + startVertex);
        }
        //条带特化的切耳算法，必须保证第一和最后一个顶点构成条带的一端
        public static void AddRibbonIndecies(SurfaceTool surfaceTool, List<Vector2> vertices, int startVertex)
        {
            int numVertices = vertices.Count;
            sRestIndices.Clear();
            for (int i = 0; i < numVertices; ++i)
                sRestIndices.Add(i);

            int numRestIndices = numVertices;

            Vector2 a, b, c;
            int cnt = 0;
            while (numRestIndices > 3)
            {
                int start = 0;
                int end = numRestIndices - 1;
                int bestEarPos = -1;
                float bestEarScore = -1;

                //左切
                int i0 = sRestIndices[end];
                int i1 = sRestIndices[start];
                int i2 = sRestIndices[start + 1];

                a = vertices[i0];
                b = vertices[i1];
                c = vertices[i2];

                if (!IncludeOther(vertices, sRestIndices, i0, i1, i2) && (a.Y - b.Y) * (c.X - b.X) + (b.X - a.X) * (c.Y - b.Y) >= 0)
                {
                    bestEarPos = start;
                    bestEarScore = GetEarScore(a, b, c);
                }
                //右切
                i0 = sRestIndices[end - 1];
                i1 = sRestIndices[end];
                i2 = sRestIndices[start];

                a = vertices[i0];
                b = vertices[i1];
                c = vertices[i2];

                if (!IncludeOther(vertices, sRestIndices, i0, i1, i2) && (a.Y - b.Y) * (c.X - b.X) + (b.X - a.X) * (c.Y - b.Y) >= 0)
                {
                    if (bestEarPos >= 0)
                    {
                        float earScore = GetEarScore(a, b, c);
                        if (earScore > bestEarScore)
                            bestEarPos = end;
                    }
                    else
                    {
                        bestEarPos = end;
                    }
                }

                //左+1切
                if (bestEarPos < 0)
                {
                    i0 = sRestIndices[start];
                    i1 = sRestIndices[start + 1];
                    i2 = sRestIndices[start + 2];

                    a = vertices[i0];
                    b = vertices[i1];
                    c = vertices[i2];

                    if (!IncludeOther(vertices, sRestIndices, i0, i1, i2) && (a.Y - b.Y) * (c.X - b.X) + (b.X - a.X) * (c.Y - b.Y) >= 0)
                    {
                        bestEarPos = start + 1;
                        bestEarScore = GetEarScore(a, b, c);
                    }
                    //右+1切
                    i0 = sRestIndices[end - 2];
                    i1 = sRestIndices[end - 1];
                    i2 = sRestIndices[end];

                    a = vertices[i0];
                    b = vertices[i1];
                    c = vertices[i2];

                    if (!IncludeOther(vertices, sRestIndices, i0, i1, i2) && (a.Y - b.Y) * (c.X - b.X) + (b.X - a.X) * (c.Y - b.Y) >= 0)
                    {
                        if (bestEarPos >= 0)
                        {
                            float earScore = GetEarScore(a, b, c);
                            if (earScore > bestEarScore)
                                bestEarPos = end - 1;
                        }
                        else
                        {
                            bestEarPos = end - 1;
                        }
                    }
                }

                if (bestEarPos == -1)
                {
                    // 没有耳朵（退化情况）
                    break;
                }

                // 切掉最佳耳朵
                int i0b = sRestIndices[(bestEarPos + numRestIndices - 1) % numRestIndices];
                int i1b = sRestIndices[bestEarPos];
                int i2b = sRestIndices[(bestEarPos + 1) % numRestIndices];

                MeshAddTriangleIndecies(surfaceTool, i0b + startVertex, i1b + startVertex, i2b + startVertex);
                sRestIndices.RemoveAt(bestEarPos);
                numRestIndices--;

                cnt++;
                // if (cnt >= 6)
                //     break;
            }

            // 剩下最后一个三角
            if (numRestIndices == 3)
            {
                MeshAddTriangleIndecies(surfaceTool,
                    sRestIndices[0] + startVertex,
                    sRestIndices[1] + startVertex,
                    sRestIndices[2] + startVertex);
            }
        }
        private static bool IncludeOther(List<Vector2> vertices, List<int> RestIndices, int i0, int i1, int i2)
        {
            Vector2 a = vertices[i0];
            Vector2 b = vertices[i1];
            Vector2 c = vertices[i2];
            for (int i = 0; i < RestIndices.Count; i++)
            {
                int index = RestIndices[i];
                if (index != i0 && index != i1 && index != i2)
                {
                    Vector2 v = vertices[index];
                    if (IsPointInTriangle(v, a, b, c, true))
                        return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 评分函数：返回三角形最小角度，越大越好
        /// </summary>
        private static float GetEarScore(Vector2 a, Vector2 b, Vector2 c)
        {
            float ab = (b - a).Length();
            float bc = (c - b).Length();
            float ca = (a - c).Length();

            // 利用余弦定理计算角度
            float angleA = Mathf.Acos(Mathf.Clamp(((ab * ab) + (ca * ca) - (bc * bc)) / (2 * ab * ca), -1f, 1f));
            float angleB = Mathf.Acos(Mathf.Clamp(((ab * ab) + (bc * bc) - (ca * ca)) / (2 * ab * bc), -1f, 1f));
            float angleC = Mathf.Pi - angleA - angleB;

            return Mathf.Min(angleA, Mathf.Min(angleB, angleC));
        }

        public static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2, out Vector2 intersection)
        {
            intersection = Vector2.Zero;

            Vector2 r = p2 - p1;
            Vector2 s = q2 - q1;
            float rxs = r.Cross(s);

            if (Mathf.Abs(rxs) < 0.01f)
            {
                // 平行或重合：无唯一交点
                return false;
            }

            float t = (q1 - p1).Cross(s) / rxs;
            intersection = p1 + t * r;
            return true;
        }
        public static bool IsClockwise(List<Vector2> vertices)
        {
            if (vertices.Count < 3)
                return false;
            float area = 0f;
            int n = vertices.Count;

            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                area += (vertices[j].X - vertices[i].X) * (vertices[j].Y + vertices[i].Y);
            }
            return area < 0;
        }
        public static bool IsClockwise(Vector2[] vertices)
        {
            return Geometry2D.IsPolygonClockwise(vertices);
        }
        static double AngleFromTo(Vector2 u, Vector2 v)
        {
            double ux = u.X, uy = u.Y, vx = v.X, vy = v.Y;
            double cross = ux * vy - uy * vx;
            double dot = ux * vx + uy * vy;
            double ang = Mathf.Atan2(cross, dot); // (-π, π]
            if (ang < 0) ang += 2.0 * Mathf.Pi;   // -> [0, 2π)
            return ang;
        }
        public const double EPS = 1e-12;
        public static bool IsInsideAngle(Vector2 a, Vector2 b, Vector2 c, Vector2 d, bool inclusive = true)
        {
            Vector2 ba = a - b;
            Vector2 bc = c - b;
            Vector2 bd = d - b;

            if (ba.X * ba.X + ba.Y * ba.Y < EPS) return false;
            if (bc.X * bc.X + bc.Y * bc.Y < EPS) return false;
            if (bd.X * bd.X + bd.Y * bd.Y < EPS) return false;

            double angBC = AngleFromTo(ba, bc); // CCW span from BA to BC
            double angBD = AngleFromTo(ba, bd);

            if (inclusive)
                return angBD <= angBC + EPS;
            else
                return angBD < angBC - EPS;
        }
        public static void Rotate<T>(List<T> list, int index)
        {
            int n = list.Count;
            if (n == 0) return;

            index %= n;
            if (index < 0) index += n;
            if (index == 0) return; // 已经是首位

            for (int i = 0; i < index; i++)
                list.Add(list[i]);
            list.RemoveRange(0, index);
        }
        public static void Rotate2<T>(List<T> list, int index)
        {
            int n = list.Count;
            if (n == 0) return;

            index %= n;
            if (index < 0) index += n;
            if (index == 0) return; // 已经是首位

            // 反转 [0, index-1]
            list.Reverse(0, index);
            // 反转 [index, n-1]
            list.Reverse(index, n - index);
            // 反转整个 [0, n-1]
            list.Reverse(0, n);
        }

        // 判断点是否在多边形内部
        public static bool IsPointInPolygon(Vector2 p, List<Vector2> polygon)
        {
            bool inside = false;
            int count = polygon.Count;
            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                Vector2 pi = polygon[i];
                Vector2 pj = polygon[j];

                // 射线法：判断是否跨越水平射线
                if (((pi.Y > p.Y) != (pj.Y > p.Y)) &&
                    (p.X < (pj.X - pi.X) * (p.Y - pi.Y) / (pj.Y - pi.Y) + pi.X))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        // 判断两条线段是否相交（包括端点重合）
        private static bool SegmentIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            float Cross(Vector2 a, Vector2 b, Vector2 c)
                => (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);

            bool OnSegment(Vector2 a, Vector2 b, Vector2 p)
                => Math.Min(a.X, b.X) <= p.X && p.X <= Math.Max(a.X, b.X) &&
                   Math.Min(a.Y, b.Y) <= p.Y && p.Y <= Math.Max(a.Y, b.Y);

            float c1 = Cross(p1, p2, q1);
            float c2 = Cross(p1, p2, q2);
            float c3 = Cross(q1, q2, p1);
            float c4 = Cross(q1, q2, p2);

            if (c1 == 0 && OnSegment(p1, p2, q1)) return true;
            if (c2 == 0 && OnSegment(p1, p2, q2)) return true;
            if (c3 == 0 && OnSegment(q1, q2, p1)) return true;
            if (c4 == 0 && OnSegment(q1, q2, p2)) return true;

            return (c1 * c2 < 0 && c3 * c4 < 0);
        }

        /// <summary>
        /// 判断线段 [a,b] 是否与多边形相交（多边形按逆时针给定）
        /// 规则：
        ///   - 起点 a 可以在多边形边上
        ///   - 终点 b 不能在多边形边上
        /// </summary>
        public static bool SegmentIntersectsPolygon(Vector2 a, Vector2 b, List<Vector2> polygon)
        {
            int n = polygon.Count;
            for (int i = 0; i < n; i++)
            {
                Vector2 p1 = polygon[i];
                Vector2 p2 = polygon[(i + 1) % n];

                if (SegmentIntersect(a, b, p1, p2))
                {
                    // 起点 a 在线上是允许的
                    if ((a == p1 || a == p2))
                        continue;

                    // 终点 b 在线上不允许
                    if (b == p1 || b == p2)
                        return true;

                    // 一般相交情况
                    return true;
                }
            }

            // 如果线段完全在多边形内部
            if (IsPointInPolygon(a, polygon) && IsPointInPolygon(b, polygon))
                return true;

            return false;
        }
        public static bool IsConvex(List<Vector2> vertices)
        {
            if (vertices == null || vertices.Count < 3)
                return false;

            int n = vertices.Count;
            bool? sign = null;

            for (int i = 0; i < n; i++)
            {
                Vector2 a = vertices[i];
                Vector2 b = vertices[(i + 1) % n];
                Vector2 c = vertices[(i + 2) % n];

                float cross = (b.X - a.X) * (c.Y - b.Y) - (b.Y - a.Y) * (c.X - b.X);

                if (cross != 0) // 忽略共线
                {
                    if (sign == null)
                        sign = cross > 0;
                    else if (sign != (cross > 0))
                        return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// 从 srcTex 的指定区域提取像素，反转 Alpha 后生成新的 Texture2D。
        /// </summary>
        /// <param name="srcTex">源贴图</param>
        /// <param name="region">要截取的区域（像素坐标，左上角为原点）</param>
        /// <returns>新的 Texture2D（仅包含处理后的区域）</returns>
        public static Texture2D ExtractAndInvertAlpha(Texture2D srcTex, Rect2I region)
        {
            // 拿到 CPU Image
            Image img = srcTex.GetImage();

            if (img.IsCompressed())
            {
                img = img.Duplicate() as Image;
                img.Decompress();
            }

            // 裁剪指定区域
            Image subImg = img.GetRegion(region);

            int width = subImg.GetWidth();
            int height = subImg.GetHeight();


            // 遍历像素，反转 Alpha
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = subImg.GetPixel(x, y);
                    c.A = 1.0f - c.A;
                    subImg.SetPixel(x, y, c);
                }
            }

            // 用处理过的 Image 创建新的 Texture2D
            return ImageTexture.CreateFromImage(subImg);
        }
    }
}
