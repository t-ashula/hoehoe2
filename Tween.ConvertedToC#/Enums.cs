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
        //特殊
        Nicoms,
        //廃止
        Unu = -1
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
        Timeline,           //タイムライン取得
        Reply,              //返信取得
        DirectMessegeRcv,   //受信DM取得
        DirectMessegeSnt,   //送信DM取得
        PostMessage,        //発言POST
        FavAdd,             //Fav追加
        FavRemove,          //Fav削除
        Follower,           //Followerリスト取得
        OpenUri,            //Uri開く
        Favorites,          //Fav取得
        Retweet,            //Retweetする
        PublicSearch,       //公式検索
        List,               //Lists
        Related,            //関連発言
        UserStream,         //UserStream
        UserTimeline,       //UserTimeline
        BlockIds,           //Blocking/ids
        Configuration,      //Twitter Configuration読み込み
        ErrorState          //エラー表示のみで後処理終了(認証エラー時など)
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
}