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
    using System.Security.Cryptography;
    using System.Text;

    public static class CryptoUtils
    {
        public static string EncryptString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            // 文字列をバイト型配列にする
            byte[] bytesIn = Encoding.UTF8.GetBytes(str);

            // DESCryptoServiceProviderオブジェクトの作成
            using (var des = new DESCryptoServiceProvider())
            {
                // 共有キーと初期化ベクタを決定
                // パスワードをバイト配列にする
                byte[] bytesKey = Encoding.UTF8.GetBytes("_tween_encrypt_key_");

                // 共有キーと初期化ベクタを設定
                des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
                des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

                // 暗号化されたデータを書き出すためのMemoryStream
                using (var memStream = new MemoryStream())
                {
                    // DES暗号化オブジェクトの作成
                    using (var desdecrypt = des.CreateEncryptor())
                    {
                        // 書き込むためのCryptoStreamの作成
                        using (var cryptStream = new CryptoStream(memStream, desdecrypt, CryptoStreamMode.Write))
                        {
                            // 書き込む
                            cryptStream.Write(bytesIn, 0, bytesIn.Length);
                            cryptStream.FlushFinalBlock();

                            // 暗号化されたデータを取得
                            byte[] bytesOut = memStream.ToArray();

                            // 閉じる
                            cryptStream.Close();
                            memStream.Close();

                            // Base64で文字列に変更して結果を返す
                            return Convert.ToBase64String(bytesOut);
                        }
                    }
                }
            }
        }

        public static string DecryptString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            // DESCryptoServiceProviderオブジェクトの作成
            using (var des = new DESCryptoServiceProvider())
            {
                // 共有キーと初期化ベクタを決定
                // パスワードをバイト配列にする
                byte[] bytesKey = Encoding.UTF8.GetBytes("_tween_encrypt_key_");

                // 共有キーと初期化ベクタを設定
                des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
                des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

                // Base64で文字列をバイト配列に戻す
                byte[] bytesIn = Convert.FromBase64String(str);

                // 暗号化されたデータを読み込むためのMemoryStream
                using (var memStream = new MemoryStream(bytesIn))
                {
                    // DES復号化オブジェクトの作成
                    using (var desdecrypt = des.CreateDecryptor())
                    {
                        // 読み込むためのCryptoStreamの作成
                        using (var cryptStreem = new CryptoStream(memStream, desdecrypt, CryptoStreamMode.Read))
                        {
                            // 復号化されたデータを取得するためのStreamReader
                            using (var reader = new StreamReader(cryptStreem, Encoding.UTF8))
                            {
                                // 復号化されたデータを取得する
                                string result = reader.ReadToEnd();

                                // 閉じる
                                reader.Close();
                                cryptStreem.Close();
                                memStream.Close();

                                return result;
                            }
                        }
                    }
                }
            }
        }

        public static string TryEncrypt(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                password = string.Empty;
            }

            if (password.Length > 0)
            {
                try
                {
                    return EncryptString(password);
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        public static string TryDecrypt(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                password = string.Empty;
            }

            if (password.Length > 0)
            {
                try
                {
                    password = DecryptString(password);
                }
                catch (Exception)
                {
                    password = string.Empty;
                }
            }

            return password;
        }

        private static byte[] ResizeBytesArray(byte[] bytes, int newSize)
        {
            byte[] newBytes = new byte[newSize];
            if (bytes.Length <= newSize)
            {
                int i = 0;
                for (i = 0; i < bytes.Length; i++)
                {
                    newBytes[i] = bytes[i];
                }
            }
            else
            {
                int pos = 0;
                int i = 0;
                for (i = 0; i < bytes.Length; i++)
                {
                    newBytes[pos] = (byte)(newBytes[pos] ^ bytes[i]);
                    pos += 1;
                    if (pos >= newBytes.Length)
                    {
                        pos = 0;
                    }
                }
            }

            return newBytes;
        }
    }
}