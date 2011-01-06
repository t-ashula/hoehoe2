Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Web
Imports System.Reflection

Public Class PostBrowser
    Private Const detailHtmlFormatMono1 As String = "<html><head><style type=""text/css""><!-- pre {font-family: """
    Private Const detailHtmlFormat2 As String = """, sans-serif; font-size: "
    Private Const detailHtmlFormat3 As String = "pt; word-wrap: break-word; color:rgb("
    Private Const detailHtmlFormat4 As String = ");} a:link, a:visited, a:active, a:hover {color:rgb("
    Private Const detailHtmlFormat5 As String = "); } --></style></head><body style=""margin:0px; background-color:rgb("
    Private Const detailHtmlFormatMono6 As String = ");""><pre>"
    Private Const detailHtmlFormatMono7 As String = "</pre></body></html>"
    Private Const detailHtmlFormat1 As String = "<html><head><style type=""text/css""><!-- p {font-family: """
    Private Const detailHtmlFormat6 As String = ");""><p>"
    Private Const detailHtmlFormat7 As String = "</p></body></html>"
    Private detailHtmlFormatHeader As String
    Private detailHtmlFormatFooter As String

    Private _post As PostClass
    Public Property Post As PostClass
        Get
            Return Me._post
        End Get
        Set(ByVal value As PostClass)
            Me._post = value


            If value Is Nothing Then Exit Property

            Dim dTxt As String = createDetailHtml(If(value.IsDeleted, "(DELETED)", value.OriginalData))

            If Me.IsDumpPostMode Then
                Dim sb As New StringBuilder(512)

                sb.Append("-----Start PostClass Dump<br>")
                sb.AppendFormat("Data           : {0}<br>", value.Data)
                sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", value.Data)
                sb.AppendFormat("Id             : {0}<br>", value.Id.ToString)
                'sb.AppendFormat("ImageIndex     : {0}<br>", _curPost.ImageIndex.ToString)
                sb.AppendFormat("ImageUrl       : {0}<br>", value.ImageUrl)
                sb.AppendFormat("InReplyToId    : {0}<br>", value.InReplyToId.ToString)
                sb.AppendFormat("InReplyToUser  : {0}<br>", value.InReplyToUser)
                sb.AppendFormat("IsDM           : {0}<br>", value.IsDm.ToString)
                sb.AppendFormat("IsFav          : {0}<br>", value.IsFav.ToString)
                sb.AppendFormat("IsMark         : {0}<br>", value.IsMark.ToString)
                sb.AppendFormat("IsMe           : {0}<br>", value.IsMe.ToString)
                sb.AppendFormat("IsOwl          : {0}<br>", value.IsOwl.ToString)
                sb.AppendFormat("IsProtect      : {0}<br>", value.IsProtect.ToString)
                sb.AppendFormat("IsRead         : {0}<br>", value.IsRead.ToString)
                sb.AppendFormat("IsReply        : {0}<br>", value.IsReply.ToString)

                For Each nm As String In value.ReplyToList
                    sb.AppendFormat("ReplyToList    : {0}<br>", nm)
                Next

                sb.AppendFormat("Name           : {0}<br>", value.Name)
                sb.AppendFormat("NickName       : {0}<br>", value.Nickname)
                sb.AppendFormat("OriginalData   : {0}<br>", value.OriginalData)
                sb.AppendFormat("(PlainText)    : <xmp>{0}</xmp><br>", value.OriginalData)
                sb.AppendFormat("PDate          : {0}<br>", value.PDate.ToString)
                sb.AppendFormat("Source         : {0}<br>", value.Source)
                sb.AppendFormat("Uid            : {0}<br>", value.Uid)
                sb.AppendFormat("FilterHit      : {0}<br>", value.FilterHit)
                sb.AppendFormat("RetweetedBy    : {0}<br>", value.RetweetedBy)
                sb.AppendFormat("RetweetedId    : {0}<br>", value.RetweetedId)
                sb.AppendFormat("SearchTabName  : {0}<br>", value.RelTabName)
                sb.Append("-----End PostClass Dump<br>")

                Me.WebBrowser1.Visible = False
                Me.WebBrowser1.DocumentText = detailHtmlFormatHeader + sb.ToString + detailHtmlFormatFooter
                Me.WebBrowser1.Visible = True
            Else
                Try
                    If Me.WebBrowser1.DocumentText <> dTxt Then
                        Me.WebBrowser1.Visible = False
                        Me.WebBrowser1.DocumentText = dTxt

                        Me.Links.Clear()
                        For Each lnk As Match In Regex.Matches(dTxt, "<a target=""_self"" href=""(?<url>http[^""]+)""", RegexOptions.IgnoreCase)
                            Me.Links.Add(lnk.Result("${url}"))
                        Next
                    End If
                Catch ex As System.Runtime.InteropServices.COMException
                    '原因不明
                Finally
                    Me.WebBrowser1.Visible = True
                End Try
            End If

        End Set
    End Property

    Private _links As New List(Of String)
    Public ReadOnly Property Links As List(Of String)
        Get
            Return Me._links
        End Get
    End Property

    Private _isMonospace As Boolean
    Public Property IsMonospace As Boolean
        Get
            Return Me._isMonospace
        End Get
        Set(ByVal value As Boolean)
            Me._isMonospace = value
            If value Then
                Me.detailHtmlFormatHeader = detailHtmlFormatMono1
                Me.detailHtmlFormatFooter = detailHtmlFormatMono7
            Else
                Me.detailHtmlFormatHeader = detailHtmlFormat1
                Me.detailHtmlFormatFooter = detailHtmlFormat7
            End If

            Me.detailHtmlFormatHeader += Me.Font.Name + detailHtmlFormat2 + Me.Font.Size.ToString() + detailHtmlFormat3 + Me.Color.R.ToString() + "," + Me.Color.G.ToString() + "," + Me.Color.B.ToString() + detailHtmlFormat4 + Me.LinkColor.R.ToString() + "," + Me.LinkColor.G.ToString() + "," + Me.LinkColor.B.ToString() + detailHtmlFormat5 + Me.BackColor.R.ToString() + "," + Me.BackColor.G.ToString() + "," + Me.BackColor.B.ToString()

            If value Then
                Me.detailHtmlFormatHeader += detailHtmlFormatMono6
            Else
                Me.detailHtmlFormatHeader += detailHtmlFormat6
            End If
        End Set
    End Property

    Private _font As Font = MyBase.Font
    Public Overrides Property Font As Font
        Get
            Return Me._font
        End Get
        Set(ByVal value As Font)
            Me._font = value
        End Set
    End Property

    Private _color As Color
    Public Property Color As Color
        Get
            Return Me._color
        End Get
        Set(ByVal value As Color)
            Me._color = value
        End Set
    End Property


    Private _backColor As Color = MyBase.BackColor
    Public Overrides Property BackColor As Color
        Get
            Return _backColor
        End Get
        Set(ByVal value As Color)
            _backColor = value
        End Set
    End Property


    Private _linkColor As Color
    Public Property LinkColor As Color
        Get
            Return _linkColor
        End Get
        Set(ByVal value As Color)
            _linkColor = value
        End Set
    End Property

    Public Property IsDumpPostMode As Boolean = False

    Public Function createDetailHtml(ByVal orgdata As String) As String
        Return Me.detailHtmlFormatHeader + orgdata + Me.detailHtmlFormatFooter
    End Function

    Private Sub PostBrowser_Navigated(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserNavigatedEventArgs) Handles WebBrowser1.Navigated
        If e.Url.AbsoluteUri <> "about:blank" Then
            Me.Post = Me.Post
            Dim hoge = Me.OpenUriAsync
            hoge(e.Url.OriginalString) 'wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww
        End If
    End Sub

    Private Sub PostBrowser_Navigating(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs) Handles WebBrowser1.Navigating
        If e.Url.Scheme = "data" Then
            Me.StatusText = Me.WebBrowser1.StatusText.Replace("&", "&&")
        ElseIf e.Url.AbsoluteUri <> "about:blank" Then
            e.Cancel = True

            If e.Url.AbsoluteUri.StartsWith("http://twitter.com/search?q=%23") OrElse _
               e.Url.AbsoluteUri.StartsWith("https://twitter.com/search?q=%23") Then
                'ハッシュタグの場合は、タブで開く
                Dim urlStr As String = HttpUtility.UrlDecode(e.Url.AbsoluteUri)
                Dim hash As String = urlStr.Substring(urlStr.IndexOf("#"))
                HashSupl.AddItem(hash)
                HashMgr.AddHashToHistory(hash.Trim, False)
                Dim hoge = Me.AddNewTabForSearch
                hoge(hash)
                Exit Sub
            Else
                Dim m As Match = Regex.Match(e.Url.AbsoluteUri, "^https?://twitter.com/(#!/)?(?<name>[a-zA-Z0-9_]+)$")
                If m.Success AndAlso IsTwitterId(m.Result("${name}")) Then
                    Dim hoge = Me.AddNewTabForUserTimeline
                    hoge(m.Result("${name}"))
                Else
                    Dim hoge = Me.OpenUriAsync
                    hoge(e.Url.OriginalString)
                End If
            End If
        End If
    End Sub

    Private Sub PostBrowser_StatusTextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles WebBrowser1.StatusTextChanged
        If Me.WebBrowser1.StatusText.StartsWith("http") OrElse Me.WebBrowser1.StatusText.StartsWith("ftp") _
                OrElse Me.WebBrowser1.StatusText.StartsWith("data") Then
            Me.StatusText = Me.WebBrowser1.StatusText.Replace("&", "&&")
        End If
    End Sub

    Private Sub ScrollDownPostBrowser(ByVal forward As Boolean)
        Dim doc As HtmlDocument = Me.WebBrowser1.Document
        If doc Is Nothing Then Exit Sub
        If doc.Body Is Nothing Then Exit Sub

        If forward Then
            doc.Body.ScrollTop += AppendSettingDialog.Instance.FontDetail.Height
        Else
            doc.Body.ScrollTop -= AppendSettingDialog.Instance.FontDetail.Height
        End If
    End Sub

    Private Sub PageDownPostBrowser(ByVal forward As Boolean)
        Dim doc As HtmlDocument = Me.WebBrowser1.Document
        If doc Is Nothing Then Exit Sub
        If doc.Body Is Nothing Then Exit Sub

        If forward Then
            doc.Body.ScrollTop += Me.WebBrowser1.ClientRectangle.Height - AppendSettingDialog.Instance.FontDetail.Height
        Else
            doc.Body.ScrollTop -= Me.WebBrowser1.ClientRectangle.Height - AppendSettingDialog.Instance.FontDetail.Height
        End If
    End Sub

    Public Function WebBrowser_GetSelectionText(ByRef ComponentInstance As WebBrowser) As String
        '発言詳細で「選択文字列をコピー」を行う
        'WebBrowserコンポーネントのインスタンスを渡す
        Dim typ As Type = ComponentInstance.ActiveXInstance.GetType()
        Dim _SelObj As Object = typ.InvokeMember("selection", BindingFlags.GetProperty, Nothing, ComponentInstance.Document.DomDocument, Nothing)
        Dim _objRange As Object = _SelObj.GetType().InvokeMember("createRange", BindingFlags.InvokeMethod, Nothing, _SelObj, Nothing)
        Return DirectCast(_objRange.GetType().InvokeMember("text", BindingFlags.GetProperty, Nothing, _objRange, Nothing), String)
    End Function

    Public Event StatusTextChanged(ByVal sender As Object, ByVal e As EventArgs)

    Private _statusText As String
    Public Property StatusText() As String
        Get
            Return _statusText
        End Get
        Private Set(ByVal value As String)
            Dim needRaiseEvent As Boolean = Me._statusText <> value
            _statusText = value
            If needRaiseEvent Then
                RaiseEvent StatusTextChanged(Me, EventArgs.Empty)
            End If
        End Set
    End Property


    Public Property OpenUriAsync As Action(Of String)
    Public Property HashSupl As AtIdSupplement
    Public Property HashMgr As HashtagManage
    Public Property AddNewTabForSearch As Action(Of String)
    Public Property IsTwitterId As Func(Of String, Boolean)
    Public Property AddNewTabForUserTimeline As Action(Of String)
End Class
