Public Class ApiInformationChangedEventArgs
    Inherits EventArgs
    Public MaxCount As Integer
    Public RemainCount As Integer
    Public ResetTime As New DateTime
    Public ResetTimeInSeconds As Integer
    Public UsingCount As Integer
End Class

Public Class ApiInfo
    Public MaxCount As Integer = -1
    Public RemainCount As Integer = -1
    Public ResetTime As New DateTime
    Public ResetTimeInSeconds As Integer = -1
    Public UsingCount As Integer = -1
End Class

Public Class ApiInformation
    Private Shared _MaxCount As Integer = -1
    Private Shared _RemainCount As Integer = -1
    Private Shared _ResetTime As New DateTime
    Private Shared _ResetTimeInSeconds As Integer = -1
    Private Shared _UsingCount As Integer = -1

    Private Shared ReadOnly _lockobj As New Object
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
    Public Shared Event RateLimitStatusHeaderChanged(ByVal sender As Object, ByVal e As ApiInformationChangedEventArgs)

    Private Shared Sub Raise_Changed()
        Dim arg As New ApiInformationChangedEventArgs
        SyncLock _lockobj
            arg.MaxCount = MaxCount
            arg.RemainCount = RemainCount
            arg.ResetTime = ResetTime
            arg.ResetTimeInSeconds = ResetTimeInSeconds
            arg.UsingCount = UsingCount
        End SyncLock
        RaiseEvent Changed(ApiInformation, arg)
        SyncLock _lockobj
            _MaxCount = arg.MaxCount
            _RemainCount = arg.RemainCount
            _ResetTime = arg.ResetTime
            _ResetTimeInSeconds = arg.ResetTimeInSeconds
            _UsingCount = arg.UsingCount
        End SyncLock
    End Sub

    Public Shared Property MaxCount As Integer
        Get
            Return _MaxCount
        End Get
        Set(ByVal value As Integer)
            If _MaxCount <> value Then
                _MaxCount = value
                Raise_Changed()
            End If
        End Set
    End Property

    Public Shared Property RemainCount As Integer
        Get
            Return _RemainCount
        End Get
        Set(ByVal value As Integer)
            If _RemainCount <> value Then
                _RemainCount = value
                Raise_Changed()
            End If
        End Set
    End Property

    Public Shared Property ResetTime As DateTime
        Get
            Return _ResetTime
        End Get
        Set(ByVal value As DateTime)
            If _ResetTime <> value Then
                _ResetTime = value
                Raise_Changed()
            End If
        End Set
    End Property

    Public Shared Property ResetTimeInSeconds As Integer
        Get
            Return _ResetTimeInSeconds
        End Get
        Set(ByVal value As Integer)
            If _ResetTimeInSeconds <> value Then
                _ResetTimeInSeconds = value
                Raise_Changed()
            End If
        End Set
    End Property

    Public Shared Property UsingCount As Integer
        Get
            Return _UsingCount
        End Get
        Set(ByVal value As Integer)
            If _UsingCount <> value Then
                _UsingCount = value
                Raise_Changed()
            End If
        End Set
    End Property
End Class
