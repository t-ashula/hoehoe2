Imports System.Net
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.IO
Imports System.Text
Imports System.Security

Public Class HttpConnectionOAuth
    Inherits HttpConnection

    '''<summary>
    '''OAuth������oauth_timestamp�Z�o�p����t�i1970/1/1 00:00:00�j
    '''</summary>
    Private Shared ReadOnly UnixEpoch As New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)

    '''<summary>
    '''OAuth������oauth_nonce�Z�o�p�����N���X
    '''</summary>
    Private Shared ReadOnly NonceRandom As New Random

    '''<summary>
    '''OAuth�̔F�؃v���Z�X���̂ݎg�p���郊�N�G�X�g�g�[�N��
    '''</summary>
    Private Shared requestToken As String

    '''<summary>
    '''OAuth�̃A�N�Z�X�g�[�N���B�i�����\�i���[�U�[�������̉\���͂���j�B
    '''</summary>
    Private Shared token As String = ""

    '''<summary>
    '''OAuth�̏����쐬�p�閧�A�N�Z�X�g�[�N���B�i�����\�i���[�U�[�������̉\���͂���j�B
    '''</summary>
    Private Shared tokenSecret As String = ""

    '''<summary>
    '''OAuth�̃R���V���[�}�[��
    '''</summary>
    Private Const ConsumerKey As String = "EANjQEa5LokuVld682tTDA"

    '''<summary>
    '''OAuth�̏����쐬�p�閧�R���V���[�}�[�f�[�^
    '''</summary>
    Private Const ConsumerSecret As String = "zXfwkzmuO6FcHtoikleV3EVgdh5vVAs6ft6ZxtYTYM"

    '''<summary>
    '''OAuth�̃��N�G�X�g�g�[�N���擾��URI
    '''</summary>
    Private Const RequestTokenUrl As String = "http://twitter.com/oauth/request_token"

    '''<summary>
    '''OAuth�̃��[�U�[�F�ؗp�y�[�WURI
    '''</summary>
    '''<remarks>
    '''�N�G���uoauth_token=���N�G�X�g�g�[�N���v��t�����āA����URI���u���E�U�ŊJ���B���[�U�[�����F������s����PIN�R�[�h���\�������B
    '''</remarks>
    Private Const AuthorizeUrl As String = "http://twitter.com/oauth/authorize"

    '''<summary>
    '''OAuth�̃A�N�Z�X�g�[�N���擾��URI
    '''</summary>
    Private Const AccessTokenUrl As String = "http://twitter.com/oauth/access_token"

    '''<summary>
    '''HTTP�ʐM���ăR���e���c���擾����i������R���e���c�j
    '''</summary>
    '''<remarks>
    '''�ʐM�^�C���A�E�g�Ȃ�WebException���n���h�����Ă��Ȃ����߁A�Ăяo�����ŏ������K�v�B
    '''�^�C���A�E�g�w��⃌�X�|���X�w�b�_�擾�͏ȗ����Ă���B
    '''���X�|���X�̃{�f�B�X�g���[���𕶎���ɕϊ�����content�����Ɋi�[���Ė߂��B�����G���R�[�h�͖��w��
    '''</remarks>
    '''<param name="method">HTTP�̃��\�b�h</param>
    '''<param name="requestUri">URI</param>
    '''<param name="param">key=value�ɓW�J����āA�N�G���iGET���j�E�{�f�B�iPOST���j�ɕt������鑗�M���</param>
    '''<param name="content">[IN/OUT]HTTP���X�|���X�̃{�f�B���f�[�^�ԋp�p�B�Ăяo�����ŏ��������K�v</param>
    '''<param name="headerInfo">[IN/OUT]HTTP�����̃w�b�_���</param>
    '''<returns>�ʐM���ʂ�HttpStatusCode</returns>
    Protected Function GetContent(ByVal method As RequestMethod, _
            ByVal requestUri As Uri, _
            ByVal param As SortedList(Of String, String), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String)) As HttpStatusCode
        'content���C���X�^���X����Ă��邩�`�F�b�N
        If content Is Nothing Then Throw New ArgumentNullException("content")
        '�F�؍ς��`�F�b�N
        If String.IsNullOrEmpty(token) Then Throw New Exception("Sequence error. (Token is blank.)")

        Dim webReq As HttpWebRequest = CreateRequest(method, _
                                                    requestUri, _
                                                    param, _
                                                    False)
        'OAuth�F�؃w�b�_��t��
        AppendOAuthInfo(webReq, param, token, tokenSecret)

        Return GetResponse(webReq, content, headerInfo, False)
    End Function

#Region "�F�؏���"
    Protected Function AuthorizeAccount() As Boolean
        Dim authUri As Uri = GetAuthorizePageUri()
        If authUri Is Nothing Then Return False
        System.Diagnostics.Process.Start(authUri.PathAndQuery)
        Dim inputForm As New InputTabName
        inputForm.FormTitle = "Input PIN code"
        inputForm.FormDescription = "Input the PIN code shown in the browser after you accept OAuth request."
        If inputForm.ShowDialog() = DialogResult.OK AndAlso Not String.IsNullOrEmpty(inputForm.TabName) Then
            Return GetAccessToken(inputForm.TabName)
        Else
            Return False
        End If
    End Function

    Private Shared Function GetAuthorizePageUri() As Uri
        Const tokenKey As String = "oauth_token"
        requestToken = ""
        Dim reqTokenData As NameValueCollection = GetOAuthToken(New Uri(RequestTokenUrl), "")
        If reqTokenData IsNot Nothing Then
            requestToken = reqTokenData.Item(tokenKey)
            Dim ub As New UriBuilder(AuthorizeUrl)
            ub.Query = String.Format("{0}={1}", tokenKey, requestToken)
            Return ub.Uri
        Else
            Return Nothing
        End If
    End Function

    Private Shared Function GetAccessToken(ByVal pinCode As String) As Boolean
        If String.IsNullOrEmpty(requestToken) Then Throw New Exception("Sequence error.(requestToken is blank)")

        Dim accessTokenData As NameValueCollection = GetOAuthToken(New Uri(AccessTokenUrl), pinCode)

        If accessTokenData IsNot Nothing Then
            Token = accessTokenData.Item("oauth_token")
            TokenSecret = accessTokenData.Item("oauth_token_secret")
            If Token = "" Then Return False
            Return True
        Else
            Return False
        End If
    End Function

    Private Shared Function GetOAuthToken(ByVal requestUri As Uri, ByVal pinCode As String) As NameValueCollection
        Dim webReq As HttpWebRequest = Nothing
        If String.IsNullOrEmpty(pinCode) Then
            webReq = HttpConnection.CreateRequest(RequestMethod.ReqGet, requestUri, Nothing, False)
        Else
            webReq = HttpConnection.CreateRequest(RequestMethod.ReqPost, requestUri, Nothing, False)
        End If
        Dim query As New SortedList(Of String, String)
        If Not String.IsNullOrEmpty(pinCode) Then query.Add("oauth_verifier", pinCode)
        AppendOAuthInfo(webReq, query, requestToken, "")
        Try
            Dim status As HttpStatusCode
            Dim contentText As String = ""
            status = HttpConnection.GetResponse(webReq, contentText, Nothing, False)
            If status = HttpStatusCode.OK Then
                Return ParseQueryString(contentText)
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#End Region

#Region "OAuth�F�ؗp�w�b�_�쐬�E�t������"
    Private Shared Sub AppendOAuthInfo(ByVal webRequest As HttpWebRequest, _
                                        ByVal query As SortedList(Of String, String), _
                                        ByVal token As String, _
                                        ByVal tokenSecret As String)
        Dim parameter As New SortedList(Of String, String)
        parameter.Add("oauth_consumer_key", ConsumerKey)
        parameter.Add("oauth_signature_method", "HMAC-SHA1")
        parameter.Add("oauth_timestamp", GetTimestamp())
        parameter.Add("oauth_nonce", GetNonce())
        parameter.Add("oauth_version", "1.0")
        If Not String.IsNullOrEmpty(token) Then parameter.Add("oauth_token", token)
        If query IsNot Nothing Then
            For Each item As KeyValuePair(Of String, String) In query
                parameter.Add(item.Key, item.Value)
            Next
        End If
        parameter.Add("oauth_signature", CreateSignature(tokenSecret, webRequest.Method, webRequest.RequestUri, parameter))
        Dim sb As New StringBuilder("OAuth ")
        For Each item As KeyValuePair(Of String, String) In parameter
            If item.Key.StartsWith("oauth_") Then
                sb.AppendFormat("{0}=""{1}"",", item.Key, UrlEncode(item.Value))
            End If
        Next
        webRequest.Headers.Add(HttpRequestHeader.Authorization, sb.ToString)
    End Sub

    Private Shared Function CreateSignature(ByVal tokenSecret As String, _
                                            ByVal method As String, _
                                            ByVal uri As Uri, _
                                            ByVal parameter As SortedList(Of String, String) _
                                        ) As String
        Dim paramString As String = CreateQueryString(parameter)
        Dim url As String = String.Format("{0}://{1}{2}", uri.Scheme, uri.Host, uri.AbsolutePath)
        Dim signatureBase As String = String.Format("{0}&{1}&{2}", method, UrlEncode(url), UrlEncode(paramString))
        Dim key As String = UrlEncode(ConsumerSecret) + "&"
        If Not String.IsNullOrEmpty(tokenSecret) Then key += UrlEncode(tokenSecret)
        Dim hmac As New Cryptography.HMACSHA1(Encoding.ASCII.GetBytes(key))
        Dim hash As Byte() = hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBase))
        Return Convert.ToBase64String(hash)
    End Function

    Private Shared Function GetTimestamp() As String
        Return Convert.ToInt64((DateTime.UtcNow - UnixEpoch).TotalSeconds).ToString()
    End Function

    Private Shared Function GetNonce() As String
        Return NonceRandom.Next(123400, 9999999).ToString()
    End Function
#End Region

    Protected Shared Sub Initialize(ByVal token As String, ByVal tokenSecret As String)
        HttpConnectionOAuth.Token = token
        HttpConnectionOAuth.TokenSecret = tokenSecret
    End Sub
End Class
