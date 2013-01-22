// Hoehoe - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011- t.ashula (@t_ashula) <office@ashula.info>
//
// All rights reserved.
// This file is part of Hoehoe.
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

namespace Hoehoe
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class ToolStripAPIGauge : ToolStripControlHost
    {
        #region private

        private Size _originalSize;
        private int _gaugeHeight;
        private int _maxCount = 350;
        private int _remainCount;
        private DateTime _resetTime;

        #endregion private

        #region constructor

        public ToolStripAPIGauge()
            : base(new Control())
        {
            AutoToolTip = true;
            Control.Paint += Draw;
            Control.TextChanged += Control_TextChanged;
            Control.SizeChanged += Control_SizeChanged;
        }

        #endregion constructor

        #region properties

        public int GaugeHeight
        {
            get
            {
                return _gaugeHeight;
            }

            set
            {
                _gaugeHeight = value;
                if (Control != null && !Control.IsDisposed)
                {
                    Control.Refresh();
                }
            }
        }

        public int MaxCount
        {
            get
            {
                return _maxCount;
            }

            set
            {
                _maxCount = value;
                if (Control != null && !Control.IsDisposed)
                {
                    SetText(_remainCount, _maxCount);
                    Control.Refresh();
                }
            }
        }

        public int RemainCount
        {
            get
            {
                return _remainCount;
            }

            set
            {
                _remainCount = value;
                if (Control != null && !Control.IsDisposed)
                {
                    SetText(_remainCount, _maxCount);
                    Control.Refresh();
                }
            }
        }

        public DateTime ResetTime
        {
            get
            {
                return _resetTime;
            }

            set
            {
                _resetTime = value;
                if (Control != null && !Control.IsDisposed)
                {
                    SetText(_remainCount, _maxCount);
                    Control.Refresh();
                }
            }
        }

        #endregion properties

        #region event handler

        private void Draw(object sender, PaintEventArgs e)
        {
            double minute = (ResetTime - DateTime.Now).TotalMinutes;
            var apiGaugeBounds = new Rectangle(0, Convert.ToInt32((Control.Height - (_gaugeHeight * 2)) / 2), Convert.ToInt32(Control.Width * (RemainCount / _maxCount)), _gaugeHeight);
            var timeGaugeBounds = new Rectangle(0, apiGaugeBounds.Top + _gaugeHeight, Convert.ToInt32(Control.Width * (minute / 60)), _gaugeHeight);
            e.Graphics.FillRectangle(Brushes.LightBlue, apiGaugeBounds);
            e.Graphics.FillRectangle(Brushes.LightPink, timeGaugeBounds);
            e.Graphics.DrawString(Control.Text, Control.Font, SystemBrushes.ControlText, 0, Convert.ToSingle(timeGaugeBounds.Top - (Control.Font.Height / 2)));
        }

        private void Control_TextChanged(object sender, EventArgs e)
        {
            Control.SizeChanged -= Control_SizeChanged;
            using (Graphics g = Control.CreateGraphics())
            {
                Control.Size = new Size(Convert.ToInt32(Math.Max(g.MeasureString(Control.Text, Control.Font).Width, _originalSize.Width)), Control.Size.Height);
            }

            Control.SizeChanged += Control_SizeChanged;
        }

        private void Control_SizeChanged(object sender, EventArgs e)
        {
            _originalSize = Control.Size;
        }

        #endregion event handler

        #region private methods

        private void SetText(int remain, int max)
        {
            const string TextFormat = "API {0}/{1}";
            string toolTipTextFormat = "API rest {0}/{1}" + Environment.NewLine + "(reset after {2} minutes)";
            if (_remainCount > -1 && _maxCount > -1)
            {
                // 正常
                Control.Text = string.Format(TextFormat, _remainCount, _maxCount);
            }
            else if (RemainCount > -1)
            {
                // uppercount不正
                Control.Text = string.Format(TextFormat, _remainCount, "???");
            }
            else if (_maxCount < -1)
            {
                // remaincount不正
                Control.Text = string.Format(TextFormat, "???", _maxCount);
            }
            else
            {
                // 両方とも不正
                Control.Text = string.Format(TextFormat, "???", "???");
            }

            double minute = Math.Ceiling((ResetTime - DateTime.Now).TotalMinutes);
            string minuteText = minute >= 0 ? minute.ToString() : "???";
            ToolTipText = string.Format(toolTipTextFormat, _remainCount, _maxCount, minuteText);
        }

        #endregion private methods
    }
}