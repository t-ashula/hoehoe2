// Tween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
// All rights reserved.
//
// This file is part of Tween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hoehoe
{
    public class ToolStripAPIGauge : ToolStripControlHost
    {
        private Size originalSize;

        public ToolStripAPIGauge()
            : base(new Control())
        {
            this.AutoToolTip = true;
            this.Control.Paint += Draw;
            this.Control.TextChanged += Control_TextChanged;
            this.Control.SizeChanged += Control_SizeChanged;
        }

        private int _gaugeHeight;

        public int GaugeHeight
        {
            get { return _gaugeHeight; }
            set
            {
                this._gaugeHeight = value;
                if (this.Control != null && !this.Control.IsDisposed)
                {
                    this.Control.Refresh();
                }
            }
        }

        private int _maxCount = 350;

        public int MaxCount
        {
            get { return this._maxCount; }
            set
            {
                this._maxCount = value;
                if (this.Control != null && !this.Control.IsDisposed)
                {
                    this.SetText(this._remainCount, this._maxCount);
                    this.Control.Refresh();
                }
            }
        }

        private int _remainCount;

        public int RemainCount
        {
            get { return this._remainCount; }
            set
            {
                this._remainCount = value;
                if (this.Control != null && !this.Control.IsDisposed)
                {
                    this.SetText(this._remainCount, this._maxCount);
                    this.Control.Refresh();
                }
            }
        }

        private DateTime _resetTime;

        public DateTime ResetTime
        {
            get { return this._resetTime; }
            set
            {
                this._resetTime = value;
                if (this.Control != null && !this.Control.IsDisposed)
                {
                    this.SetText(this._remainCount, this._maxCount);
                    this.Control.Refresh();
                }
            }
        }

        private void Draw(object sender, PaintEventArgs e)
        {
            double minute = (this.ResetTime - DateTime.Now).TotalMinutes;
            Rectangle apiGaugeBounds = new Rectangle(0, Convert.ToInt32((this.Control.Height - (this._gaugeHeight * 2)) / 2), Convert.ToInt32(this.Control.Width * (this.RemainCount / this._maxCount)), this._gaugeHeight);
            Rectangle timeGaugeBounds = new Rectangle(0, apiGaugeBounds.Top + this._gaugeHeight, Convert.ToInt32(this.Control.Width * (minute / 60)), this._gaugeHeight);
            e.Graphics.FillRectangle(Brushes.LightBlue, apiGaugeBounds);
            e.Graphics.FillRectangle(Brushes.LightPink, timeGaugeBounds);
            e.Graphics.DrawString(this.Control.Text, this.Control.Font, SystemBrushes.ControlText, 0, Convert.ToSingle(timeGaugeBounds.Top - (this.Control.Font.Height / 2)));
        }

        private void Control_TextChanged(object sender, EventArgs e)
        {
            this.Control.SizeChanged -= this.Control_SizeChanged;
            using (Graphics g = this.Control.CreateGraphics())
            {
                this.Control.Size = new Size(Convert.ToInt32(Math.Max(g.MeasureString(this.Control.Text, this.Control.Font).Width, this.originalSize.Width)), this.Control.Size.Height);
            }
            this.Control.SizeChanged += this.Control_SizeChanged;
        }

        private void Control_SizeChanged(object sender, EventArgs e)
        {
            this.originalSize = this.Control.Size;
        }

        private void SetText(int remain, int max)
        {
            string textFormat = "API {0}/{1}";
            string toolTipTextFormat = "API rest {0}/{1}" + Environment.NewLine + "(reset after {2} minutes)";

            if (this._remainCount > -1 && this._maxCount > -1)
            {
                // 正常
                this.Control.Text = string.Format(textFormat, this._remainCount, this._maxCount);
            }
            else if (this.RemainCount > -1)
            {
                // uppercount不正
                this.Control.Text = string.Format(textFormat, this._remainCount, "???");
            }
            else if (this._maxCount < -1)
            {
                // remaincount不正
                this.Control.Text = string.Format(textFormat, "???", this._maxCount);
            }
            else
            {
                // 両方とも不正
                this.Control.Text = string.Format(textFormat, "???", "???");
            }

            double minute = Math.Ceiling((this.ResetTime - DateTime.Now).TotalMinutes);
            string minuteText = minute >= 0 ? minute.ToString() : "???";

            this.ToolTipText = string.Format(toolTipTextFormat, this._remainCount, this._maxCount, minuteText);
        }
    }
}