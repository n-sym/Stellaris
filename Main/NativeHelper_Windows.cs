using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Stellaris
{
    internal class NativeMethods : NativeLibrary
    {
        static string path
        {
            get
            {
                string fileName = "NativeMethods_Windows.dll";
                string path = Path.Combine(Ste.CurrentDirectory, "runtimes", fileName);
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
            return LoadLibraryW(file);
        }
        private void* PlatfromFindMethod(string name)
        {
            return GetProcAddress(libraryHandle, name);
        }

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern void* GetProcAddress(void* hModule, string procName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern void* LoadLibraryW(string lpszLib);
    }
    internal static class LibraryHelper
    {
        public static IntPtr Load(string fileName)
        {
            if (Ste.Platform == Platform.Windows)
            {
                return LibraryHelper_Windows.LoadLibraryW(fileName);
            }
            if (Ste.Platform == Platform.Linux)
            {
                return LibraryHelper_Linux.dlopen(fileName, 0x0001);
            }
            return LibraryHelper_Android.dlopen(fileName, 0x0001);
        }
        public static IntPtr Find(IntPtr handle, string name)
        {
            if (Ste.Platform == Platform.Windows)
            {
                return LibraryHelper_Windows.GetProcAddress(handle, name);
            }
            if (Ste.Platform == Platform.Linux)
            {
                return LibraryHelper_Linux.dlsym(handle, name);
            }
            return LibraryHelper_Android.dlsym(handle, name);
        }
    }
    internal static class LibraryHelper_Windows
    {
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibraryW(string lpszLib);
    }
    internal static class LibraryHelper_Linux
    {
        [DllImport("libdl.so.2")]
        public static extern IntPtr dlopen(string path, int flags);

        [DllImport("libdl.so.2")]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);
    }
    internal static class LibraryHelper_Android
    {
        [DllImport("dl")]
        public static extern IntPtr dlopen(string path, int flags);

        [DllImport("dl")]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);
    }
}
