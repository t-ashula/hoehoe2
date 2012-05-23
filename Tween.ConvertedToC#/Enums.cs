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

    public enum IconSizes
    {
        IconNone = 0,
        Icon16 = 1,
        Icon24 = 2,
        Icon48 = 3,
        Icon48_2 = 4
    }

    public enum NameBalloonEnum
    {
        None,
        UserID,
        NickName
    }

    public enum DispTitleEnum
    {
        None,
        Ver,
        Post,
        UnreadRepCount,
        UnreadAllCount,
        UnreadAllRepCount,
        UnreadCountAllCount,
        OwnStatus
    }

    public enum LogUnitEnum
    {
        Minute,
        Hour,
        Day
    }

    public enum UploadFileType
    {
        Invalid,
        Picture,
        MultiMedia
    }

    public enum UrlConverter
    {
        TinyUrl,
        Isgd,
        Twurl,
        Bitly,
        Jmp,
        Uxnu,
        Nicoms,        // 特殊        
        Unu = -1 // 廃止
    }

    public enum OutputzUrlmode
    {
        twittercom,
        twittercomWithUsername
    }

    public enum HITRESULT
    {
        None,
        Copy,
        CopyAndMark,
        Move,
        Exclude
    }

    public enum HttpTimeOut
    {
        MinValue = 10,
        MaxValue = 120,
        DefaultValue = 20
    }

    /// <summary>
    /// Backgroundworkerへ処理種別を通知するための引数用Enum
    /// </summary>
    public enum WorkerType
    {
        /// <summary>タイムライン取得</summary>
        Timeline,

        /// <summary>返信取得</summary>
        Reply,
        
        /// <summary>受信DM取得</summary>
        DirectMessegeRcv,
        
        /// <summary>送信DM取得</summary>
        DirectMessegeSnt,
        
        /// <summary>発言POST</summary>
        PostMessage,
        
        /// <summary>Fav追加</summary>
        FavAdd,
        
        /// <summary>Fav削除</summary>
        FavRemove,
        
        /// <summary>Followerリスト取得</summary>
        Follower,
        
        /// <summary>Uri開く</summary>
        OpenUri,
        
        /// <summary>Fav取得</summary>
        Favorites,
        
        /// <summary>Retweetする</summary>
        Retweet,
        
        /// <summary>公式検索</summary>
        PublicSearch,
        
        /// <summary>Lists</summary>
        List,
        
        /// <summary>関連発言</summary>
        Related,
        
        /// <summary>UserStream</summary>
        UserStream,
        
        /// <summary>UserTimeline</summary>
        UserTimeline,
        
        /// <summary>Blocking/ids</summary>
        BlockIds,
        
        /// <summary>Twitter Configuration読み込み</summary>
        Configuration,
        
        /// <summary>エラー表示のみで後処理終了(認証エラー時など)</summary>
        ErrorState
    }

    public enum AccountState
    {
        Valid,
        Invalid
    }

    public enum ReplyIconState
    {
        None,
        StaticIcon,
        BlinkIcon
    }

    [Flags]
    public enum EventType
    {
        None = 0,
        Favorite = 1,
        Unfavorite = 2,
        Follow = 4,
        ListMemberAdded = 8,
        ListMemberRemoved = 16,
        Block = 32,
        Unblock = 64,
        UserUpdate = 128,
        Deleted = 256,
        ListCreated = 512,
        ListUpdated = 1024,
        All = (None | Favorite | Unfavorite | Follow | ListMemberAdded | ListMemberRemoved | Block | Unblock | UserUpdate | Deleted | ListCreated | ListUpdated)
    }

    [Flags]
    public enum TabUsageType
    {
        Undefined = 0,
        Home = 1,            // Unique
        Mentions = 2,        // Unique
        DirectMessage = 4,   // Unique
        Favorites = 8,       // Unique
        UserDefined = 16,
        LocalQuery = 32,     // Pin(no save/no save query/distribute/no update(normal update))
        Profile = 64,        // Pin(save/no distribute/manual update)
        PublicSearch = 128,  // Pin(save/no distribute/auto update)
        Lists = 256,
        Related = 512,
        UserTimeline = 1024
    }

    public enum ApiAccessLevel
    {
        None,
        Unknown,
        Read,
        ReadWrite,
        ReadWriteAndDirectMessage
    }
}