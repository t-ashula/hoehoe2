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

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Hoehoe.TweenCustomControl
{
    public sealed class DetailsListView : ListView
    {
        #region private fields

        private Rectangle _changeBounds;
        private SCROLLINFO _scrollInfo;

        #endregion

        #region constructor

        public DetailsListView()
        {
            AllowColumnReorder = true;
            Dock = DockStyle.Fill;
            FullRowSelect = true;
            HideSelection = false;
            Location = new Point(0, 0);
            Margin = new Padding(0);
            Name = string.Format("CList{0}", Environment.TickCount);
            ShowItemToolTips = true;
            Size = new Size(380, 260);
            UseCompatibleStateImageBehavior = false;
            View = View.Details;
            OwnerDraw = true;
            VirtualMode = true;
            AllowDrop = true;
            View = View.Details;
            FullRowSelect = true;
            HideSelection = false;
            DoubleBuffered = true;
            _scrollInfo = new SCROLLINFO
                {
                    cbSize = (uint)Marshal.SizeOf(_scrollInfo),
                    fMask = (uint)ScrollInfoMask.SIF_POS
                };
        }

        #endregion

        #region events

        public event EventHandler VScrolled;

        public event EventHandler HScrolled;

        #endregion

        #region enums

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

        #endregion

        #region public methods

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
            Items[itemIndex].SubItems[subitemIndex].BackColor = backColor;
            SetUpdateBounds(itemIndex, subitemIndex);
            Update();
            _changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemForeColor(int itemIndex, int subitemIndex, Color foreColor)
        {
            Items[itemIndex].SubItems[subitemIndex].ForeColor = foreColor;
            SetUpdateBounds(itemIndex, subitemIndex);
            Update();
            _changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemFont(int itemIndex, int subitemIndex, Font fnt)
        {
            Items[itemIndex].SubItems[subitemIndex].Font = fnt;
            SetUpdateBounds(itemIndex, subitemIndex);
            Update();
            _changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemFontAndColor(int itemIndex, int subitemIndex, Color foreColor, Font fnt)
        {
            Items[itemIndex].SubItems[subitemIndex].ForeColor = foreColor;
            Items[itemIndex].SubItems[subitemIndex].Font = fnt;
            SetUpdateBounds(itemIndex, subitemIndex);
            Update();
            _changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemStyles(int itemIndex, int subitemIndex, Color backColor, Color foreColor, Font fnt)
        {
            Items[itemIndex].SubItems[subitemIndex].BackColor = backColor;
            Items[itemIndex].SubItems[subitemIndex].ForeColor = foreColor;
            Items[itemIndex].SubItems[subitemIndex].Font = fnt;
            SetUpdateBounds(itemIndex, subitemIndex);
            Update();
            _changeBounds = Rectangle.Empty;
        }

        public void SelectAllItem()
        {
            //// foreach (ListViewItem lvi in Items) { lvi.Selected = true; }
            for (int i = 0; i < VirtualListSize; i++)
            {
                SelectedIndices.Add(i);
            }
        }

        #endregion

        #region protected method

        [DebuggerStepThrough]
        protected override void WndProc(ref Message m)
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

            int horizontalPos = -1;
            int verticalPos = -1;

            switch (m.Msg)
            {
                case WM_ERASEBKGND:
                    if (_changeBounds != Rectangle.Empty)
                    {
                        m.Msg = 0;
                    }

                    break;

                case WM_PAINT:
                    if (_changeBounds != Rectangle.Empty)
                    {
                        Win32Api.ValidateRect(Handle, IntPtr.Zero);
                        Invalidate(_changeBounds);
                        _changeBounds = Rectangle.Empty;
                    }

                    break;

                case WM_HSCROLL:
                    OnHScrolled(this, EventArgs.Empty);
                    break;

                case WM_VSCROLL:
                    OnVScrolled(this, EventArgs.Empty);
                    break;

                case WM_MOUSEWHEEL:
                case WM_MOUSEHWHEEL:
                case WM_KEYDOWN:
                    if (GetScrollInfo(Handle, ScrollBarDirection.SB_VERT, ref _scrollInfo) != 0)
                    {
                        verticalPos = _scrollInfo.nPos;
                    }

                    if (GetScrollInfo(Handle, ScrollBarDirection.SB_HORZ, ref _scrollInfo) != 0)
                    {
                        horizontalPos = _scrollInfo.nPos;
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
                // Substringでlengthが0以下。アイコンサイズが影響？
            }
            catch (AccessViolationException)
            {
                // WndProcのさらに先で発生する。
            }

            if (IsDisposed)
            {
                return;
            }

            if (verticalPos != -1)
            {
                if (GetScrollInfo(Handle, ScrollBarDirection.SB_VERT, ref _scrollInfo) != 0 && verticalPos != _scrollInfo.nPos)
                {
                    OnVScrolled(this, EventArgs.Empty);
                }
            }

            if (horizontalPos != -1)
            {
                if (GetScrollInfo(Handle, ScrollBarDirection.SB_HORZ, ref _scrollInfo) != 0 && horizontalPos != _scrollInfo.nPos)
                {
                    OnHScrolled(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region private methods

        [DllImport("user32.dll")]
        private static extern int GetScrollInfo(IntPtr hWnd, ScrollBarDirection fnBar, ref SCROLLINFO lpsi);

        private void OnHScrolled(object sender, EventArgs e)
        {
            if (HScrolled != null)
            {
                HScrolled(this, e);
            }
        }

        private void OnVScrolled(object sender, EventArgs e)
        {
            if (VScrolled != null)
            {
                VScrolled(this, e);
            }
        }

        private void SetUpdateBounds(int itemIndex, int subItemIndex)
        {
            try
            {
                if (itemIndex > Items.Count)
                {
                    throw new ArgumentOutOfRangeException("itemIndex");
                }

                if (subItemIndex > Columns.Count)
                {
                    throw new ArgumentOutOfRangeException("subItemIndex");
                }

                ListViewItem item = Items[itemIndex];
                _changeBounds = item.UseItemStyleForSubItems ?
                    item.Bounds :
                    GetSubItemBounds(itemIndex, subItemIndex);
            }
            catch (ArgumentException)
            {
                // タイミングによりBoundsプロパティが取れない？
                _changeBounds = Rectangle.Empty;
            }
        }

        private Rectangle GetSubItemBounds(int itemIndex, int subitemIndex)
        {
            ListViewItem item = Items[itemIndex];
            if (subitemIndex == 0 && Columns.Count > 0)
            {
                Rectangle col0 = item.Bounds;
                return new Rectangle(col0.Left, col0.Top, item.SubItems[1].Bounds.X + 1, col0.Height);
            }

            return item.SubItems[subitemIndex].Bounds;
        }

        #endregion

        #region inner types

        [StructLayout(LayoutKind.Sequential)]
        private struct SCROLLINFO
        {
            public uint cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        #endregion
    }
}