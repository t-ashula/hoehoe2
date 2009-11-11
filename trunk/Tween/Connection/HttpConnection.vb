Imports System.Net
Imports System.IO
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Text

'''<summary>
'''HttpWebRequest,HttpWebResponse���g�p������{�I�ȒʐM�@�\��񋟂���
'''</summary>
'''<remarks>
'''�v���L�V���Ȃǂ�ݒ肷�邽�߁A�g�p�O�ɐÓI���\�b�hInitializeConnection���Ăяo�����ƁB
'''�ʐM�����ɂ���ĕK�v�ɂȂ�HTTP�w�b�_�̕t���Ȃǂ́A�h���N���X��GetContent���\�b�h���I�[�o�[���C�h���čs���B
'''</remarks>
Public Class HttpConnection
    '''<summary>
    '''�v���L�V
    '''</summary>
    Private Shared proxy As WebProxy = Nothing

    '''<summary>
    '''���[�U�[���I�������v���L�V�̕���
    '''</summary>
    Private Shared proxyType As ProxyType = ProxyType.IE

    '''<summary>
    '''�N�b�L�[�ۑ��p�R���e�i
    '''</summary>
    Private Shared cookieContainer As New CookieContainer

    '''<summary>
    '''�������ς݃t���O
    '''</summary>
    Private Shared isInitialize As Boolean = False

    '''<summary>
    '''HTTP�ʐM�̃��\�b�h
    '''</summary>
    '''<remarks>
    '''���̃��\�b�h�iHEAD,PUT,CONNECT�Ȃǁj���K�v�ȏꍇ�͒ǉ����邱��
    '''</remarks>
    Protected Enum RequestMethod
        ReqGet
        ReqPost
    End Enum

    '''<summary>
    '''HttpWebRequest�I�u�W�F�N�g���擾����
    '''</summary>
    '''<remarks>
    '''�K�v�ȃw�b�_�ނ͌Ăяo�����ŕt�����邱��
    '''�iTimeout,AutomaticDecompression,AllowAutoRedirect,UserAgent,ContentType,Accept,HttpRequestHeader.Authorization,�J�X�^���w�b�_�j
    '''<param name="method">HTTP�ʐM���\�b�h�iGET/POST�Ȃǁj</param>
    '''<param name="requestUri">�ʐM��URI</param>
    '''<param name="param">GET���̃N�G���A�܂���POST���̃{�f�B�f�[�^</param>
    '''<param name="withCookie">�ʐM��cookie���g�p���邩</param>
    '''<returns>�����Ŏw�肳�ꂽ���e�𔽉f����HttpWebRequest�I�u�W�F�N�g</returns>
    Protected Shared Function CreateRequest(ByVal method As RequestMethod, _
                                            ByVal requestUri As Uri, _
                                            ByVal param As SortedList(Of String, String), _
                                            ByVal withCookie As Boolean _
                                        ) As HttpWebRequest
        If Not isInitialize Then Throw New Exception("Sequence error.(not initialized)")

        'GET���\�b�h�̏ꍇ�̓N�G����url������
        Dim ub As New UriBuilder(requestUri.AbsoluteUri)
        If method = RequestMethod.ReqGet Then
            ub.Query = CreateQueryString(param)
        End If

        Dim webReq As HttpWebRequest = DirectCast(WebRequest.Create(ub.Uri), HttpWebRequest)

        '�v���L�V�ݒ�
        If proxyType <> proxyType.IE Then webReq.Proxy = proxy

        If method = RequestMethod.ReqGet Then
            webReq.Method = "GET"
        Else
            webReq.Method = "POST"
            webReq.ContentType = "application/x-www-form-urlencoded"
            'POST���\�b�h�̏ꍇ�́A�{�f�B�f�[�^�Ƃ��ăN�G���\�����ď�������
            Using writer As New StreamWriter(webReq.GetRequestStream)
                writer.Write(CreateQueryString(param))
            End Using
        End If
        'cookie�ݒ�
        If withCookie Then webReq.CookieContainer = cookieContainer
        '�^�C���A�E�g�ݒ�
        webReq.Timeout = DefaultTimeout

        Return webReq
    End Function

    '''<summary>
    '''HTTP�̉������������A�X�g���[���̃R�s�[��ԋp
    '''</summary>
    '''<remarks>
    '''���_�C���N�g�����̏ꍇ�iAllowAutoRedirect=False�̏ꍇ�̂݁j�́AheaderInfo�C���X�^���X�������Location��ǉ����ă��_�C���N�g���ԋp�B�{�f�B�f�[�^�͏������Ȃ��B
    '''WebException�̓n���h�����Ă��Ȃ��̂ŁA�Ăяo�����ŃL���b�`���邱��
    '''</remarks>
    '''<param name="webRequest">HTTP�ʐM���N�G�X�g�I�u�W�F�N�g</param>
    '''<param name="contentStream">[OUT]HTTP�����̃{�f�B�X�g���[���̃R�s�[�������ݗp</param>
    '''<param name="headerInfo">[IN/OUT]HTTP�����̃w�b�_���B�w�b�_�����L�[�ɂ��ċ�f�[�^�̃R���N�V������n�����ƂŁA�Y���̃w�b�_���f�[�^�ɐݒ肵�Ė߂�</param>
    '''<param name="withCookie">�ʐM��cookie���g�p����</param>
    '''<returns>HTTP�����̃X�e�[�^�X�R�[�h</returns>
    Protected Shared Function GetResponse(ByVal webRequest As HttpWebRequest, _
                                        ByVal contentStream As Stream, _
                                        ByVal headerInfo As Dictionary(Of String, String), _
                                        ByVal withCookie As Boolean _
                                    ) As HttpStatusCode
        Using webRes As HttpWebResponse = CType(webRequest.GetResponse(), HttpWebResponse)
            Dim statusCode As HttpStatusCode = webRes.StatusCode
            'cookie�ێ�
            If withCookie Then SaveCookie(webRes.Cookies)
            '���_�C���N�g�����̏ꍇ�́A���_�C���N�g���ݒ肵�ďI��
            GetHeaderInfo(webRes, headerInfo)
            '�����̃X�g���[�����R�s�[���Ė߂�
            If webRes.ContentLength > 0 Then
                Using stream As Stream = webRes.GetResponseStream()
                    If stream IsNot Nothing Then CopyStream(stream, contentStream)
                End Using
            End If
            Return statusCode
        End Using
    End Function

    Protected Shared Function GetResponse(ByVal webRequest As HttpWebRequest, _
                                        ByRef contentText As String, _
                                        ByVal headerInfo As Dictionary(Of String, String), _
                                        ByVal withCookie As Boolean _
                                    ) As HttpStatusCode
        Using webRes As HttpWebResponse = CType(webRequest.GetResponse(), HttpWebResponse)
            Dim statusCode As HttpStatusCode = webRes.StatusCode
            'cookie�ێ�
            If withCookie Then SaveCookie(webRes.Cookies)
            '���_�C���N�g�����̏ꍇ�́A���_�C���N�g���ݒ肵�ďI��
            GetHeaderInfo(webRes, headerInfo)
            '�����̃X�g���[�����e�L�X�g�ɏ����o���Ė߂�
            If contentText Is Nothing Then Throw New ArgumentNullException("contentText")
            If webRes.ContentLength > 0 Then
                Using sr As StreamReader = New StreamReader(webRes.GetResponseStream)
                    contentText = sr.ReadToEnd()
                End Using
            End If
            Return statusCode
        End Using
    End Function

    Protected Shared Function GetResponse(ByVal webRequest As HttpWebRequest, _
                                        ByVal contentBitmap As Bitmap, _
                                        ByVal headerInfo As Dictionary(Of String, String), _
                                        ByVal withCookie As Boolean _
                                    ) As HttpStatusCode
        Using webRes As HttpWebResponse = CType(webRequest.GetResponse(), HttpWebResponse)
            Dim statusCode As HttpStatusCode = webRes.StatusCode
            'cookie�ێ�
            If withCookie Then SaveCookie(webRes.Cookies)
            '���_�C���N�g�����̏ꍇ�́A���_�C���N�g���ݒ肵�ďI��
            GetHeaderInfo(webRes, headerInfo)
            '�����̃X�g���[����Bitmap�ɂ��Ė߂�
            If webRes.ContentLength > 0 Then contentBitmap = New Bitmap(webRes.GetResponseStream)
            Return statusCode
        End Using
    End Function

    Private Shared Sub SaveCookie(ByVal cookieCollection As CookieCollection)
        For Each ck As Cookie In cookieCollection
            If ck.Domain.StartsWith(".") Then
                ck.Domain = ck.Domain.Substring(1, ck.Domain.Length - 1)
                cookieContainer.Add(ck)
            End If
        Next
    End Sub

    '''<summary>
    '''in/out�̃X�g���[���C���X�^���X���󂯎��A�R�s�[���ĕԋp
    '''</summary>
    '''<param name="inStream">�R�s�[���X�g���[���C���X�^���X�B�ǂݎ��ł��邱��</param>
    '''<param name="outStream">�R�s�[��X�g���[���C���X�^���X�B�������݉ł��邱��</param>
    Private Shared Sub CopyStream(ByVal inStream As Stream, ByVal outStream As Stream)
        If inStream Is Nothing Then Throw New ArgumentNullException("inStream")
        If outStream Is Nothing Then Throw New ArgumentNullException("outStream")
        If Not inStream.CanRead Then Throw New ArgumentException("Input stream can not read.")
        If Not outStream.CanWrite Then Throw New ArgumentException("Output stream can not write.")
        If inStream.CanSeek AndAlso inStream.Length = 0 Then Throw New ArgumentException("Input stream do not have data.")

        Do
            Dim buffer(1024) As Byte
            Dim i As Integer = buffer.Length
            i = inStream.Read(buffer, 0, i)
            If i = 0 Then Exit Do
            outStream.Write(buffer, 0, i)
        Loop
    End Sub

    '''<summary>
    '''headerInfo�̃L�[���Ŏw�肳�ꂽHTTP�w�b�_�����擾�E�i�[����Bredirect��������Location�w�b�_�̓��e��ǋL����
    '''</summary>
    '''<param name="webResponse">HTTP����</param>
    '''<param name="headerInfo">[IN/OUT]�L�[�Ƀw�b�_�����w�肵���f�[�^��̃R���N�V�����B�擾�����l���f�[�^�ɃZ�b�g���Ė߂�</param>
    Private Shared Sub GetHeaderInfo(ByVal webResponse As HttpWebResponse, _
                                    ByVal headerInfo As Dictionary(Of String, String))

        If headerInfo Is Nothing Then Exit Sub

        If headerInfo.Count > 0 Then
            Dim keys(headerInfo.Count - 1) As String
            headerInfo.Keys.CopyTo(keys, 0)
            For Each key As String In keys
                If Array.IndexOf(webResponse.Headers.AllKeys, key) > -1 Then
                    headerInfo.Item(key) = webResponse.Headers.Item(key)
                Else
                    headerInfo.Item(key) = ""
                End If
            Next
        End If

        Dim statusCode As HttpStatusCode = webResponse.StatusCode
        If statusCode = HttpStatusCode.MovedPermanently OrElse _
           statusCode = HttpStatusCode.Found OrElse _
           statusCode = HttpStatusCode.SeeOther OrElse _
           statusCode = HttpStatusCode.TemporaryRedirect Then
            If headerInfo.ContainsKey("Location") Then
                headerInfo.Item("Location") = webResponse.Headers.Item("Location")
            Else
                headerInfo.Add("Location", webResponse.Headers.Item("Location"))
            End If
        End If
    End Sub

    '''<summary>
    '''�N�G���R���N�V������key=value�`���̕�����ɍ\�����Ė߂�
    '''</summary>
    '''<param name="param">�N�G���A�܂��̓|�X�g�f�[�^�ƂȂ�key-value�R���N�V����</param>
    Protected Shared Function CreateQueryString(ByVal param As SortedList(Of String, String)) As String
        If param Is Nothing OrElse param.Count = 0 Then Return String.Empty

        Dim query As New StringBuilder
        For Each key As String In param.Keys
            query.AppendFormat("{0}={1}&", UrlEncode(key), UrlEncode(param(key)))
        Next
        Return query.ToString(0, query.Length - 1)
    End Function

    '''<summary>
    '''�N�G���`���ikey1=value1&key2=value2&...�j�̕������key-value�R���N�V�����ɋl�ߒ���
    '''</summary>
    '''<param name="queryString">�N�G��������</param>
    '''<returns>key-value�̃R���N�V����</returns>
    Protected Shared Function ParseQueryString(ByVal queryString As String) As NameValueCollection
        Dim query As New NameValueCollection
        Dim parts() As String = queryString.Split("&"c)
        For Each part As String In parts
            Dim index As Integer = part.IndexOf("="c)
            If index = -1 Then
                query.Add(Uri.UnescapeDataString(part), "")
            Else
                query.Add(Uri.UnescapeDataString(part.Substring(0, index)), Uri.UnescapeDataString(part.Substring(index + 1)))
            End If
        Next
        Return query
    End Function

    '''<summary>
    '''2�o�C�g�������l������Url�G���R�[�h
    '''</summary>
    '''<param name="str">�G���R�[�h���镶����</param>
    '''<returns>�G���R�[�h���ʕ�����</returns>
    Protected Shared Function UrlEncode(ByVal stringToEncode As String) As String
        Const UnreservedChars As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~"
        Dim sb As New StringBuilder
        Dim bytes As Byte() = Encoding.UTF8.GetBytes(stringToEncode)

        For Each b As Byte In bytes
            If UnreservedChars.IndexOf(Chr(b)) <> -1 Then
                sb.Append(Chr(b))
            Else
                sb.AppendFormat("%{0:X2}", b)
            End If
        Next
        Return sb.ToString()
    End Function

#Region "DefaultTimeout"
    '''<summary>
    '''�ʐM�^�C���A�E�g���ԁims�j
    '''</summary>
    Private Shared timeout As Integer = 20000

    '''<summary>
    '''�ʐM�^�C���A�E�g���ԁims�j�B10�`120�b�͈̔͂Ŏw��B�͈͊O��20�b�Ƃ���
    '''</summary>
    Protected Shared Property DefaultTimeout() As Integer
        Get
            Return timeout
        End Get
        Set(ByVal value As Integer)
            Const TimeoutMinValue As Integer = 10000
            Const TimeoutMaxValue As Integer = 120000
            Const TimeoutDefaultValue As Integer = 20000
            If value < TimeoutMinValue OrElse value > TimeoutMaxValue Then
                ' �͈͊O�Ȃ�f�t�H���g�l�ݒ�
                timeout = TimeoutDefaultValue
            Else
                timeout = value
            End If
        End Set
    End Property
#End Region

    '''<summary>
    '''�ʐM�N���X�̏����������B�^�C���A�E�g�l�ƃv���L�V��ݒ肷��
    '''</summary>
    '''<remarks>
    '''�ʐM�J�n�O�ɍŒ��x�Ăяo������
    '''</remarks>
    '''<param name="timeout">�^�C���A�E�g�l�i�b�j</param>
    '''<param name="proxyType">�Ȃ��E�w��EIE�f�t�H���g</param>
    '''<param name="proxyAddress">�v���L�V�̃z�X�g��orIP�A�h���X</param>
    '''<param name="proxyPort">�v���L�V�̃|�[�g�ԍ�</param>
    '''<param name="proxyUser">�v���L�V�F�؂��K�v�ȏꍇ�̃��[�U���B�s�v�Ȃ�󕶎�</param>
    '''<param name="proxyPassword">�v���L�V�F�؂��K�v�ȏꍇ�̃p�X���[�h�B�s�v�Ȃ�󕶎�</param>
    Public Shared Sub InitializeConnection( _
            ByVal timeout As Integer, _
            ByVal proxyType As ProxyType, _
            ByVal proxyAddress As String, _
            ByVal proxyPort As Integer, _
            ByVal proxyUser As String, _
            ByVal proxyPassword As String)
        isInitialize = True
        ServicePointManager.Expect100Continue = False
        DefaultTimeout = timeout * 1000     's -> ms
        Select Case proxyType
            Case proxyType.None
                proxy = Nothing
            Case proxyType.Specified
                proxy = New WebProxy("http://" + proxyAddress + ":" + proxyPort.ToString)
                If Not String.IsNullOrEmpty(proxyUser) OrElse Not String.IsNullOrEmpty(proxyPassword) Then
                    proxy.Credentials = New NetworkCredential(proxyUser, proxyPassword)
                End If
            Case proxyType.IE
                'IE�ݒ�i�V�X�e���ݒ�j�̓f�t�H���g�l�Ȃ̂ŏ������Ȃ�
        End Select
        proxyType = proxyType
    End Sub
End Class
