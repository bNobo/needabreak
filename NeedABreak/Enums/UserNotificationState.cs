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
namespace NeedABreak.Utils
{
    public enum UserNotificationState
    {
        /// <summary>
        /// The state is unknown, this is the default value.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// A screen saver is displayed, the machine is locked,
        /// or a nonactive Fast User Switching session is in progress.
        /// </summary>
        NotPresent = 1,

        /// <summary>
        /// A full-screen application is running or Presentation Settings are applied.
        /// Presentation Settings allow a user to put their machine into a state fit
        /// for an uninterrupted presentation, such as a set of PowerPoint slides, with a single click.
        /// </summary>
        Busy = 2,

        /// <summary>
        /// A full-screen (exclusive mode) Direct3D application is running.
        /// </summary>
        RunningDirect3dFullScreen = 3,

        /// <summary>
        /// The user has activated Windows presentation settings to block notifications and pop-up messages.
        /// </summary>
        PresentationMode = 4,

        /// <summary>
        /// None of the other states are found, notifications can be freely sent.
        /// </summary>
        AcceptsNotifications = 5,

        /// <summary>
        /// Introduced in Windows 7. The current user is in "quiet time", which is the first hour after
        /// a new user logs into his or her account for the first time. During this time, most notifications
        /// should not be sent or shown. This lets a user become accustomed to a new computer system
        /// without those distractions.
        /// Quiet time also occurs for each user after an operating system upgrade or clean installation.
        /// </summary>
        QuietTime = 6
    };
}
