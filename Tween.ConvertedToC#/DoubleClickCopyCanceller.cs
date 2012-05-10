using System;
using System.Windows.Forms;

namespace Tween
{
    // Tween - Client of Twitter
    // Copyright (c) 2007-2010 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
    //           (c) 2008-2010 Moz (@syo68k) <http://iddy.jp/profile/moz/>
    //           (c) 2008-2010 takeshik (@takeshik) <http://www.takeshik.org/>
    // All rights reserved.
    //
    // This file is part of Tween.
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

    public class DoubleClickCopyCanceller : NativeWindow, IDisposable
    {
        const int WM_GETTEXTLENGTH = 0xe;
        const int WM_GETTEXT = 0xd;
        const int WM_LBUTTONDBLCLK = 0x203;

        bool _doubleClick = false;

        public DoubleClickCopyCanceller(Control control)
        {
            this.AssignHandle(control.Handle);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == WM_LBUTTONDBLCLK)
            {
                _doubleClick = true;
            }
            if (_doubleClick)
            {
                if (m.Msg == WM_GETTEXTLENGTH)
                {
                    _doubleClick = false;
                    m.Result = (IntPtr)0;
                    return;
                }
            }
            base.WndProc(ref m);
        }

        public void Dispose()
        {
            this.ReleaseHandle();
            GC.SuppressFinalize(this);
        }
    }
}