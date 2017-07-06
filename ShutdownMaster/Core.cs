using ShutdownMaster.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShutdownMaster
{
    public static class Core
    {
        [Obsolete("Works but use Win32Shutdown instead", false)]
        public static bool CmdShutdown(long seconds, bool forceShutdown)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                // /c tells cmd that we want to execute the command that follows (and then exit)
                Arguments = "/c shutdown /s /t " + seconds
            };

            if (forceShutdown)
                startInfo.Arguments += " /f";

            Process process = new Process();
            process.StartInfo = startInfo;

            process.Start();

            string result = process.StandardOutput.ReadToEnd();
            return string.IsNullOrWhiteSpace(result);
        }

        public static void Win32Shutdown(UInt32 seconds, bool forceShutdown, bool rebootAfterShutdown)
        {
            if (!NativeMethods.InitiateSystemShutdown(NativeMethods.LOCAL_MACHINE, String.Format("System shutdown in {0} minutes", (seconds / 60)), seconds, forceShutdown,
                 rebootAfterShutdown))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public static void Win32AbortSystemShutdown()
        {
            if (!NativeMethods.AbortSystemShutdown(null))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public static UInt32 CalculateTime(DateTime calendarDate, int hours, int minutes)
        {

            UInt32 shutdownSeconds = 0;
            DateTime currentTime = DateTime.Now;

            //Create a new DateTime with the given parameter
            DateTime shutdownDate = new DateTime(calendarDate.Year, calendarDate.Month, calendarDate.Day, hours, minutes, 0);

            if (DateTime.Compare(currentTime, shutdownDate) > 0)
                throw new Exception("Die gewählte Zeit darf nicht kleiner als die jetzige Zeit sein!");


            //Calculate the milliseconds between the shutdown date and the current time
            shutdownSeconds = Convert.ToUInt32(shutdownDate.Subtract(currentTime).TotalSeconds);

            if (shutdownSeconds > NativeMethods.MAX_SHUTDOWN_TIMEOUT)
                throw new Exception("Das gewählte Datum ist zu groß!");

            return shutdownSeconds;
        }
    }
}
