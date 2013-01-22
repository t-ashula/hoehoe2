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
    using System.Drawing;
    using System.Windows.Forms;

    public partial class TweenMain
    {
        #region enums

        // 検索処理タイプ
        private enum InTabSearchType
        {
            DialogSearch,
            NextSearch,
            PrevSearch
        }

        [Flags]
        private enum ModifierState : int
        {
            None = 0,
            Alt = 1,
            Shift = 2,
            Ctrl = 4,
            NotFlags = 8
        }

        private enum FocusedControl : int
        {
            None,
            ListTab,
            StatusText,
            PostBrowser
        }

        #endregion enums

        #region inner types

        /// <summary>
        /// URL短縮のUndo用
        /// </summary>
        private class UrlUndoInfo
        {
            public string Before { get; set; }

            public string After { get; set; }
        }

        private class ReplyChain
        {
            public ReplyChain(long originalId, long inReplyToId, TabPage originalTab)
            {
                OriginalId = originalId;
                InReplyToId = inReplyToId;
                OriginalTab = originalTab;
            }

            public long OriginalId { get; private set; }

            public long InReplyToId { get; private set; }

            public TabPage OriginalTab { get; private set; }
        }

        /// <summary>
        /// Backgroundworkerの処理結果通知用引数構造体
        /// </summary>
        private class GetWorkerResult
        {
            // 処理結果詳細メッセージ。エラー時に値がセットされる
            public string RetMsg = string.Empty;

            // 取得対象ページ番号
            public int Page;

            // 取得終了ページ番号（継続可能ならインクリメントされて返る。pageと比較して継続判定）
            public int EndPage;

            // 処理種別
            public WorkerType WorkerType;

            // 新規取得したアイコンイメージ
            public Dictionary<string, Image> Imgs;

            // Fav追加・削除時のタブ名
            public string TabName = string.Empty;

            // Fav追加・削除時のID
            public List<long> Ids;

            // Fav追加・削除成功分のID
            public List<long> SIds;

            public bool NewDM;
            public int AddCount;
            public PostingStatus PStatus;
        }

        /// <summary>
        /// Backgroundworkerへ処理内容を通知するための引数用構造体
        /// </summary>
        private class GetWorkerArg
        {
            // 処理対象ページ番号
            public int Page;

            // 処理終了ページ番号（起動時の読み込みページ数。通常時はpageと同じ値をセット）
            public int EndPage;

            // 処理種別
            public WorkerType WorkerType;

            // URLをブラウザで開くときのアドレス
            public string Url = string.Empty;

            // 発言POST時の発言内容
            public PostingStatus PStatus = new PostingStatus();

            // Fav追加・削除時のItemIndex
            public List<long> Ids;

            // Fav追加・削除成功分のItemIndex
            public List<long> SIds;

            // Fav追加・削除時のタブ名
            public string TabName = string.Empty;
        }

        private class PostingStatus
        {
            public string Status;
            public long InReplyToId;
            public string InReplyToName;
            public string ImageService; // 画像投稿サービス名
            public string ImagePath;

            public PostingStatus()
            {
                ImagePath = string.Empty;
                ImageService = string.Empty;
                InReplyToName = string.Empty;
                InReplyToId = 0;
                Status = string.Empty;
            }

            public PostingStatus(string status, long replyToId, string replyToName)
                : this()
            {
                Status = status;
                InReplyToId = replyToId;
                InReplyToName = replyToName;
            }
        }

        private class GetApiInfoArgs
        {
            public Twitter Tw;
            public ApiInfo Info;
        }

        private class FollowRemoveCommandArgs
        {
            public Twitter Tw;
            public string Id;
        }

        private class ShowFriendshipArgs
        {
            public Twitter Tw;
            public List<FriendshipInfo> Ids = new List<FriendshipInfo>();

            public class FriendshipInfo
            {
                public string Id = string.Empty;
                public bool IsFollowing;
                public bool IsFollowed;
                public bool IsError;

                public FriendshipInfo(string id)
                {
                    Id = id;
                }
            }
        }

        private class GetUserInfoArgs
        {
            public Twitter Tw;
            public string Id;
            public DataModels.Twitter.User User;
        }

        #endregion inner types
    }
}