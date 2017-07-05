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


using System;
using System.Diagnostics;
using System.Windows;

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
            if (calendar.SelectedDate == null || comboBoxHours.SelectedItem == null || comboBoxMinutes.SelectedItem == null || checkBoxForceShutdown.IsChecked == null)
                return;

            long? time = CalculateTime((DateTime)calendar.SelectedDate, (int)comboBoxHours.SelectedItem, (int)comboBoxMinutes.SelectedItem);
            if (time == null)
                return;

            if (CmdShutdown((long)time, (bool)checkBoxForceShutdown.IsChecked))
            {
                MessageBox.Show("Erfolgreich!" + ((bool)checkBoxForceShutdown.IsChecked).ToString(), "INFORMATION", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Nicht erfolgreich!", "FEHLER", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private long? CalculateTime(DateTime calendarDate, int hours, int minutes)
        {
            long shutdownSeconds = 0;
            DateTime currentTime = DateTime.Now;

            //Create a new DateTime with the given parameter
            DateTime shutdownDate = new DateTime(calendarDate.Year, calendarDate.Month, calendarDate.Day, hours, minutes, 0);

            try
            {
                //Calculate the milliseconds between the shutdown date and the current time
                shutdownSeconds = (long)shutdownDate.Subtract(currentTime).TotalSeconds;
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Das gewählte Datum ist zu groß!", "FEHLER", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            if (shutdownSeconds < 0)
            {
                MessageBox.Show("Das gewählte Datum ist unmöglich!", "FEHLER", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            //MessageBox.Show(shutdownSeconds.ToString());
            return shutdownSeconds;
        }


        //----------------------------------------------------------------------------------------------//


        private void buttonInstantShutdown_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxForceShutdown.IsChecked == null)
                return;

            CmdShutdown(0, (bool)checkBoxForceShutdown.IsChecked);
        }


        //----------------------------------------------------------------------------------------------//


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
    }
}
