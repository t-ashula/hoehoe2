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
    using R = Properties.Resources;

    public partial class HashtagManage
    {
        private readonly AtIdSupplement _hashSupl;       // 入力補助画面
        private string _useHash = string.Empty; // I/F用
        private bool _isPermanent;
        private bool _isHead;
        private bool _isNotAddToAtReply = true;
        private bool _isAdd; // 編集モード

        public HashtagManage(AtIdSupplement hashSuplForm, string[] history, string permanentHash, bool isPermanent, bool isHead, bool isNotAddToAtReply)
        {
            // この呼び出しは、Windows フォーム デザイナで必要です。
            InitializeComponent();

            // InitializeComponent() 呼び出しの後で初期化を追加します。
            _hashSupl = hashSuplForm;
            HistoryHashList.Items.AddRange(history);
            _useHash = permanentHash;
            _isPermanent = isPermanent;
            _isHead = isHead;
            _isNotAddToAtReply = isNotAddToAtReply;
        }

        public List<string> HashHistories
        {
            get
            {
                var hash = new List<string>();
                foreach (string item in HistoryHashList.Items)
                {
                    hash.Add(item);
                }

                return hash;
            }
        }

        public string UseHash
        {
            get { return _useHash; }
        }

        public bool IsPermanent
        {
            get { return _isPermanent; }
        }

        public bool IsHead
        {
            get { return _isHead; }
        }

        public bool IsNotAddToAtReply
        {
            get { return _isNotAddToAtReply; }
        }

        public void AddHashToHistory(string hash, bool isIgnorePermanent)
        {
            hash = hash.Trim();
            if (!string.IsNullOrEmpty(hash))
            {
                if (isIgnorePermanent || !_isPermanent)
                {
                    // 無条件に先頭に挿入
                    int idx = GetIndexOf(HistoryHashList.Items, hash);
                    if (idx != -1)
                    {
                        HistoryHashList.Items.RemoveAt(idx);
                    }

                    HistoryHashList.Items.Insert(0, hash);
                }
                else
                {
                    // 固定されていたら2行目に挿入
                    int idx = GetIndexOf(HistoryHashList.Items, hash);
                    if (_isPermanent)
                    {
                        if (idx > 0)
                        {
                            // 重複アイテムが2行目以降にあれば2行目へ
                            HistoryHashList.Items.RemoveAt(idx);
                            HistoryHashList.Items.Insert(1, hash);
                        }
                        else if (idx == -1)
                        {
                            // 重複アイテムなし
                            if (HistoryHashList.Items.Count == 0)
                            {
                                // リストが空なら追加
                                HistoryHashList.Items.Add(hash);
                            }
                            else
                            {
                                // リストにアイテムがあれば2行目へ
                                HistoryHashList.Items.Insert(1, hash);
                            }
                        }
                    }
                }
            }
        }

        public void ToggleHash()
        {
            if (string.IsNullOrEmpty(_useHash))
            {
                if (HistoryHashList.Items.Count > 0)
                {
                    _useHash = HistoryHashList.Items[0].ToString();
                }
            }
            else
            {
                _useHash = string.Empty;
            }
        }

        public void ClearHashtag()
        {
            _useHash = string.Empty;
        }

        public void SetPermanentHash(string hash)
        {
            // 固定ハッシュタグの変更
            _useHash = hash.Trim();
            AddHashToHistory(_useHash, false);
            _isPermanent = true;
        }

        private void ChangeMode(bool isEdit)
        {
            GroupHashtag.Enabled = !isEdit;
            GroupDetail.Enabled = isEdit;
            TableLayoutButtons.Enabled = !isEdit;
            if (isEdit)
            {
                UseHashText.Focus();
            }
            else
            {
                HistoryHashList.Focus();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            UseHashText.Text = string.Empty;
            ChangeMode(true);
            _isAdd = true;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (HistoryHashList.SelectedIndices.Count == 0)
            {
                return;
            }

            UseHashText.Text = HistoryHashList.SelectedItems[0].ToString();
            ChangeMode(true);
            _isAdd = false;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (HistoryHashList.SelectedIndices.Count == 0)
            {
                return;
            }

            if (MessageBox.Show(R.DeleteHashtagsMessage1, "Delete Hashtags", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
            {
                return;
            }

            for (int i = 0; i < HistoryHashList.SelectedIndices.Count; i++)
            {
                if (UseHashText.Text == HistoryHashList.SelectedItems[0].ToString())
                {
                    UseHashText.Text = string.Empty;
                }

                HistoryHashList.Items.RemoveAt(HistoryHashList.SelectedIndices[0]);
            }

            if (HistoryHashList.Items.Count > 0)
            {
                HistoryHashList.SelectedIndex = 0;
            }
        }

        private void UnSelectButton_Click(object sender, EventArgs e)
        {
            do
            {
                HistoryHashList.SelectedIndices.Clear();
            }
            while (HistoryHashList.SelectedIndices.Count > 0);
        }

        private int GetIndexOf(ListBox.ObjectCollection list, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return -1;
            }

            int idx = 0;
            foreach (object l in list)
            {
                var src = l as string;
                if (string.IsNullOrEmpty(src))
                {
                    idx++;
                    continue;
                }

                if (string.Compare(src, value, true) == 0)
                {
                    return idx;
                }

                idx++;
            }

            // Not Found
            return -1;
        }

        private void HashtagManage_Shown(object sender, EventArgs e)
        {
            // オプション
            CheckPermanent.Checked = _isPermanent;
            RadioHead.Checked = _isHead;
            RadioLast.Checked = !_isHead;

            // リスト選択
            if (HistoryHashList.Items.Contains(_useHash))
            {
                HistoryHashList.SelectedItem = _useHash;
            }
            else
            {
                if (HistoryHashList.Items.Count > 0)
                {
                    HistoryHashList.SelectedIndex = 0;
                }
            }

            ChangeMode(false);
        }

        private void UseHashText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '#')
            {
                _hashSupl.ShowDialog();
                if (!string.IsNullOrEmpty(_hashSupl.InputText))
                {
                    string front = string.Empty;
                    string last = string.Empty;
                    int selStart = UseHashText.SelectionStart;
                    if (selStart > 0)
                    {
                        front = UseHashText.Text.Substring(0, selStart);
                    }

                    if (selStart < UseHashText.Text.Length)
                    {
                        last = UseHashText.Text.Substring(selStart);
                    }

                    UseHashText.Text = front + _hashSupl.InputText + last;
                    UseHashText.SelectionStart = selStart + _hashSupl.InputText.Length;
                }

                e.Handled = true;
            }
        }

        private void HistoryHashList_DoubleClick(object sender, EventArgs e)
        {
            OkButton_Click(null, null);
        }

        private void PermOkButton_Click(object sender, EventArgs e)
        {
            // ハッシュタグの整形
            string hashStr = UseHashText.Text;
            if (!AdjustHashtags(ref hashStr, true))
            {
                return;
            }

            UseHashText.Text = hashStr;
            if (!_isAdd && HistoryHashList.SelectedIndices.Count > 0)
            {
                int idx = HistoryHashList.SelectedIndices[0];
                HistoryHashList.Items.RemoveAt(idx);
                do
                {
                    HistoryHashList.SelectedIndices.Clear();
                }
                while (HistoryHashList.SelectedIndices.Count > 0);
                HistoryHashList.Items.Insert(idx, hashStr);
                HistoryHashList.SelectedIndex = idx;
            }
            else
            {
                AddHashToHistory(hashStr, false);
                do
                {
                    HistoryHashList.SelectedIndices.Clear();
                }
                while (HistoryHashList.SelectedIndices.Count > 0);
                HistoryHashList.SelectedIndex = HistoryHashList.Items.IndexOf(hashStr);
            }

            ChangeMode(false);
        }

        private void PermCancelButton_Click(object sender, EventArgs e)
        {
            if (HistoryHashList.Items.Count > 0 && HistoryHashList.SelectedIndices.Count > 0)
            {
                UseHashText.Text = HistoryHashList.Items[HistoryHashList.SelectedIndices[0]].ToString();
            }
            else
            {
                UseHashText.Text = string.Empty;
            }

            ChangeMode(false);
        }

        private void HistoryHashList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteButton_Click(null, null);
            }
            else if (e.KeyCode == Keys.Insert)
            {
                AddButton_Click(null, null);
            }
        }

        private bool AdjustHashtags(ref string hashtag, bool isShowWarn)
        {
            // ハッシュタグの整形
            hashtag = hashtag.Trim();
            if (string.IsNullOrEmpty(hashtag))
            {
                if (isShowWarn)
                {
                    MessageBox.Show("emply hashtag.", "Hashtag warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }

                return false;
            }

            hashtag = hashtag.Replace("＃", "#");
            hashtag = hashtag.Replace("　", " ");
            string adjust = string.Empty;
            foreach (string hash in hashtag.Split(' '))
            {
                if (hash.Length > 0)
                {
                    if (!hash.StartsWith("#"))
                    {
                        if (isShowWarn)
                        {
                            MessageBox.Show("Invalid hashtag. -> " + hash, "Hashtag warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }

                        return false;
                    }

                    if (hash.Length == 1)
                    {
                        if (isShowWarn)
                        {
                            MessageBox.Show("empty hashtag.", "Hashtag warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }

                        return false;
                    }

                    adjust += hash + " ";
                }
            }

            hashtag = adjust.Trim();
            return true;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            string hash = string.Empty;
            foreach (string hs in HistoryHashList.SelectedItems)
            {
                hash += hs + " ";
            }

            hash = hash.Trim();
            if (!string.IsNullOrEmpty(hash))
            {
                AddHashToHistory(hash, true);
                _isPermanent = CheckPermanent.Checked;
            }
            else
            {
                _isPermanent = false; // 使用ハッシュが未選択ならば、固定オプション外す
            }

            _isHead = RadioHead.Checked;
            _useHash = hash;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void HashtagManage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (GroupDetail.Enabled)
                {
                    PermOkButton_Click(null, null);
                }
                else
                {
                    OkButton_Click(null, null);
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                if (GroupDetail.Enabled)
                {
                    PermCancelButton_Click(null, null);
                }
                else
                {
                    CancelButton_Click(null, null);
                }
            }
        }

        private void CheckNotAddToAtReply_CheckedChanged(object sender, EventArgs e)
        {
            _isNotAddToAtReply = CheckNotAddToAtReply.Checked;
        }
    }
}