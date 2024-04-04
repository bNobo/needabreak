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
using Microsoft.Win32;
using System;

namespace NeedABreak.Utils
{
    public static class RegistryTool
    {
        /// <summary>
        /// Check that key is not null then close and dispose it
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public static void ActOnRegistryKey(RegistryKey key, Action<RegistryKey> action)
        {
            if (key != null)
            {
                action(key);
                key.Close();
                key.Dispose();
            }
        }

        public static RegistryKey GetRunRegistryKey(bool writable)
        {
            // Not possible to put \ in string because it breaks Fody during build :/ 
            // Using Replace to workaround the problem
            return Registry.CurrentUser
                .OpenSubKey("Software/Microsoft/Windows/CurrentVersion/Run"
                .Replace('/', '\\'), writable);
        }

    }
}
