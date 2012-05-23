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
    using System.IO;
    using System.Windows.Forms;
    using Microsoft.Win32;

    public static class MySpecialPath
    {
        public static string CommonAppDataPath
        {
            get { return GetFileSystemPath(Environment.SpecialFolder.CommonApplicationData); }
        }

        public static string LocalUserAppDataPath
        {
            get { return GetFileSystemPath(Environment.SpecialFolder.LocalApplicationData); }
        }

        public static RegistryKey CommonAppDataRegistry
        {
            get { return GetRegistryPath(Registry.LocalMachine); }
        }

        public static RegistryKey UserAppDataRegistry
        {
            get { return GetRegistryPath(Registry.CurrentUser); }
        }

        public static string UserAppDataPath()
        {
            return GetFileSystemPath(Environment.SpecialFolder.ApplicationData);
        }

        public static string UserAppDataPath(string productName)
        {
            return GetFileSystemPath(Environment.SpecialFolder.ApplicationData, productName);
        }

        private static string GetFileSystemPath(Environment.SpecialFolder folder)
        {
            return GetFileSystemPath(folder, Application.ProductName);
        }

        private static string GetFileSystemPath(Environment.SpecialFolder folder, string productName)
        {
            // パスを取得
            string path = System.IO.Path.Combine(Environment.GetFolderPath(folder), Application.CompanyName, productName);

            // パスのフォルダを作成
            lock (typeof(Application))
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            return path;
        }

        private static RegistryKey GetRegistryPath(RegistryKey key)
        {
            // パスを取得
            string basePath = key == Registry.LocalMachine ? "SOFTWARE" : "Software";
            string path = string.Format("{0}\\{1}\\{2}", basePath, Application.CompanyName, Application.ProductName);

            // パスのレジストリ・キーの取得（および作成）
            return key.CreateSubKey(path);
        }
    }
}