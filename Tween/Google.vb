Imports System.Web
Imports System.Text.RegularExpressions

' http://code.google.com/intl/ja/apis/ajaxlanguage/documentation/#fonje
' デベロッパー ガイド - Google AJAX Language API - Google Code

Public Class Google

    Private Const TranslateEndPoint As String = "http://ajax.googleapis.com/ajax/services/language/translate"





    Public Function Translate(ByVal isHtml As Boolean, ByVal source As String, ByRef destination As String) As Boolean
        Dim http As New HttpVarious()
        Dim apiurl As String = TranslateEndPoint
        Dim headers As New Dictionary(Of String, String)
        headers.Add("v", "1.0")
        headers.Add("hl", "ja")             ' TODO:現在のcultureを反映させる
        headers.Add("langpair", "|ja")      ' TODO:現在のcultureを反映させる
        headers.Add("format", "html")

        headers.Add("q", HttpUtility.UrlPathEncode(source))

        Dim content As String = ""
        If http.GetData(apiurl, headers, content) Then
            Dim body As String = Regex.Match(content, """translatedText"":""(?<body>.*)"",""detectedSourceLanguage"":""").Groups("body").Value
            Dim _body As String = Regex.Replace(body, "\\u00", "%")
            Dim buf As String = HttpUtility.UrlDecode(_body)

            destination = String.Copy(buf)
            Return True
        End If
        Return False
    End Function

End Class
