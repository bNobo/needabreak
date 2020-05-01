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
using NeedABreak.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NeedABreak
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            App.Logger.Debug("MainWindow ctor start");
            InitializeComponent();
            LoadRegistryConfig();
            ExecuteFirstRunActions();
            LoadUserSettings();
            App.Logger.Debug("MainWindow ctor end");
        }

        private void LoadUserSettings()
        {
            AutomaticSuspensionMenuItem.IsChecked = Properties.Settings.Default.AutomaticSuspension;
        }

        private void ExecuteFirstRunActions()
        {
            App.Logger.DebugFormat("IsFirstRun = {0}",
                            Properties.Settings.Default.IsFirstRun);

            if (Properties.Settings.Default.IsFirstRun)
            {
                // Setting IsChecked to true will raise Checked event
                LaunchOnStartupMenuItem.IsChecked = true;
                // Update IsFirstRun so this code won't execute next time application start
                Properties.Settings.Default.IsFirstRun = false;
                Properties.Settings.Default.Save();
            }
        }

        private void LoadRegistryConfig()
        {
            RegistryKey run = RegistryTool.GetRunRegistryKey(false);

            RegistryTool.ActOnRegistryKey(run, x =>
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
            RegistryKey run = RegistryTool.GetRunRegistryKey(true);
            RegistryTool.ActOnRegistryKey(run, x => x.DeleteValue("NeedABreak"));
        }

        private void LaunchOnStartupMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            RegistryKey run = RegistryTool.GetRunRegistryKey(true);

            RegistryTool.ActOnRegistryKey(run,
                x => x.SetValue("NeedABreak", System.Reflection.Assembly.GetExecutingAssembly().Location));
        }

        private const string CoffeeSuspendedUri = "pack://application:,,,/NeedABreak;component/coffee_suspended.ico";
        private const string CoffeeCupUri = "pack://application:,,,/NeedABreak;component/coffee cup.ico";
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
            SessionLock.LockSession();
            _imminentLocking = false;
        }

        public void ShowBalloonTip()
        {
            var template = (DataTemplate)FindResource("BalloonTipTemplate");
            var balloon = (FrameworkElement)template.LoadContent();
            balloon.Name = "uxBalloonTip";
            uxTaskbarIcon.ShowCustomBalloon(balloon, System.Windows.Controls.Primitives.PopupAnimation.Scroll, 10000);
        }

        public MainWindowViewModel GetViewModel()
        {
            return DataContext as MainWindowViewModel;
        }

        private void UpdateToolTip(string text)
        {
            var viewModel = GetViewModel();
#if DEBUG
            viewModel.TrayToolTipText = $"[DEBUG] {text}";
#else
            viewModel.TrayToolTipText = text;
#endif
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
            if (App.IsSuspended)
            {
                UpdateToolTip(Properties.Resources.suspended_title);
                return;
            }

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

        private Lazy<AboutBoxWindow> aboutBoxFactory =
            new Lazy<AboutBoxWindow>(() => new AboutBoxWindow());

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutBoxWindow aboutBox = aboutBoxFactory.Value;
            aboutBox.Show();
            aboutBox.Activate();
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
            App.InitStartTime();    // annulation, le compte à rebours repart de zéro
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

        private void LockButton_Click(object sender, RoutedEventArgs e)
        {
            Interlocked.Exchange(ref _cancellationTokenSource, new CancellationTokenSource()).Cancel();
            SessionLock.LockSession();
        }

        private void SuspendResumeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsSuspended)
            {
                App.Resume();
            }
            else
            {
                App.Suspend();
            }
        }

        public void OnSessionUnlock()
        {
            if (App.IsSuspended && App.SuspensionCause == SuspensionCause.Manual)
            {
                ShowSuspendBalloonTip();
            }
        }

        private void ShowResumeBalloonTip()
        {
            uxTaskbarIcon.ShowBalloonTip(
                Properties.Resources.resumed_title,
                Properties.Resources.resumed_message,
                Hardcodet.Wpf.TaskbarNotification.BalloonIcon.None);
        }

        private void ShowSuspendBalloonTip()
        {
            uxTaskbarIcon.ShowBalloonTip(
                Properties.Resources.suspended_title,
                Properties.Resources.suspended_message,
                Hardcodet.Wpf.TaskbarNotification.BalloonIcon.None);
        }

        public void NotifySuspensionStateChanged()
        {
            var viewModel = GetViewModel();

            if (App.IsSuspended)
            {
                viewModel.UpdateSuspendResumeMenuItemToResume();
                ShowSuspendBalloonTip();
                uxTaskbarIcon.IconSource = new BitmapImage(new Uri(CoffeeSuspendedUri, UriKind.Absolute));
            }
            else
            {
                viewModel.UpdateSuspendResumeMenuItemToSuspend();
                ShowResumeBalloonTip();
                uxTaskbarIcon.IconSource = new BitmapImage(new Uri(CoffeeCupUri, UriKind.Absolute));
            }

            viewModel.NotifyIsSuspendedChanged();
        }

        private void AutomaticSuspensionMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutomaticSuspension = true;
            Properties.Settings.Default.Save();
        }

        private void AutomaticSuspensionMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutomaticSuspension = false;
            Properties.Settings.Default.Save();

            if (App.IsSuspended && App.SuspensionCause == SuspensionCause.Automatic)
            {
                App.Resume(); 
            }
        }
    }
}
