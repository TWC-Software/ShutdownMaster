/////////////////////////////////////////////////////////////////////////////////////////
//  ShutdownMaster: NativeMethods 
//  
//  This file is part of ShutdownMaster.
// 
//  ShutdownMaster is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//   ShutdownMaster is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//
//   You should have received a copy of the GNU General Public License
//   along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
/////////////////////////////////////////////////////////////////////////////////////////

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
