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
using log4net;
using NeedABreak.Properties;
using NeedABreak.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace NeedABreak
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static DateTime _startCountdown;
#if !DEBUG
        private static System.Threading.Mutex mutex; 
#endif
        public static int Delay { get; set; } = Settings.Delay;
        private static Timer _timer = Delay > 120 ? new Timer(60000) : new Timer(10000);
#if DEBUG
        private static Timer _debugTimer = new Timer(1000);
#endif

        /// <summary>
        /// Vrai quand l'application est en mode "ne pas déranger"
        /// </summary>
        public static bool IsSuspended { get; set; }

        public static SuspensionCause SuspensionCause { get; set; }

        // hack: Timer to workaround an issue on Windows 11 (PreviewOpenTooltip event not raised) : https://github.com/hardcodet/wpf-notifyicon/issues/65
        private static Timer _updateToolTipTimer;

        // store dayStart to enable reset of today's screen time in the event of the user not shutting down its computer every day
        private static DateTime _dayStart;
        private static TimeSpan _cumulativeScreenTime;
        private static DateTime _startShowingScreen;

        public static ILog Logger { get; private set; }

        private static Settings Settings => Settings.Default;

        static App()
        {
            // Uncomment to force a different language for UI testing
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
            ConfigureLog4Net();

#if DEBUG
            Logger.Debug($"User settings path = {ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath}");
#endif
            TimeSpan interruptionDuration = DateTime.Now - Settings.ExitTime;

            if (Settings.DayStart == DateTime.Today)
            {
                _cumulativeScreenTime = Settings.TodayScreenTime;

                if (interruptionDuration.TotalMinutes < 5)
                {
                    // If the interruption was less than 5 minutes it is considered screen time (no break)
                    _cumulativeScreenTime += interruptionDuration;
                }
            }
            
            _dayStart = DateTime.Today;
            _startShowingScreen = DateTime.Now;

            if (interruptionDuration.TotalMinutes < 5)
            {
                // Restore countdown when interruption was less than five minutes
                _startCountdown = Settings.StartCountDown;
            }
            else
            {
                // Countdown starts now
                _startCountdown = DateTime.Now;
            }

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

            _timer.Elapsed += Timer_Elapsed;
#if DEBUG
            _debugTimer.Elapsed += _debugTimer_Elapsed;
            _debugTimer.Start();
#endif
            _updateToolTipTimer = new Timer();
            _updateToolTipTimer.Interval = 1000;
            _updateToolTipTimer.Elapsed += UpdateToolTipTimer_Elapsed;
            _updateToolTipTimer.Start();

            StartTimer();
            Microsoft.Win32.SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

            Exit += App_Exit;

            Logger.Debug("App ctor end");
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            // Store today screen time to restore it when app is launched
            Settings.TodayScreenTime = GetTodayScreenTime();
            Settings.DayStart = _dayStart;
            Settings.ExitTime = DateTime.Now;
            Settings.StartCountDown = _startCountdown;
            Settings.Save();
        }

        private async void UpdateToolTipTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await Current.Dispatcher.InvokeAsync(() =>
            {
                var mainWindow = GetMainWindow();
                mainWindow.UpdateToolTip();
            });
        }

#if DEBUG
        private void _debugTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("inactive time = " + UserActivity.GetInactiveTime());
        }
#endif

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsSuspended && SuspensionCause == SuspensionCause.Manual)
            {
                // App was manually suspended, only user can unsuspend it
                return;
            }

            if (Settings.AutomaticSuspension)
            {
                UserNotificationState state = QueryUserNotificationState.GetState();

                if (IsSuspended)
                {
                    // App was automatically suspended, shall we unsuspend it ?
                    if (state == UserNotificationState.AcceptsNotifications)
                    {
                        Resume();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    // App is not suspended, shall we automatically suspend it ?
                    switch (state)
                    {
                        case UserNotificationState.Busy:
                        case UserNotificationState.RunningDirect3dFullScreen:
                        case UserNotificationState.PresentationMode:
                            Suspend(SuspensionCause.Automatic);
                            return;
                    }
                }
            }

            double minutesLeft = GetMinutesLeft();

            if (minutesLeft <= 0)
            {
                await TimesUp();
            }
            else if (minutesLeft <= 1)
            {
                await TimesAlmostUp();
            }
        }

        private async Task TimesUp()
        {
            // stop timer to avoid reintrance in case user stay active for more than 60 seconds
            StopTimer();

            await Current.Dispatcher.Invoke(async () =>
            {
                await WaitForUserToBeIdleAsync();

                var mainWindow = GetMainWindow();
                await mainWindow.StartLockWorkStationAsync();
            });

            StartTimer();
        }

        private static void StartTimer()
        {
            _timer.Start();
        }

        private static void StopTimer()
        {
            _timer.Stop();
        }

        /// <summary>
        /// Wait for user to be idle in order to avoid annoying him.
        /// </summary>
        /// <returns></returns>
        private static Task WaitForUserToBeIdleAsync()
        {
            var userActivity = new UserActivity();

            return userActivity.WaitForUserToBeIdleAsync();
        }

        private static async Task TimesAlmostUp()
        {
            await Current.Dispatcher.InvokeAsync(() =>
            {
                var mainWindow = GetMainWindow();
                mainWindow.ShowCustomBalloon();
            });
        }

        public static double GetMinutesLeft()
        {
            var minutesElapsed = (DateTime.Now - _startCountdown).TotalMinutes;
            var minutesLeft = Delay / 60 - minutesElapsed;
            return minutesLeft;
        }

        private static MainWindow GetMainWindow()
        {
            return Current.MainWindow as MainWindow;
        }

        private static void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionUnlock)
            {
                Logger.Debug("SessionUnlock");
                
                Current.Dispatcher.BeginInvoke(() =>
                {
                    var mainWindow = GetMainWindow();
                    mainWindow.OnSessionUnlock();
                });

                if (!IsSuspended)
                {
                    ResetCountdown();
                }

                StartTimer();
                _updateToolTipTimer.Start();
                _startShowingScreen = DateTime.Now;
            }
            else if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionLock)
            {
                Logger.Debug("SessionLock");
                StopTimer();
                _updateToolTipTimer.Stop();
                _cumulativeScreenTime += DateTime.Now - _startShowingScreen;
            }
        }

        internal static void ResetCountdown()
        {
            _startCountdown = DateTime.Now;
        }

        internal static void ShiftCountdown()
        {
            _startCountdown += TimeSpan.FromMinutes(Delay / 1080d);       // time skew proportional to Delay. For a 90 minutes delay it gives 5 minutes time skew which delay lock time to 5 minutes (Delay modification is forbidden)
        }

        internal static void Suspend(SuspensionCause suspensionCause = SuspensionCause.Manual)
        {
            IsSuspended = true;
            SuspensionCause = suspensionCause;
            NotifySuspensionStateChanged();
        }

        internal static void Resume()
        {
            IsSuspended = false;
            SuspensionCause = SuspensionCause.Undefined;
            NotifySuspensionStateChanged();
        }

        private static void NotifySuspensionStateChanged()
        {
            Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var mainWindow = GetMainWindow();
                mainWindow.NotifySuspensionStateChanged();
            }));
        }

        public static TimeSpan GetTodayScreenTime()
        {
            if (_dayStart != DateTime.Today)
            {
                // day changed, reset today's screen time
                _dayStart = DateTime.Today;
                _cumulativeScreenTime = TimeSpan.Zero;
                _startShowingScreen = DateTime.Now;
            }

            return _cumulativeScreenTime + (DateTime.Now - _startShowingScreen);
        }
    }
}
