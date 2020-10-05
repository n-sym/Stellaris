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
                string fileName = "NativeMethods." + (Ste.Platform == Platform.Windows ? "dll" : "so");
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
    public unsafe class NativeLibrary : IDisposable
    {
        IntPtr nativeMethods;
        bool disposed;
        public NativeLibrary(string path)
        {
            nativeMethods = LibraryHelper.Load(path);
        }
        public T GetMethodDelegate<T>(string name)
        {
            return Marshal.GetDelegateForFunctionPointer<T>(LibraryHelper.Find(nativeMethods, name));
        }
        public void* GetMethodPtr(string name)
        {
            return LibraryHelper.Find(nativeMethods, name).ToPointer();
        }
        ~NativeLibrary()
        {
            if (!disposed) Dispose();
        }
        public void Dispose()
        {
            Marshal.FreeHGlobal(nativeMethods);
            disposed = true;
        }
    }
    #region LibraryHelper
    public static class LibraryHelper
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
    #endregion
}
