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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace NeedABreak
{
    public class BorderSelectedAdorner : Adorner
    {
        public BorderSelectedAdorner(UIElement adornedElement)
            : base(adornedElement)
        {

        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.RenderSize);
            var brush = new SolidColorBrush(Color.FromRgb(0x00, 0xB6, 0x7C));
            var pen = new Pen(brush, 6);
            var whitePen = new Pen(Brushes.White, 8);
            drawingContext.DrawRectangle(Brushes.Transparent, pen, adornedElementRect);
            var miniRect = new Rect(adornedElementRect.Right - 50, adornedElementRect.Bottom - 50, 
                50, 50);
            drawingContext.DrawRectangle(brush, pen, miniRect);
            var p1 = new Point(adornedElementRect.Right - 40, adornedElementRect.Bottom - 25);
            var p2 = new Point(p1.X + 10, p1.Y + 10);
            var p4 = new Point(p1.X + 7, p1.Y + 7);
            var p3 = new Point(p2.X + 20, p2.Y - 20);
            drawingContext.DrawLine(whitePen, p1, p2);
            drawingContext.DrawLine(whitePen, p4, p3);
        }

        
    }
}
