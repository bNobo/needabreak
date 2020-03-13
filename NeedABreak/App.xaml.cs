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

//todo: add a "lock now !" button on the main window
//todo: add a star button to allow user to select his favorite time frame. The selected time frame will be automatically selected at startup.
//todo: store time left before screen locking in cas of reboot
//todo: detect other instances on the same network to synchronize break with friends
//todo: checkbox to disable balloon tip
//todo: About box with link to INRS site and license
//todo: balloon tip to remind user about looking away from screen every 20 minutes (deactivable)
//todo: postpone delay configuration
namespace NeedABreak
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static DateTime startTime;
        private static System.Threading.Mutex mutex;
		public static int Delay = 5400;      // Seconds	(put a low value here to facilitate debugging)
		private Timer timer;

        static App()
        {
            // Uncomment to force a different language for UI testing
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
            ConfigureLog4Net();
        }

        public App()
        {
            Logger.Debug("App ctor start");
            mutex = new System.Threading.Mutex(false, "Local\\NeedABreakInstance");
            if (!mutex.WaitOne(0, false))
            {
                Logger.Info("Application already running");
                Current.Shutdown();
                return;
            }
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
                InitStartTime();
                timer.Start();
            }
            else if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionLock)
            {
                timer.Stop();
            }
        }

        internal static void InitStartTime()
        {
            startTime = DateTime.UtcNow;
        }

		internal static void ShiftStartTime()
		{
			startTime += TimeSpan.FromMinutes(5);		// 5 minutes time skew which delay lock time to 5 minutes (Delay modification is forbidden)
		}

        private static void ConfigureLog4Net()
        {
            // default log path (can be changed in .config)
            log4net.GlobalContext.Properties["LogFilePath"] = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "NeedABreak Logs", "needabreak.log");
            log4net.Config.XmlConfigurator.Configure();
            Logger = LogManager.GetLogger(typeof(App));
        }
    }
}
