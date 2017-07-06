/////////////////////////////////////////////////////////////////////////////////////////
//  ShutdownMaster: MainWindow 
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

using ShutdownMaster.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ShutdownMaster
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetupComboboxes();
        }

        private void SetupComboboxes()
        {
            for (int i = 0; i < 24; i++)
            {
                comboBoxHours.Items.Add(i);
            }

            for (int i = 0; i < 60; i++)
            {
                comboBoxMinutes.Items.Add(i);
            }

            comboBoxHours.SelectedItem = DateTime.Now.Hour;
            comboBoxMinutes.SelectedItem = DateTime.Now.Minute;
        }


        //----------------------------------------------------------------------------------------------//


        private void buttonShutdown_Click(object sender, RoutedEventArgs e)
        {
            if (calendar.SelectedDate == null || comboBoxHours.SelectedItem == null || comboBoxMinutes.SelectedItem == null || checkBoxForceShutdown.IsChecked == null || checkBoxReboot.IsChecked == null)
                return;

            try
            {
                UInt32 time = CalculateTime((DateTime)calendar.SelectedDate, (int)comboBoxHours.SelectedItem, (int)comboBoxMinutes.SelectedItem);

                Win32Shutdown(time, (bool)checkBoxForceShutdown.IsChecked, (bool)checkBoxReboot.IsChecked);
                MessageBox.Show("Success", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private UInt32 CalculateTime(DateTime calendarDate, int hours, int minutes)
        {

            UInt32 shutdownSeconds = 0;
            DateTime currentTime = DateTime.Now;

            //Create a new DateTime with the given parameter
            DateTime shutdownDate = new DateTime(calendarDate.Year, calendarDate.Month, calendarDate.Day, hours, minutes, 0);


            //Calculate the milliseconds between the shutdown date and the current time
            shutdownSeconds = Convert.ToUInt32(shutdownDate.Subtract(currentTime).TotalSeconds);

            if (shutdownSeconds > NativeMethods.MAX_SHUTDOWN_TIMEOUT)
                throw new Exception("Das gewählte Datum ist zu groß!");

            //MessageBox.Show(shutdownSeconds.ToString());
            return shutdownSeconds;
        }


        //----------------------------------------------------------------------------------------------//


        private void buttonInstantShutdown_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxForceShutdown.IsChecked == null || checkBoxReboot.IsChecked == null)
                return;

            try
            {
                Win32Shutdown(0, (bool)checkBoxForceShutdown.IsChecked, (bool)checkBoxReboot.IsChecked);
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //----------------------------------------------------------------------------------------------//

        [Obsolete("Works but use Win32Shutdown instead", false)]
        private bool CmdShutdown(long seconds, bool forceShutdown)
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

        private void Win32Shutdown(UInt32 seconds, bool forceShutdown, bool rebootAfterShutdown)
        {
            if (!NativeMethods.InitiateSystemShutdown(NativeMethods.LOCAL_MACHINE, "Hi", seconds, forceShutdown,
                 rebootAfterShutdown))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }


        //----------------------------------------------------------------------------------------------//

        private void Calendar_OnLoaded(object sender, RoutedEventArgs e)
        {
            //Blocks all dates in past
            calendar.BlackoutDates.AddDatesInPast();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
