Public Class PostBrowser
    Private _post As PostClass
    Public Property Post As PostClass
        Set(ByVal value As PostClass)
            Me._post = value
        End Set
        Get
            Return Me._post
        End Get
    End Property

    Private _isMonospace As Boolean
    Public Property isMonospace As Boolean
        Set(ByVal value As Boolean)
            Me._isMonospace = value
        End Set
        Get
            Return Me._isMonospace
        End Get
    End Property

    Private _font As Font = MyBase.Font
    Public Overrides Property Font As Font
        Set(ByVal value As Font)
            Me._font = value
        End Set
        Get
            Return Me._font
        End Get
    End Property
End Class
