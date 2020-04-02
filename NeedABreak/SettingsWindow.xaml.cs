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
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace NeedABreak
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
            //DataContextChanged += SettingsWindow_DataContextChanged;
            Loaded += SettingsWindow_Loaded;
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var delay = (int)Tag;
            FrameworkElement element;

            if (delay == 45)
            {
                element = Border45;
            }
            else if (delay == 60)
            {
                element = Border60;
            }
            else if (delay == 90)
            {
                element = Border90;
            }
            else
            {
                element = Border120;
            }

            AddBorderSelectedAdorner(element);
        }

        private void SettingsWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var delay = (int)Tag;
            FrameworkElement element;

            if (delay == 45)
            {
                element = Border45;
            }
            else if (delay == 60)
            {
                element = Border60;
            }
            else if (delay == 90)
            {
                element = Border90;
            }
            else
            {
                element = Border120;
            }

            AddBorderSelectedAdorner(element);
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            var element = sender as Border;
            element.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xD3, 0xB2));
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            var element = sender as Border;
            element.Background = new SolidColorBrush(Color.FromRgb(0xA6, 0xE5, 0xD1));
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearAdorners();
            var element = sender as FrameworkElement;
            AddBorderSelectedAdorner(element);

            if (element.Name == "Border45")
            {
                SetCurrentValue(TagProperty, 45);
            }
            else if (element.Name == "Border60")
            {
                SetCurrentValue(TagProperty, 60);
            }
            else if ((element.Name == "Border90"))
            {
                SetCurrentValue(TagProperty, 90);
            }
            else
            {
                SetCurrentValue(TagProperty, 120);
            }
        }

        private static void AddBorderSelectedAdorner(FrameworkElement element)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            adornerLayer.Add(new BorderSelectedAdorner(element));
        }

        private void ClearAdorners()
        {
            RemoveAdorners(Border45, Border60, Border90, Border120);
        }

        private void RemoveAdorners(params UIElement[] elements)
        {
            foreach (var element in elements)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(element);
                var adorners = adornerLayer.GetAdorners(element);

                if (adorners != null)
                {
                    foreach (var adorner in adorners)
                    {
                        adornerLayer.Remove(adorner);
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void LockNow(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("rundll32.exe", "user32.dll LockWorkStation");



        }

    }
}
