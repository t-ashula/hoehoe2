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
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// フォームのコンストラクタでこのクラスのインスタンス作成すること
    /// インスタンス変数はwitheventsで宣言し、HotkeyPressedイベントを受け取ること
    /// グローバルホットキーはRegisterOriginalHotkeyで登録。複数種類登録できるが、重複チェックはしていないので注意。
    /// 再設定前にはUnregisterAllOriginalHotkeyを呼ぶこと
    /// </summary>
    public class HookGlobalHotkey : NativeWindow, IDisposable
    {
        #region privates

        private Form targetForm;
        private Dictionary<int, KeyEventValue> hotkeyIds;
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        #endregion privates

        #region constructor

        public HookGlobalHotkey(Form targetForm)
        {
            this.hotkeyIds = new Dictionary<int, KeyEventValue>();
            this.targetForm = targetForm;
            this.targetForm.HandleCreated += this.OnHandleCreated;
            this.targetForm.HandleDestroyed += this.OnHandleDestroyed;
        }

        ~HookGlobalHotkey()
        {
            this.Dispose(false);
        }

        #endregion constructor

        #region event

        public event KeyEventHandler HotkeyPressed;

        #endregion event

        #region enums

        [Flags]
        public enum ModKeys : int
        {
            None = 0,
            Alt = 0x1,
            Ctrl = 0x2,
            Shift = 0x4,
            Win = 0x8
        }

        #endregion enums

        #region public methods

        public void OnHandleCreated(object sender, EventArgs e)
        {
            this.AssignHandle(this.targetForm.Handle);
        }

        public void OnHandleDestroyed(object sender, EventArgs e)
        {
            this.ReleaseHandle();
        }

        public bool RegisterOriginalHotkey(Keys hotkey, int hotkeyValue, ModKeys modifiers)
        {
            Keys modKey = Keys.None;
            if ((modifiers & ModKeys.Alt) == ModKeys.Alt)
            {
                modKey = modKey | Keys.Alt;
            }

            if ((modifiers & ModKeys.Ctrl) == ModKeys.Ctrl)
            {
                modKey = modKey | Keys.Control;
            }

            if ((modifiers & ModKeys.Shift) == ModKeys.Shift)
            {
                modKey = modKey | Keys.Shift;
            }

            if ((modifiers & ModKeys.Win) == ModKeys.Win)
            {
                modKey = modKey | Keys.LWin;
            }

            KeyEventArgs key = new KeyEventArgs(hotkey | modKey);
            foreach (KeyValuePair<int, KeyEventValue> kvp in this.hotkeyIds)
            {
                // 登録済みなら正常終了
                if (kvp.Value.KeyEvent.KeyData == key.KeyData && kvp.Value.Value == hotkeyValue)
                {
                    return true;
                }
            }

            int hotkeyId = Win32Api.RegisterGlobalHotKey(hotkeyValue, (int)modifiers, this.targetForm);
            if (hotkeyId > 0)
            {
                this.hotkeyIds.Add(hotkeyId, new KeyEventValue(key, hotkeyValue));
                return true;
            }

            return false;
        }

        public void UnregisterAllOriginalHotkey()
        {
            foreach (int hotkeyId in this.hotkeyIds.Keys)
            {
                Win32Api.UnregisterGlobalHotKey(hotkeyId, this.targetForm);
            }

            this.hotkeyIds.Clear();
        }

        #region " IDisposable Support "

        // このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion " IDisposable Support "

        #endregion public methods

        #region protected methods

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_HOTKEY = 0x312;
            if (m.Msg == WM_HOTKEY)
            {
                int wparam = m.WParam.ToInt32();
                if (this.hotkeyIds.ContainsKey(wparam))
                {
                    if (this.HotkeyPressed != null)
                    {
                        this.HotkeyPressed(this, this.hotkeyIds[wparam].KeyEvent);
                    }
                }

                return;
            }

            base.WndProc(ref m);
        }

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: 明示的に呼び出されたときにマネージ リソースを解放します
                }

                // TODO: 共有のアンマネージ リソースを解放します
                if (this.targetForm != null && !this.targetForm.IsDisposed)
                {
                    this.UnregisterAllOriginalHotkey();
                    this.targetForm.HandleCreated -= this.OnHandleCreated;
                    this.targetForm.HandleDestroyed -= this.OnHandleDestroyed;
                }
            }

            this.disposedValue = true;
        }

        #endregion protected methods

        #region inner classes

        private class KeyEventValue
        {
            public KeyEventValue(KeyEventArgs keyEvent, int value)
            {
                this.KeyEvent = keyEvent;
                this.Value = value;
            }

            public KeyEventArgs KeyEvent { get; private set; }

            public int Value { get; private set; }
        }

        #endregion inner classes
    }
}