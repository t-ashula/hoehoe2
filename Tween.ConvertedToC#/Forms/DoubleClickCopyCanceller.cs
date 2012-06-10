// Hoehoe - Client of Twitter
// Copyright (c) 2007-2010 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2010 Moz (@syo68k) <http://iddy.jp/profile/moz/>
//           (c) 2008-2010 takeshik (@takeshik) <http://www.takeshik.org/>
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
    using System.Windows.Forms;

    public class DoubleClickCopyCanceller : NativeWindow, IDisposable
    {
        #region privates

        private bool doubleClick;

        #endregion privates

        #region constructor

        public DoubleClickCopyCanceller(Control control)
        {
            this.AssignHandle(control.Handle);
        }

        #endregion constructor

        #region public methods

        public void Dispose()
        {
            this.ReleaseHandle();
            GC.SuppressFinalize(this);
        }

        #endregion public methods

        #region protected methods

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_GETTEXTLENGTH = 0xe;
            const int WM_LBUTTONDBLCLK = 0x203;

            if (m.Msg == WM_LBUTTONDBLCLK)
            {
                this.doubleClick = true;
            }

            if (this.doubleClick)
            {
                if (m.Msg == WM_GETTEXTLENGTH)
                {
                    this.doubleClick = false;
                    m.Result = IntPtr.Zero; // (IntPtr)0;
                    return;
                }
            }

            base.WndProc(ref m);
        }

        #endregion protected methods
    }
}