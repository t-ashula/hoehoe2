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
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Tween
{
    public abstract class SettingBase<T> where T : class, new()
    {
        private static object lockObj = new object();

        protected static T LoadSettings(string fileId)
        {
            try
            {
                lock (lockObj)
                {
                    using (FileStream fs = new FileStream(GetSettingFilePath(fileId), FileMode.Open))
                    {
                        fs.Position = 0;
                        XmlSerializer xs = new XmlSerializer(typeof(T));
                        T instance = (T)xs.Deserialize(fs);
                        fs.Close();
                        return instance;
                    }
                }
            }
            catch (FileNotFoundException )
            {
                return new T();
            }
            catch (Exception )
            {
                string backupFile = Path.Combine(Path.Combine(Tween.My.MyProject.Application.Info.DirectoryPath, "TweenBackup1st"), typeof(T).Name + fileId + ".xml");
                if (File.Exists(backupFile))
                {
                    try
                    {
                        lock (lockObj)
                        {
                            using (FileStream fs = new FileStream(backupFile, FileMode.Open))
                            {
                                fs.Position = 0;
                                XmlSerializer xs = new XmlSerializer(typeof(T));
                                T instance = (T)xs.Deserialize(fs);
                                fs.Close();
                                MessageBox.Show("File: " + GetSettingFilePath(fileId) + Environment.NewLine + "Use old setting file, because application can't read this setting file.");
                                return instance;
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                    }
                }
                MessageBox.Show("File: " + GetSettingFilePath(fileId) + Environment.NewLine + "Use default setting, because application can't read this setting file.");
                return new T();
            }
        }

        protected static T LoadSettings()
        {
            return LoadSettings("");
        }

        protected static void SaveSettings(T instance, string fileId)
        {
            int cnt = 0;
            bool err = false;
            string fileName = GetSettingFilePath(fileId);
            if (instance == null)
            {
                return;
            }
            do
            {
                err = false;
                cnt += 1;
                try
                {
                    lock (lockObj)
                    {
                        using (FileStream fs = new FileStream(fileName, FileMode.Create))
                        {
                            fs.Position = 0;
                            XmlSerializer xs = new XmlSerializer(typeof(T));
                            xs.Serialize(fs, instance);
                            fs.Flush();
                            fs.Close();
                        }
                        FileInfo fi = new FileInfo(fileName);
                        if (fi.Length == 0)
                        {
                            if (cnt > 3)
                            {
                                throw new Exception();
                            }
                            Thread.Sleep(1000);
                            err = true;
                        }
                    }
                }
                catch (Exception )
                {
                    //検証エラー or 書き込みエラー
                    if (cnt > 3)
                    {
                        //リトライオーバー
                        MessageBox.Show("Can't write setting XML.(" + fileName + ")", "Save Settings", MessageBoxButtons.OK);
                        return;
                    }
                    //リトライ
                    Thread.Sleep(1000);
                    err = true;
                }
            } while (err);
        }

        protected static void SaveSettings(T instance)
        {
            SaveSettings(instance, "");
        }

        public static string GetSettingFilePath(string fileId)
        {
            return Path.Combine(MyCommon.SettingPath, typeof(T).Name + fileId + ".xml");
        }
    }
}