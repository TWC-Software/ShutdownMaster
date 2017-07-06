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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
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
                UInt32 time = Core.CalculateTime((DateTime)calendar.SelectedDate, (int)comboBoxHours.SelectedItem, (int)comboBoxMinutes.SelectedItem);

                Core.Win32Shutdown(time, (bool)checkBoxForceShutdown.IsChecked, (bool)checkBoxReboot.IsChecked);
                MessageBox.Show("Success", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //----------------------------------------------------------------------------------------------//


        private void buttonInstantShutdown_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxForceShutdown.IsChecked == null || checkBoxReboot.IsChecked == null)
                return;

            try
            {
                Core.Win32Shutdown(0, (bool)checkBoxForceShutdown.IsChecked, (bool)checkBoxReboot.IsChecked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //----------------------------------------------------------------------------------------------//


        private void buttonAbortShutdown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Core.Win32AbortSystemShutdown();
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
