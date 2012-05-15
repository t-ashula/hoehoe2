// Tween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
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

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Tween.TweenCustomControl
{
    public sealed class DetailsListView : ListView
    {
        private Rectangle _changeBounds;
        private EventHandlerList _handlers = new EventHandlerList();

        public event EventHandler VScrolled;

        public event EventHandler HScrolled;

        public DetailsListView()
        {
            this.View = View.Details;
            this.FullRowSelect = true;
            this.HideSelection = false;
            this.DoubleBuffered = true;
            _si = new SCROLLINFO() { cbSize = Marshal.SizeOf(_si), fMask = (int)ScrollInfoMask.SIF_POS };
        }

        public void ChangeItemBackColor(int index, Color backColor)
        {
            ChangeSubItemBackColor(index, 0, backColor);
        }

        public void ChangeItemForeColor(int index, Color foreColor)
        {
            ChangeSubItemForeColor(index, 0, foreColor);
        }

        public void ChangeItemFont(int index, Font fnt)
        {
            ChangeSubItemFont(index, 0, fnt);
        }

        public void ChangeItemFontAndColor(int index, Color foreColor, Font fnt)
        {
            ChangeSubItemStyles(index, 0, BackColor, foreColor, fnt);
        }

        public void ChangeItemStyles(int index, Color backColor, Color foreColor, Font fnt)
        {
            ChangeSubItemStyles(index, 0, backColor, foreColor, fnt);
        }

        public void ChangeSubItemBackColor(int itemIndex, int subitemIndex, Color backColor)
        {
            this.Items[itemIndex].SubItems[subitemIndex].BackColor = backColor;
            SetUpdateBounds(itemIndex, subitemIndex);
            this.Update();
            this._changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemForeColor(int itemIndex, int subitemIndex, Color foreColor)
        {
            this.Items[itemIndex].SubItems[subitemIndex].ForeColor = foreColor;
            SetUpdateBounds(itemIndex, subitemIndex);
            this.Update();
            this._changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemFont(int itemIndex, int subitemIndex, Font fnt)
        {
            this.Items[itemIndex].SubItems[subitemIndex].Font = fnt;
            SetUpdateBounds(itemIndex, subitemIndex);
            this.Update();
            this._changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemFontAndColor(int itemIndex, int subitemIndex, Color foreColor, Font fnt)
        {
            this.Items[itemIndex].SubItems[subitemIndex].ForeColor = foreColor;
            this.Items[itemIndex].SubItems[subitemIndex].Font = fnt;
            SetUpdateBounds(itemIndex, subitemIndex);
            this.Update();
            this._changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemStyles(int itemIndex, int subitemIndex, Color backColor, Color foreColor, Font fnt)
        {
            this.Items[itemIndex].SubItems[subitemIndex].BackColor = backColor;
            this.Items[itemIndex].SubItems[subitemIndex].ForeColor = foreColor;
            this.Items[itemIndex].SubItems[subitemIndex].Font = fnt;
            SetUpdateBounds(itemIndex, subitemIndex);
            this.Update();
            this._changeBounds = Rectangle.Empty;
        }

        private void SetUpdateBounds(int itemIndex, int subItemIndex)
        {
            try
            {
                if (itemIndex > this.Items.Count)
                {
                    throw new ArgumentOutOfRangeException("itemIndex");
                }
                if (subItemIndex > this.Columns.Count)
                {
                    throw new ArgumentOutOfRangeException("subItemIndex");
                }
                ListViewItem item = this.Items[itemIndex];
                if (item.UseItemStyleForSubItems)
                {
                    this._changeBounds = item.Bounds;
                }
                else
                {
                    this._changeBounds = this.GetSubItemBounds(itemIndex, subItemIndex);
                }
            }
            catch (ArgumentException)
            {
                //タイミングによりBoundsプロパティが取れない？
                this._changeBounds = Rectangle.Empty;
            }
        }

        private Rectangle GetSubItemBounds(int itemIndex, int subitemIndex)
        {
            ListViewItem item = this.Items[itemIndex];
            if (subitemIndex == 0 & this.Columns.Count > 0)
            {
                Rectangle col0 = item.Bounds;
                return new Rectangle(col0.Left, col0.Top, item.SubItems[1].Bounds.X + 1, col0.Height);
            }
            else
            {
                return item.SubItems[subitemIndex].Bounds;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SCROLLINFO
        {
            public int cbSize;
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        private enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        private enum ScrollInfoMask
        {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS)
        }

        [DllImport("user32.dll")]
        private static extern int GetScrollInfo(IntPtr hWnd, ScrollBarDirection fnBar, ref SCROLLINFO lpsi);

        private SCROLLINFO _si;

        [DebuggerStepThrough()]
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_ERASEBKGND = 0x14;
            const int WM_PAINT = 0xf;
            const int WM_MOUSEWHEEL = 0x20a;
            const int WM_MOUSEHWHEEL = 0x20e;
            const int WM_HSCROLL = 0x114;
            const int WM_VSCROLL = 0x115;
            const int WM_KEYDOWN = 0x100;
            const int LVM_SETITEMCOUNT = 0x102f;
            const long LVSICF_NOSCROLL = 0x2;
            const long LVSICF_NOINVALIDATEALL = 0x1;

            int hPos = -1;
            int vPos = -1;

            switch (m.Msg)
            {
                case WM_ERASEBKGND:
                    if (this._changeBounds != Rectangle.Empty)
                    {
                        m.Msg = 0;
                    }
                    break;
                case WM_PAINT:
                    if (this._changeBounds != Rectangle.Empty)
                    {
                        Win32Api.ValidateRect(this.Handle, IntPtr.Zero);
                        this.Invalidate(this._changeBounds);
                        this._changeBounds = Rectangle.Empty;
                    }
                    break;
                case WM_HSCROLL:
                    if (HScrolled != null)
                    {
                        HScrolled(this, EventArgs.Empty);
                    }

                    break;
                case WM_VSCROLL:
                    if (VScrolled != null)
                    {
                        VScrolled(this, EventArgs.Empty);
                    }

                    break;
                case WM_MOUSEWHEEL:
                case WM_MOUSEHWHEEL:
                case WM_KEYDOWN:
                    if (GetScrollInfo(this.Handle, ScrollBarDirection.SB_VERT, ref _si) != 0)
                    {
                        vPos = _si.nPos;
                    }
                    if (GetScrollInfo(this.Handle, ScrollBarDirection.SB_HORZ, ref _si) != 0)
                    {
                        hPos = _si.nPos;
                    }
                    break;
                case LVM_SETITEMCOUNT:
                    m.LParam = new IntPtr(LVSICF_NOSCROLL | LVSICF_NOINVALIDATEALL);
                    break;
            }

            try
            {
                base.WndProc(ref m);
            }
            catch (ArgumentOutOfRangeException)
            {
                //Substringでlengthが0以下。アイコンサイズが影響？
            }
            catch (AccessViolationException)
            {
                //WndProcのさらに先で発生する。
            }
            if (this.IsDisposed)
            {
                return;
            }

            if (vPos != -1)
            {
                if (GetScrollInfo(this.Handle, ScrollBarDirection.SB_VERT, ref _si) != 0 && vPos != _si.nPos)
                {
                    if (VScrolled != null)
                    {
                        VScrolled(this, EventArgs.Empty);
                    }
                }
            }
            if (hPos != -1)
            {
                if (GetScrollInfo(this.Handle, ScrollBarDirection.SB_HORZ, ref _si) != 0 && hPos != _si.nPos)
                {
                    if (HScrolled != null)
                    {
                        HScrolled(this, EventArgs.Empty);
                    }
                }
            }
        }
    }
}