using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Tween
{
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

	public abstract class SettingBase<T> where T : class, new()
	{


		private static object lockObj = new object();
		protected static T LoadSettings(string FileId)
		{
			try {
				lock (lockObj) {
					using (System.IO.FileStream fs = new System.IO.FileStream(GetSettingFilePath(FileId), System.IO.FileMode.Open)) {
						fs.Position = 0;
						System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
						T instance = (T)xs.Deserialize(fs);
						fs.Close();
						return instance;
					}
				}
			} catch (System.IO.FileNotFoundException ex) {
				return new T();
			} catch (Exception ex) {
				string backupFile = System.IO.Path.Combine(System.IO.Path.Combine(Tween.My.MyProject.Application.Info.DirectoryPath, "TweenBackup1st"), typeof(T).Name + FileId + ".xml");
				if (System.IO.File.Exists(backupFile)) {
					try {
						lock (lockObj) {
							using (System.IO.FileStream fs = new System.IO.FileStream(backupFile, System.IO.FileMode.Open)) {
								fs.Position = 0;
								System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
								T instance = (T)xs.Deserialize(fs);
								fs.Close();
								MessageBox.Show("File: " + GetSettingFilePath(FileId) + Environment.NewLine + "Use old setting file, because application can't read this setting file.");
								return instance;
							}
						}
					} catch (Exception ex2) {
					}
				}
				MessageBox.Show("File: " + GetSettingFilePath(FileId) + Environment.NewLine + "Use default setting, because application can't read this setting file.");
				return new T();
				//ex.Data.Add("FilePath", GetSettingFilePath(FileId))
				//Dim fi As New IO.FileInfo(GetSettingFilePath(FileId))
				//ex.Data.Add("FileSize", fi.Length.ToString())
				//ex.Data.Add("FileData", IO.File.ReadAllText(GetSettingFilePath(FileId)))
				//Throw
			}
		}

		protected static T LoadSettings()
		{
			return LoadSettings("");
		}

		protected static void SaveSettings(T Instance, string FileId)
		{
			int cnt = 0;
			bool err = false;
			string fileName = GetSettingFilePath(FileId);
			if (Instance == null)
				return;
			do {
				err = false;
				cnt += 1;
				try {
					lock (lockObj) {
						using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create)) {
							fs.Position = 0;
							System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
							xs.Serialize(fs, Instance);
							fs.Flush();
							fs.Close();
						}
						System.IO.FileInfo fi = new System.IO.FileInfo(fileName);
						if (fi.Length == 0) {
							if (cnt > 3) {
								throw new Exception();
								return;
							}
							System.Threading.Thread.Sleep(1000);
							err = true;
						}
					}
				} catch (Exception ex) {
					//検証エラー or 書き込みエラー
					if (cnt > 3) {
						//リトライオーバー
						MessageBox.Show("Can't write setting XML.(" + fileName + ")", "Save Settings", MessageBoxButtons.OK);
						//Throw New System.InvalidOperationException("Can't write setting XML.(" + fileName + ")")
						return;
					}
					//リトライ
					System.Threading.Thread.Sleep(1000);
					err = true;
				}
			} while (err);
		}

		protected static void SaveSettings(T Instance)
		{
			SaveSettings(Instance, "");
		}

		public static string GetSettingFilePath(string FileId)
		{
			return System.IO.Path.Combine(MyCommon.settingPath, typeof(T).Name + FileId + ".xml");
		}
	}
}
