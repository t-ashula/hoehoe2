using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TweenUp
{
    /// <summary>
    /// The my special path.
    /// </summary>
    public class MySpecialPath
    {
        /// <summary>
        /// The user app data path.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string UserAppDataPath()
        {
            return GetFileSystemPath(Environment.SpecialFolder.ApplicationData);
        }

        /// <summary>
        /// The user app data path.
        /// </summary>
        /// <param name="productName">
        /// The product name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string UserAppDataPath(string productName)
        {
            return GetFileSystemPath(Environment.SpecialFolder.ApplicationData, productName);
        }

        /// <summary>
        /// Gets the common app data path.
        /// </summary>
        public static string CommonAppDataPath
        {
            get { return GetFileSystemPath(Environment.SpecialFolder.CommonApplicationData); }
        }

        /// <summary>
        /// Gets the local user app data path.
        /// </summary>
        public static string LocalUserAppDataPath
        {
            get { return GetFileSystemPath(Environment.SpecialFolder.LocalApplicationData); }
        }

        /// <summary>
        /// Gets the common app data registry.
        /// </summary>
        public static RegistryKey CommonAppDataRegistry
        {
            get { return GetRegistryPath(Registry.LocalMachine); }
        }

        /// <summary>
        /// Gets the user app data registry.
        /// </summary>
        public static RegistryKey UserAppDataRegistry
        {
            get { return GetRegistryPath(Registry.CurrentUser); }
        }

        private static string GetFileSystemPath(Environment.SpecialFolder folder)
        {
            // パスを取得
            string path = string.Format("{0}{3}{1}{3}{2}", Environment.GetFolderPath(folder), Application.CompanyName, Application.ProductName, Path.DirectorySeparatorChar);

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

        private static string GetFileSystemPath(Environment.SpecialFolder folder, string productName)
        {
            // パスを取得
            var path = string.Format("{0}{3}{1}{3}{2}", Environment.GetFolderPath(folder), Application.CompanyName, productName, Path.DirectorySeparatorChar);

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
            var basePath = ReferenceEquals(key, Registry.LocalMachine) ? "SOFTWARE" : "Software";
            var path = string.Format("{0}\\{1}\\{2}", basePath, Application.CompanyName, Application.ProductName);

            // パスのレジストリ・キーの取得（および作成）
            return key.CreateSubKey(path);
        }
    }
}