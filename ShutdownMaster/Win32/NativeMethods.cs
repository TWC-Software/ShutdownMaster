using System;
using System.Runtime.InteropServices;

namespace ShutdownMaster.Win32
{
    public static class NativeMethods
    {
        [DllImport(DLLFiles.ADVAPI32, EntryPoint = "InitiateSystemShutdownW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InitiateSystemShutdown(
            string lpMachineName,
            string lpMessage,
            UInt32 dwTimeout,
            bool bForceAppsClosed,
            bool bRebootAfterShutdown);

        public const string LOCAL_MACHINE = null;
        public const UInt32 MAX_SHUTDOWN_TIMEOUT = 10 * 365 * 24 * 60 * 60;
    }
}
