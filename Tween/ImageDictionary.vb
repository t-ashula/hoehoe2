Imports System.Drawing
Imports System.IO
Imports System.Threading

Public Class ImageDictionary
    Implements IDictionary(Of String, Image), IDisposable

    Private ReadOnly lockObject As New Object()

    Private memoryCacheCount As Integer
    Private innerDictionary As Dictionary(Of String, Image)
    Private sortedKeyList As List(Of String)    '古いもの順
    Private waitStack As Stack(Of KeyValuePair(Of String, Action(Of Image)))

    Public Sub New(ByVal memoryCacheCount As Integer)
        SyncLock Me.lockObject
            Me.innerDictionary = New Dictionary(Of String, Image)(memoryCacheCount + 1)
            Me.sortedKeyList = New List(Of String)(memoryCacheCount + 1)
            Me.memoryCacheCount = memoryCacheCount
            Me.waitStack = New Stack(Of KeyValuePair(Of String, Action(Of Image)))
        End SyncLock
    End Sub

    Public Sub Add(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Add
        Me.Add(item.Key, item.Value)
    End Sub

    Public Sub Add(ByVal key As String, ByVal value As Image) Implements System.Collections.Generic.IDictionary(Of String, Image).Add
        SyncLock Me.lockObject
            If Me.innerDictionary.ContainsKey(key) Then Exit Sub
            Me.innerDictionary.Add(key, value)
            Me.sortedKeyList.Add(key)
            Me.waitStack.Push(New KeyValuePair(Of String, Action(Of Image))(key, Nothing))
            Me.DisposeOldImage()
        End SyncLock
    End Sub

    Public Function Remove(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Remove
        Return Me.Remove(item.Key)
    End Function

    Public Function Remove(ByVal key As String) As Boolean Implements System.Collections.Generic.IDictionary(Of String, Image).Remove
        SyncLock Me.lockObject
            Me.sortedKeyList.Remove(key)
            Me.innerDictionary(key).Dispose()
            Return Me.innerDictionary.Remove(key)
        End SyncLock
    End Function

    Default ReadOnly Property Item(ByVal key As String, ByVal callBack As Action(Of Image)) As Image
        Get
            SyncLock Me.lockObject
                Me.sortedKeyList.Remove(key)
                Me.sortedKeyList.Add(key)
                If Me.innerDictionary(key) IsNot Nothing Then
                    callBack(New Bitmap(Me.innerDictionary(key)))
                Else
                    'スタックに積む
                    Me.waitStack.Push(New KeyValuePair(Of String, Action(Of Image))(key, callBack))
                End If
                Me.DisposeOldImage()
            End SyncLock

            Return Nothing
        End Get
    End Property

    Default Public Property Item(ByVal key As String) As Image Implements System.Collections.Generic.IDictionary(Of String, Image).Item
        Get
            SyncLock Me.lockObject
                Me.sortedKeyList.Remove(key)
                Me.sortedKeyList.Add(key)
                Me.DisposeOldImage()
                If Me.innerDictionary(key) IsNot Nothing Then
                    Return New Bitmap(Me.innerDictionary(key))
                Else
                    Return Nothing
                End If
            End SyncLock
        End Get
        Set(ByVal value As Image)
            SyncLock Me.lockObject
                Me.sortedKeyList.Remove(key)
                Me.sortedKeyList.Add(key)
                Me.DisposeOldImage()
                Me.innerDictionary(key).Dispose()
                Me.innerDictionary(key) = value
            End SyncLock
        End Set
    End Property

    Public Sub Clear() Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Clear
        SyncLock Me.lockObject
            For Each value As Image In Me.innerDictionary.Values
                value.Dispose()
            Next

            Me.innerDictionary.Clear()
            Me.sortedKeyList.Clear()
        End SyncLock
    End Sub

    Public Function Contains(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Contains
        SyncLock Me.lockObject
            Return Me.innerDictionary.ContainsKey(item.Key) AndAlso Me.innerDictionary(item.Key) Is item.Value
        End SyncLock
    End Function

    Public Sub CopyTo(ByVal array() As System.Collections.Generic.KeyValuePair(Of String, Image), ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).CopyTo
        SyncLock Me.lockObject
            Dim index As Integer = arrayIndex
            For Each Item As KeyValuePair(Of String, Image) In Me.innerDictionary
                If array.Length - 1 < index Then
                    Exit For
                End If

                array(index) = New KeyValuePair(Of String, Image)(Item.Key, Item.Value)
                index += 1
            Next
        End SyncLock
    End Sub

    Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Count
        Get
            SyncLock Me.lockObject
                Return Me.innerDictionary.Count
            End SyncLock
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).IsReadOnly
        Get
            Return False
        End Get
    End Property

    Public Function ContainsKey(ByVal key As String) As Boolean Implements System.Collections.Generic.IDictionary(Of String, Image).ContainsKey
        Return Me.innerDictionary.ContainsKey(key)
    End Function

    Public ReadOnly Property Keys As System.Collections.Generic.ICollection(Of String) Implements System.Collections.Generic.IDictionary(Of String, Image).Keys
        Get
            SyncLock Me.lockObject
                Return Me.innerDictionary.Keys
            End SyncLock
        End Get
    End Property

    Public Function TryGetValue(ByVal key As String, ByRef value As Image) As Boolean Implements System.Collections.Generic.IDictionary(Of String, Image).TryGetValue
        SyncLock Me.lockObject
            If Me.innerDictionary.ContainsKey(key) Then
                value = Me.innerDictionary(key)
                Return True
            Else
                Return False
            End If
        End SyncLock
    End Function

    Public ReadOnly Property Values As System.Collections.Generic.ICollection(Of Image) Implements System.Collections.Generic.IDictionary(Of String, Image).Values
        Get
            SyncLock Me.lockObject
                Dim imgList As New List(Of Image)(Me.memoryCacheCount)
                For Each value As Image In Me.innerDictionary.Values
                    imgList.Add(value)
                Next
                Return imgList
            End SyncLock
        End Get
    End Property

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of System.Collections.Generic.KeyValuePair(Of String, Image)) Implements System.Collections.Generic.IEnumerable(Of System.Collections.Generic.KeyValuePair(Of String, Image)).GetEnumerator
        Throw New NotImplementedException()
    End Function

    Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Throw New NotImplementedException()
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        SyncLock Me.lockObject
            For Each item As Image In Me.innerDictionary.Values
                If item IsNot Nothing Then
                    item.Dispose()
                End If
            Next
        End SyncLock
    End Sub

    Private Sub DisposeOldImage()
        If Me.sortedKeyList.Count > Me.memoryCacheCount Then
            Dim key As String = Me.sortedKeyList(Me.sortedKeyList.Count - Me.memoryCacheCount - 1)
            If Me.innerDictionary(key) IsNot Nothing Then
                'Me.innerDictionary(key).Dispose()
                Me.innerDictionary(key) = Nothing
            End If
        End If
    End Sub

    '取得一時停止
    Private _pauseGetImage As Boolean = False
    Public Property PauseGetImage As Boolean
        Get
            Return Me._pauseGetImage
        End Get
        Set(ByVal value As Boolean)
            Me._pauseGetImage = value

            Static popping As Boolean = False

            If Not Me._pauseGetImage AndAlso Not popping AndAlso Me.waitStack.Count > 0 Then
                popping = True
                '最新から処理し
                Dim imgDlProc As ThreadStart
                imgDlProc = Sub()
                                While Me.waitStack.Count > 0 AndAlso Not Me._pauseGetImage
                                    Dim req As KeyValuePair(Of String, Action(Of Image))
                                    SyncLock lockObject
                                        req = Me.waitStack.Pop
                                    End SyncLock
                                    Dim proc As New GetImageDelegate(AddressOf GetImage)
                                    proc.BeginInvoke(req, Nothing, Nothing)
                                End While
                                popping = False
                            End Sub
                imgDlProc.BeginInvoke(Nothing, Nothing)
            End If
        End Set
    End Property
    Delegate Sub GetImageDelegate(ByVal arg1 As KeyValuePair(Of String, Action(Of Image)))
    Private Sub GetImage(ByVal downloadAsyncInfo As KeyValuePair(Of String, Action(Of Image)))
        Dim callbackImage As Image = Nothing
        SyncLock lockObject
            If Me.innerDictionary(downloadAsyncInfo.Key) IsNot Nothing Then
                callbackImage = New Bitmap(Me.innerDictionary(downloadAsyncInfo.Key))
            End If
        End SyncLock
        If callbackImage IsNot Nothing Then
            If downloadAsyncInfo.Value IsNot Nothing Then
                downloadAsyncInfo.Value.Invoke(callbackImage)
            End If
            Exit Sub
        End If
        Dim hv As New HttpVarious()
        Dim dlImage As Image = hv.GetImage(downloadAsyncInfo.Key, 10000)
        SyncLock lockObject
            If Me.innerDictionary(downloadAsyncInfo.Key) Is Nothing Then Me.innerDictionary(downloadAsyncInfo.Key) = dlImage
            callbackImage = New Bitmap(dlImage)
        End SyncLock
        If downloadAsyncInfo.Value IsNot Nothing Then
            downloadAsyncInfo.Value.Invoke(callbackImage)
        End If
    End Sub
End Class