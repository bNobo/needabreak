/*
 NEED A BREAK is an application intended to help you take care of your health while you work on a computer. 
 It will encourage you to regularly have a break in order to rest your back and your eyes.
    Copyright (C) 2020  Benoit Rocco

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace NeedABreak
{
    /// <summary>
    /// Interaction logic for AboutBoxWindow.xaml
    /// </summary>
    public partial class AboutBoxWindow : MetroWindow
    {
        public AboutBoxWindow()
        {
            InitializeComponent();
            Closing += AboutBoxWindow_Closing;

            Version.Text = $"v{Assembly.GetEntryAssembly().GetName().Version}";
        }

        private void AboutBoxWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo(e.Uri.AbsoluteUri);
            psi.UseShellExecute = true;
            Process.Start(psi);

            e.Handled = true;
        }
    }
}
