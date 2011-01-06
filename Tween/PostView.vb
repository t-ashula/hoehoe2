Imports System.Text.RegularExpressions

Public Class PostView
    Private _post As PostClass
    Public Property Post As PostClass
        Set(ByVal value As PostClass)
            Me._post = value
            Me.PostBrowser.Post = value

            'SourceLinkLabel
            If value.IsDm Then
                Me.SourceLinkLabel.Tag = Nothing
                Me.SourceLinkLabel.Text = ""
            Else
                Dim mc As Match = Regex.Match(value.SourceHtml, "<a href=""(?<sourceurl>.+?)""")
                If mc.Success Then
                    Dim src As String = mc.Groups("sourceurl").Value
                    Me.SourceLinkLabel.Tag = mc.Groups("sourceurl").Value
                    mc = Regex.Match(src, "^https?://")
                    If Not mc.Success Then
                        src = src.Insert(0, "http://twitter.com")
                    End If
                    Me.SourceLinkLabel.Tag = src
                Else
                    Me.SourceLinkLabel.Tag = Nothing
                End If
                If String.IsNullOrEmpty(value.Source) Then
                    Me.SourceLinkLabel.Text = ""
                Else
                    Me.SourceLinkLabel.Text = value.Source
                End If
            End If
            Me.SourceLinkLabel.TabStop = False

            'NameLabel
            If value.IsDm AndAlso Not value.IsOwl Then
                Me.NameLabel.Text = "DM TO -> "
            ElseIf value.IsDm Then
                Me.NameLabel.Text = "DM FROM <- "
            Else
                Me.NameLabel.Text = ""
            End If
            Me.NameLabel.Text += value.Name + "/" + value.Nickname
            Me.NameLabel.Tag = value.Name
            If Not String.IsNullOrEmpty(value.RetweetedBy) Then
                Me.NameLabel.Text += " (RT:" + value.RetweetedBy + ")"
            End If
            Me.NameLabel.ForeColor = System.Drawing.SystemColors.ControlText

            'DateTimeLabel
            Me.DateTimeLabel.Text = value.PDate.ToString()
            If value.IsOwl AndAlso (AppendSettingDialog.Instance.OneWayLove OrElse value.IsDm) Then NameLabel.ForeColor = Me.OneWayLoveColor
            If value.RetweetedId > 0 Then NameLabel.ForeColor = Me.RetweetColor
            If value.IsFav Then NameLabel.ForeColor = Me.FavoriteColor

            'UserPicture
            If UserPicture.Image IsNot Nothing Then UserPicture.Image.Dispose()
            If Not String.IsNullOrEmpty(value.ImageUrl) AndAlso ImageDictionary.Instance.ContainsKey(value.ImageUrl) Then
                UserPicture.Image = ImageDictionary.Instance(value.ImageUrl)
            Else
                UserPicture.Image = Nothing
            End If

            If Me.Thumbnail IsNot Nothing Then
                Thumbnail.thumbnail(value.Id, Me.PostBrowser.Links)
            End If
        End Set
        Get
            Return Me._post
        End Get
    End Property

    Private Sub SourceLinkLabel_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles SourceLinkLabel.LinkClicked
        Dim link As String = CType(SourceLinkLabel.Tag, String)
        If Not String.IsNullOrEmpty(link) Then
            Dim hoge = Me.OpenUriAsync
            hoge(link)
        End If
    End Sub

    Private Sub SourceLinkLabel_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SourceLinkLabel.MouseEnter
        Dim link As String = CType(SourceLinkLabel.Tag, String)
        If Not String.IsNullOrEmpty(link) Then
            Me.StatusText = link
        End If
    End Sub

    Private Sub SourceLinkLabel_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SourceLinkLabel.MouseLeave
        Me.StatusText = String.Empty
    End Sub

    Public Event StatusTextChanged(ByVal sender As Object, ByVal e As EventArgs)

    Private _statusText As String
    Public Property StatusText() As String
        Get
            Return _statusText
        End Get
        Private Set(ByVal value As String)
            Dim needRaiseEvent As Boolean = _statusText <> value
            _statusText = value
            If needRaiseEvent Then
                RaiseEvent StatusTextChanged(Me, EventArgs.Empty)
            End If
        End Set
    End Property


    Public Property OneWayLoveColor As Color
    Public Property RetweetColor As Color
    Public Property FavoriteColor As Color
    Public Property Thumbnail As Thumbnail
    Public Property OpenUriAsync As Action(Of String)
End Class
