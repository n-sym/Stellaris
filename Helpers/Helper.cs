using Microsoft.Xna.Framework;
using Stellaris.Curves;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stellaris
{
    public static class Helper
    {
        static int seed = 0;
        /// <summary>
        /// 读取数组中索引为index那一项的值，如果越界则返回第一项或最后一项的值
        /// </summary>
        public static T TryGetValue<T>(this T[] array, int index)
        {
            if (array == null) return default;
            if (array.Length == 0) return default;
            if (index < 0) return array[0];
            if (index > array.Length - 1) return array[array.Length - 1];
            return array[index];
        }
        public static T TryGetValue<T>(this List<T> list, int index)
        {
            if (list == null) return default;
            if (list.Count == 0) return default;
            if (index < 0) return list[0];
            if (index > list.Count - 1) return list.Last();
            return list[index];
        }
        public static string ToStringAlt<T>(this T[] array)
        {
            string result = "";
            for (int i = 0; i < array.Length; i++)
            {
                result += array[i].ToString() + ",";
            }
            return result;
        }
        public static string[] ToStringArray<T>(this T[] array)
        {
            string[] result = new string[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i].ToString();
            }
            return result;
        }
        public static T[] CutOut<T>(this T[] array, int start, int end)
        {
            T[] result = new T[end - start + 1];
            for (int i = start; i < end + 1; i++)
            {
                result[i - start] = array.TryGetValue(i);
            }
            return result;
        }
        public static void Plus(this short[] array, short num)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] += num;
            }
        }
        public static Vector2 RandomAngleVec(float length, float min = 0f, float max = 6.283f)
        {
            Random random = seed == 0 ? new Random() : new Random(seed);
            float radian = min.LinearInterpolationTo(max, (float)random.NextDouble(), 1f);
            float c = (float)Math.Cos(radian);
            float s = (float)Math.Sin(radian);
            seed = random.Next();
            return new Vector2(c * length, s * length);
        }
        public static Vector2 RandomAngleVec(float length, Vector2 center, float min = 0f, float max = 6.283f)
        {
            Random random = new Random();
            float radian = min.LinearInterpolationTo(max, (float)random.NextDouble(), 1f);
            float c = (float)Math.Cos(radian);
            float s = (float)Math.Sin(radian);
            return new Vector2(c * length, s * length) + center;
        }
        public static float AngleBetween(Vector2 a, Vector2 b)
        {
            return Math.Abs(a.Angle() - b.Angle());
        }
        public static Color ToColor(this Vector4 vec)
        {
            return new Color(vec.X, vec.Y, vec.Z, vec.W);
        }
        /// <summary>
        /// 线性插值
        /// </summary>
        public static float LinearInterpolation(float a, float b, float progress, float max)
        {
            return progress / max * b + (max - progress) / max * a;
        }
        /// <summary>
        /// 线性插值
        /// </summary>
        public static float LinearInterpolationTo(this float a, float b, float progress, float max)
        {
            return progress / max * b + (max - progress) / max * a;
        }
        /// <summary>
        /// 线性插值
        /// </summary>
        public static Vector2 LinearInterpolation(Vector2 a, Vector2 b, float progress, float max)
        {
            return progress / max * b + (max - progress) / max * a;
        }
        /// <summary>
        /// 拉格朗日插值
        /// </summary>
        public static float LagrangeInterpolation(float[] x, float[] y, float n)
        {
            float result = 0;
            for (int i = 0; i < x.Length; i++)
            {
                float item = 1;
                for (int j = 0; j < x.Length; j++)
                {
                    if (j == i) continue;
                    item *= (n - x[j]) / (x[i] - x[j]);
                }
                result += item * y[i];
            }
            return result;
        }
        /// <summary>
        /// 拉格朗日插值
        /// </summary>
        /// <param name="data">样本数据</param>
        /// <param name="precision">精度</param>
        /// <returns>插值过后的数据</returns>
        public static Vector2[] LagrangeInterpolation(Vector2[] data, int precision)
        {
            if (precision < 2) return new Vector2[1] { Vector2.Zero };
            Vector2[] result = new Vector2[data.Length * (precision + 1) - 2];
            float[] x = new float[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                x[i] = data[i].X;
            }
            float[] y = new float[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                y[i] = data[i].Y;
            }
            for (int i = 0; i < data.Length - 1; i++)
            {
                for (int j = 0; j < precision + 1; j++)
                {
                    float n = LinearInterpolation(x[i], x[i + 1], j, precision + 1);
                    result[i * (precision + 1) + j] = new Vector2(n, LagrangeInterpolation(x, y, n));
                }
            }
            result[result.Length - 1] = data[data.Length - 1];
            return result;
        }
        /// <summary>
        /// 拉格朗日插值
        /// </summary>
        /// <param name="data">样本数据</param>
        /// <param name="start">首项索引</param>
        /// <param name="end">末项索引</param>
        /// <param name="precision">精度</param>
        /// <returns></returns>
        public static Vector2[] LagrangeInterpolation(Vector2[] data, int start, int end, int precision)
        {
            Vector2[] result = new Vector2[(end + 1 - start) * (precision + 1) - 2];
            float[] x = new float[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                x[i] = data[i].X;
            }
            float[] y = new float[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                y[i] = data[i].Y;
            }
            for (int i = start; i < end; i++)
            {
                for (int j = 0; j < precision + 1; j++)
                {
                    float n = LinearInterpolation(x[i], x[i + 1], j, precision + 1);
                    result[(i - start) * (precision + 1) + j] = new Vector2(n, LagrangeInterpolation(x, y, n));
                }
            }
            result[result.Length - 1] = data[end];
            return result;
        }
        public static float ParabolicInterpolation(float a, float b, float progress, float max, CurveConcavity concavity)
        {
            if (concavity == CurveConcavity.Convex)
            {
                return LinearInterpolation(a, b, progress * progress / max, max);
            }
            else
            {
                progress -= max;
                return LinearInterpolation(a, b, -progress * progress / max + max, max);
            }
        }
    }
}
