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
using PropertyChanged;
using System.ComponentModel;


namespace NeedABreak
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public int Seconds { get; set; }
        public int CentiSeconds { get; set; }
        public string TrayToolTipText { get; set; }
        public string SuspendResumeMenuItemText { get; set; }
        public string SuspendResumeMenuItemToolTip { get; set; }
        public bool IsSuspended { get { return App.IsSuspended; } }

        public MainWindowViewModel()
        {
            // Mandatory : non-null and non-empty initialisation, instead tooltip does not appear.
            TrayToolTipText = "'Need a break' just started";
            SuspendResumeMenuItemText = Properties.Resources.suspend;
            SuspendResumeMenuItemToolTip = Properties.Resources.suspend_tooltip;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void UpdateSuspendResumeMenuItemToSuspend()
        {
            SuspendResumeMenuItemText = Properties.Resources.suspend;
            SuspendResumeMenuItemToolTip = Properties.Resources.suspend_tooltip;
        }

        internal void UpdateSuspendResumeMenuItemToResume()
        {
            SuspendResumeMenuItemText = Properties.Resources.resume;
            SuspendResumeMenuItemToolTip = Properties.Resources.resume_tooltip;
        }

        internal void NotifyIsSuspendedChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSuspended)));
        }
    }
}
