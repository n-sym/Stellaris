using Microsoft.Xna.Framework;
using System;
using System.Reflection;

namespace Stellaris.Main
{
    internal static class ReflectionHelper
    {
        static Assembly mg = typeof(Vector2).Assembly;
        public static Type GetMGClass(string name)
        {
            //throw new Exception(mg.GetTypes().ToString_());
            return mg.GetType(name, true);
        }
        public static FieldInfo GetPublicStaticField(this Type type, string name)
        {
            return type.GetField(name, BindingFlags.Public | BindingFlags.Static);
        }
        public static MethodInfo GetPublicInstanceMethod(this Type type, string name)
        {
            return type.GetMethod(name, BindingFlags.Public | BindingFlags.Instance);
        }
        public static MethodInfo GetPublicStaticMethod(this Type type, string name)
        {
            return type.GetMethod(name, BindingFlags.Public | BindingFlags.Static);
        }
    }
}
