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
using System.Collections.Generic;
using System.Windows.Forms;

namespace Tween
{
    //ソート比較クラス：ID比較のみ
    public sealed class IdComparerClass : IComparer<long>
    {
        /// <summary>
        /// 比較する方法
        /// </summary>
        public enum ComparerMode
        {
            Id,
            Data,
            Name,
            Nickname,
            Source
        }

        private SortOrder _order;
        private ComparerMode _mode;
        private Dictionary<long, PostClass> _statuses;

        private Comparison<long> _CmpMethod;

        /// <summary>
        /// 昇順か降順か Setの際は同時に比較関数の切り替えを行う
        /// </summary>
        public SortOrder Order
        {
            get { return _order; }
            set
            {
                _order = value;
                SetCmpMethod(_mode, _order);
            }
        }

        /// <summary>
        /// 並び替えの方法 Setの際は同時に比較関数の切り替えを行う
        /// </summary>
        public ComparerMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                SetCmpMethod(_mode, _order);
            }
        }

        /// <summary>
        /// ListViewItemComparerクラスのコンストラクタ（引数付は未使用）
        /// </summary>
        /// <param name="col">並び替える列番号</param>
        /// <param name="ord">昇順か降順か</param>
        /// <param name="cmod">並び替えの方法</param>

        public IdComparerClass()
        {
            _order = SortOrder.Ascending;
            _mode = ComparerMode.Id;
            SetCmpMethod(_mode, _order);
        }

        public Dictionary<long, PostClass> posts
        {
            get { return _statuses; }
            set { _statuses = value; }
        }

        // 指定したソートモードとソートオーダーに従い使用する比較関数のアドレスを返す
        public Comparison<long> CmpMethod(ComparerMode _sortmode, SortOrder _sortorder)
        {
            //get
            {
                Comparison<long> _method = null;
                if (_sortorder == SortOrder.Ascending)
                {
                    // 昇順
                    switch (_sortmode)
                    {
                        case ComparerMode.Data:
                            _method = Compare_ModeData_Ascending;
                            break;
                        case ComparerMode.Id:
                            _method = Compare_ModeId_Ascending;
                            break;
                        case ComparerMode.Name:
                            _method = Compare_ModeName_Ascending;
                            break;
                        case ComparerMode.Nickname:
                            _method = Compare_ModeNickName_Ascending;
                            break;
                        case ComparerMode.Source:
                            _method = Compare_ModeSource_Ascending;
                            break;
                    }
                }
                else
                {
                    // 降順
                    switch (_sortmode)
                    {
                        case ComparerMode.Data:
                            _method = Compare_ModeData_Descending;
                            break;
                        case ComparerMode.Id:
                            _method = Compare_ModeId_Descending;
                            break;
                        case ComparerMode.Name:
                            _method = Compare_ModeName_Descending;
                            break;
                        case ComparerMode.Nickname:
                            _method = Compare_ModeNickName_Descending;
                            break;
                        case ComparerMode.Source:
                            _method = Compare_ModeSource_Descending;
                            break;
                    }
                }
                return _method;
            }
        }

        // ソートモードとソートオーダーに従い使用する比較関数のアドレスを返す
        // (overload 現在の使用中の比較関数のアドレスを返す)
        public Comparison<long> CmpMethod()
        {
            { return _CmpMethod; }
        }

        // ソートモードとソートオーダーに従い比較関数のアドレスを切り替え
        private void SetCmpMethod(ComparerMode mode, SortOrder order)
        {
            _CmpMethod = this.CmpMethod(mode, order);
        }

        //xがyより小さいときはマイナスの数、大きいときはプラスの数、
        //同じときは0を返す (こちらは未使用 一応比較関数群呼び出しの形のまま残しておく)
        public int Compare(long x, long y)
        {
            return _CmpMethod(x, y);
        }

        // 比較用関数群 いずれもステータスIDの順序を考慮する
        // 本文比較　昇順
        public int Compare_ModeData_Ascending(long x, long y)
        {
            int result = string.Compare(_statuses[x].TextFromApi, _statuses[y].TextFromApi);
            if (result == 0)
                result = x.CompareTo(y);
            return result;
        }

        // 本文比較　降順
        public int Compare_ModeData_Descending(long x, long y)
        {
            int result = string.Compare(_statuses[y].TextFromApi, _statuses[x].TextFromApi);
            if (result == 0)
                result = y.CompareTo(x);
            return result;
        }

        // ステータスID比較　昇順
        public int Compare_ModeId_Ascending(long x, long y)
        {
            return x.CompareTo(y);
        }

        // ステータスID比較　降順
        public int Compare_ModeId_Descending(long x, long y)
        {
            return y.CompareTo(x);
        }

        // 表示名比較　昇順
        public int Compare_ModeName_Ascending(long x, long y)
        {
            int result = string.Compare(_statuses[x].ScreenName, _statuses[y].ScreenName);
            if (result == 0)
                result = x.CompareTo(y);
            return result;
        }

        // 表示名比較　降順
        public int Compare_ModeName_Descending(long x, long y)
        {
            int result = string.Compare(_statuses[y].ScreenName, _statuses[x].ScreenName);
            if (result == 0)
                result = y.CompareTo(x);
            return result;
        }

        // ユーザー名比較　昇順
        public int Compare_ModeNickName_Ascending(long x, long y)
        {
            int result = string.Compare(_statuses[x].Nickname, _statuses[y].Nickname);
            if (result == 0)
                result = x.CompareTo(y);
            return result;
        }

        // ユーザー名比較　降順
        public int Compare_ModeNickName_Descending(long x, long y)
        {
            int result = string.Compare(_statuses[y].Nickname, _statuses[x].Nickname);
            if (result == 0)
                result = y.CompareTo(x);
            return result;
        }

        // Source比較　昇順
        public int Compare_ModeSource_Ascending(long x, long y)
        {
            int result = string.Compare(_statuses[x].Source, _statuses[y].Source);
            if (result == 0)
                result = x.CompareTo(y);
            return result;
        }

        // Source比較　降順
        public int Compare_ModeSource_Descending(long x, long y)
        {
            int result = string.Compare(_statuses[y].Source, _statuses[x].Source);
            if (result == 0)
                result = y.CompareTo(x);
            return result;
        }
    }
}