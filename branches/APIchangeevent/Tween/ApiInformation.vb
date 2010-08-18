Public Class ApiInformationChangedEventArgs
    Inherits EventArgs
    Public Property MaxCount As Integer
        Get
            Return ApiInformation.MaxCount
        End Get
        Set(ByVal value As Integer)
            ApiInformation.MaxCount = value
        End Set
    End Property
    Public Property RemainCount As Integer
        Get
            Return ApiInformation.RemainCount
        End Get
        Set(ByVal value As Integer)
            ApiInformation.RemainCount = value
        End Set
    End Property
    Public Property ResetTime As DateTime
        Get
            Return ApiInformation.ResetTime
        End Get
        Set(ByVal value As DateTime)
            ApiInformation.ResetTime = value
        End Set
    End Property

    Public Property ResetTimeInSeconds As Integer
        Get
            Return ApiInformation.ResetTimeInSeconds
        End Get
        Set(ByVal value As Integer)
            ApiInformation.ResetTimeInSeconds = value
        End Set
    End Property

    Public Shared ReadOnly Property UsingCount As Integer
        Get
            Return ApiInformation.UsingCount
        End Get
    End Property
End Class


Public Class ApiInformation
    Private Shared _MaxCount As Integer = -1
    Private Shared _RemainCount As Integer = -1
    Private Shared _ResetTime As New DateTime
    Private Shared _ResetTimeInSeconds As Integer = -1
    Private Shared _UsingCount As Integer = -1

    Private Shared _lockobj As Object

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

    Public Shared Property MaxCount As Integer
        Get
            Return _MaxCount
        End Get
        Set(ByVal value As Integer)
            _MaxCount = value
        End Set
    End Property

    Public Shared Property RemainCount As Integer
        Get
            Return _RemainCount
        End Get
        Set(ByVal value As Integer)
            _RemainCount = value
        End Set
    End Property

    Public Shared Property ResetTime As DateTime
        Get
            Return _ResetTime
        End Get
        Set(ByVal value As DateTime)
            _ResetTime = value
        End Set
    End Property

    Public Shared Property ResetTimeInSeconds As Integer
        Get
            Return _ResetTimeInSeconds
        End Get
        Set(ByVal value As Integer)
            _ResetTimeInSeconds = value
        End Set
    End Property

    Public Shared Property UsingCount As Integer
        Get
            Return _UsingCount
        End Get
        Set(ByVal value As Integer)
            _UsingCount = value
        End Set
    End Property
End Class
