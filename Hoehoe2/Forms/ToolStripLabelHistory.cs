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

namespace Hoehoe.TweenCustomControl
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;

    public sealed class ToolStripLabelHistory : ToolStripStatusLabel
    {
        #region private

        private const int Maxcnt = 20;
        private readonly LinkedList<LogEntry> _logEntries;

        #endregion

        #region constructor

        public ToolStripLabelHistory()
        {
            _logEntries = new LinkedList<LogEntry>();
        }

        #endregion

        #region enums

        public enum LogLevel
        {
            Lowest = 0,
            Debug = 16,
            Info = 32,
            Notice = 64,
            Warn = 128,
            Err = 192,
            Fatal = 255,
            Highest = 256
        }

        #endregion

        #region properties

        public override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                _logEntries.AddLast(new LogEntry(DateTime.Now, value));
                while (_logEntries.Count > Maxcnt)
                {
                    _logEntries.RemoveFirst();
                }

                base.Text = value;
            }
        }

        public string TextHistory
        {
            get
            {
                var sb = new StringBuilder();
                foreach (LogEntry e in _logEntries)
                {
                    sb.AppendLine(e.ToString());
                }

                return sb.ToString();
            }
        }

        #endregion

        #region inner class

        public class LogEntry
        {
            public LogEntry(LogLevel logLevel, DateTime timestamp, string summary, string detail)
            {
                LogLevel = logLevel;
                Timestamp = timestamp;
                Summary = summary;
                Detail = detail;
            }

            public LogEntry(DateTime timestamp, string summary)
                : this(LogLevel.Debug, timestamp, summary, summary)
            {
            }

            public LogLevel LogLevel { get; private set; }

            public DateTime Timestamp { get; private set; }

            public string Summary { get; private set; }

            public string Detail { get; private set; }

            public override string ToString()
            {
                return string.Format("{0:T}: {1}", Timestamp, Summary);
            }
        }

        #endregion
    }
}