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
using NeedABreak.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace NeedABreak.Utils
{
    [ContentProperty("Name")]
    public class TextResourceExtension : MarkupExtension
    {
        public string Name { get; set; }

        public TextResourceExtension()
        {
            Name = "undefined";
        }

        public TextResourceExtension(string name)
        {
            Name = name;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                // Message à destination du développeur, n'a pas besoin d'être traduit
                return "Empty resource name";
            }

            var translation = Resources.ResourceManager.GetString(Name);

            if (translation == null)
            {
                // Message à destination du développeur, n'a pas besoin d'être traduit
                return string.Format("[{0}] missing", Name);
            }

            return translation;
        }
    }
}
