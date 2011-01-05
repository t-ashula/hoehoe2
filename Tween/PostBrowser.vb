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
        End Set
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

            Me.detailHtmlFormatHeader += Me.Font.Name + detailHtmlFormat2 + Me.Font.Size.ToString() + detailHtmlFormat3 + Me.Color.R.ToString() + "," + Me.Color.G.ToString() + "," + _clDetail.B.ToString() + detailHtmlFormat4 + Me.LinkColor.R.ToString() + "," + Me.LinkColor.G.ToString() + "," + Me.LinkColor.B.ToString() + detailHtmlFormat5 + Me.BackColor.R.ToString() + "," + Me.BackColor.G.ToString() + "," + Me.BackColor.B.ToString()

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
End Class
