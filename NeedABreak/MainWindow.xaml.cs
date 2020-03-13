/*
 NEED A BREAK is an application intended to help you take care of your health while you work on a computer. 
 It will encourage you to regularly have a break in order to rest your back and your eyes.
    Copyright (C) 2020  Benoît Rocco

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
using Microsoft.Win32;
using PropertyChanged;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NeedABreak
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        public MainWindow()
        {
            App.Logger.Debug("MainWindow ctor start");
            InitializeComponent();
            LoadRegistryConfig();
            App.Logger.Debug("MainWindow ctor end");
        }

        private void LoadRegistryConfig()
        {
            RegistryKey run = GetRunRegistryKey(false);

            ActOnRegistryKey(run, x =>
            {
                string needABreak = (string)run.GetValue("NeedABreak");

                if (needABreak != null)
                {
                    LaunchOnStartupMenuItem.IsChecked = true;
                }
            });

            LaunchOnStartupMenuItem.Checked += LaunchOnStartupMenuItem_Checked;
            LaunchOnStartupMenuItem.Unchecked += LaunchOnStartupMenuItem_Unchecked;
        }

        private void LaunchOnStartupMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            RegistryKey run = GetRunRegistryKey(true);
            ActOnRegistryKey(run, x => x.DeleteValue("NeedABreak"));
        }

        private void LaunchOnStartupMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            RegistryKey run = GetRunRegistryKey(true);

            ActOnRegistryKey(run,
                x => x.SetValue("NeedABreak", System.Reflection.Assembly.GetExecutingAssembly().Location));
        }

        /// <summary>
        /// Vérifie que la clé n'est pas nulle avant d'effectuer une action dessus. Ferme et dispose la
        /// clé une fois terminé.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        private static void ActOnRegistryKey(RegistryKey key, Action<RegistryKey> action)
        {
            if (key != null)
            {
                action(key);
                key.Close();
                key.Dispose();
            }
        }

        private static RegistryKey GetRunRegistryKey(bool writable)
        {
            //var run = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            //string s = @"Software\Microsoft\Windows\CurrentVersion\Run";
            //string s = "Software" + '\\' + "Microsoft" + '\\' + "Windows" + '\\' + "CurrentVersion" + '\\' + "Run";
            //StringBuilder sb = new StringBuilder(@"Software\Microsoft\Windows\CurrentVersion\Run");

            // Si je mets des \ dans la chaine ça fait planter Fody lors de la compil :/
            return Registry.CurrentUser.OpenSubKey("Software/Microsoft/Windows/CurrentVersion/Run"
                .Replace('/', '\\'), writable);
        }

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private bool _imminentLocking = false;

        public async Task StartLockWorkStationAsync()
        {
            _imminentLocking = true;
            var viewModel = GetViewModel();
            Show();
            var token = _cancellationTokenSource.Token;

            for (int i = 1000; i >= 0; i--)
            {
                viewModel.CentiSeconds = i;
                viewModel.Seconds = i / 100;

                try
                {
                    await Task.Delay(10, token);
                }
                catch (TaskCanceledException)
                {
                    Hide();
                    _imminentLocking = false;
                    return;
                }

                if (token.IsCancellationRequested)
                {
                    Hide();
                    _imminentLocking = false;
                    return;
                }
            }

            Hide();
            LockWorkStation();
            _imminentLocking = false;
        }

		public void ShowBalloonTip()
		{
			//uxTaskbarIcon.ShowBalloonTip(Properties.Resources.verrouillage_imminent_title, Properties.Resources.verrouillage_imminent_detail, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
			var template = (DataTemplate)FindResource("BalloonTipTemplate");
			var balloon = (FrameworkElement)template.LoadContent();
			balloon.Name = "uxBalloonTip";
			uxTaskbarIcon.ShowCustomBalloon(balloon, System.Windows.Controls.Primitives.PopupAnimation.Scroll, 10000);
		}

		private MainWindowViewModel GetViewModel()
        {
            return DataContext as MainWindowViewModel;
        }

        private void UpdateToolTip(string text)
        {
            var viewModel = GetViewModel();
            viewModel.TrayToolTipText = text;
        }

		private static void ShowSettingsWindow()
		{
			Window settingsWindow = null;

			foreach (Window window in App.Current.Windows)
			{
				if (window is SettingsWindow)
				{
					settingsWindow = window;
					break;
				}
			}

			if (settingsWindow == null)
			{
				settingsWindow = new SettingsWindow();
				settingsWindow.DataContext = new SettingsWindowViewModel();
			}

			settingsWindow.Show();
			settingsWindow.Activate();
		}

		private void TaskbarIcon_PreviewTrayToolTipOpen(object sender, RoutedEventArgs e)
        {
            if (_imminentLocking)
            {
                UpdateToolTip(Properties.Resources.Imminent_locking);
                return;
            }

            var minutesLeft = App.GetMinutesLeft();

            if (minutesLeft <= 1)
            {
                UpdateToolTip(Properties.Resources.Less_than_a_minute_before_locking);
                return;
            }

            minutesLeft = Math.Round(minutesLeft);

            if (minutesLeft == 1)
            {
                UpdateToolTip(Properties.Resources.one_minute_before_locking);
            }
            else
            {
                UpdateToolTip(string.Format(Properties.Resources.minutes_before_locking, minutesLeft));
            }
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ShowSettingsWindow();
		}

		private void CloseBalloon_Click(object sender, RoutedEventArgs e)
		{
			uxTaskbarIcon.CloseBalloon();
		}
		
		private void ReporterBalloon_Click(object sender, RoutedEventArgs e)
		{
			uxTaskbarIcon.CloseBalloon();
			App.ShiftStartTime();		
		}

		private void AnnulerBalloon_Click(object sender, RoutedEventArgs e)
		{
			uxTaskbarIcon.CloseBalloon();
			App.InitStartTime();	// annulation, le compte à rebours repart de zéro
		}
		
		private void ReporterButton_Click(object sender, RoutedEventArgs e)
		{
			Interlocked.Exchange(ref _cancellationTokenSource, new CancellationTokenSource()).Cancel();
			App.ShiftStartTime();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Interlocked.Exchange(ref _cancellationTokenSource, new CancellationTokenSource()).Cancel();
			App.InitStartTime();
		}
	}
}
