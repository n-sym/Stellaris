using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris
{
    internal static class NativeMethods
    {
        static IntPtr nativeMethods;
        public static void Initialize()
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
            nativeMethods = LibraryHelper.Load(path);
        }
        public static T GetMethod<T>(string name)
        {
            return Marshal.GetDelegateForFunctionPointer<T>(LibraryHelper.Find(nativeMethods, name));
        }
    }
    #region LibraryHelper
    public static class LibraryHelper
    {
        public static IntPtr Load(string fileName)
        {
            if(Ste.Platform == Platform.Windows)
            {
                return LibraryHelper_Windows.LoadLibraryW(fileName);
            }
            if(Ste.Platform == Platform.Linux)
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
