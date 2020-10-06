using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Stellaris
{
    internal class NativeMethods : NativeLibrary
    {
        static string path
        {
            get
            {
                string fileName = "NativeMethods_Linux.so";
                string path = Path.Combine(Ste.CurrentDirectory, fileName);
                using (Stream s = File.Create(path))
                {
                    using (Stream t = typeof(Ste).Assembly.GetManifestResourceStream("Stellaris.Main." + fileName))
                    {
                        t.CopyTo(s);
                    }
                }
                return path;
            }
        }
        public NativeMethods() : base(path)
        {
        }
    }
    /// <summary>
    /// 提供当前平台可用的管理本机库的简单封装
    /// </summary>
    public unsafe class NativeLibrary : IDisposable
    {
        void* libraryHandle;
        bool disposed;
        /// <summary>
        /// 从路径加载一个本机库
        /// </summary>
        public NativeLibrary(string path)
        {
            libraryHandle = PlatfromLoadLibrary(path);
        }
        /// <summary>
        /// 借助Marshal，绑定方法到委托上
        /// </summary>
        /// <typeparam name="T">非泛型委托</typeparam>
        /// <param name="name">方法名</param>
        /// <returns></returns>
        public T GetMethodDelegate<T>(string name)
        {
            return Marshal.GetDelegateForFunctionPointer<T>((IntPtr)PlatfromFindMethod(name));
        }
        /// <summary>
        /// 获取指向方法的函数指针
        /// </summary>
        /// <param name="name">方法名</param>
        /// <returns></returns>
        public void* GetMethodPtr(string name)
        {
            return PlatfromFindMethod(name);
        }
        /// <summary>
        /// 释放库
        /// </summary>
        ~NativeLibrary()
        {
            if (!disposed) Dispose();
        }
        /// <summary>
        /// 释放库
        /// </summary>
        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)libraryHandle);
            disposed = true;
        }

        private void* PlatfromLoadLibrary(string file)
        {
            return dlopen(file, 0x0001);
        }
        private void* PlatfromFindMethod(string name)
        {
            return dlsym(libraryHandle, name);
        }

        [DllImport("libdl.so.2")]
        public static extern void* dlopen(string path, int flags);

        [DllImport("libdl.so.2")]
        public static extern void* dlsym(void* handle, string symbol);
    }
}
