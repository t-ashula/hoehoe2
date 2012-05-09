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
namespace Tween
{

	public partial class OpenURL
	{


		private string _selUrl;
		private void OK_Button_Click(System.Object sender, System.EventArgs e)
		{
			if (UrlList.SelectedItems.Count == 0) {
				this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			} else {
				_selUrl = UrlList.SelectedItem.ToString();
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
			}
			this.Close();
		}

		private void Cancel_Button_Click(System.Object sender, System.EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Close();
		}

		public void ClearUrl()
		{
			UrlList.Items.Clear();
		}

		public void AddUrl(OpenUrlItem openUrlItem)
		{
			UrlList.Items.Add(openUrlItem);
		}

		public string SelectedUrl {
			get {
				if (UrlList.SelectedItems.Count == 1) {
					return _selUrl;
				} else {
					return "";
				}
			}
		}

		private void OpenURL_Shown(object sender, System.EventArgs e)
		{
			UrlList.Focus();
			if (UrlList.Items.Count > 0) {
				UrlList.SelectedIndex = 0;
			}
		}

		private void UrlList_DoubleClick(System.Object sender, System.EventArgs e)
		{
			if (UrlList.SelectedItem == null) {
				return;
			}

			if (UrlList.IndexFromPoint(UrlList.PointToClient(Control.MousePosition)) == ListBox.NoMatches) {
				return;
			}

			if (UrlList.Items[UrlList.IndexFromPoint(UrlList.PointToClient(Control.MousePosition))] == null) {
				return;
			}
			OK_Button_Click(sender, e);
		}

		private void UrlList_KeyDown(System.Object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.J && UrlList.SelectedIndex < UrlList.Items.Count - 1) {
				e.SuppressKeyPress = true;
				UrlList.SelectedIndex += 1;
			}
			if (e.KeyCode == Keys.K && UrlList.SelectedIndex > 0) {
				e.SuppressKeyPress = true;
				UrlList.SelectedIndex -= 1;
			}
			if (e.Control && e.KeyCode == Keys.Oem4) {
				e.SuppressKeyPress = true;
				Cancel_Button_Click(null, null);
			}
		}
		public OpenURL()
		{
			Shown += OpenURL_Shown;
			InitializeComponent();
		}
	}
}
namespace Tween
{

	public class OpenUrlItem
	{
		private string _url;
		private string _linkText;

		private string _href;
		public OpenUrlItem(string linkText, string url, string href)
		{
			this._linkText = linkText;
			this._url = url;
			this._href = href;
		}

		public string Text {
			get {
				if (this._linkText.StartsWith("@") || this._linkText.StartsWith("＠") || this._linkText.StartsWith("#") || this._linkText.StartsWith("＃")) {
					return this._linkText;
				}
				if (this._linkText.TrimEnd('/') == this._url.TrimEnd('/')) {
					return this._url;
				} else {
					return this._linkText + "  >>>  " + this.Url;
				}
			}
		}

		public string Url {
			get { return this._url; }
		}

		public override string ToString()
		{
			return this._href;
		}

		public string Href {
			get { return this._href; }
		}
	}
}
