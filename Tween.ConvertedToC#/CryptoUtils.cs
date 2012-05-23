using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Hoehoe
{
    public static class CryptoUtils
    {
        public static string EncryptString(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return "";
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
                using (var msOut = new MemoryStream())
                {
                    // DES暗号化オブジェクトの作成
                    using (var desdecrypt = des.CreateEncryptor())
                    {
                        // 書き込むためのCryptoStreamの作成
                        using (var cryptStream = new CryptoStream(msOut, desdecrypt, CryptoStreamMode.Write))
                        {
                            // 書き込む
                            cryptStream.Write(bytesIn, 0, bytesIn.Length);
                            cryptStream.FlushFinalBlock();
                            // 暗号化されたデータを取得
                            byte[] bytesOut = msOut.ToArray();

                            // 閉じる
                            cryptStream.Close();
                            msOut.Close();

                            // Base64で文字列に変更して結果を返す
                            return Convert.ToBase64String(bytesOut);
                        }
                    }
                }
            }
        }

        public static string DecryptString(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return "";
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
                using (var msIn = new MemoryStream(bytesIn))
                {
                    // DES復号化オブジェクトの作成
                    using (var desdecrypt = des.CreateDecryptor())
                    {
                        // 読み込むためのCryptoStreamの作成
                        using (var cryptStreem = new CryptoStream(msIn, desdecrypt, CryptoStreamMode.Read))
                        {
                            // 復号化されたデータを取得するためのStreamReader
                            using (var srOut = new System.IO.StreamReader(cryptStreem, Encoding.UTF8))
                            {
                                // 復号化されたデータを取得する
                                string result = srOut.ReadToEnd();

                                // 閉じる
                                srOut.Close();
                                cryptStreem.Close();
                                msIn.Close();

                                return result;
                            }
                        }
                    }
                }
            }
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