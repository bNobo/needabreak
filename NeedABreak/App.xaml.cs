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

//todo: ajouter un bouton "verrouiller maintenant !" en plus de "Annuler"
//todo: ajouter une étoile sur les boutons afin de permettre à l'utilisateur de choisir sa durée favorite. Cette dernière sera sélectionnée automatiquement au démarrage de l'application
//todo: reprendre le temps restant là où il était quand l'application redémarre (par exemple suite à redémarrage du pc)
//todo: détection d'autres instances sur le réseau, possibilité d'ajouter en temps qu'ami et de synchroniser les verrouillages
//todo: coche pour permettre à l'utilisateur de désactiver le balloon tip
//todo: écran à propos avec lien vers le site de l'INRS
//todo: balloon tip toutes les 20 minutes pour rappeler à l'utilisateur de quitter l'écran des yeux et regarder au loin (désactivable)
//todo: paramétrage de la durée du décompte avant verrouillage
//todo: pouvoir choisir la durée du rappel
//todo: afficher le temps restant à la seconde près au lieu de minutes
//todo: Afficher dans le tooltip l’heure à laquelle le pc va se verrouiller
namespace NeedABreak
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static DateTime startTime;
        private static System.Threading.Mutex mutex;
		public static int Delay = 10;      // Secondes	(peut être remplacé par une valeur faible pour faciliter le débug)
		//public static int Delay = 60;      // Secondes
		private Timer timer;

        static App()
        {
            // Uncomment to force a different language for UI testing
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
            ConfigureLog4Net();
        }

        public App()
        {
            Logger.Debug("App ctor start");
            mutex = new System.Threading.Mutex(false, "Local\\NeedABreakInstance");
            if (!mutex.WaitOne(0, false))
            {
                App.Logger.Info("Application already running");
                App.Current.Shutdown();
                return;
            }
            InitializeComponent();
            InitStartTime();
            if (Delay > 120)
            {
                timer = new Timer(60000);
            }
            else
            {	// Pour faciliter le débug timer toutes les 10 secondes quand délai paramétré inférieur à 2 minutes mais ça fait péter le compte à rebours (réenclenchement chaque seconde)
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
			startTime += TimeSpan.FromMinutes(5);		// On décale l'heure de début de 5 minutes ce qui va décaler d'autant l'heure de verrouillage (modification Delay interdite)
		}

        private static void ConfigureLog4Net()
        {
            // Chemin par défaut du fichier de logs (on peut le changer dans le .config)
            log4net.GlobalContext.Properties["LogFilePath"] = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "NeedABreak Logs", "needabreak.log");
            // En cas de problème coll contient la liste des erreurs
            var coll = log4net.Config.XmlConfigurator.Configure();
            Logger = LogManager.GetLogger(typeof(App));
        }
    }
}
