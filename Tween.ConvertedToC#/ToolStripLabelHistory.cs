using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
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

using System.Text;

namespace Tween.TweenCustomControl
{

	public sealed class ToolStripLabelHistory : ToolStripStatusLabel
	{

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

		public class LogEntry
		{

			private readonly LogLevel _logLevel;

			private readonly DateTime _timestamp;

			private readonly string _summary;

			private readonly string _detail;
			public LogLevel LogLevel {
				get { return _logLevel; }
			}

			public DateTime Timestamp {
				get { return _timestamp; }
			}

			public string Summary {
				get { return _summary; }
			}

			public string Detail {
				get { return _detail; }
			}

			public LogEntry(LogLevel logLevel, DateTime timestamp, string summary, string detail)
			{
				_logLevel = logLevel;
				_timestamp = timestamp;
				_summary = summary;
				_detail = detail;
			}

			public LogEntry(DateTime timestamp, string summary) : this(LogLevel.Debug, timestamp, summary, summary)
			{
			}

			public override string ToString()
			{
				return Timestamp.ToString("T") + ": " + Summary;
			}
		}


		private LinkedList<LogEntry> _logs;

		private const int MAXCNT = 20;
		public override string Text {
			get { return base.Text; }
			set {
				_logs.AddLast(new LogEntry(DateTime.Now, value));
				while (_logs.Count > MAXCNT) {
					_logs.RemoveFirst();
				}
				base.Text = value;
			}
		}

		public string TextHistory {
			get {
				StringBuilder sb = new StringBuilder();
				foreach (LogEntry e in _logs) {
					sb.AppendLine(e.ToString());
				}
				return sb.ToString();
			}
		}

		public ToolStripLabelHistory()
		{
			_logs = new LinkedList<LogEntry>();
		}
	}
}
