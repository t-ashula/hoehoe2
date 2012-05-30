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

    public partial class HashtagManage
    {
        private AtIdSupplement hashSupl;       // 入力補助画面
        private string useHash = string.Empty; // I/F用
        private bool isPermanent = false;
        private bool isHead = false;
        private bool isNotAddToAtReply = true;
        private bool isAdd; // 編集モード

        public HashtagManage(AtIdSupplement hashSuplForm, string[] history, string permanentHash, bool isPermanent, bool isHead, bool isNotAddToAtReply)
        {
            // この呼び出しは、Windows フォーム デザイナで必要です。
            this.InitializeComponent();

            // InitializeComponent() 呼び出しの後で初期化を追加します。
            this.hashSupl = hashSuplForm;
            this.HistoryHashList.Items.AddRange(history);
            this.useHash = permanentHash;
            this.isPermanent = isPermanent;
            this.isHead = isHead;
            this.isNotAddToAtReply = isNotAddToAtReply;
        }

        public List<string> HashHistories
        {
            get
            {
                List<string> hash = new List<string>();
                foreach (string item in this.HistoryHashList.Items)
                {
                    hash.Add(item);
                }

                return hash;
            }
        }

        public string UseHash
        {
            get { return this.useHash; }
        }

        public bool IsPermanent
        {
            get { return this.isPermanent; }
        }

        public bool IsHead
        {
            get { return this.isHead; }
        }

        public bool IsNotAddToAtReply
        {
            get { return this.isNotAddToAtReply; }
        }

        public void AddHashToHistory(string hash, bool isIgnorePermanent)
        {
            hash = hash.Trim();
            if (!string.IsNullOrEmpty(hash))
            {
                if (isIgnorePermanent || !this.isPermanent)
                {
                    // 無条件に先頭に挿入
                    int idx = this.GetIndexOf(this.HistoryHashList.Items, hash);
                    if (idx != -1)
                    {
                        this.HistoryHashList.Items.RemoveAt(idx);
                    }

                    this.HistoryHashList.Items.Insert(0, hash);
                }
                else
                {
                    // 固定されていたら2行目に挿入
                    int idx = this.GetIndexOf(this.HistoryHashList.Items, hash);
                    if (this.isPermanent)
                    {
                        if (idx > 0)
                        {
                            // 重複アイテムが2行目以降にあれば2行目へ
                            this.HistoryHashList.Items.RemoveAt(idx);
                            this.HistoryHashList.Items.Insert(1, hash);
                        }
                        else if (idx == -1)
                        {
                            // 重複アイテムなし
                            if (this.HistoryHashList.Items.Count == 0)
                            {
                                // リストが空なら追加
                                this.HistoryHashList.Items.Add(hash);
                            }
                            else
                            {
                                // リストにアイテムがあれば2行目へ
                                this.HistoryHashList.Items.Insert(1, hash);
                            }
                        }
                    }
                }
            }
        }

        public void ToggleHash()
        {
            if (string.IsNullOrEmpty(this.useHash))
            {
                if (this.HistoryHashList.Items.Count > 0)
                {
                    this.useHash = this.HistoryHashList.Items[0].ToString();
                }
            }
            else
            {
                this.useHash = string.Empty;
            }
        }

        public void ClearHashtag()
        {
            this.useHash = string.Empty;
        }

        public void SetPermanentHash(string hash)
        {
            // 固定ハッシュタグの変更
            this.useHash = hash.Trim();
            this.AddHashToHistory(this.useHash, false);
            this.isPermanent = true;
        }

        private void ChangeMode(bool isEdit)
        {
            this.GroupHashtag.Enabled = !isEdit;
            this.GroupDetail.Enabled = isEdit;
            this.TableLayoutButtons.Enabled = !isEdit;
            if (isEdit)
            {
                this.UseHashText.Focus();
            }
            else
            {
                this.HistoryHashList.Focus();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            this.UseHashText.Text = string.Empty;
            this.ChangeMode(true);
            this.isAdd = true;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (this.HistoryHashList.SelectedIndices.Count == 0)
            {
                return;
            }

            this.UseHashText.Text = this.HistoryHashList.SelectedItems[0].ToString();
            this.ChangeMode(true);
            this.isAdd = false;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (this.HistoryHashList.SelectedIndices.Count == 0)
            {
                return;
            }

            if (MessageBox.Show(Hoehoe.Properties.Resources.DeleteHashtagsMessage1, "Delete Hashtags", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            for (int i = 0; i < this.HistoryHashList.SelectedIndices.Count; i++)
            {
                if (this.UseHashText.Text == this.HistoryHashList.SelectedItems[0].ToString())
                {
                    this.UseHashText.Text = string.Empty;
                }

                this.HistoryHashList.Items.RemoveAt(this.HistoryHashList.SelectedIndices[0]);
            }

            if (this.HistoryHashList.Items.Count > 0)
            {
                this.HistoryHashList.SelectedIndex = 0;
            }
        }

        private void UnSelectButton_Click(object sender, EventArgs e)
        {
            do
            {
                this.HistoryHashList.SelectedIndices.Clear();
            }
            while (this.HistoryHashList.SelectedIndices.Count > 0);
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
                string src = l as string;
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

        private void HashtagManage_Shown(object sender, System.EventArgs e)
        {
            // オプション
            this.CheckPermanent.Checked = this.isPermanent;
            this.RadioHead.Checked = this.isHead;
            this.RadioLast.Checked = !this.isHead;

            // リスト選択
            if (this.HistoryHashList.Items.Contains(this.useHash))
            {
                this.HistoryHashList.SelectedItem = this.useHash;
            }
            else
            {
                if (this.HistoryHashList.Items.Count > 0)
                {
                    this.HistoryHashList.SelectedIndex = 0;
                }
            }

            this.ChangeMode(false);
        }

        private void UseHashText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '#')
            {
                this.hashSupl.ShowDialog();
                if (!string.IsNullOrEmpty(this.hashSupl.InputText))
                {
                    string front = string.Empty;
                    string last = string.Empty;
                    int selStart = this.UseHashText.SelectionStart;
                    if (selStart > 0)
                    {
                        front = this.UseHashText.Text.Substring(0, selStart);
                    }

                    if (selStart < this.UseHashText.Text.Length)
                    {
                        last = this.UseHashText.Text.Substring(selStart);
                    }

                    this.UseHashText.Text = front + this.hashSupl.InputText + last;
                    this.UseHashText.SelectionStart = selStart + this.hashSupl.InputText.Length;
                }

                e.Handled = true;
            }
        }

        private void HistoryHashList_DoubleClick(object sender, EventArgs e)
        {
            this.OkButton_Click(null, null);
        }

        private void PermOkButton_Click(object sender, EventArgs e)
        {
            // ハッシュタグの整形
            string hashStr = this.UseHashText.Text;
            if (!this.AdjustHashtags(ref hashStr, true))
            {
                return;
            }

            this.UseHashText.Text = hashStr;
            int idx = 0;
            if (!this.isAdd && this.HistoryHashList.SelectedIndices.Count > 0)
            {
                idx = this.HistoryHashList.SelectedIndices[0];
                this.HistoryHashList.Items.RemoveAt(idx);
                do
                {
                    this.HistoryHashList.SelectedIndices.Clear();
                }
                while (this.HistoryHashList.SelectedIndices.Count > 0);
                this.HistoryHashList.Items.Insert(idx, hashStr);
                this.HistoryHashList.SelectedIndex = idx;
            }
            else
            {
                this.AddHashToHistory(hashStr, false);
                do
                {
                    this.HistoryHashList.SelectedIndices.Clear();
                }
                while (this.HistoryHashList.SelectedIndices.Count > 0);
                this.HistoryHashList.SelectedIndex = this.HistoryHashList.Items.IndexOf(hashStr);
            }

            this.ChangeMode(false);
        }

        private void PermCancelButton_Click(object sender, EventArgs e)
        {
            if (this.HistoryHashList.Items.Count > 0 && this.HistoryHashList.SelectedIndices.Count > 0)
            {
                this.UseHashText.Text = this.HistoryHashList.Items[this.HistoryHashList.SelectedIndices[0]].ToString();
            }
            else
            {
                this.UseHashText.Text = string.Empty;
            }

            this.ChangeMode(false);
        }

        private void HistoryHashList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                this.DeleteButton_Click(null, null);
            }
            else if (e.KeyCode == Keys.Insert)
            {
                this.AddButton_Click(null, null);
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
            foreach (string hs in this.HistoryHashList.SelectedItems)
            {
                hash += hs + " ";
            }

            hash = hash.Trim();
            if (!string.IsNullOrEmpty(hash))
            {
                this.AddHashToHistory(hash, true);
                this.isPermanent = this.CheckPermanent.Checked;
            }
            else
            {
                this.isPermanent = false; // 使用ハッシュが未選択ならば、固定オプション外す
            }

            this.isHead = this.RadioHead.Checked;
            this.useHash = hash;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void HashtagManage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (this.GroupDetail.Enabled)
                {
                    this.PermOkButton_Click(null, null);
                }
                else
                {
                    this.OkButton_Click(null, null);
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                if (this.GroupDetail.Enabled)
                {
                    this.PermCancelButton_Click(null, null);
                }
                else
                {
                    this.CancelButton_Click(null, null);
                }
            }
        }

        private void CheckNotAddToAtReply_CheckedChanged(object sender, EventArgs e)
        {
            this.isNotAddToAtReply = this.CheckNotAddToAtReply.Checked;
        }
    }
}