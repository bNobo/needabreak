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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NeedABreak.Utils
{
    public static class QueryUserNotificationState
    {
        [DllImport("shell32.dll")]
        static extern int SHQueryUserNotificationState(out UserNotificationState userNotificationState);

        public static UserNotificationState GetState()
        {
            UserNotificationState state;
            
            int res = SHQueryUserNotificationState(out state);

            if (res != 0)
            {
                App.Logger.Error($"SHQueryUserNotificationState returned an error, HRESULT = 0x{res:X8}");
            }

            return state;
        }
    }
}
