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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeedABreak
{
    public enum SuspensionCause
    {
        /// <summary>
        /// The suspension cause is undefined, probably becase the application is not suspended
        /// </summary>
        Undefined,
        /// <summary>
        /// The application was manually suspended by the user
        /// </summary>
        Manual,
        /// <summary>
        /// The application was self-suspended because of fullscreen application or presentation detected
        /// </summary>
        Automatic
    }
}
