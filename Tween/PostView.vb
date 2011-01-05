Imports System.Text.RegularExpressions

Public Class PostView
    Private Thumbnail As Thumbnail ' = New Thumbnail(Me)

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

            Thumbnail.thumbnail(value.Id, Me.PostBrowser.Links)
        End Set
        Get
            Return Me._post
        End Get
    End Property

    Public Property OneWayLoveColor As Color
    Public Property RetweetColor As Color
    Public Property FavoriteColor As Color
End Class
