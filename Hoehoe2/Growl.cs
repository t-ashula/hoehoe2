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
    using R = Properties.Resources;

    public class GrowlHelper
    {
        private Assembly _connector;
        private Assembly _core;
        private object _growlNTreply;
        private object _growlNTdm;
        private object _growlNTnew;
        private object _growlNTusevent;
        private object _growlApp;
        private object _targetConnector;
        private object _targetCore;
        private readonly string _appName = string.Empty;
        private bool _initialized;

        public GrowlHelper(string appName)
        {
            _appName = appName;
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
            get { return _appName; }
        }

        public bool IsAvailable
        {
            get { return _connector != null && _core != null && _initialized; }
        }

        public bool RegisterGrowl()
        {
            _initialized = false;
            string dir = Application.StartupPath;
            string connectorPath = Path.Combine(dir, "Growl.Connector.dll");
            string corePath = Path.Combine(dir, "Growl.CoreLibrary.dll");
            try
            {
                if (!IsDllExists)
                {
                    return false;
                }

                _connector = Assembly.LoadFile(connectorPath);
                _core = Assembly.LoadFile(corePath);
            }
            catch (Exception)
            {
                return false;
            }

            try
            {
                _targetConnector = _connector.CreateInstance("Growl.Connector.GrowlConnector");
                _targetCore = _core.CreateInstance("Growl.CoreLibrary");
                Type t = _connector.GetType("Growl.Connector.NotificationType");
                _growlNTreply = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] { "REPLY", "Reply" });
                _growlNTdm = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] { "DIRECT_MESSAGE", "DirectMessage" });
                _growlNTnew = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] { "NOTIFY", "新着通知" });
                _growlNTusevent = t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] { "USERSTREAM_EVENT", "UserStream Event" });

                object encryptType = _connector.GetType("Growl.Connector.Cryptography+SymmetricAlgorithmType").InvokeMember("PlainText", BindingFlags.GetField, null, null, null);
                _targetConnector.GetType().InvokeMember("EncryptionAlgorithm", BindingFlags.SetProperty, null, _targetConnector, new[] { encryptType });
                _growlApp = _connector.CreateInstance("Growl.Connector.Application", false, BindingFlags.Default, null, new object[] { _appName }, null, null);

                if (File.Exists(Path.Combine(Application.StartupPath, "Icons\\Tween.png")))
                {
                    // Icons\Tween.pngを使用
                    ConstructorInfo ci = _core.GetType("Growl.CoreLibrary.Resource").GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
                    object data = ci.Invoke(new object[] { Path.Combine(Application.StartupPath, "Icons\\Tween.png") });
                    PropertyInfo pi = _growlApp.GetType().GetProperty("Icon");
                    pi.SetValue(_growlApp, data, null);
                }
                else if (File.Exists(Path.Combine(Application.StartupPath, "Icons\\MIcon.ico")))
                {
                    // アイコンセットにMIcon.icoが存在する場合それを使用
                    ConstructorInfo cibd = _core.GetType("Growl.CoreLibrary.BinaryData").GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(byte[]) }, null);
                    var tc = new TypeConverter();
                    object bdata = cibd.Invoke(new object[] { IconToByteArray(Path.Combine(Application.StartupPath, "Icons\\MIcon.ico")) });
                    ConstructorInfo cires = _core.GetType("Growl.CoreLibrary.Resource").GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { bdata.GetType() }, null);
                    object data = cires.Invoke(new[] { bdata });
                    PropertyInfo pi = _growlApp.GetType().GetProperty("Icon");
                    pi.SetValue(_growlApp, data, null);
                }
                else
                {
                    // 内蔵アイコンリソースを使用
                    ConstructorInfo cibd = _core.GetType("Growl.CoreLibrary.BinaryData").GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(byte[]) }, null);
                    var tc = new TypeConverter();
                    object bdata = cibd.Invoke(new object[] { IconToByteArray(R.MIcon) });
                    ConstructorInfo cires = _core.GetType("Growl.CoreLibrary.Resource").GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { bdata.GetType() }, null);
                    object data = cires.Invoke(new[] { bdata });
                    PropertyInfo pi = _growlApp.GetType().GetProperty("Icon");
                    pi.SetValue(_growlApp, data, null);
                }

                MethodInfo mi = _targetConnector.GetType().GetMethod("Register", new[] { _growlApp.GetType(), _connector.GetType("Growl.Connector.NotificationType[]") });

                t = _connector.GetType("Growl.Connector.NotificationType");
                var arglist = new ArrayList();
                arglist.Add(_growlNTreply);
                arglist.Add(_growlNTdm);
                arglist.Add(_growlNTnew);
                arglist.Add(_growlNTusevent);

                mi.Invoke(_targetConnector, new[] { _growlApp, arglist.ToArray(t) });

                // コールバックメソッドの登録
                Type growlConnectorType = _connector.GetType("Growl.Connector.GrowlConnector");
                EventInfo notificationCallback = growlConnectorType.GetEvent("NotificationCallback");
                Type delegateType = notificationCallback.EventHandlerType;
                MethodInfo handler = typeof(GrowlHelper).GetMethod("GrowlCallbackHandler", BindingFlags.NonPublic | BindingFlags.Instance);
                Delegate d = Delegate.CreateDelegate(delegateType, this, handler);
                MethodInfo methodAdd = notificationCallback.GetAddMethod();
                object[] addHandlerArgs = { d };
                methodAdd.Invoke(_targetConnector, addHandlerArgs);

                _initialized = true;
            }
            catch (Exception)
            {
                _initialized = false;
                return false;
            }

            return true;
        }

        public void Notify(NotifyType notificationType, string id, string title, string text, Image icon = null, string url = "")
        {
            if (!_initialized)
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

            object n;
            if (icon != null || !string.IsNullOrEmpty(url))
            {
                Type gcore = _core.GetType("Growl.CoreLibrary.Resource");
                object res = gcore.InvokeMember("op_Implicit", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, icon != null ? new object[] { icon } : new object[] { url });
                object priority = _connector.GetType("Growl.Connector.Priority").InvokeMember("Normal", BindingFlags.GetField, null, null, null);
                n = _connector.GetType("Growl.Connector.Notification")
                    .InvokeMember("Notification", BindingFlags.CreateInstance, null, _connector, new[] { _appName, notificationName, id, title, text, res, false, priority, "aaa" });
            }
            else
            {
                n = _connector.GetType("Growl.Connector.Notification")
                    .InvokeMember("Notification", BindingFlags.CreateInstance, null, _connector, new object[] { _appName, notificationName, id, title, text });
            }
            ////_targetConnector.GetType.InvokeMember("Notify", BindingFlags.InvokeMethod, Nothing, _targetConnector, New Object() {n})
            object cc = _connector.GetType("Growl.Connector.CallbackContext")
                .InvokeMember(null, BindingFlags.CreateInstance, null, _connector, new object[] { "some fake information", notificationName });
            _targetConnector.GetType().InvokeMember("Notify", BindingFlags.InvokeMethod, null, _targetConnector, new[] { n, cc });
        }

        private void OnNotifyClicked(NotifyCallbackEventArgs e)
        {
            if (NotifyClicked != null)
            {
                NotifyClicked(this, e);
            }
        }

        private byte[] IconToByteArray(string filename)
        {
            return IconToByteArray(new Icon(filename));
        }

        private byte[] IconToByteArray(Icon icondata)
        {
            using (var ms = new MemoryStream())
            {
                var ic = new Icon(icondata, 48, 48);
                ic.ToBitmap().Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private void GrowlCallbackHandler(object response, object callbackData, object state)
        {
            try
            {
                // 定数取得
                object constClick = _core.GetType("Growl.CoreLibrary.CallbackResult").GetField("CLICK", BindingFlags.Public | BindingFlags.Static).GetRawConstantValue();

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

                    OnNotifyClicked(new NotifyCallbackEventArgs(nt, notifyId));
                }
            }
            catch (Exception)
            {
                // nothing
            }
        }

        public class NotifyCallbackEventArgs : EventArgs
        {
            public NotifyCallbackEventArgs(NotifyType notifyType, string statusId)
            {
                if (statusId.Length > 1)
                {
                    StatusId = Convert.ToInt64(statusId);
                    NotifyType = notifyType;
                }
            }

            public long StatusId { get; set; }

            public NotifyType NotifyType { get; set; }
        }
    }
}