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

namespace Hoehoe.TweenCustomControl
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public sealed class DetailsListView : ListView
    {
        #region private fields
        private Rectangle changeBounds;
        private EventHandlerList handlers = new EventHandlerList();
        private SCROLLINFO scrollInfo;
        #endregion

        #region constructor
        public DetailsListView()
        {
            this.View = View.Details;
            this.FullRowSelect = true;
            this.HideSelection = false;
            this.DoubleBuffered = true;
            this.scrollInfo = new SCROLLINFO()
            {
                cbSize = (uint)Marshal.SizeOf(this.scrollInfo),
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
            this.ChangeSubItemBackColor(index, 0, backColor);
        }

        public void ChangeItemForeColor(int index, Color foreColor)
        {
            this.ChangeSubItemForeColor(index, 0, foreColor);
        }

        public void ChangeItemFont(int index, Font fnt)
        {
            this.ChangeSubItemFont(index, 0, fnt);
        }

        public void ChangeItemFontAndColor(int index, Color foreColor, Font fnt)
        {
            this.ChangeSubItemStyles(index, 0, this.BackColor, foreColor, fnt);
        }

        public void ChangeItemStyles(int index, Color backColor, Color foreColor, Font fnt)
        {
            this.ChangeSubItemStyles(index, 0, backColor, foreColor, fnt);
        }

        public void ChangeSubItemBackColor(int itemIndex, int subitemIndex, Color backColor)
        {
            this.Items[itemIndex].SubItems[subitemIndex].BackColor = backColor;
            this.SetUpdateBounds(itemIndex, subitemIndex);
            this.Update();
            this.changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemForeColor(int itemIndex, int subitemIndex, Color foreColor)
        {
            this.Items[itemIndex].SubItems[subitemIndex].ForeColor = foreColor;
            this.SetUpdateBounds(itemIndex, subitemIndex);
            this.Update();
            this.changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemFont(int itemIndex, int subitemIndex, Font fnt)
        {
            this.Items[itemIndex].SubItems[subitemIndex].Font = fnt;
            this.SetUpdateBounds(itemIndex, subitemIndex);
            this.Update();
            this.changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemFontAndColor(int itemIndex, int subitemIndex, Color foreColor, Font fnt)
        {
            this.Items[itemIndex].SubItems[subitemIndex].ForeColor = foreColor;
            this.Items[itemIndex].SubItems[subitemIndex].Font = fnt;
            this.SetUpdateBounds(itemIndex, subitemIndex);
            this.Update();
            this.changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemStyles(int itemIndex, int subitemIndex, Color backColor, Color foreColor, Font fnt)
        {
            this.Items[itemIndex].SubItems[subitemIndex].BackColor = backColor;
            this.Items[itemIndex].SubItems[subitemIndex].ForeColor = foreColor;
            this.Items[itemIndex].SubItems[subitemIndex].Font = fnt;
            this.SetUpdateBounds(itemIndex, subitemIndex);
            this.Update();
            this.changeBounds = Rectangle.Empty;
        }

        public void SelectAllItem()
        {
            //// foreach (ListViewItem lvi in this.Items) { lvi.Selected = true; }
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
                    if (this.changeBounds != Rectangle.Empty)
                    {
                        m.Msg = 0;
                    }

                    break;
                case WM_PAINT:
                    if (this.changeBounds != Rectangle.Empty)
                    {
                        Win32Api.ValidateRect(this.Handle, IntPtr.Zero);
                        this.Invalidate(this.changeBounds);
                        this.changeBounds = Rectangle.Empty;
                    }

                    break;
                case WM_HSCROLL:
                    this.OnHScrolled(this, EventArgs.Empty);
                    break;
                case WM_VSCROLL:
                    this.OnVScrolled(this, EventArgs.Empty);
                    break;
                case WM_MOUSEWHEEL:
                case WM_MOUSEHWHEEL:
                case WM_KEYDOWN:
                    if (GetScrollInfo(this.Handle, ScrollBarDirection.SB_VERT, ref this.scrollInfo) != 0)
                    {
                        verticalPos = this.scrollInfo.nPos;
                    }

                    if (GetScrollInfo(this.Handle, ScrollBarDirection.SB_HORZ, ref this.scrollInfo) != 0)
                    {
                        horizontalPos = this.scrollInfo.nPos;
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

            if (this.IsDisposed)
            {
                return;
            }

            if (verticalPos != -1)
            {
                if (GetScrollInfo(this.Handle, ScrollBarDirection.SB_VERT, ref this.scrollInfo) != 0 && verticalPos != this.scrollInfo.nPos)
                {
                    this.OnVScrolled(this, EventArgs.Empty);
                }
            }

            if (horizontalPos != -1)
            {
                if (GetScrollInfo(this.Handle, ScrollBarDirection.SB_HORZ, ref this.scrollInfo) != 0 && horizontalPos != this.scrollInfo.nPos)
                {
                    this.OnHScrolled(this, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region private methods

        [DllImport("user32.dll")]
        private static extern int GetScrollInfo(IntPtr hWnd, ScrollBarDirection fnBar, ref SCROLLINFO lpsi);

        private void OnHScrolled(object sender, EventArgs e)
        {
            if (this.HScrolled != null)
            {
                this.HScrolled(this, e);
            }
        }

        private void OnVScrolled(object sender, EventArgs e)
        {
            if (this.VScrolled != null)
            {
                this.VScrolled(this, e);
            }
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
                    this.changeBounds = item.Bounds;
                }
                else
                {
                    this.changeBounds = this.GetSubItemBounds(itemIndex, subItemIndex);
                }
            }
            catch (ArgumentException)
            {
                // タイミングによりBoundsプロパティが取れない？
                this.changeBounds = Rectangle.Empty;
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