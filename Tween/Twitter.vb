﻿' Tween - Client of Twitter
' Copyright (c) 2007-2010 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
'           (c) 2008-2010 Moz (@syo68k) <http://iddy.jp/profile/moz/>
'           (c) 2008-2010 takeshik (@takeshik) <http://www.takeshik.org/>
' All rights reserved.
' 
' This file is part of Tween.
' 
' This program is free software; you can redistribute it and/or modify it
' under the terms of the GNU General Public License as published by the Free
' Software Foundation; either version 3 of the License, or (at your option)
' any later version.
' 
' This program is distributed in the hope that it will be useful, but
' WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
' or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
' for more details. 
' 
' You should have received a copy of the GNU General Public License along
' with this program. If not, see <http://www.gnu.org/licenses/>, or write to
' the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
' Boston, MA 02110-1301, USA.

Imports System.Web
Imports System.Xml
Imports System.Text
Imports System.Threading
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports System.Diagnostics
Imports System.Net
Imports System.Reflection
Imports System.Reflection.MethodBase
Imports System.Runtime.Serialization.Json
Imports System.Linq
Imports System.Xml.Linq
Imports System.Runtime.Serialization
Imports System.Net.NetworkInformation

Public Class Twitter
    Implements IDisposable

    Delegate Sub GetIconImageDelegate(ByVal post As PostClass)
    Private ReadOnly LockObj As New Object
    Private followerId As New List(Of Long)
    Private _GetFollowerResult As Boolean = False

    Private _followersCount As Integer = 0
    Private _friendsCount As Integer = 0
    Private _statusesCount As Integer = 0
    Private _location As String = ""
    Private _bio As String = ""
    Private _protocol As String = "https://"

    'プロパティからアクセスされる共通情報
    Private _uid As String
    Private _iconSz As Integer
    Private _getIcon As Boolean
    Private _dIcon As IDictionary(Of String, Image)

    Private _tinyUrlResolve As Boolean
    Private _restrictFavCheck As Boolean

    Private _hubServer As String
    'Private _countApi As Integer
    'Private _countApiReply As Integer
    Private _readOwnPost As Boolean
    Private _hashList As New List(Of String)

    '共通で使用する状態
    Private _remainCountApi As Integer = -1

    Private op As New Outputz
    'max_idで古い発言を取得するために保持（lists分は個別タブで管理）
    Private minHomeTimeline As Long = Long.MaxValue
    Private minMentions As Long = Long.MaxValue
    Private minDirectmessage As Long = Long.MaxValue
    Private minDirectmessageSent As Long = Long.MaxValue

    Private twCon As New HttpTwitter

    Private _deletemessages As New List(Of PostClass)

    Public Function Authenticate(ByVal username As String, ByVal password As String) As String

        Dim res As HttpStatusCode
        Dim content As String = ""

        TwitterApiInfo.Initialize()
        Try
            res = twCon.AuthUserAndPass(username, password, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                _uid = username.ToLower
                Me.ReconnectUserStream()
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Check your Username/Password." + Environment.NewLine + content
                Else
                    Return "Auth error:" + errMsg
                End If
            Case HttpStatusCode.Forbidden
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Err:Forbidden"
                Else
                    Return "Err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select

    End Function

    Public Sub ClearAuthInfo()
        Twitter.AccountState = ACCOUNT_STATE.Invalid
        TwitterApiInfo.Initialize()
        twCon.ClearAuthInfo()
        _UserIdNo = ""
    End Sub

    Private Function GetErrorMessageJson(ByVal content As String) As String
        Try
            If Not String.IsNullOrEmpty(content) Then
                Using jsonReader As XmlDictionaryReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(content), XmlDictionaryReaderQuotas.Max)
                    Dim xElm As XElement = XElement.Load(jsonReader)
                    If xElm.Element("error") IsNot Nothing Then
                        Return xElm.Element("error").Value
                    Else
                        Return ""
                    End If
                End Using
            Else
                Return ""
            End If
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Sub Initialize(ByVal token As String, ByVal tokenSecret As String, ByVal username As String)
        'xAuth認証
        If String.IsNullOrEmpty(token) OrElse String.IsNullOrEmpty(tokenSecret) OrElse String.IsNullOrEmpty(username) Then
            Twitter.AccountState = ACCOUNT_STATE.Invalid
        End If
        TwitterApiInfo.Initialize()
        twCon.Initialize(token, tokenSecret, username)
        _uid = username.ToLower
        _UserIdNo = ""
    End Sub

    Public Sub Initialize(ByVal username As String, ByVal password As String)
        'BASIC認証
        If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            Twitter.AccountState = ACCOUNT_STATE.Invalid
        End If
        TwitterApiInfo.Initialize()
        twCon.Initialize(username, password)
        _uid = username.ToLower
        _UserIdNo = ""
    End Sub

    Public Function PreProcessUrl(ByVal orgData As String) As String
        Dim posl1 As Integer
        Dim posl2 As Integer = 0
        'Dim IDNConveter As IdnMapping = New IdnMapping()
        Dim href As String = "<a href="""

        Do While True
            If orgData.IndexOf(href, posl2, StringComparison.Ordinal) > -1 Then
                Dim urlStr As String = ""
                ' IDN展開
                posl1 = orgData.IndexOf(href, posl2, StringComparison.Ordinal)
                posl1 += href.Length
                posl2 = orgData.IndexOf("""", posl1, StringComparison.Ordinal)
                urlStr = orgData.Substring(posl1, posl2 - posl1)

                If Not urlStr.StartsWith("http://") AndAlso Not urlStr.StartsWith("https://") AndAlso Not urlStr.StartsWith("ftp://") Then
                    Continue Do
                End If

                Dim replacedUrl As String = IDNDecode(urlStr)
                If replacedUrl Is Nothing Then Continue Do
                If replacedUrl = urlStr Then Continue Do

                orgData = orgData.Replace("<a href=""" + urlStr, "<a href=""" + replacedUrl)
                posl2 = 0
            Else
                Exit Do
            End If
        Loop
        Return orgData
    End Function

    Private Function GetPlainText(ByVal orgData As String) As String
        Return HttpUtility.HtmlDecode(Regex.Replace(orgData, "(?<tagStart><a [^>]+>)(?<text>[^<]+)(?<tagEnd></a>)", "${text}"))
    End Function

    ' htmlの簡易サニタイズ(詳細表示に不要なタグの除去)

    Private Function SanitizeHtml(ByVal orgdata As String) As String
        Dim retdata As String = orgdata

        retdata = Regex.Replace(retdata, "<(script|object|applet|image|frameset|fieldset|legend|style).*" & _
            "</(script|object|applet|image|frameset|fieldset|legend|style)>", "", RegexOptions.IgnoreCase)

        retdata = Regex.Replace(retdata, "<(frame|link|iframe|img)>", "", RegexOptions.IgnoreCase)

        Return retdata
    End Function

    Private Function AdjustHtml(ByVal orgData As String) As String
        Dim retStr As String = orgData
        Dim m As Match = Regex.Match(retStr, "<a [^>]+>[#|＃](?<1>[a-zA-Z0-9_]+)</a>")
        While m.Success
            SyncLock LockObj
                _hashList.Add("#" + m.Groups(1).Value)
            End SyncLock
            m = m.NextMatch
        End While
        retStr = Regex.Replace(retStr, "<a [^>]*href=""/", "<a href=""" + _protocol + "twitter.com/")
        retStr = retStr.Replace("<a href=", "<a target=""_self"" href=")
        retStr = retStr.Replace(vbLf, "<br>")

        '半角スペースを置換(Thanks @anis774)
        Dim ret As Boolean = False
        Do
            ret = EscapeSpace(retStr)
        Loop While Not ret

        Return SanitizeHtml(retStr)
    End Function

    Private Function EscapeSpace(ByRef html As String) As Boolean
        '半角スペースを置換(Thanks @anis774)
        Dim isTag As Boolean = False
        For i As Integer = 0 To html.Length - 1
            If html(i) = "<"c Then
                isTag = True
            End If
            If html(i) = ">"c Then
                isTag = False
            End If

            If (Not isTag) AndAlso (html(i) = " "c) Then
                html = html.Remove(i, 1)
                html = html.Insert(i, "&nbsp;")
                Return False
            End If
        Next
        Return True
    End Function

    'Private Sub GetIconImage(ByVal post As PostClass)
    '    Dim img As Image

    '    Try
    '        If Not _getIcon Then
    '            post.ImageUrl = Nothing
    '            TabInformations.GetInstance.AddPost(post)
    '            Exit Sub
    '        End If

    '        If _dIcon.ContainsKey(post.ImageUrl) AndAlso _dIcon(post.ImageUrl) IsNot Nothing Then
    '            TabInformations.GetInstance.AddPost(post)
    '            Exit Sub
    '        End If

    '        Dim httpVar As New HttpVarious
    '        img = httpVar.GetImage(post.ImageUrl, 10000)
    '        If img Is Nothing Then
    '            _dIcon.Add(post.ImageUrl, Nothing)
    '            TabInformations.GetInstance.AddPost(post)
    '            Exit Sub
    '        End If

    '        If _endingFlag Then Exit Sub

    '        SyncLock LockObj
    '            If Not _dIcon.ContainsKey(post.ImageUrl) Then
    '                Try
    '                    _dIcon.Add(post.ImageUrl, img)
    '                Catch ex As InvalidOperationException
    '                    'タイミングにより追加できない場合がある？（キー重複ではない）
    '                    post.ImageUrl = Nothing
    '                Catch ex As System.OverflowException
    '                    '不正なアイコン？DrawImageに失敗する場合あり
    '                    post.ImageUrl = Nothing
    '                Catch ex As OutOfMemoryException
    '                    'DrawImageで発生
    '                    post.ImageUrl = Nothing
    '                End Try
    '            End If
    '        End SyncLock
    '        TabInformations.GetInstance.AddPost(post)
    '    Catch ex As ArgumentException
    '        'タイミングによってはキー重複
    '    Finally
    '        img = Nothing
    '        post = Nothing
    '    End Try
    'End Sub

    Private Structure PostInfo
        Public CreatedAt As String
        Public Id As String
        Public Text As String
        Public UserId As String
        Public Sub New(ByVal Created As String, ByVal IdStr As String, ByVal txt As String, ByVal uid As String)
            CreatedAt = Created
            Id = IdStr
            Text = txt
            UserId = uid
        End Sub
        Public Shadows Function Equals(ByVal dst As PostInfo) As Boolean
            If Me.CreatedAt = dst.CreatedAt AndAlso Me.Id = dst.Id AndAlso Me.Text = dst.Text AndAlso Me.UserId = dst.UserId Then
                Return True
            Else
                Return False
            End If
        End Function
    End Structure

    Private Function IsPostRestricted(ByVal status As TwitterDataModel.Status) As Boolean
        Static _prev As New PostInfo("", "", "", "")
        Dim _current As New PostInfo("", "", "", "")

        _current.CreatedAt = status.CreatedAt
        _current.Id = status.IdStr
        If status.Text Is Nothing Then
            _current.Text = ""
        Else
            _current.Text = status.Text
        End If
        _current.UserId = status.User.IdStr

        If _current.Equals(_prev) Then
            Return True
        End If
        _prev.CreatedAt = _current.CreatedAt
        _prev.Id = _current.Id
        _prev.Text = _current.Text
        _prev.UserId = _current.UserId

        Return False
    End Function

    Public Function PostStatus(ByVal postStr As String, ByVal reply_to As Long) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        postStr = postStr.Trim()

        If Regex.Match(postStr, "^DM? +(?<id>[a-zA-Z0-9_]+) +(?<body>.+)", RegexOptions.IgnoreCase Or RegexOptions.Singleline).Success Then
            Return SendDirectMessage(postStr)
        End If

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.UpdateStatus(postStr, reply_to, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Dim status As TwitterDataModel.Status
                Try
                    status = CreateDataFromJson(Of TwitterDataModel.Status)(content)
                Catch ex As SerializationException
                    TraceOut(ex.Message + Environment.NewLine + content)
                    Return "Err:Json Parse Error(DataContractJsonSerializer)"
                Catch ex As Exception
                    TraceOut(content)
                    Return "Err:Invalid Json!"
                End Try
                _followersCount = status.User.FollowersCount
                _friendsCount = status.User.FriendsCount
                _statusesCount = status.User.StatusesCount
                _location = status.User.Location
                _bio = status.User.Description
                _UserIdNo = status.User.IdStr

                If IsPostRestricted(status) Then
                    Return "OK:Delaying?"
                End If
                If op.Post(postStr.Length) Then
                    Return ""
                Else
                    Return "Outputz:Failed"
                End If
            Case HttpStatusCode.Forbidden, HttpStatusCode.BadRequest
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Warn:" + res.ToString
                Else
                    Return "Warn:" + errMsg
                End If
            Case HttpStatusCode.Conflict, _
                HttpStatusCode.ExpectationFailed, _
                HttpStatusCode.Gone, _
                HttpStatusCode.LengthRequired, _
                HttpStatusCode.MethodNotAllowed, _
                HttpStatusCode.NotAcceptable, _
                HttpStatusCode.NotFound, _
                HttpStatusCode.PaymentRequired, _
                HttpStatusCode.PreconditionFailed, _
                HttpStatusCode.RequestedRangeNotSatisfiable, _
                HttpStatusCode.RequestEntityTooLarge, _
                HttpStatusCode.RequestTimeout, _
                HttpStatusCode.RequestUriTooLong
                '仕様書にない400系エラー。サーバまでは到達しているのでリトライしない
                Return "Warn:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Check your Username/Password."
                Else
                    Return "Auth err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function SendDirectMessage(ByVal postStr As String) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        postStr = postStr.Trim()

        Dim res As HttpStatusCode
        Dim content As String = ""

        Dim mc As Match = Regex.Match(postStr, "^DM? +(?<id>[a-zA-Z0-9_]+) +(?<body>.+)", RegexOptions.IgnoreCase Or RegexOptions.Singleline)

        Try
            res = twCon.SendDirectMessage(mc.Groups("body").Value, mc.Groups("id").Value, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Dim status As TwitterDataModel.Directmessage
                Try
                    status = CreateDataFromJson(Of TwitterDataModel.Directmessage)(content)
                Catch ex As SerializationException
                    TraceOut(ex.Message + Environment.NewLine + content)
                    Return "Err:Json Parse Error(DataContractJsonSerializer)"
                Catch ex As Exception
                    TraceOut(content)
                    Return "Err:Invalid Json!"
                End Try
                _followersCount = status.Sender.FollowersCount
                _friendsCount = status.Sender.FriendsCount
                _statusesCount = status.Sender.StatusesCount
                _location = status.Sender.Location
                _bio = status.Sender.Description
                _UserIdNo = status.Sender.IdStr

                If op.Post(postStr.Length) Then
                    Return ""
                Else
                    Return "Outputz:Failed"
                End If
            Case HttpStatusCode.Forbidden, HttpStatusCode.BadRequest
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Warn:" + res.ToString
                Else
                    Return "Warn:" + errMsg
                End If
            Case HttpStatusCode.Conflict, _
                HttpStatusCode.ExpectationFailed, _
                HttpStatusCode.Gone, _
                HttpStatusCode.LengthRequired, _
                HttpStatusCode.MethodNotAllowed, _
                HttpStatusCode.NotAcceptable, _
                HttpStatusCode.NotFound, _
                HttpStatusCode.PaymentRequired, _
                HttpStatusCode.PreconditionFailed, _
                HttpStatusCode.RequestedRangeNotSatisfiable, _
                HttpStatusCode.RequestEntityTooLarge, _
                HttpStatusCode.RequestTimeout, _
                HttpStatusCode.RequestUriTooLong
                '仕様書にない400系エラー。サーバまでは到達しているのでリトライしない
                Return "Warn:" + res.ToString
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Check your Username/Password."
                Else
                    Return "Auth err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function RemoveStatus(ByVal id As Long) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode

        Try
            res = twCon.DestroyStatus(id)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.NotFound
                Return ""
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select

    End Function

    Public Function PostRetweet(ByVal id As Long, ByVal read As Boolean) As String
        If _endingFlag Then Return ""
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        'データ部分の生成
        Dim target As Long = id
        If TabInformations.GetInstance.Item(id).RetweetedId > 0 Then
            target = TabInformations.GetInstance.Item(id).RetweetedId '再RTの場合は元発言をRT
        End If

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.RetweetStatus(target, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case Is <> HttpStatusCode.OK
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Twitter.AccountState = ACCOUNT_STATE.Valid

        'Dim dlgt As GetIconImageDelegate    'countQueryに合わせる
        'Dim ar As IAsyncResult              'countQueryに合わせる
        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
        Catch ex As Exception
            TraceOut(content)
            'MessageBox.Show("不正なXMLです。(TL-LoadXml)")
            Return "Invalid XML!"
        End Try

        'ReTweetしたものをTLに追加
        Dim xentryNode As XmlNode = xdoc.DocumentElement.SelectSingleNode("/status")
        If xentryNode Is Nothing Then Return "Invalid XML!"
        Dim xentry As XmlElement = CType(xentryNode, XmlElement)
        Dim post As New PostClass
        Try
            post.Id = Long.Parse(xentry.Item("id").InnerText)
            '二重取得回避
            SyncLock LockObj
                If TabInformations.GetInstance.ContainsKey(post.Id) Then Return ""
            End SyncLock
            'Retweet判定
            Dim xRnode As XmlNode = xentry.SelectSingleNode("./retweeted_status")
            If xRnode Is Nothing Then Return "Invalid XML!"

            Dim xRentry As XmlElement = CType(xRnode, XmlElement)
            post.PDate = DateTime.ParseExact(xRentry.Item("created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
            'Id
            post.RetweetedId = Long.Parse(xRentry.Item("id").InnerText)
            '本文
            post.Data = xRentry.Item("text").InnerText
            'Source取得（htmlの場合は、中身を取り出し）
            post.Source = xRentry.Item("source").InnerText
            'Reply先
            Long.TryParse(xRentry.Item("in_reply_to_status_id").InnerText, post.InReplyToId)
            post.InReplyToUser = xRentry.Item("in_reply_to_screen_name").InnerText
            post.IsFav = TabInformations.GetInstance.GetTabByType(TabUsageType.Favorites).Contains(post.RetweetedId)

            '以下、ユーザー情報
            Dim xRUentry As XmlElement = CType(xRentry.SelectSingleNode("./user"), XmlElement)
            post.Uid = Long.Parse(xRUentry.Item("id").InnerText)
            post.Name = xRUentry.Item("screen_name").InnerText
            post.Nickname = xRUentry.Item("name").InnerText
            post.ImageUrl = xRUentry.Item("profile_image_url").InnerText
            post.IsProtect = Boolean.Parse(xRUentry.Item("protected").InnerText)
            post.IsMe = True

            'Retweetした人(自分のはず)
            Dim xUentry As XmlElement = CType(xentry.SelectSingleNode("./user"), XmlElement)
            post.RetweetedBy = xUentry.Item("screen_name").InnerText

            'HTMLに整形
            post.OriginalData = CreateHtmlAnchor(post.Data, post.ReplyToList)
            post.Data = HttpUtility.HtmlDecode(post.Data)
            post.Data = post.Data.Replace("<3", "♡")
            'Source整形
            CreateSource(post)

            post.IsRead = read
            post.IsReply = post.ReplyToList.Contains(_uid)
            post.IsExcludeReply = False

            If post.IsMe Then
                post.IsOwl = False
            Else
                If followerId.Count > 0 Then post.IsOwl = Not followerId.Contains(post.Uid)
            End If
            If post.IsMe AndAlso _readOwnPost Then post.IsRead = True

            post.IsDm = False
        Catch ex As Exception
            TraceOut(content)
            'MessageBox.Show("不正なXMLです。(TL-Parse)")
            Return "Invalid XML!"
        End Try

        'Me._dIcon.Add(post.ImageUrl, Nothing)
        TabInformations.GetInstance.AddPost(post)

        ''非同期アイコン取得＆StatusDictionaryに追加
        'dlgt = New GetIconImageDelegate(AddressOf GetIconImage)
        'ar = dlgt.BeginInvoke(post, Nothing, Nothing)

        ''アイコン取得完了待ち
        'Try
        '    dlgt.EndInvoke(ar)
        'Catch ex As Exception
        '    '最後までendinvoke回す（ゾンビ化回避）
        '    ex.Data("IsTerminatePermission") = False
        '    Throw
        'End Try

        Return ""
    End Function

    Public Function RemoveDirectMessage(ByVal id As Long, ByVal post As PostClass) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode

        If post.IsMe Then
            _deletemessages.Add(post)
        End If
        Try
            res = twCon.DestroyDirectMessage(id)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.NotFound
                Return ""
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostFollowCommand(ByVal screenName As String) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.CreateFriendships(screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText + "(" + GetCurrentMethod.Name + ")"
                Catch ex As Exception
                    Return "Err:Forbidden" + "(" + GetCurrentMethod.Name + ")"
                End Try
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostRemoveCommand(ByVal screenName As String) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.DestroyFriendships(screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText + "(" + GetCurrentMethod.Name + ")"
                Catch ex As Exception
                    Return "Err:Forbidden" + "(" + GetCurrentMethod.Name + ")"
                End Try
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostCreateBlock(ByVal screenName As String) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.CreateBlock(screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText + "(" + GetCurrentMethod.Name + ")"
                Catch ex As Exception
                    Return "Err:Forbidden" + "(" + GetCurrentMethod.Name + ")"
                End Try
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostDestroyBlock(ByVal screenName As String) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.DestroyBlock(screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText + "(" + GetCurrentMethod.Name + ")"
                Catch ex As Exception
                    Return "Err:Forbidden" + "(" + GetCurrentMethod.Name + ")"
                End Try
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostReportSpam(ByVal screenName As String) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.ReportSpam(screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText + "(" + GetCurrentMethod.Name + ")"
                Catch ex As Exception
                    Return "Err:Forbidden" + "(" + GetCurrentMethod.Name + ")"
                End Try
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function GetFriendshipInfo(ByVal screenName As String, ByRef isFollowing As Boolean, ByRef isFollowed As Boolean) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.ShowFriendships(_uid, screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Dim xdoc As New XmlDocument
                Dim result As String = ""
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Try
                    xdoc.LoadXml(content)
                    isFollowing = Boolean.Parse(xdoc.SelectSingleNode("/relationship/source/following").InnerText)
                    isFollowed = Boolean.Parse(xdoc.SelectSingleNode("/relationship/source/followed_by").InnerText)
                Catch ex As Exception
                    result = "Err:Invalid XML."
                End Try
                Return result
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function GetUserInfo(ByVal screenName As String, ByRef user As TwitterDataModel.User) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        user = Nothing
        Try
            res = twCon.ShowUserInfo(screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Try
                    user = CreateDataFromJson(Of TwitterDataModel.User)(content)
                Catch ex As SerializationException
                    TraceOut(ex.Message + Environment.NewLine + content)
                    Return "Err:Json Parse Error(DataContractJsonSerializer)"
                Catch ex As Exception
                    TraceOut(content)
                    Return "Err:Invalid Json!"
                End Try
                Return ""
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Check your Username/Password."
                Else
                    Return "Auth err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function GetStatus_Retweeted_Count(ByVal StatusId As Long, ByRef retweeted_count As Integer) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim xmlBuf As String = ""

        retweeted_count = 0

        ' 注：dev.twitter.comに記述されているcountパラメータは間違い。100が正しい
        For i As Integer = 1 To 100

            Try
                res = twCon.Statusid_retweeted_by_ids(StatusId, 100, i, content)
            Catch ex As Exception
                Return "Err:" + ex.Message
            End Try

            Select Case res
                Case HttpStatusCode.OK
                    Dim xdoc As New XmlDocument
                    Dim xnode As XmlNodeList
                    Dim result As String = ""
                    Twitter.AccountState = ACCOUNT_STATE.Valid
                    Try
                        xdoc.LoadXml(content)
                        xnode = xdoc.GetElementsByTagName("ids")
                        retweeted_count += xnode.ItemOf(0).ChildNodes.Count
                        If xnode.ItemOf(0).ChildNodes.Count < 100 Then Exit For
                    Catch ex As Exception
                        retweeted_count = -1
                        result = "Err:Invalid XML."
                        xmlBuf = Nothing
                    End Try
                Case HttpStatusCode.BadRequest
                    retweeted_count = -1
                    Return "Err:API Limits?"
                Case HttpStatusCode.Unauthorized
                    retweeted_count = -1
                    Twitter.AccountState = ACCOUNT_STATE.Invalid
                    Return "Check your Username/Password."
                Case Else
                    retweeted_count = -1
                    Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
            End Select
        Next
        Return ""
    End Function

    Public Function PostFavAdd(ByVal id As Long) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.CreateFavorites(id, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                If Not _restrictFavCheck Then Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText + "(" + GetCurrentMethod.Name + ")"
                Catch ex As Exception
                    Return "Err:Forbidden" + "(" + GetCurrentMethod.Name + ")"
                End Try
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select

        'http://twitter.com/statuses/show/id.xml APIを発行して本文を取得

        'Dim content As String = ""
        content = ""
        Try
            res = twCon.ShowStatuses(id, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Try
                    Using rd As Xml.XmlTextReader = New Xml.XmlTextReader(New System.IO.StringReader(content))
                        rd.Read()
                        While rd.EOF = False
                            If rd.IsStartElement("favorited") Then
                                If rd.ReadElementContentAsBoolean() = True Then
                                    Return ""  '正常にふぁぼれている
                                Else
                                    Return "NG(Restricted?)"  '正常応答なのにふぁぼれてないので制限っぽい
                                End If
                            Else
                                rd.Read()
                            End If
                        End While
                        rd.Close()
                        Return "Err:Invalid XML!"
                    End Using
                Catch ex As XmlException
                    Return ""
                End Try
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select

    End Function

    Public Function PostFavRemove(ByVal id As Long) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.DestroyFavorites(id, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText
                Catch ex As Exception
                    Return "Err:Forbidden"
                End Try
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostUpdateProfile(ByVal name As String, ByVal url As String, ByVal location As String, ByVal description As String) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.UpdateProfile(name, url, location, description, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText + "(" + GetCurrentMethod.Name + ")"
                Catch ex As Exception
                    Return "Err:Forbidden" + "(" + GetCurrentMethod.Name + ")"
                End Try
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostUpdateProfileImage(ByVal filename As String) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.UpdateProfileImage(New FileInfo(filename), content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText + "(" + GetCurrentMethod.Name + ")"
                Catch ex As Exception
                    Return "Err:Forbidden" + "(" + GetCurrentMethod.Name + ")"
                End Try
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public ReadOnly Property Username() As String
        Get
            Return twCon.AuthenticatedUsername
        End Get
    End Property

    Public ReadOnly Property Password() As String
        Get
            Return twCon.Password
        End Get
    End Property

    Private Shared _accountState As ACCOUNT_STATE = ACCOUNT_STATE.Valid
    Public Shared Property AccountState() As ACCOUNT_STATE
        Get
            Return _accountState
        End Get
        Set(ByVal value As ACCOUNT_STATE)
            _accountState = value
        End Set
    End Property

    Public WriteOnly Property GetIcon() As Boolean
        Set(ByVal value As Boolean)
            _getIcon = value
        End Set
    End Property

    Public WriteOnly Property TinyUrlResolve() As Boolean
        Set(ByVal value As Boolean)
            _tinyUrlResolve = value
        End Set
    End Property

    Public WriteOnly Property RestrictFavCheck() As Boolean
        Set(ByVal value As Boolean)
            _restrictFavCheck = value
        End Set
    End Property

    Public WriteOnly Property IconSize() As Integer
        Set(ByVal value As Integer)
            _iconSz = value
        End Set
    End Property

#Region "バージョンアップ"
    Public Function GetVersionInfo() As String
        Dim content As String = ""
        If Not (New HttpVarious).GetData("http://tween.sourceforge.jp/version2.txt?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Nothing, content) Then
            Throw New Exception("GetVersionInfo Failed")
        End If
        Return content
    End Function

    Public Function GetTweenBinary(ByVal strVer As String) As String
        Try
            If Not (New HttpVarious).GetDataToFile("http://tween.sourceforge.jp/Tween" + strVer + ".gz?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), _
                                                Path.Combine(Application.StartupPath(), "TweenNew.exe")) Then
                Return "Err:Download failed"
            End If
            If Directory.Exists(Path.Combine(Application.StartupPath(), "en")) = False Then
                Directory.CreateDirectory(Path.Combine(Application.StartupPath(), "en"))
            End If
            If Not (New HttpVarious).GetDataToFile("http://tween.sourceforge.jp/TweenRes" + strVer + ".gz?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), _
                                                Path.Combine(Application.StartupPath(), "en\Tween.resourcesNew.dll")) Then
                Return "Err:Download failed"
            End If
            If Not (New HttpVarious).GetDataToFile("http://tween.sourceforge.jp/TweenUp.gz?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), _
                                                Path.Combine(Application.StartupPath(), "TweenUp.exe")) Then
                Return "Err:Download failed"
            End If
            If Not (New HttpVarious).GetDataToFile("http://tween.sourceforge.jp/TweenDll" + strVer + ".gz?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), _
                                                Path.Combine(Application.StartupPath(), "TweenNew.XmlSerializers.dll")) Then
                Return "Err:Download failed"
            End If
            Return ""
        Catch ex As Exception
            Return "Err:Download failed"
        End Try
    End Function
#End Region

    Public Property DetailIcon() As IDictionary(Of String, Image)
        Get
            Return _dIcon
        End Get
        Set(ByVal value As IDictionary(Of String, Image))
            _dIcon = value
        End Set
    End Property

    'Public WriteOnly Property CountApi() As Integer
    '    'API時の取得件数
    '    Set(ByVal value As Integer)
    '        _countApi = value
    '    End Set
    'End Property

    'Public WriteOnly Property CountApiReply() As Integer
    '    'API時のMentions取得件数
    '    Set(ByVal value As Integer)
    '        _countApiReply = value
    '    End Set
    'End Property

    Public Property ReadOwnPost() As Boolean
        Get
            Return _readOwnPost
        End Get
        Set(ByVal value As Boolean)
            _readOwnPost = value
        End Set
    End Property

    Public ReadOnly Property FollowersCount() As Integer
        Get
            Return _followersCount
        End Get
    End Property

    Public ReadOnly Property FriendsCount() As Integer
        Get
            Return _friendsCount
        End Get
    End Property

    Public ReadOnly Property StatusesCount() As Integer
        Get
            Return _statusesCount
        End Get
    End Property

    Public ReadOnly Property Location() As String
        Get
            Return _location
        End Get
    End Property

    Public ReadOnly Property Bio() As String
        Get
            Return _bio
        End Get
    End Property

    Public WriteOnly Property UseSsl() As Boolean
        Set(ByVal value As Boolean)
            HttpTwitter.UseSsl = value
            If value Then
                _protocol = "https://"
            Else
                _protocol = "http://"
            End If
        End Set
    End Property

    Public Function GetTimelineApi(ByVal read As Boolean, _
                            ByVal gType As WORKERTYPE, _
                            ByVal more As Boolean, _
                            ByVal startup As Boolean) As String

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim count As Integer = Setting.Instance.CountApi
        If gType = WORKERTYPE.Reply Then count = Setting.Instance.CountApiReply
        If Setting.Instance.UseAdditionalCount Then
            If more AndAlso Setting.Instance.MoreCountApi <> 0 Then
                count = Setting.Instance.MoreCountApi
            ElseIf startup AndAlso Setting.Instance.FirstCountApi <> 0 AndAlso gType = WORKERTYPE.Timeline Then
                count = Setting.Instance.FirstCountApi
            End If
        End If
        Try
            If gType = WORKERTYPE.Timeline Then
                If more Then
                    res = twCon.HomeTimeline(count, Me.minHomeTimeline, 0, content)
                Else
                    res = twCon.HomeTimeline(count, 0, 0, content)
                End If
            Else
                If more Then
                    res = twCon.Mentions(count, Me.minMentions, 0, content)
                Else
                    res = twCon.Mentions(count, 0, 0, content)
                End If
            End If
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        If gType = WORKERTYPE.Timeline Then
            Return CreatePostsFromJson(content, gType, Nothing, read, count, Me.minHomeTimeline)
        Else
            Return CreatePostsFromJson(content, gType, Nothing, read, count, Me.minMentions)
        End If
    End Function

    Public Function GetUserTimelineApi(ByVal read As Boolean,
                                       ByVal count As Integer,
                                       ByVal userName As String,
                                       ByVal tab As TabClass) As String

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        If count = 0 Then count = 20
        Try
            If String.IsNullOrEmpty(userName) Then
                Dim target As PostClass = tab.RelationTargetPost
                If target Is Nothing Then Return ""
                res = twCon.UserTimeline(target.Uid, "", count, 0, 0, content)
            Else
                res = twCon.UserTimeline(0, userName, count, 0, 0, content)
            End If
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Dim items As List(Of TwitterDataModel.Status)
        Try
            items = CreateDataFromJson(Of List(Of TwitterDataModel.Status))(content)
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid Json!"
        End Try

        For Each status As TwitterDataModel.Status In items
            Dim item As PostClass = CreatePostsFromStatusData(status)
            If item Is Nothing Then Continue For
            item.IsRead = read
            If item.IsMe AndAlso Not read AndAlso _readOwnPost Then item.IsRead = True
            If tab IsNot Nothing Then item.RelTabName = tab.TabName
            '非同期アイコン取得＆StatusDictionaryに追加
            TabInformations.GetInstance.AddPost(item)
        Next

        Return ""
    End Function

    Private Function CreatePostsFromStatusData(ByVal status As TwitterDataModel.Status) As PostClass
        Dim post As New PostClass

        post.Id = status.Id
        If status.RetweetedStatus IsNot Nothing Then
            Dim retweeted As TwitterDataModel.RetweetedStatus = status.RetweetedStatus

            post.PDate = DateTimeParse(retweeted.CreatedAt)

            'Id
            post.RetweetedId = retweeted.Id
            '本文
            post.Data = retweeted.Text
            'Source取得（htmlの場合は、中身を取り出し）
            post.Source = retweeted.Source
            'Reply先
            Long.TryParse(retweeted.InReplyToStatusId, post.InReplyToId)
            post.InReplyToUser = retweeted.InReplyToScreenName
            post.IsFav = TabInformations.GetInstance.GetTabByType(TabUsageType.Favorites).Contains(post.RetweetedId)

            '以下、ユーザー情報
            Dim user As TwitterDataModel.User = retweeted.User

            post.Uid = user.Id
            post.Name = user.ScreenName
            post.Nickname = user.Name
            post.ImageUrl = user.ProfileImageUrl
            post.IsProtect = user.Protected
            If post.IsMe Then _UserIdNo = post.Uid.ToString()

            'Retweetした人
            post.RetweetedBy = status.User.ScreenName
        Else
            post.PDate = DateTimeParse(status.CreatedAt)
            '本文
            post.Data = status.Text
            'Source取得（htmlの場合は、中身を取り出し）
            post.Source = status.Source
            Long.TryParse(status.InReplyToStatusId, post.InReplyToId)
            post.InReplyToUser = status.InReplyToScreenName

            post.IsFav = status.Favorited

            '以下、ユーザー情報
            Dim user As TwitterDataModel.User = status.User

            post.Uid = user.Id
            post.Name = user.ScreenName
            post.Nickname = user.Name
            post.ImageUrl = user.ProfileImageUrl
            post.IsProtect = user.Protected
            post.IsMe = post.Name.ToLower.Equals(_uid)
            If post.IsMe Then _UserIdNo = post.Uid.ToString
        End If
        'HTMLに整形
        post.OriginalData = CreateHtmlAnchor(post.Data, post.ReplyToList)
        post.Data = HttpUtility.HtmlDecode(post.Data)
        post.Data = post.Data.Replace("<3", "♡")
        'Source整形
        CreateSource(post)

        post.IsReply = post.ReplyToList.Contains(_uid)
        post.IsExcludeReply = False

        If post.IsMe Then
            post.IsOwl = False
        Else
            If followerId.Count > 0 Then post.IsOwl = Not followerId.Contains(post.Uid)
        End If

        post.IsDm = False
        Return post
    End Function

    Private Function CreatePostsFromJson(ByVal content As String, ByVal gType As WORKERTYPE, ByVal tab As TabClass, ByVal read As Boolean, ByVal count As Integer, ByRef minimumId As Long) As String
        Dim items As List(Of TwitterDataModel.Status)
        Try
            items = CreateDataFromJson(Of List(Of TwitterDataModel.Status))(content)
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid Json!"
        End Try

        For Each status As TwitterDataModel.Status In items
            Dim post As PostClass = Nothing

            post = CreatePostsFromStatusData(status)

            If minimumId > post.Id Then minimumId = post.Id
            '二重取得回避
            SyncLock LockObj
                If tab Is Nothing Then
                    If TabInformations.GetInstance.ContainsKey(post.Id) Then Continue For
                Else
                    If TabInformations.GetInstance.ContainsKey(post.Id, tab.TabName) Then Continue For
                End If
            End SyncLock

            post.IsRead = read
            If post.IsMe AndAlso Not read AndAlso _readOwnPost Then post.IsRead = True

            If tab IsNot Nothing Then post.RelTabName = tab.TabName
            '非同期アイコン取得＆StatusDictionaryに追加
            TabInformations.GetInstance.AddPost(post)
        Next

        Return ""
    End Function

    Public Overloads Function GetListStatus(ByVal read As Boolean, _
                            ByVal tab As TabClass, _
                            ByVal more As Boolean, _
                            ByVal startup As Boolean) As String

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim page As Integer = 0
        Dim count As Integer = Setting.Instance.CountApi
        If Setting.Instance.UseAdditionalCount Then
            If more AndAlso Setting.Instance.MoreCountApi <> 0 Then
                count = Setting.Instance.MoreCountApi
            ElseIf startup AndAlso Setting.Instance.FirstCountApi <> 0 Then
                count = Setting.Instance.FirstCountApi
            End If
        End If
        Try
            If more Then
                res = twCon.GetListsStatuses(tab.ListInfo.UserId.ToString, tab.ListInfo.Id.ToString, count, tab.OldestId, 0, content)
            Else
                res = twCon.GetListsStatuses(tab.ListInfo.UserId.ToString, tab.ListInfo.Id.ToString, count, 0, 0, content)
            End If
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Return CreatePostsFromXml(content, WORKERTYPE.List, tab, read, count, tab.OldestId)
    End Function

    Public Function GetRelatedResultsApi(ByVal read As Boolean, _
                            ByVal tab As TabClass) As String

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.GetRelatedResults(tab.RelationTargetPost.Id, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Dim targetItem As PostClass = tab.RelationTargetPost
        If targetItem Is Nothing Then
            Return ""
        Else
            targetItem = targetItem.Copy()
        End If
        targetItem.RelTabName = tab.TabName
        TabInformations.GetInstance.AddPost(targetItem)

        Dim replyToItem As PostClass = Nothing
        Dim replyToUserName As String = targetItem.InReplyToUser
        If targetItem.InReplyToId > 0 AndAlso TabInformations.GetInstance.Item(targetItem.InReplyToId) IsNot Nothing Then
            replyToItem = TabInformations.GetInstance.Item(targetItem.InReplyToId).Copy
            replyToItem.RelTabName = tab.TabName
        End If

        Dim items As List(Of TwitterDataModel.RelatedResult)
        Try
            items = CreateDataFromJson(Of List(Of TwitterDataModel.RelatedResult))(content)
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid Json!"
        End Try

        For Each relatedData As TwitterDataModel.RelatedResult In items
            For Each result As TwitterDataModel.RelatedTweet In relatedData.Results
                Dim item As PostClass = CreatePostsFromStatusData(result.Status)
                If item Is Nothing Then Continue For
                If targetItem.InReplyToId = item.Id Then replyToItem = Nothing
                item.IsRead = read
                If item.IsMe AndAlso Not read AndAlso _readOwnPost Then item.IsRead = True
                If tab IsNot Nothing Then item.RelTabName = tab.TabName
                '非同期アイコン取得＆StatusDictionaryに追加
                TabInformations.GetInstance.AddPost(item)
            Next
        Next

        '発言者・返信先ユーザーの直近10発言取得
        'Dim rslt As String = Me.GetUserTimelineApi(read, 10, "", tab)
        'If Not String.IsNullOrEmpty(rslt) Then Return rslt
        'If Not String.IsNullOrEmpty(replyToUserName) Then
        '    rslt = Me.GetUserTimelineApi(read, 10, replyToUserName, tab)
        'End If
        'Return rslt
        Return ""
    End Function

    Private Function CreatePostsFromXml(ByVal content As String, ByVal gType As WORKERTYPE, ByVal tab As TabClass, ByVal read As Boolean, ByVal count As Integer, ByRef minimumId As Long) As String
        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
        Catch ex As Exception
            TraceOut(content)
            'MessageBox.Show("不正なXMLです。(TL-LoadXml)")
            Return "Invalid XML!"
        End Try

        For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("./status")
            Dim xentry As XmlElement = CType(xentryNode, XmlElement)
            Dim post As New PostClass
            Try
                post.Id = Long.Parse(xentry.Item("id").InnerText)
                If minimumId > post.Id Then minimumId = post.Id
                '二重取得回避
                SyncLock LockObj
                    If tab Is Nothing Then
                        If TabInformations.GetInstance.ContainsKey(post.Id) Then Continue For
                    Else
                        If TabInformations.GetInstance.ContainsKey(post.Id, tab.TabName) Then Continue For
                    End If
                End SyncLock
                'Retweet判定
                Dim xRnode As XmlNode = xentry.SelectSingleNode("./retweeted_status")
                If xRnode IsNot Nothing Then
                    Dim xRentry As XmlElement = CType(xRnode, XmlElement)
                    post.PDate = DateTime.ParseExact(xRentry.Item("created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
                    'Id
                    post.RetweetedId = Long.Parse(xRentry.Item("id").InnerText)
                    '本文
                    post.Data = xRentry.Item("text").InnerText
                    'Source取得（htmlの場合は、中身を取り出し）
                    post.Source = xRentry.Item("source").InnerText
                    'Reply先
                    Long.TryParse(xRentry.Item("in_reply_to_status_id").InnerText, post.InReplyToId)
                    post.InReplyToUser = xRentry.Item("in_reply_to_screen_name").InnerText
                    post.IsFav = TabInformations.GetInstance.GetTabByType(TabUsageType.Favorites).Contains(post.RetweetedId)
                    'post.IsFav = Boolean.Parse(xentry.Item("favorited").InnerText)

                    '以下、ユーザー情報
                    Dim xRUentry As XmlElement = CType(xRentry.SelectSingleNode("./user"), XmlElement)
                    post.Uid = Long.Parse(xRUentry.Item("id").InnerText)
                    post.Name = xRUentry.Item("screen_name").InnerText
                    post.Nickname = xRUentry.Item("name").InnerText
                    post.ImageUrl = xRUentry.Item("profile_image_url").InnerText
                    post.IsProtect = Boolean.Parse(xRUentry.Item("protected").InnerText)
                    post.IsMe = post.Name.ToLower.Equals(_uid)
                    If post.IsMe Then _UserIdNo = post.Uid.ToString()

                    'Retweetした人
                    Dim xUentry As XmlElement = CType(xentry.SelectSingleNode("./user"), XmlElement)
                    post.RetweetedBy = xUentry.Item("screen_name").InnerText
                Else
                    post.PDate = DateTime.ParseExact(xentry.Item("created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
                    '本文
                    post.Data = xentry.Item("text").InnerText
                    'Source取得（htmlの場合は、中身を取り出し）
                    post.Source = xentry.Item("source").InnerText
                    Long.TryParse(xentry.Item("in_reply_to_status_id").InnerText, post.InReplyToId)
                    post.InReplyToUser = xentry.Item("in_reply_to_screen_name").InnerText
                    'in_reply_to_user_idを使うか？
                    post.IsFav = Boolean.Parse(xentry.Item("favorited").InnerText)

                    '以下、ユーザー情報
                    Dim xUentry As XmlElement = CType(xentry.SelectSingleNode("./user"), XmlElement)
                    post.Uid = Long.Parse(xUentry.Item("id").InnerText)
                    post.Name = xUentry.Item("screen_name").InnerText
                    post.Nickname = xUentry.Item("name").InnerText
                    post.ImageUrl = xUentry.Item("profile_image_url").InnerText
                    post.IsProtect = Boolean.Parse(xUentry.Item("protected").InnerText)
                    post.IsMe = post.Name.ToLower.Equals(_uid)
                    If post.IsMe Then _UserIdNo = post.Uid.ToString()
                End If
                'HTMLに整形
                post.OriginalData = CreateHtmlAnchor(post.Data, post.ReplyToList)
                post.Data = HttpUtility.HtmlDecode(post.Data)
                post.Data = post.Data.Replace("<3", "♡")
                'Source整形
                CreateSource(post)

                post.IsRead = read
                If gType = WORKERTYPE.Timeline OrElse tab IsNot Nothing Then
                    post.IsReply = post.ReplyToList.Contains(_uid)
                Else
                    post.IsReply = True
                End If
                post.IsExcludeReply = False

                If post.IsMe Then
                    post.IsOwl = False
                Else
                    If followerId.Count > 0 Then post.IsOwl = Not followerId.Contains(post.Uid)
                End If
                If post.IsMe AndAlso Not read AndAlso _readOwnPost Then post.IsRead = True

                post.IsDm = False
                If tab IsNot Nothing Then post.RelTabName = tab.TabName
            Catch ex As Exception
                TraceOut(content)
                'MessageBox.Show("不正なXMLです。(TL-Parse)")
                Continue For
            End Try

            TabInformations.GetInstance.AddPost(post)

        Next

        Return ""
    End Function

    Public Function GetSearch(ByVal read As Boolean, _
                            ByVal tab As TabClass, _
                            ByVal more As Boolean) As String

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim page As Integer = 0
        Dim sinceId As Long = 0
        Dim count As Integer = 100
        If Setting.Instance.UseAdditionalCount AndAlso
            Setting.Instance.SearchCountApi <> 0 Then
            count = Setting.Instance.SearchCountApi
        End If
        If more Then
            page = tab.GetSearchPage(count)
        Else
            sinceId = tab.SinceId
        End If

        Try
            ' TODO:一時的に40>100件に 件数変更UI作成の必要あり
            res = twCon.Search(tab.SearchWords, tab.SearchLang, count, page, sinceId, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Select Case res
            Case HttpStatusCode.BadRequest
                Return "Invalid query"
            Case HttpStatusCode.NotFound
                Return "Invalid query"
            Case HttpStatusCode.PaymentRequired 'API Documentには420と書いてあるが、該当コードがないので402にしてある
                Return "Search API Limit?"
            Case HttpStatusCode.OK
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select

        If Not TabInformations.GetInstance.ContainsTab(tab) Then Return ""

        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid ATOM!"
        End Try
        Dim nsmgr As New XmlNamespaceManager(xdoc.NameTable)
        nsmgr.AddNamespace("search", "http://www.w3.org/2005/Atom")
        nsmgr.AddNamespace("twitter", "http://api.twitter.com/")
        For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("/search:feed/search:entry", nsmgr)
            Dim xentry As XmlElement = CType(xentryNode, XmlElement)
            Dim post As New PostClass
            Try
                post.Id = Long.Parse(xentry.Item("id").InnerText.Split(":"c)(2))
                If TabInformations.GetInstance.ContainsKey(post.Id, tab.TabName) Then Continue For
                post.PDate = DateTime.Parse(xentry.Item("published").InnerText)
                '本文
                post.Data = xentry.Item("title").InnerText
                'Source取得（htmlの場合は、中身を取り出し）
                post.Source = xentry.Item("twitter:source").InnerText
                post.InReplyToId = 0
                post.InReplyToUser = ""
                post.IsFav = False

                '以下、ユーザー情報
                Dim xUentry As XmlElement = CType(xentry.SelectSingleNode("./search:author", nsmgr), XmlElement)
                post.Uid = 0
                post.Name = xUentry.Item("name").InnerText.Split(" "c)(0).Trim
                post.Nickname = xUentry.Item("name").InnerText.Substring(post.Name.Length).Trim
                If post.Nickname.Length > 2 Then
                    post.Nickname = post.Nickname.Substring(1, post.Nickname.Length - 2)
                Else
                    post.Nickname = post.Name
                End If
                post.ImageUrl = CType(xentry.SelectSingleNode("./search:link[@type='image/png']", nsmgr), XmlElement).GetAttribute("href")
                post.IsProtect = False
                post.IsMe = post.Name.ToLower.Equals(_uid)

                'HTMLに整形
                post.OriginalData = CreateHtmlAnchor(HttpUtility.HtmlEncode(post.Data), post.ReplyToList)
                post.Data = HttpUtility.HtmlDecode(post.Data)
                'Source整形
                CreateSource(post)

                post.IsRead = read
                post.IsReply = post.ReplyToList.Contains(_uid)
                post.IsExcludeReply = False

                post.IsOwl = False
                If post.IsMe AndAlso Not read AndAlso _readOwnPost Then post.IsRead = True
                post.IsDm = False
                post.RelTabName = tab.TabName
                If Not more AndAlso post.Id > tab.SinceId Then tab.SinceId = post.Id
            Catch ex As Exception
                TraceOut(content)
                Continue For
            End Try

            'Me._dIcon.Add(post.ImageUrl, Nothing)
            TabInformations.GetInstance.AddPost(post)

        Next

        '' TODO
        '' 遡るための情報max_idやnext_pageの情報を保持する

#If 0 Then
        Dim xNode As XmlNode = xdoc.DocumentElement.SelectSingleNode("/search:feed/twitter:warning", nsmgr)

        If xNode IsNot Nothing Then
            Return "Warn:" + xNode.InnerText + "(" + GetCurrentMethod.Name + ")"
        End If
#End If

        Return ""
    End Function

    Private Function CreateDirectMessagesFromJson(ByVal content As String, ByVal gType As WORKERTYPE, ByVal read As Boolean) As String
        Dim item As List(Of TwitterDataModel.Directmessage)
        Try
            If gType = WORKERTYPE.UserStream Then
                Dim itm As List(Of TwitterDataModel.DirectmessageEvent) = CreateDataFromJson(Of List(Of TwitterDataModel.DirectmessageEvent))(content)
                item = New List(Of TwitterDataModel.Directmessage)
                For Each dat As TwitterDataModel.DirectmessageEvent In itm
                    item.Add(dat.Directmessage)
                Next
            Else
                item = CreateDataFromJson(Of List(Of TwitterDataModel.Directmessage))(content)
            End If
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid Json!"
        End Try

        For Each message As TwitterDataModel.Directmessage In item
            Dim post As New PostClass
            Try
                post.Id = message.Id
                If gType <> WORKERTYPE.UserStream Then
                    If gType = WORKERTYPE.DirectMessegeRcv Then
                        If minDirectmessage > post.Id Then minDirectmessage = post.Id
                    Else
                        If minDirectmessageSent > post.Id Then minDirectmessageSent = post.Id
                    End If
                End If

                '二重取得回避
                SyncLock LockObj
                    If TabInformations.GetInstance.GetTabByType(TabUsageType.DirectMessage).Contains(post.Id) Then Continue For
                End SyncLock
                'sender_id
                'recipient_id
                post.PDate = DateTimeParse(message.CreatedAt)
                '本文
                post.Data = message.Text
                'HTMLに整形
                post.OriginalData = CreateHtmlAnchor(post.Data, post.ReplyToList)
                post.Data = HttpUtility.HtmlDecode(post.Data)
                post.Data = post.Data.Replace("<3", "♡")
                post.IsFav = False

                '以下、ユーザー情報
                Dim user As TwitterDataModel.User
                If gType = WORKERTYPE.UserStream Then
                    If twCon.AuthenticatedUsername.Equals(message.Recipient.ScreenName, StringComparison.CurrentCultureIgnoreCase) Then
                        user = message.Sender
                        post.IsMe = False
                        post.IsOwl = True
                    Else
                        user = message.Recipient
                        post.IsMe = True
                        post.IsOwl = False
                    End If
                Else
                    If gType = WORKERTYPE.DirectMessegeRcv Then
                        user = message.Sender
                        post.IsMe = False
                        post.IsOwl = True
                    Else
                        user = message.Recipient
                        post.IsMe = True
                        post.IsOwl = False
                    End If
                End If

                post.Uid = user.id
                post.Name = user.ScreenName
                post.Nickname = user.Name
                post.ImageUrl = user.ProfileImageUrl
                post.IsProtect = user.protected
            Catch ex As Exception
                TraceOut(content)
                MessageBox.Show("Parse Error(CreateDirectMessagesFromJson)")
                Continue For
            End Try

            post.IsRead = read
            If post.IsMe AndAlso Not read AndAlso _readOwnPost Then post.IsRead = True
            post.IsReply = False
            post.IsExcludeReply = False
            post.IsDm = True

            TabInformations.GetInstance.AddPost(post)
        Next

        Return ""

    End Function

    Public Function GetDirectMessageApi(ByVal read As Boolean, _
                            ByVal gType As WORKERTYPE, _
                            ByVal more As Boolean) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            If gType = WORKERTYPE.DirectMessegeRcv Then
                If more Then
                    res = twCon.DirectMessages(20, minDirectmessage, 0, content)
                Else
                    res = twCon.DirectMessages(20, 0, 0, content)
                End If
            Else
                If more Then
                    res = twCon.DirectMessagesSent(20, minDirectmessageSent, 0, content)
                Else
                    res = twCon.DirectMessagesSent(20, 0, 0, content)
                End If
            End If
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Return CreateDirectMessagesFromJson(content, gType, read)
    End Function

    Public Function GetFavoritesApi(ByVal read As Boolean, _
                        ByVal gType As WORKERTYPE) As String

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim count As Integer = Setting.Instance.CountApi
        If Setting.Instance.UseAdditionalCount AndAlso
            Setting.Instance.FavoritesCountApi <> 0 Then
            count = Setting.Instance.FavoritesCountApi
        End If
        Try
            res = twCon.Favorites(count, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Dim serializer As New DataContractJsonSerializer(GetType(List(Of TwitterDataModel.Status)))
        Dim item As List(Of TwitterDataModel.Status)

        Try
            item = CreateDataFromJson(Of List(Of TwitterDataModel.Status))(content)
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid Json!"
        End Try

        For Each status As TwitterDataModel.Status In item
            Dim post As New PostClass
            Try
                post.Id = status.Id
                '二重取得回避
                SyncLock LockObj
                    If TabInformations.GetInstance.GetTabByType(TabUsageType.Favorites).Contains(post.Id) Then Continue For
                End SyncLock
                'Retweet判定
                If status.RetweetedStatus IsNot Nothing Then
                    Dim retweeted As TwitterDataModel.RetweetedStatus = status.RetweetedStatus
                    post.PDate = DateTimeParse(retweeted.CreatedAt)

                    'Id
                    post.RetweetedId = post.Id
                    '本文
                    post.Data = retweeted.text
                    'Source取得（htmlの場合は、中身を取り出し）
                    post.Source = retweeted.source
                    'Reply先
                    Long.TryParse(retweeted.InReplyToStatusId, post.InReplyToId)
                    post.InReplyToUser = retweeted.InReplyToScreenName
                    post.IsFav = retweeted.favorited

                    '以下、ユーザー情報
                    Dim user As TwitterDataModel.User = retweeted.User
                    post.Uid = user.Id
                    post.Name = user.ScreenName
                    post.Nickname = user.Name
                    post.ImageUrl = user.ProfileImageUrl
                    post.IsProtect = user.Protected
                    post.IsMe = post.Name.ToLower.Equals(_uid)
                    If post.IsMe Then _UserIdNo = post.Uid.ToString()

                    'Retweetした人
                    post.RetweetedBy = status.User.ScreenName
                Else
                    post.PDate = DateTimeParse(status.CreatedAt)

                    '本文
                    post.Data = status.Text
                    'Source取得（htmlの場合は、中身を取り出し）
                    post.Source = status.Source
                    Long.TryParse(status.InReplyToStatusId, post.InReplyToId)
                    post.InReplyToUser = status.InReplyToScreenName

                    post.IsFav = status.Favorited

                    '以下、ユーザー情報
                    Dim user As TwitterDataModel.User = status.User
                    post.Uid = user.Id
                    post.Name = user.ScreenName
                    post.Nickname = user.Name
                    post.ImageUrl = user.ProfileImageUrl
                    post.IsProtect = user.Protected
                    post.IsMe = post.Name.ToLower.Equals(_uid)
                    If post.IsMe Then _UserIdNo = post.Uid.ToString
                End If
                'HTMLに整形
                post.OriginalData = CreateHtmlAnchor(post.Data, post.ReplyToList)
                post.Data = HttpUtility.HtmlDecode(post.Data)
                post.Data = post.Data.Replace("<3", "♡")
                'Source整形
                CreateSource(post)

                post.IsRead = read
                post.IsReply = post.ReplyToList.Contains(_uid)
                post.IsExcludeReply = False

                If post.IsMe Then
                    post.IsOwl = False
                Else
                    If followerId.Count > 0 Then post.IsOwl = Not followerId.Contains(post.Uid)
                End If

                post.IsDm = False
            Catch ex As Exception
                TraceOut(content)
                Continue For
            End Try

            TabInformations.GetInstance.AddPost(post)

        Next

        Return ""
    End Function

    Public Function GetFollowersApi() As String
        If _endingFlag Then Return ""
        Dim cursor As Long = -1
        Dim tmpFollower As New List(Of Long)(followerId)

        followerId.Clear()
        Do
            Dim ret As String = FollowerApi(cursor)
            If Not String.IsNullOrEmpty(ret) Then
                followerId.Clear()
                followerId.AddRange(tmpFollower)
                _GetFollowerResult = False
                Return ret
            End If
        Loop While cursor > 0

        TabInformations.GetInstance.RefreshOwl(followerId)

        _GetFollowerResult = True
        Return ""
    End Function

    Public ReadOnly Property GetFollowersSuccess() As Boolean
        Get
            Return _GetFollowerResult
        End Get
    End Property

    Private Function FollowerApi(ByRef cursor As Long) As String
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.FollowerIds(cursor, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid XML!"
        End Try

        Try
            For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("/id_list/ids/id")
                followerId.Add(Long.Parse(xentryNode.InnerText))
            Next
            cursor = Long.Parse(xdoc.DocumentElement.SelectSingleNode("/id_list/next_cursor").InnerText)
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid XML!"
        End Try

        Return ""

    End Function

    Public Function GetListsApi() As String
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim cursor As Long = -1

        Dim lists As New List(Of ListElement)
        Do
            Try
                res = twCon.GetLists(Me.Username, cursor, content)
            Catch ex As Exception
                Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
            End Try

            Select Case res
                Case HttpStatusCode.OK
                    Twitter.AccountState = ACCOUNT_STATE.Valid
                Case HttpStatusCode.Unauthorized
                    Twitter.AccountState = ACCOUNT_STATE.Invalid
                    Return "Check your Username/Password."
                Case HttpStatusCode.BadRequest
                    Return "Err:API Limits?"
                Case Else
                    Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
            End Select

            Dim xdoc As New XmlDocument
            Try
                xdoc.LoadXml(content)
            Catch ex As Exception
                TraceOut(content)
                Return "Invalid XML!"
            End Try

            Try
                For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("/lists_list/lists/list")
                    lists.Add(New ListElement(xentryNode, Me))
                Next
                cursor = Long.Parse(xdoc.DocumentElement.SelectSingleNode("/lists_list/next_cursor").InnerText)
            Catch ex As Exception
                TraceOut(content)
                Return "Invalid XML!"
            End Try
        Loop While cursor <> 0

        cursor = -1
        content = ""
        Do
            Try
                res = twCon.GetListsSubscriptions(Me.Username, cursor, content)
            Catch ex As Exception
                Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
            End Try

            Select Case res
                Case HttpStatusCode.OK
                    Twitter.AccountState = ACCOUNT_STATE.Valid
                Case HttpStatusCode.Unauthorized
                    Twitter.AccountState = ACCOUNT_STATE.Invalid
                    Return "Check your Username/Password."
                Case HttpStatusCode.BadRequest
                    Return "Err:API Limits?"
                Case Else
                    Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
            End Select

            Dim xdoc As New XmlDocument
            Try
                xdoc.LoadXml(content)
            Catch ex As Exception
                TraceOut(content)
                Return "Invalid XML!"
            End Try

            Try
                For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("/lists_list/lists/list")
                    lists.Add(New ListElement(xentryNode, Me))
                Next
                cursor = Long.Parse(xdoc.DocumentElement.SelectSingleNode("/lists_list/next_cursor").InnerText)
            Catch ex As Exception
                TraceOut(content)
                Return "Invalid XML!"
            End Try
        Loop While cursor <> 0

        TabInformations.GetInstance.SubscribableLists = lists
        Return ""
    End Function

    Public Function DeleteList(ByVal list_id As String) As String
        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.DeleteListID(Me.Username, list_id, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Return ""
    End Function

    Public Function EditList(ByVal list_id As String, ByVal new_name As String, ByVal isPrivate As Boolean, ByVal description As String, ByRef list As ListElement) As String
        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim modeString As String = "public"
        If isPrivate Then
            modeString = "private"
        End If

        Try
            res = twCon.PostListID(Me.Username, list_id, new_name, modeString, description, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
            Dim newList As New ListElement(xdoc.DocumentElement, Me)
            list.Description = newList.Description
            list.Id = newList.Id
            list.IsPublic = newList.IsPublic
            list.MemberCount = newList.MemberCount
            list.Name = newList.Name
            list.SubscriberCount = newList.SubscriberCount
            list.Slug = newList.Slug
            list.Nickname = newList.Nickname
            list.Username = newList.Username
            list.UserId = newList.UserId
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid XML!"
        End Try

        Return ""
    End Function

    Public Function GetListMembers(ByVal list_id As String, ByVal lists As List(Of UserInfo), ByRef cursor As Long) As String
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        'Dim cursor As Long = -1

        'Do
        Try
            res = twCon.GetListMembers(Me.Username, list_id, cursor, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid XML!"
        End Try

        Try
            For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("/users_list/users/user")
                lists.Add(New UserInfo(xentryNode))
            Next
            cursor = Long.Parse(xdoc.DocumentElement.SelectSingleNode("/users_list/next_cursor").InnerText)
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid XML!"
        End Try
        'Loop While cursor <> 0

        Return ""
    End Function

    Public Function CreateListApi(ByVal listName As String, ByVal isPrivate As Boolean, ByVal description As String) As String
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.PostLists(Me.Username, listName, isPrivate, description, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)

            TabInformations.GetInstance().SubscribableLists.Add(New ListElement(xdoc.DocumentElement, Me))
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid XML!"
        End Try

        Return ""
    End Function

    Public Function ContainsUserAtList(ByVal list_name As String, ByVal user As String, ByRef value As Boolean) As String
        value = False

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = Me.twCon.GetListMembersID(Me.Username, list_name, user, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case HttpStatusCode.NotFound
                value = False
                Return ""
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
            value = xdoc.DocumentElement.Name = "user"
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid XML!"
        End Try

        Return ""
    End Function

    Public Function AddUserToList(ByVal list_name As String, ByVal user As String) As String
        Dim content As String = ""
        Dim res As HttpStatusCode

        Try
            res = twCon.PostListMembers(Me.Username, list_name, user, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Return ""
    End Function

    Public Function RemoveUserToList(ByVal list_name As String, ByVal user As String) As String
        Dim content As String = ""
        Dim res As HttpStatusCode

        Try
            res = twCon.DeleteListMembers(Me.Username, list_name, user, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Return ""
    End Function

    Private Class range
        Public Property fromIndex As Integer
        Public Property toIndex As Integer
        Public Sub New(ByVal fromIndex As Integer, ByVal toIndex As Integer)
            Me.fromIndex = fromIndex
            Me.toIndex = toIndex
        End Sub
    End Class
    Public Function CreateHtmlAnchor(ByVal Text As String, ByVal AtList As List(Of String)) As String
        Dim retStr As String = Text.Replace("&gt;", "<<<<<tweenだいなり>>>>>").Replace("&lt;", "<<<<<tweenしょうなり>>>>>")
        'uriの正規表現
        'Const rgUrl As String = "(?<before>(?:[^\""':!=]|^|\:))" + _
        '                            "(?<url>(?<protocol>https?://|www\.)" + _
        '                            "(?<domain>(?:[\.-]|[^\p{P}\s])+\.[a-z]{2,}(?::[0-9]+)?)" + _
        '                            "(?<path>/[a-z0-9!*'();:&=+$/%#\[\]\-_.,~@^]*[a-z0-9)=#/]?)?" + _
        '                            "(?<query>\?[a-z0-9!*'();:&=+$/%#\[\]\-_.,~]*[a-z0-9_&=#])?)"
        Const url_valid_general_path_chars As String = "[a-z0-9!*';:=+$/%#\[\]\-_,~]"
        Const url_valid_url_path_ending_chars As String = "[a-z0-9=#/]"
        Const pth As String = "(?<path>/(?:(?:\(" + url_valid_general_path_chars + "+\))" +
            "|@" + url_valid_general_path_chars + "+/" +
            "|[.,]?" + url_valid_general_path_chars +
            ")*" +
            url_valid_url_path_ending_chars + "?)?"
        Const qry As String = "(?<query>\?[a-z0-9!*'();:&=+$/%#\[\]\-_.,~]*[a-z0-9_&=#])?"
        Const rgUrl As String = "(?<before>(?:[^\""':!=]|^|\:))" +
                                    "(?<url>(?<protocol>https?://|www\.)" +
                                    "(?<domain>(?:[\.-]|[^\p{P}\s])+\.[a-z]{2,}(?::[0-9]+)?)" +
                                    pth +
                                    qry +
                                    ")"
        '絶対パス表現のUriをリンクに置換
        retStr = Regex.Replace(retStr,
                               rgUrl,
                               New MatchEvaluator(Function(mu As Match)
                                                      Dim sb As New StringBuilder(mu.Result("${before}<a href="""))
                                                      If mu.Result("${protocol}").StartsWith("w", StringComparison.OrdinalIgnoreCase) Then
                                                          sb.Append("http://")
                                                      End If
                                                      sb.Append(mu.Result("${url}"">")).Append(mu.Result("${url}")).Append("</a>")
                                                      Return sb.ToString
                                                  End Function),
                               RegexOptions.IgnoreCase)

        '@先をリンクに置換（リスト）
        retStr = Regex.Replace(retStr,
                               "(^|[^a-zA-Z0-9_/])([@＠]+)([a-zA-Z0-9_]{1,20}/[a-zA-Z][a-zA-Z0-9\p{IsLatin-1Supplement}\-]{0,79})",
                               "$1$2<a href=""/$3"">$3</a>")

        Dim m As Match = Regex.Match(retStr, "(^|[^a-zA-Z0-9_])[@＠]([a-zA-Z0-9_]{1,20})")
        While m.Success
            If Not AtList.Contains(m.Result("$2").ToLower) Then AtList.Add(m.Result("$2").ToLower)
            m = m.NextMatch
        End While
        '@先をリンクに置換
        retStr = Regex.Replace(retStr,
                               "(^|[^a-zA-Z0-9_/])([@＠])([a-zA-Z0-9_]{1,20})",
                               "$1$2<a href=""/$3"">$3</a>")

        'ハッシュタグを抽出し、リンクに置換
        Dim anchorRange As New List(Of range)
        For i As Integer = 0 To retStr.Length - 1
            Dim index As Integer = retStr.IndexOf("<a ", i)
            If index > -1 AndAlso index < retStr.Length Then
                i = index
                Dim toIndex As Integer = retStr.IndexOf("</a>", index)
                If toIndex > -1 Then
                    anchorRange.Add(New range(index, toIndex + 3))
                    i = toIndex
                End If
            End If
        Next
        retStr = Regex.Replace(retStr,
                               "(^|[^a-zA-Z0-9/&])([#＃])([0-9a-zA-Z_]*[a-zA-Z_]+[a-zA-Z0-9_\xc0-\xd6\xd8-\xf6\xf8-\xff]*)",
                               New MatchEvaluator(Function(mh As Match)
                                                      For Each rng As range In anchorRange
                                                          If mh.Index >= rng.fromIndex AndAlso
                                                           mh.Index <= rng.toIndex Then Return mh.Result("$0")
                                                      Next
                                                      If IsNumeric(mh.Result("$3")) Then Return mh.Result("$0")
                                                      SyncLock LockObj
                                                          _hashList.Add("#" + mh.Result("$3"))
                                                      End SyncLock
                                                      Return mh.Result("$1") + "<a href=""" & _protocol & "twitter.com/search?q=%23" + mh.Result("$3") + """>" + mh.Result("$2$3") + "</a>"
                                                  End Function),
                                              RegexOptions.IgnoreCase)

        'Dim mhs As MatchCollection = Regex.Matches(retStr, "(^|[^a-zA-Z0-9/&])[#＃]([0-9a-zA-Z_]*[a-zA-Z_]+[a-zA-Z_\xc0-\xd6\xd8-\xf6\xf8-\xff]*)")
        'For Each mt As Match In mhs
        '    If Not IsNumeric(mt.Result("$2")) Then
        '        'retStr = retStr.Replace(mt.Result("$1") + mt.Result("$2"), "<a href=""" + _protocol + "twitter.com/search?q=%23" + mt.Result("$2") + """>#" + mt.Result("$2") + "</a>")
        '        SyncLock LockObj
        '            _hashList.Add("#" + mt.Result("$2"))
        '        End SyncLock
        '    End If
        'Next
        'retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9/&])([#＃])([0-9a-zA-Z_]*[a-zA-Z_]+[a-zA-Z0-9_\xc0-\xd6\xd8-\xf6\xf8-\xff]*)", "$1<a href=""" & _protocol & "twitter.com/search?q=%23$3"">$2$3</a>")

        retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9_/&#＃@＠>=.])(sm|nm)([0-9]{1,10})", "$1<a href=""http://www.nicovideo.jp/watch/$2$3"">$2$3</a>")

        retStr = retStr.Replace("<<<<<tweenだいなり>>>>>", "&gt;").Replace("<<<<<tweenしょうなり>>>>>", "&lt;")

        retStr = AdjustHtml(ShortUrl.Resolve(PreProcessUrl(retStr))) 'IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与
        Return retStr
    End Function

    'Source整形
    Private Sub CreateSource(ByRef post As PostClass)
        If post.Source.StartsWith("<") Then
            If Not post.Source.Contains("</a>") Then
                post.Source += "</a>"
            End If
            Dim mS As Match = Regex.Match(post.Source, ">(?<source>.+)<")
            If mS.Success Then
                post.SourceHtml = String.Copy(ShortUrl.Resolve(PreProcessUrl(post.Source)))
                post.Source = HttpUtility.HtmlDecode(mS.Result("${source}"))
            Else
                post.Source = ""
                post.SourceHtml = ""
            End If
        Else
            If post.Source = "web" Then
                post.SourceHtml = My.Resources.WebSourceString
            ElseIf post.Source = "Keitai Mail" Then
                post.SourceHtml = My.Resources.KeitaiMailSourceString
            Else
                post.SourceHtml = String.Copy(post.Source)
            End If
        End If
    End Sub

    Public Function GetInfoApi(ByVal info As ApiInfo) As Boolean
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return True

        If _endingFlag Then Return True

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.RateLimitStatus(content)
        Catch ex As Exception
            TwitterApiInfo.Initialize()
            Return False
        End Try

        If res <> HttpStatusCode.OK Then Return False

        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
            Dim arg As New ApiInformationChangedEventArgs

            arg.ApiInfo.MaxCount = Integer.Parse(xdoc.SelectSingleNode("/hash/hourly-limit").InnerText)
            arg.ApiInfo.RemainCount = Integer.Parse(xdoc.SelectSingleNode("/hash/remaining-hits").InnerText)
            arg.ApiInfo.ResetTime = DateTime.Parse(xdoc.SelectSingleNode("/hash/reset-time").InnerText)
            arg.ApiInfo.ResetTimeInSeconds = Integer.Parse(xdoc.SelectSingleNode("/hash/reset-time-in-seconds").InnerText)
            If info IsNot Nothing Then
                arg.ApiInfo.UsingCount = info.UsingCount

                info.MaxCount = arg.ApiInfo.MaxCount
                info.RemainCount = arg.ApiInfo.RemainCount
                info.ResetTime = arg.ApiInfo.ResetTime
                info.ResetTimeInSeconds = arg.ApiInfo.ResetTimeInSeconds
            End If

            RaiseEvent ApiInformationChanged(Me, arg)
            TwitterApiInfo.WriteBackEventArgs(arg)
            Return True
        Catch ex As Exception
            TwitterApiInfo.Initialize()
            Return False
        End Try
    End Function

    Public Function GetHashList() As String()
        Dim hashArray As String()
        SyncLock LockObj
            hashArray = _hashList.ToArray
            _hashList.Clear()
        End SyncLock
        Return hashArray
    End Function

    Public ReadOnly Property AccessToken() As String
        Get
            Return twCon.AccessToken
        End Get
    End Property

    Public ReadOnly Property AccessTokenSecret() As String
        Get
            Return twCon.AccessTokenSecret
        End Get
    End Property

    Public Property UserIdNo As String

    Public Event ApiInformationChanged(ByVal sender As Object, ByVal e As ApiInformationChangedEventArgs)

    Private Sub Twitter_ApiInformationChanged(ByVal sender As Object, ByVal e As ApiInformationChangedEventArgs) Handles Me.ApiInformationChanged
    End Sub

#Region "UserStream"
    Public Property TrackWord As String = ""
    Public Property AllAtReply As Boolean = False

    Public Event NewPostFromStream()
    Public Event UserStreamStarted()
    Public Event UserStreamStopped()
    Public Event UserStreamGetFriendsList()
    Public Event PostDeleted(ByVal id As Long, ByRef post As PostClass)
    Public Event UserStreamEventReceived(ByVal eventType As FormattedEvent)
    Private WithEvents userStream As TwitterUserstream

    Public Class FormattedEvent
        Public Property CreatedAt As DateTime
        Public Property [Event] As String
        Public Property Username As String
        Public Property Target As String

    End Class

    Public Property StoredEvent As New List(Of FormattedEvent)

    Private EventNameTable() As String = {
        "favorite",
        "unfavorite",
        "follow",
        "list_member_added",
        "list_member_removed",
        "block"
    }

    Private Sub userStream_StatusArrived(ByVal line As String) Handles userStream.StatusArrived
        If String.IsNullOrEmpty(line) Then Exit Sub

        Dim isDm As Boolean = False

        Using jsonReader As XmlDictionaryReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(line), XmlDictionaryReaderQuotas.Max)
            Dim xElm As XElement = XElement.Load(jsonReader)
            If xElm.Element("friends") IsNot Nothing Then
                Debug.Print("friends")
                Exit Sub
            ElseIf xElm.Element("delete") IsNot Nothing Then
                Debug.Print("delete")
                Dim post As PostClass = Nothing
                Dim id As Int64
                If xElm.Element("delete").Element("direct_message") IsNot Nothing Then
                    id = CLng(xElm.Element("delete").Element("direct_message").Element("id").Value)
                    RaiseEvent PostDeleted(id, post)
                Else
                    id = CLng(xElm.Element("delete").Element("status").Element("id").Value)
                    RaiseEvent PostDeleted(id, post)
                End If
                CreateDeleteEvent(DateTime.Now, id, post)
                Exit Sub
            ElseIf xElm.Element("limit") IsNot Nothing Then
                Debug.Print(line)
                Exit Sub
            ElseIf xElm.Element("event") IsNot Nothing Then
                Debug.Print("event: " + xElm.Element("event").Value)
                CreateEventFromJson(line)
                Exit Sub
            ElseIf xElm.Element("direct_message") IsNot Nothing Then
                Debug.Print("direct_message")
                isDm = True
            End If
        End Using

        Dim res As New StringBuilder
        res.Length = 0
        res.Append("[")
        res.Append(line)
        res.Append("]")

        If isDm Then
            CreateDirectMessagesFromJson(res.ToString, WORKERTYPE.UserStream, False)
        Else
            CreatePostsFromJson(res.ToString, WORKERTYPE.Timeline, Nothing, False, Nothing, Nothing)
        End If

        RaiseEvent NewPostFromStream()
    End Sub

    Private Sub CreateDeleteEvent(ByVal createdat As DateTime, ByVal id As Int64, ByVal post As PostClass)
        Dim evt As New FormattedEvent
        evt.CreatedAt = createdat
        If post Is Nothing Then
            Dim tmp As PostClass = (From p In _deletemessages Where p.Id = id).FirstOrDefault
            If tmp IsNot Nothing Then
                post = tmp
                _deletemessages.Remove(post)
            End If
        End If
        If post Is Nothing Then
            'evt.Event = "DELETE(UNKNOWN)"
            'evt.Username = "--UNKNOWN--"
            'evt.Target = "--UNKNOWN--"
            '保持していない発言に対しての削除イベントは無視
            Exit Sub
        Else
            If post.IsDm Then
                evt.Event = "DELETE(DM)"
            Else
                evt.Event = "DELETE(Post)"
            End If
            evt.Username = post.Name
            evt.Target = If(post.Data.Length > 5, post.Data.Substring(0, 5) + "...", post.Data) + " [" + post.PDate.ToString + "]"
        End If
        Me.StoredEvent.Insert(0, evt)
        RaiseEvent UserStreamEventReceived(evt)
    End Sub

    Private Sub CreateEventFromJson(ByVal content As String)
        Dim eventData As TwitterDataModel.EventData = Nothing
        Try
            eventData = CreateDataFromJson(Of TwitterDataModel.EventData)(content)
        Catch ex As SerializationException
            TraceOut(ex, "Event Serialize Exception!" + Environment.NewLine + content)
        Catch ex As Exception
            TraceOut(ex, "Event Exception!" + Environment.NewLine + content)
        End Try

        Dim evt As New FormattedEvent
        evt.CreatedAt = DateTimeParse(eventData.CreatedAt)
        evt.Event = eventData.Event
        evt.Username = eventData.Source.ScreenName
        Select Case eventData.Event
            Case "follow"
                If eventData.Target.ScreenName.ToLower.Equals(_uid) Then
                    If Not Me.followerId.Contains(eventData.Source.Id) Then Me.followerId.Add(eventData.Source.Id)
                Else
                    Exit Sub    'Block後のUndoをすると、SourceとTargetが逆転したfollowイベントが帰ってくるため。
                End If
                evt.Target = ""
            Case "favorite", "unfavorite"
                evt.Target = eventData.TargetObject.Text
            Case "list_member_added", "list_member_removed"
                evt.Target = eventData.TargetObject.Name
            Case "block"
                evt.Target = ""
            Case Else
                TraceOut("Unknown Event:" + evt.Event + Environment.NewLine + content)
        End Select
        Me.StoredEvent.Insert(0, evt)
        RaiseEvent UserStreamEventReceived(evt)
    End Sub

    Private Function CreateDataFromJson(Of T)(ByVal content As String) As T
        Dim data As T
        Using stream As New MemoryStream()
            Dim buf As Byte() = Encoding.Unicode.GetBytes(content)
            stream.Write(Encoding.Unicode.GetBytes(content), offset:=0, count:=buf.Length)
            stream.Seek(offset:=0, loc:=SeekOrigin.Begin)
            data = DirectCast((New DataContractJsonSerializer(GetType(T))).ReadObject(stream), T)
        End Using
        Return data
    End Function

    Private Sub userStream_Started() Handles userStream.Started
        RaiseEvent UserStreamStarted()
    End Sub

    Private Sub userStream_Stopped() Handles userStream.Stopped
        RaiseEvent UserStreamStopped()
    End Sub

    Public ReadOnly Property UserStreamEnabled As Boolean
        Get
            Return If(userStream Is Nothing, False, userStream.Enabled)
        End Get
    End Property

    Public Sub StartUserStream()
        If userStream IsNot Nothing Then
            StopUserStream()
        End If
        userStream = New TwitterUserstream(twCon)
        userStream.Start(Me.AllAtReply, Me.TrackWord)
    End Sub

    Public Sub StopUserStream()
        If userStream IsNot Nothing Then userStream.Dispose()
        userStream = Nothing
        If Not _endingFlag Then RaiseEvent UserStreamStopped()
    End Sub

    Public Sub ReconnectUserStream()
        If userStream IsNot Nothing Then
            Me.StartUserStream()
        End If
    End Sub

    Private Class TwitterUserstream
        Implements IDisposable

        Public Event StatusArrived(ByVal status As String)
        Public Event Stopped()
        Public Event Started()
        Private twCon As HttpTwitter

        Private _streamThread As Thread
        Private _streamActive As Boolean

        Private _allAtreplies As Boolean = False
        Private _trackwords As String = ""

        Public Sub New(ByVal twitterConnection As HttpTwitter)
            twCon = DirectCast(twitterConnection.Clone(), HttpTwitter)
        End Sub

        Public Sub Start(ByVal allAtReplies As Boolean, ByVal trackwords As String)
            Me.AllAtReplies = allAtReplies
            Me.TrackWords = trackwords
            _streamActive = True
            If _streamThread IsNot Nothing AndAlso _streamThread.IsAlive Then Exit Sub
            _streamThread = New Thread(AddressOf UserStreamLoop)
            _streamThread.Name = "UserStreamReceiver"
            _streamThread.IsBackground = True
            _streamThread.Start()
        End Sub

        Public ReadOnly Property Enabled() As Boolean
            Get
                Return _streamActive
            End Get
        End Property

        Public Property AllAtReplies As Boolean
            Get
                Return _allAtreplies
            End Get
            Set(ByVal value As Boolean)
                _allAtreplies = value
            End Set
        End Property

        Public Property TrackWords As String
            Get
                Return _trackwords
            End Get
            Set(ByVal value As String)
                _trackwords = value
            End Set
        End Property

        Private Sub UserStreamLoop()
            Dim st As Stream = Nothing
            Dim sr As StreamReader = Nothing
            Do
                Try
                    If Not NetworkInterface.GetIsNetworkAvailable Then
                        Thread.Sleep(30 * 1000)
                        Continue Do
                    End If

                    RaiseEvent Started()

                    twCon.UserStream(st, _allAtreplies, _trackwords)
                    sr = New StreamReader(st)

                    Do While _streamActive AndAlso Not sr.EndOfStream
                        RaiseEvent StatusArrived(sr.ReadLine())
                        'Me.LastTime = Now
                    Loop

                    If sr.EndOfStream Then
                        RaiseEvent Stopped()
                        'TraceOut("Stop:EndOfStream")
                        Thread.Sleep(10 * 1000)
                        Continue Do
                    End If
                    Exit Do
                Catch ex As WebException
                    If Not Me._streamActive Then
                        Exit Do
                    ElseIf ex.Status = WebExceptionStatus.Timeout Then
                        RaiseEvent Stopped()
                        TraceOut("Stop:Timeout")
                        Thread.Sleep(10 * 1000)
                    Else
                        RaiseEvent Stopped()
                        TraceOut("Stop:WebException " & ex.Status.ToString)
                        Thread.Sleep(10 * 1000)
                    End If
                Catch ex As ThreadAbortException
                    Exit Do
                Catch ex As IOException
                    If Not Me._streamActive Then
                        Exit Do
                    Else
                        RaiseEvent Stopped()
                        TraceOut("Stop:IOException with Active." + Environment.NewLine + ex.Message)
                        Thread.Sleep(10 * 1000)
                    End If
                Catch ex As ArgumentException
                    'System.ArgumentException: ストリームを読み取れませんでした。
                    'サーバー側もしくは通信経路上で切断された場合？タイムアウト頻発後発生
                    RaiseEvent Stopped()
                    TraceOut(ex, "Stop:ArgumentException")
                    Thread.Sleep(10 * 1000)
                Catch ex As Exception
                    TraceOut("Stop:Exception." + Environment.NewLine + ex.Message)
                    ExceptionOut(ex)
                Finally
                    If sr IsNot Nothing Then
                        twCon.RequestAbort()
                        sr.BaseStream.Close()
                    End If
                End Try
            Loop While True

            If _streamActive Then RaiseEvent Stopped()
            TraceOut("Stop:EndLoop")
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 重複する呼び出しを検出するには

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                    _streamActive = False
                    If _streamThread IsNot Nothing AndAlso _streamThread.IsAlive Then
                        _streamThread.Abort()
                        _streamThread.Join(1000)
                    End If
                End If

                ' TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下の Finalize() をオーバーライドします。
                ' TODO: 大きなフィールドを null に設定します。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: 上の Dispose(ByVal disposing As Boolean) にアンマネージ リソースを解放するコードがある場合にのみ、Finalize() をオーバーライドします。
        'Protected Overrides Sub Finalize()
        '    ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 重複する呼び出しを検出するには

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                Me.StopUserStream()
            End If

            ' TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下の Finalize() をオーバーライドします。
            ' TODO: 大きなフィールドを null に設定します。
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: 上の Dispose(ByVal disposing As Boolean) にアンマネージ リソースを解放するコードがある場合にのみ、Finalize() をオーバーライドします。
    'Protected Overrides Sub Finalize()
    '    ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
