using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace Stellaris
{
    public static class Expansions
    {
        /// <summary>
        /// 读取IList中索引为index那一项的值，如果越界则返回第一项或最后一项的值
        /// </summary>
        public static T TryGetValue<T>(this IList<T> array, int index)
        {
            if (array == null) return default;
            if (array.Count == 0) return default;
            if (index < 0) return array[0];
            if (index > array.Count - 1) return array[array.Count - 1];
            return array[index];
        }
        /// <summary>
        /// 将每一项转换成字符串然后相加
        /// </summary>
        public static string ToString_<T>(this IList<T> array)
        {
            if (array.Count == 0) return "";
            string result = "";
            for (int i = 0; i < array.Count - 1; i++)
            {
                result += array[i].ToString() + ",";
            }
            result += array[array.Count - 1].ToString();
            return result;
        }
        /// <summary>
        /// 将每一项转换成字符串然后保存到新数组中
        /// </summary>
        public static string[] ToString_Array<T>(this IList<T> array)
        {
            string[] result = new string[array.Count];
            for (int i = 0; i < array.Count; i++)
            {
                result[i] = array[i].ToString();
            }
            return result;
        }
        /// <summary>
        /// 将Stream转换成Byte数组
        /// </summary>
        public static byte[] ToByteArray(this Stream stream)
        {
            if (!stream.CanSeek) return new byte[] { 0 };
            byte[] bytes;
            stream.Position = 0;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            return bytes;
        }
        /// <summary>
        /// 将Char IList转换成int数组
        /// </summary>
        public static int[] ToCodePointArray(this IList<char> charArray)
        {
            int[] result = new int[charArray.Count];
            for (int i = 0; i < charArray.Count; i++)
            {
                result[i] = charArray[i];
            }
            return result;
        }
        public static void Plus(this int[] array, int num)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] += num;
            }
        }
        public static void Plus(this float[] array, float num)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] += num;
            }
        }
        public static void Plus(this double[] array, double num)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] += num;
            }
        }
        public static void Plus(this short[] array, short num)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] += num;
            }
        }
        public static void Plus(this Vector2[] array, Vector2 vec)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] += vec;
            }
        }
    }
}
