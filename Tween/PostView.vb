Public Class PostView
    Private _post As PostClass
    Public Property Post As PostClass
        Set(ByVal value As PostClass)
            Me._post = value
            Me.PostBrowser.Post = value
        End Set
        Get
            Return Me._post
        End Get
    End Property
End Class
