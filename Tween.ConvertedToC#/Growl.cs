using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Tween
{
    public class GrowlHelper
    {
        private Assembly _connector = null;

        private Assembly _core = null;
        private object _growlNTreply;
        private object _growlNTdm;
        private object _growlNTnew;
        private object _growlNTusevent;

        private object _growlApp;
        private object _targetConnector;

        private object _targetCore;
        private string _appName = "";

        bool _initialized = false;

        public class NotifyCallbackEventArgs : EventArgs
        {
            public long StatusId { get; set; }

            public NotifyType NotifyType { get; set; }

            public NotifyCallbackEventArgs(NotifyType notifyType, string statusId)
            {
                if (statusId.Length > 1)
                {
                    this.StatusId = Convert.ToInt64(statusId);
                    this.NotifyType = notifyType;
                }
            }
        }

        public event NotifyClickedEventHandler NotifyClicked;

        public delegate void NotifyClickedEventHandler(object sender, NotifyCallbackEventArgs e);

        public string AppName
        {
            get { return _appName; }
        }

        public enum NotifyType
        {
            Reply = 0,
            DirectMessage = 1,
            Notify = 2,
            UserStreamEvent = 3
        }

        public GrowlHelper(string appName)
        {
            _appName = appName;
        }

        public bool IsAvailable
        {
            get
            {
                if (_connector == null || _core == null || !_initialized)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private byte[] IconToByteArray(string filename)
        {
            return IconToByteArray(new Icon(filename));
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

        public static bool IsDllExists
        {
            get
            {
                string dir = Application.StartupPath;
                string connectorPath = Path.Combine(dir, "Growl.Connector.dll");
                string corePath = Path.Combine(dir, "Growl.CoreLibrary.dll");
                if (File.Exists(connectorPath) && File.Exists(corePath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
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
                    return false;
                _connector = Assembly.LoadFile(connectorPath);
                _core = Assembly.LoadFile(corePath);
            }
            catch (Exception ex)
            {
                return false;
            }

            try
            {
                _targetConnector = _connector.CreateInstance("Growl.Connector.GrowlConnector");
                _targetCore = _core.CreateInstance("Growl.CoreLibrary");
                Type _t = _connector.GetType("Growl.Connector.NotificationType");

                _growlNTreply = _t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] {
					"REPLY",
					"Reply"
				});

                _growlNTdm = _t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] {
					"DIRECT_MESSAGE",
					"DirectMessage"
				});

                _growlNTnew = _t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] {
					"NOTIFY",
					"新着通知"
				});

                _growlNTusevent = _t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] {
					"USERSTREAM_EVENT",
					"UserStream Event"
				});

                object encryptType = _connector.GetType("Growl.Connector.Cryptography+SymmetricAlgorithmType").InvokeMember("PlainText", BindingFlags.GetField, null, null, null);
                _targetConnector.GetType().InvokeMember("EncryptionAlgorithm", BindingFlags.SetProperty, null, _targetConnector, new object[] { encryptType });

                _growlApp = _connector.CreateInstance("Growl.Connector.Application", false, BindingFlags.Default, null, new object[] { _appName }, null, null);

                if (File.Exists(Path.Combine(Application.StartupPath, "Icons\\Tween.png")))
                {
                    // Icons\Tween.pngを使用
                    ConstructorInfo ci = _core.GetType("Growl.CoreLibrary.Resource").GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(System.String) }, null);

                    object data = ci.Invoke(new object[] { Path.Combine(Application.StartupPath, "Icons\\Tween.png") });
                    PropertyInfo pi = _growlApp.GetType().GetProperty("Icon");
                    pi.SetValue(_growlApp, data, null);
                }
                else if (File.Exists(Path.Combine(Application.StartupPath, "Icons\\MIcon.ico")))
                {
                    // アイコンセットにMIcon.icoが存在する場合それを使用
                    ConstructorInfo cibd = _core.GetType("Growl.CoreLibrary.BinaryData").GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(byte[]) }, null);
                    TypeConverter tc = new TypeConverter();
                    object bdata = cibd.Invoke(new object[] { IconToByteArray(Path.Combine(Application.StartupPath, "Icons\\MIcon.ico")) });

                    ConstructorInfo ciRes = _core.GetType("Growl.CoreLibrary.Resource").GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { bdata.GetType() }, null);

                    object data = ciRes.Invoke(new object[] { bdata });
                    PropertyInfo pi = _growlApp.GetType().GetProperty("Icon");
                    pi.SetValue(_growlApp, data, null);
                }
                else
                {
                    //内蔵アイコンリソースを使用
                    ConstructorInfo cibd = _core.GetType("Growl.CoreLibrary.BinaryData").GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(byte[]) }, null);
                    TypeConverter tc = new TypeConverter();
                    object bdata = cibd.Invoke(new object[] { IconToByteArray(Tween.My.Resources.Resources.MIcon) });

                    ConstructorInfo ciRes = _core.GetType("Growl.CoreLibrary.Resource").GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { bdata.GetType() }, null);

                    object data = ciRes.Invoke(new object[] { bdata });
                    PropertyInfo pi = _growlApp.GetType().GetProperty("Icon");
                    pi.SetValue(_growlApp, data, null);
                }

                MethodInfo mi = _targetConnector.GetType().GetMethod("Register", new Type[] {
					_growlApp.GetType(),
					_connector.GetType("Growl.Connector.NotificationType[]")
				});

                _t = _connector.GetType("Growl.Connector.NotificationType");

                ArrayList arglist = new ArrayList();
                arglist.Add(_growlNTreply);
                arglist.Add(_growlNTdm);
                arglist.Add(_growlNTnew);
                arglist.Add(_growlNTusevent);

                mi.Invoke(_targetConnector, new object[] {
					_growlApp,
					arglist.ToArray(_t)
				});

                // コールバックメソッドの登録
                Type tGrowlConnector = _connector.GetType("Growl.Connector.GrowlConnector");
                EventInfo evNotificationCallback = tGrowlConnector.GetEvent("NotificationCallback");
                Type tDelegate = evNotificationCallback.EventHandlerType;
                MethodInfo miHandler = typeof(GrowlHelper).GetMethod("GrowlCallbackHandler", BindingFlags.NonPublic | BindingFlags.Instance);
                Delegate d = Delegate.CreateDelegate(tDelegate, this, miHandler);
                MethodInfo miAddHandler = evNotificationCallback.GetAddMethod();
                object[] addHandlerArgs = { d };
                miAddHandler.Invoke(_targetConnector, addHandlerArgs);

                _initialized = true;
            }
            catch (Exception ex)
            {
                _initialized = false;
                return false;
            }

            return true;
        }

        public void Notify(NotifyType notificationType, string id, string title, string text, Image icon = null, string url = "")
        {
            if (!_initialized)
                return;
            string notificationName = "";
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
                Type gCore = _core.GetType("Growl.CoreLibrary.Resource");
                object res = null;
                if (icon != null)
                {
                    res = gCore.InvokeMember("op_Implicit", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { icon });
                }
                else
                {
                    res = gCore.InvokeMember("op_Implicit", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { url });
                }
                object priority = _connector.GetType("Growl.Connector.Priority").InvokeMember("Normal", BindingFlags.GetField, null, null, null);
                n = _connector.GetType("Growl.Connector.Notification").InvokeMember("Notification", BindingFlags.CreateInstance, null, _connector, new object[] {
					_appName,
					notificationName,
					id,
					title,
					text,
					res,
					false,
					priority,
					"aaa"
				});
            }
            else
            {
                n = _connector.GetType("Growl.Connector.Notification").InvokeMember("Notification", BindingFlags.CreateInstance, null, _connector, new object[] {
					_appName,
					notificationName,
					id,
					title,
					text
				});
            }
            //_targetConnector.GetType.InvokeMember("Notify", BindingFlags.InvokeMethod, Nothing, _targetConnector, New Object() {n})
            object cc = _connector.GetType("Growl.Connector.CallbackContext").InvokeMember(null, BindingFlags.CreateInstance, null, _connector, new object[] {
				"some fake information",
				notificationName
			});
            _targetConnector.GetType().InvokeMember("Notify", BindingFlags.InvokeMethod, null, _targetConnector, new object[] {
				n,
				cc
			});
        }

        private void GrowlCallbackHandler(object response, object callbackData, object state)
        {
            try
            {
                // 定数取得
                object vCLICK = _core.GetType("Growl.CoreLibrary.CallbackResult").GetField("CLICK", BindingFlags.Public | BindingFlags.Static).GetRawConstantValue();
                // 実際の値
                object vResult = callbackData.GetType().GetProperty("Result", BindingFlags.Public | BindingFlags.Instance).GetGetMethod().Invoke(callbackData, null);
                vResult = Convert.ToInt32(vResult);
                string notifyId = Convert.ToString(callbackData.GetType().GetProperty("NotificationID").GetGetMethod().Invoke(callbackData, null));
                string notifyName = Convert.ToString(callbackData.GetType().GetProperty("Type").GetGetMethod().Invoke(callbackData, null));
                if (vCLICK.Equals(vResult))
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
                    if (NotifyClicked != null)
                    {
                        NotifyClicked(this, new NotifyCallbackEventArgs(nt, notifyId));
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}