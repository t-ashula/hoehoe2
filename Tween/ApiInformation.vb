Public Class ApiInformationChangedEventArgs
    Inherits EventArgs

    Public Property MaxCount As Integer
        Get
            Return ApiInformation._MaxCount
        End Get
        Set(ByVal value As Integer)
            ApiInformation._MaxCount = value
        End Set
    End Property
    Public Property RemainCount As Integer
        Get
            Return ApiInformation._RemainCount
        End Get
        Set(ByVal value As Integer)
            ApiInformation._RemainCount = value
        End Set
    End Property
    Public Property ResetTime As DateTime
        Get
            Return ApiInformation._ResetTime
        End Get
        Set(ByVal value As DateTime)
            ApiInformation._ResetTime = value
        End Set
    End Property

    Public Property ResetTimeInSeconds As Integer
        Get
            Return ApiInformation._ResetTimeInSeconds
        End Get
        Set(ByVal value As Integer)
            ApiInformation._ResetTimeInSeconds = value
        End Set
    End Property

    Public Property UsingCount As Integer
        Get
            Return ApiInformation._UsingCount
        End Get
        Set(ByVal value As Integer)
            ApiInformation._UsingCount = value
        End Set
    End Property
End Class


Public Class ApiInfo
    Public MaxCount As Integer = -1
    Public RemainCount As Integer = -1
    Public ResetTime As New DateTime
    Public ResetTimeInSeconds As Integer = -1
    Public UsingCount As Integer = -1
End Class

Public Class ApiInformation
    Friend Shared _MaxCount As Integer = -1
    Friend Shared _RemainCount As Integer = -1
    Friend Shared _ResetTime As New DateTime
    Friend Shared _ResetTimeInSeconds As Integer = -1
    Friend Shared _UsingCount As Integer = -1

    Private Shared _lockobj As Object
    Public Shared WithEvents ApiInformation As Object = New ApiInformation

    Public Shared Sub Initialize()
        _MaxCount = -1
        _RemainCount = -1
        _ResetTime = New DateTime
        _ResetTimeInSeconds = -1
        'UsingCountは初期化対象外
    End Sub

    Public Shared Function ConvertResetTimeInSecondsToResetTime(ByVal seconds As Integer) As DateTime
        If seconds >= 0 Then
            Return System.TimeZone.CurrentTimeZone.ToLocalTime((New DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(seconds))
        Else
            Return New DateTime
        End If
    End Function

    Public Shared Event Changed(ByVal sender As Object, ByVal e As ApiInformationChangedEventArgs)

    Private Shared Sub Raise_Changed()
        Dim arg As New ApiInformationChangedEventArgs
        arg.MaxCount = MaxCount
        arg.RemainCount = RemainCount
        arg.ResetTime = ResetTime
        arg.ResetTimeInSeconds = ResetTimeInSeconds
        arg.UsingCount = UsingCount
        RaiseEvent Changed(ApiInformation, arg)
    End Sub

    Public Shared Property MaxCount As Integer
        Get
            Return _MaxCount
        End Get
        Set(ByVal value As Integer)
            _MaxCount = value
            Raise_Changed()
        End Set
    End Property

    Public Shared Property RemainCount As Integer
        Get
            Return _RemainCount
        End Get
        Set(ByVal value As Integer)
            _RemainCount = value
            Raise_Changed()
        End Set
    End Property

    Public Shared Property ResetTime As DateTime
        Get
            Return _ResetTime
        End Get
        Set(ByVal value As DateTime)
            _ResetTime = value
            Raise_Changed()
        End Set
    End Property

    Public Shared Property ResetTimeInSeconds As Integer
        Get
            Return _ResetTimeInSeconds
        End Get
        Set(ByVal value As Integer)
            _ResetTimeInSeconds = value
            Raise_Changed()
        End Set
    End Property

    Public Shared Property UsingCount As Integer
        Get
            Return _UsingCount
        End Get
        Set(ByVal value As Integer)
            _UsingCount = value
            Raise_Changed()
        End Set
    End Property
End Class
