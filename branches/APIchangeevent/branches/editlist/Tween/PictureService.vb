﻿Imports System.IO
Imports System.Net
Imports System.Xml

Public Class PictureService
    Private tw As Twitter

    Public Function Upload(ByRef filePath As String, ByRef message As String, ByVal service As String) As String
        Dim file As New FileInfo(filePath)
        If Not file.Exists Then Return "Err:File isn't exists."
        Dim st As Setting = Setting.Instance
        Dim ret As String = ""
        Dim upResult As Boolean = False
        Select Case service
            Case "TwitPic"
                ret = UpToTwitPic(file, message, upResult)
            Case "img.ly"
                ret = UpToimgly(file, message, upResult)
            Case "TwitVideo"
                ret = UpToTwitVideo(file, message, upResult)
        End Select
        If upResult Then filePath = ""
        Return ret
    End Function

    Public Function IsValidExtension(ByVal ext As String, ByVal service As String) As Boolean
        Dim ret As Boolean = False
        Select Case service
            Case "TwitPic"
                ret = (New TwitPic(tw.AccessToken, tw.AccessTokenSecret)).CheckValidExtension(ext)
            Case "img.ly"
                ret = (New imgly(tw.AccessToken, tw.AccessTokenSecret)).CheckValidExtension(ext)
            Case "TwitVideo"
                ret = (New TwitVideo).CheckValidExtension(ext)
        End Select
        Return ret
    End Function

    Public Function GetFileOpenDialogFilter(ByVal service As String) As String
        Dim ret As String = ""
        Select Case service
            Case "TwitPic"
                ret = (New TwitPic(tw.AccessToken, tw.AccessTokenSecret)).GetFileOpenDialogFilter
            Case "img.ly"
                ret = (New imgly(tw.AccessToken, tw.AccessTokenSecret)).GetFileOpenDialogFilter
            Case "TwitVideo"
                ret = (New TwitVideo).GetFileOpenDialogFilter
        End Select
        Return ret
    End Function

    Public Function GetFileType(ByVal ext As String, ByVal service As String) As UploadFileType
        Dim ret As UploadFileType = UploadFileType.Invalid
        Select Case service
            Case "TwitPic"
                ret = (New TwitPic(tw.AccessToken, tw.AccessTokenSecret)).GetFileType(ext)
            Case "img.ly"
                ret = (New imgly(tw.AccessToken, tw.AccessTokenSecret)).GetFileType(ext)
            Case "TwitVideo"
                ret = (New TwitVideo).GetFileType(ext)
        End Select
        Return ret
    End Function

    Public Function IsSupportedFileType(ByVal type As UploadFileType, ByVal service As String) As Boolean
        Dim ret As Boolean = False
        Select Case service
            Case "TwitPic"
                ret = (New TwitPic(tw.AccessToken, tw.AccessTokenSecret)).IsSupportedFileType(type)
            Case "img.ly"
                ret = (New imgly(tw.AccessToken, tw.AccessTokenSecret)).IsSupportedFileType(type)
            Case "TwitVideo"
                ret = (New TwitVideo).IsSupportedFileType(type)
        End Select
        Return ret
    End Function

    Public Function GetMaxFileSize(ByVal ext As String, ByVal service As String) As Long
        Dim ret As Long = -1
        Select Case service
            Case "TwitPic"
                ret = (New TwitPic(tw.AccessToken, tw.AccessTokenSecret)).GetMaxFileSize(ext)
            Case "img.ly"
                ret = (New imgly(tw.AccessToken, tw.AccessTokenSecret)).GetMaxFileSize(ext)
            Case "TwitVideo"
                ret = (New TwitVideo).GetMaxFileSize(ext)
        End Select
        Return ret
    End Function

    Private Function UpToTwitPic(ByVal file As FileInfo, ByRef message As String, ByVal resultUpload As Boolean) As String
        Dim content As String = ""
        Dim ret As HttpStatusCode
        'TwitPicへの投稿
        Dim svc As New TwitPic(tw.AccessToken, tw.AccessTokenSecret)
        Try
            ret = svc.Upload(file, message, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Dim url As String = ""
        If ret = HttpStatusCode.OK Then
            Dim xd As XmlDocument = New XmlDocument()
            Try
                xd.LoadXml(content)
                'URLの取得
                url = xd.SelectSingleNode("/image/url").InnerText
            Catch ex As XmlException
                Return "Err:" + ex.Message
            End Try
        Else
            Return "Err:" + ret.ToString
        End If
        'アップロードまでは成功
        resultUpload = True
        'Twitterへの投稿
        '投稿メッセージの再構成
        If message.Length + url.Length + 1 > 140 Then
            message = message.Substring(0, 140 - url.Length - 1) + " " + url
        Else
            message += " " + url
        End If
        Return tw.PostStatus(message, 0)
    End Function

    Private Function UpToimgly(ByVal file As FileInfo, ByRef message As String, ByVal resultUpload As Boolean) As String
        Dim content As String = ""
        Dim ret As HttpStatusCode
        'img.lyへの投稿
        Dim svc As New imgly(tw.AccessToken, tw.AccessTokenSecret)
        Try
            ret = svc.Upload(file, message, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Dim url As String = ""
        If ret = HttpStatusCode.OK Then
            Dim xd As XmlDocument = New XmlDocument()
            Try
                xd.LoadXml(content)
                'URLの取得
                url = xd.SelectSingleNode("/image/url").InnerText
            Catch ex As XmlException
                Return "Err:" + ex.Message
            End Try
        Else
            Return "Err:" + ret.ToString
        End If
        'アップロードまでは成功
        resultUpload = True
        'Twitterへの投稿
        '投稿メッセージの再構成
        If message.Length + url.Length + 1 > 140 Then
            message = message.Substring(0, 140 - url.Length - 1) + " " + url
        Else
            message += " " + url
        End If
        Return tw.PostStatus(message, 0)
    End Function

    Private Function UpToTwitVideo(ByVal file As FileInfo, ByRef message As String, ByVal resultUpload As Boolean) As String
        Dim content As String = ""
        Dim ret As HttpStatusCode
        'TwitVideoへの投稿
        Dim svc As New TwitVideo
        Try
            ret = svc.Upload(file, message, "", tw.Username, tw.UserIdNo, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Dim url As String = ""
        If ret = HttpStatusCode.OK Then
            Dim xd As XmlDocument = New XmlDocument()
            Try
                xd.LoadXml(content)
                Dim rslt As String = xd.SelectSingleNode("/rsp/@status").Value
                If rslt = "ok" Then
                    'URLの取得
                    url = xd.SelectSingleNode("/rsp/mediaurl").InnerText
                Else
                    Return "Err:" + xd.SelectSingleNode("/rsp/err/@msg").Value
                End If
            Catch ex As XmlException
                Return "Err:" + ex.Message
            End Try
        Else
            Return "Err:" + ret.ToString
        End If
        'アップロードまでは成功
        resultUpload = True
        'Twitterへの投稿
        '投稿メッセージの再構成
        If message.Length + url.Length + 1 > 140 Then
            message = message.Substring(0, 140 - url.Length - 1) + " " + url
        Else
            message += " " + url
        End If
        Return tw.PostStatus(message, 0)
    End Function

    Public Sub New(ByVal twInstance As Twitter)
        tw = twInstance
    End Sub
End Class
