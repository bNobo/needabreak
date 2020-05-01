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
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;

namespace NeedABreak
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static DateTime startTime;
#if !DEBUG
        private static System.Threading.Mutex mutex; 
#endif
        public static int Delay { get; set; } = 5400;      // Seconds	(put a low value here to facilitate debugging)
        private static Timer timer;
        private static DateTime suspendTime;               // Time when App was suspended
        private static bool isSuspended = false;

        public static bool IsSuspended 
        { 
            get => isSuspended; 
            set 
            { 
                isSuspended = value;
                var mainWindowViewModel = GetMainWindow().GetViewModel();
                mainWindowViewModel.NotifyIsSuspendedChanged();
            } 
        }
        static App()
        {
            // Uncomment to force a different language for UI testing
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
            ConfigureLog4Net();
        }

        private static void ConfigureLog4Net()
        {
            // default log path (can be changed in .config)
            log4net.GlobalContext.Properties["LogFilePath"] = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "NeedABreak Logs", "needabreak.log");
            log4net.Config.XmlConfigurator.Configure();
            Logger = LogManager.GetLogger(typeof(App));
        }

        public App()
        {
            Logger.Debug("App ctor start");
#if !DEBUG
            mutex = new System.Threading.Mutex(false, "Local\\NeedABreakInstance");
            if (!mutex.WaitOne(0, false))
            {
                Logger.Info("Application already running");
                Current.Shutdown();
                return;
            } 
#endif
            InitializeComponent();
            InitStartTime();
            if (Delay > 120)
            {
                timer = new Timer(60000);
            }
            else
            {	// timer every 10 secondes to facilitate debug when delay is less than 2 minutes, caveat : it breaks countdown (reinit each second)
                timer = new Timer(10000);
            }
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            Microsoft.Win32.SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            Logger.Debug("App ctor end");
        }

        public static ILog Logger { get; private set; }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            double minutesLeft = GetMinutesLeft();

            if (minutesLeft <= 0)
            {
                if (Delay <= 0)
                {
                    timer.Stop();
                }

                await Current.Dispatcher.InvokeAsync(async () =>
                {
                    var mainWindow = GetMainWindow();
                    await mainWindow.StartLockWorkStationAsync()
                        .ConfigureAwait(false);
                });
            }
            else if (minutesLeft <= 1)
            {
                await Current.Dispatcher.InvokeAsync(() =>
                {
                    var mainWindow = GetMainWindow();
                    mainWindow.ShowBalloonTip();
                });
            }
        }

        public static double GetMinutesLeft()
        {
            var minutesElapsed = (DateTime.UtcNow - startTime).TotalMinutes;
            var minutesLeft = Delay / 60 - minutesElapsed;
            return minutesLeft;
        }

        private static MainWindow GetMainWindow()
        {
            return Application.Current.MainWindow as MainWindow;
        }

        private void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionUnlock)
            {
                var mainWindow = GetMainWindow();
                mainWindow.OnSessionUnlock();

                if (IsSuspended)
                {
                    return;
                }

                InitStartTime();
                timer.Start();
            }
            else if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionLock)
            {
                if (IsSuspended)
                {
                    return;
                }

                timer.Stop();
            }
        }

        internal static void InitStartTime()
        {
            startTime = DateTime.UtcNow;
        }

        internal static void ShiftStartTime()
        {
            startTime += TimeSpan.FromMinutes(5);       // 5 minutes time skew which delay lock time to 5 minutes (Delay modification is forbidden)
        }

        internal static void Suspend()
        {
            timer.Stop();
            suspendTime = DateTime.UtcNow;
            IsSuspended = true;
        }

        internal static void Resume()
        {
            var elapsedTime = (suspendTime - startTime).TotalMinutes;
            startTime = DateTime.UtcNow.AddMinutes(-elapsedTime);
            timer.Start();
            IsSuspended = false;
        }
    }
}
