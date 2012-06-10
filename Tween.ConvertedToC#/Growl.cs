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
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;

    public class GrowlHelper
    {
        private Assembly connector;
        private Assembly core;
        private object growlNTreply;
        private object growlNTdm;
        private object growlNTnew;
        private object growlNTusevent;
        private object growlApp;
        private object targetConnector;
        private object targetCore;
        private string appName = string.Empty;
        private bool initialized;

        public GrowlHelper(string appName)
        {
            this.appName = appName;
        }

        public delegate void NotifyClickedEventHandler(object sender, NotifyCallbackEventArgs e);

        public event NotifyClickedEventHandler NotifyClicked;

        public enum NotifyType
        {
            Reply = 0,
            DirectMessage = 1,
            Notify = 2,
            UserStreamEvent = 3
        }

        public static bool IsDllExists
        {
            get
            {
                string dir = Application.StartupPath;
                string connectorPath = Path.Combine(dir, "Growl.Connector.dll");
                string corePath = Path.Combine(dir, "Growl.CoreLibrary.dll");
                return File.Exists(connectorPath) && File.Exists(corePath);
            }
        }

        public string AppName
        {
            get { return this.appName; }
        }

        public bool IsAvailable
        {
            get { return this.connector != null && this.core != null && this.initialized; }
        }

        public bool RegisterGrowl()
        {
            this.initialized = false;
            string dir = Application.StartupPath;
            string connectorPath = Path.Combine(dir, "Growl.Connector.dll");
            string corePath = Path.Combine(dir, "Growl.CoreLibrary.dll");
            try
            {
                if (!IsDllExists)
                {
                    return false;
                }

                this.connector = Assembly.LoadFile(connectorPath);
                this.core = Assembly.LoadFile(corePath);
            }
            catch (Exception)
            {
                return false;
            }

            try
            {
                this.targetConnector = this.connector.CreateInstance("Growl.Connector.GrowlConnector");
                this.targetCore = this.core.CreateInstance("Growl.CoreLibrary");
                Type t = this.connector.GetType("Growl.Connector.NotificationType");
                this.growlNTreply = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] { "REPLY", "Reply" });
                this.growlNTdm = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] { "DIRECT_MESSAGE", "DirectMessage" });
                this.growlNTnew = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] { "NOTIFY", "新着通知" });
                this.growlNTusevent = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] { "USERSTREAM_EVENT", "UserStream Event" });

                object encryptType = this.connector.GetType("Growl.Connector.Cryptography+SymmetricAlgorithmType").InvokeMember("PlainText", BindingFlags.GetField, null, null, null);
                this.targetConnector.GetType().InvokeMember("EncryptionAlgorithm", BindingFlags.SetProperty, null, this.targetConnector, new object[] { encryptType });
                this.growlApp = this.connector.CreateInstance("Growl.Connector.Application", false, BindingFlags.Default, null, new object[] { this.appName }, null, null);

                if (File.Exists(Path.Combine(Application.StartupPath, "Icons\\Tween.png")))
                {
                    // Icons\Tween.pngを使用
                    ConstructorInfo ci = this.core.GetType("Growl.CoreLibrary.Resource").GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);
                    object data = ci.Invoke(new object[] { Path.Combine(Application.StartupPath, "Icons\\Tween.png") });
                    PropertyInfo pi = this.growlApp.GetType().GetProperty("Icon");
                    pi.SetValue(this.growlApp, data, null);
                }
                else if (File.Exists(Path.Combine(Application.StartupPath, "Icons\\MIcon.ico")))
                {
                    // アイコンセットにMIcon.icoが存在する場合それを使用
                    ConstructorInfo cibd = this.core.GetType("Growl.CoreLibrary.BinaryData").GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(byte[]) }, null);
                    TypeConverter tc = new TypeConverter();
                    object bdata = cibd.Invoke(new object[] { this.IconToByteArray(Path.Combine(Application.StartupPath, "Icons\\MIcon.ico")) });
                    ConstructorInfo cires = this.core.GetType("Growl.CoreLibrary.Resource").GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { bdata.GetType() }, null);
                    object data = cires.Invoke(new object[] { bdata });
                    PropertyInfo pi = this.growlApp.GetType().GetProperty("Icon");
                    pi.SetValue(this.growlApp, data, null);
                }
                else
                {
                    // 内蔵アイコンリソースを使用
                    ConstructorInfo cibd = this.core.GetType("Growl.CoreLibrary.BinaryData").GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(byte[]) }, null);
                    TypeConverter tc = new TypeConverter();
                    object bdata = cibd.Invoke(new object[] { this.IconToByteArray(Hoehoe.Properties.Resources.MIcon) });
                    ConstructorInfo cires = this.core.GetType("Growl.CoreLibrary.Resource").GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { bdata.GetType() }, null);
                    object data = cires.Invoke(new object[] { bdata });
                    PropertyInfo pi = this.growlApp.GetType().GetProperty("Icon");
                    pi.SetValue(this.growlApp, data, null);
                }

                MethodInfo mi = this.targetConnector.GetType().GetMethod("Register", new Type[] { this.growlApp.GetType(), this.connector.GetType("Growl.Connector.NotificationType[]") });

                t = this.connector.GetType("Growl.Connector.NotificationType");
                ArrayList arglist = new ArrayList();
                arglist.Add(this.growlNTreply);
                arglist.Add(this.growlNTdm);
                arglist.Add(this.growlNTnew);
                arglist.Add(this.growlNTusevent);

                mi.Invoke(this.targetConnector, new object[] { this.growlApp, arglist.ToArray(t) });

                // コールバックメソッドの登録
                Type growlConnectorType = this.connector.GetType("Growl.Connector.GrowlConnector");
                EventInfo notificationCallback = growlConnectorType.GetEvent("NotificationCallback");
                Type delegateType = notificationCallback.EventHandlerType;
                MethodInfo handler = typeof(GrowlHelper).GetMethod("GrowlCallbackHandler", BindingFlags.NonPublic | BindingFlags.Instance);
                Delegate d = Delegate.CreateDelegate(delegateType, this, handler);
                MethodInfo methodAdd = notificationCallback.GetAddMethod();
                object[] addHandlerArgs = { d };
                methodAdd.Invoke(this.targetConnector, addHandlerArgs);

                this.initialized = true;
            }
            catch (Exception)
            {
                this.initialized = false;
                return false;
            }

            return true;
        }

        public void Notify(NotifyType notificationType, string id, string title, string text, Image icon = null, string url = "")
        {
            if (!this.initialized)
            {
                return;
            }

            string notificationName = string.Empty;
            switch (notificationType)
            {
                case NotifyType.Reply:
                    notificationName = "REPLY";
                    break;
                case NotifyType.DirectMessage:
                    notificationName = "DIRECT_MESSAGE";
                    break;
                case NotifyType.Notify:
                    notificationName = "NOTIFY";
                    break;
                case NotifyType.UserStreamEvent:
                    notificationName = "USERSTREAM_EVENT";
                    break;
            }

            object n = null;
            if (icon != null || !string.IsNullOrEmpty(url))
            {
                Type gcore = this.core.GetType("Growl.CoreLibrary.Resource");
                object res = null;
                if (icon != null)
                {
                    res = gcore.InvokeMember("op_Implicit", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { icon });
                }
                else
                {
                    res = gcore.InvokeMember("op_Implicit", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { url });
                }

                object priority = this.connector.GetType("Growl.Connector.Priority").InvokeMember("Normal", BindingFlags.GetField, null, null, null);
                n = this.connector.GetType("Growl.Connector.Notification")
                    .InvokeMember("Notification", BindingFlags.CreateInstance, null, this.connector, new object[] { this.appName, notificationName, id, title, text, res, false, priority, "aaa" });
            }
            else
            {
                n = this.connector.GetType("Growl.Connector.Notification")
                    .InvokeMember("Notification", BindingFlags.CreateInstance, null, this.connector, new object[] { this.appName, notificationName, id, title, text });
            }
            ////_targetConnector.GetType.InvokeMember("Notify", BindingFlags.InvokeMethod, Nothing, _targetConnector, New Object() {n})
            object cc = this.connector.GetType("Growl.Connector.CallbackContext")
                .InvokeMember(null, BindingFlags.CreateInstance, null, this.connector, new object[] { "some fake information", notificationName });
            this.targetConnector.GetType().InvokeMember("Notify", BindingFlags.InvokeMethod, null, this.targetConnector, new object[] { n, cc });
        }

        private void OnNotifyClicked(NotifyCallbackEventArgs e)
        {
            if (this.NotifyClicked != null)
            {
                this.NotifyClicked(this, e);
            }
        }

        private byte[] IconToByteArray(string filename)
        {
            return this.IconToByteArray(new Icon(filename));
        }

        private byte[] IconToByteArray(Icon icondata)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Icon ic = new Icon(icondata, 48, 48);
                ic.ToBitmap().Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private void GrowlCallbackHandler(object response, object callbackData, object state)
        {
            try
            {
                // 定数取得
                object constClick = this.core.GetType("Growl.CoreLibrary.CallbackResult").GetField("CLICK", BindingFlags.Public | BindingFlags.Static).GetRawConstantValue();

                // 実際の値
                object valResult = callbackData.GetType().GetProperty("Result", BindingFlags.Public | BindingFlags.Instance).GetGetMethod().Invoke(callbackData, null);
                valResult = Convert.ToInt32(valResult);
                string notifyId = Convert.ToString(callbackData.GetType().GetProperty("NotificationID").GetGetMethod().Invoke(callbackData, null));
                string notifyName = Convert.ToString(callbackData.GetType().GetProperty("Type").GetGetMethod().Invoke(callbackData, null));
                if (constClick.Equals(valResult))
                {
                    NotifyType nt = default(NotifyType);
                    switch (notifyName)
                    {
                        case "REPLY":
                            nt = NotifyType.Reply;
                            break;
                        case "DIRECT_MESSAGE":
                            nt = NotifyType.DirectMessage;
                            break;
                        case "NOTIFY":
                            nt = NotifyType.Notify;
                            break;
                        case "USERSTREAM_EVENT":
                            nt = NotifyType.UserStreamEvent;
                            break;
                    }

                    this.OnNotifyClicked(new NotifyCallbackEventArgs(nt, notifyId));
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public class NotifyCallbackEventArgs : EventArgs
        {
            public NotifyCallbackEventArgs(NotifyType notifyType, string statusId)
            {
                if (statusId.Length > 1)
                {
                    this.StatusId = Convert.ToInt64(statusId);
                    this.NotifyType = notifyType;
                }
            }

            public long StatusId { get; set; }

            public NotifyType NotifyType { get; set; }
        }
    }
}