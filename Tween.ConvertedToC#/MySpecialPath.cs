using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Tween
{
    public class MySpecialPath
    {
        public static string UserAppDataPath()
        {
            { return GetFileSystemPath(Environment.SpecialFolder.ApplicationData); }
        }

        public static string UserAppDataPath(string productName)
        {
            { return GetFileSystemPath(Environment.SpecialFolder.ApplicationData, productName); }
        }

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

        private static string GetFileSystemPath(System.Environment.SpecialFolder folder)
        {
            // パスを取得
            string path = string.Format("{0}{3}{1}{3}{2}", Environment.GetFolderPath(folder), Application.CompanyName, Application.ProductName, System.IO.Path.DirectorySeparatorChar.ToString());

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

        //GetFileSystemPath

        private static string GetFileSystemPath(System.Environment.SpecialFolder folder, string productName)
        {
            // パスを取得
            string path = string.Format("{0}{3}{1}{3}{2}", Environment.GetFolderPath(folder), Application.CompanyName, productName, System.IO.Path.DirectorySeparatorChar.ToString());

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

        //GetFileSystemPath

        private static RegistryKey GetRegistryPath(RegistryKey key)
        {
            // パスを取得
            string basePath = null;
            if (object.ReferenceEquals(key, Registry.LocalMachine))
            {
                basePath = "SOFTWARE";
            }
            else
            {
                basePath = "Software";
            }
            string path = string.Format("{0}\\{1}\\{2}", basePath, Application.CompanyName, Application.ProductName);
            // パスのレジストリ・キーの取得（および作成）
            return key.CreateSubKey(path);
        }

        //GetRegistryPath
    }
}