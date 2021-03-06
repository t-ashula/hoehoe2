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
using System.Drawing;
using System.Runtime.InteropServices;

namespace Hoehoe
{
    public class ShieldIcon
    {
        private readonly Image _icondata;
        private readonly SHSTOCKICONINFO _sii;

        public ShieldIcon()
        {
            const int SIID_SHIELD = 77;
            const uint SHGFI_ICON = 0x100;
            const uint SHGFI_SMALLICON = 0x1;

            // NT6 kernelかどうか検査
            if (!IsNT6())
            {
                _icondata = null;
                return;
            }

            try
            {
                _sii = new SHSTOCKICONINFO();
                _sii.cbSize = Marshal.SizeOf(_sii);
                _sii.hIcon = IntPtr.Zero;
                SHGetStockIconInfo(SIID_SHIELD, SHGFI_ICON | SHGFI_SMALLICON, ref _sii);
                _icondata = Bitmap.FromHicon(_sii.hIcon);
            }
            catch (Exception)
            {
                _icondata = null;
            }
        }

        public Image Icon
        {
            // Return icondata
            // シールドアイコンのデータを返さないように　あとでどうにかする
            get { return null; }
        }

        public void Dispose()
        {
            if (_icondata != null)
            {
                _icondata.Dispose();
            }
        }

        private static bool IsNT6()
        {
            // NT6 kernelかどうか検査
            return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major == 6;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SHGetStockIconInfo(int siid, uint flags, ref SHSTOCKICONINFO psii);

        // [DllImport("shell32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        // private static extern bool DestroyIcon(IntPtr hIcon);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHSTOCKICONINFO
        {
            public int cbSize;
            public IntPtr hIcon;
            public int iSysImageIndex;
            public int iIcon;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szPath;
        }
    }
}