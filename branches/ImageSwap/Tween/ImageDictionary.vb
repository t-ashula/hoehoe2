Imports System.Drawing
Imports System.IO

Public Class ImageDictionary
    Implements IDictionary(Of String, Image), IDisposable

    Private ReadOnly lockObject As New Object()

    Private memoryCacheCount As Integer
    Private innerDictionary As Dictionary(Of String, Image)
    Private sortedKeyList As List(Of String)    '古いもの順
    Private fileCacheProcList As New Queue(Of Threading.ThreadStart)

    Public Sub New(ByVal memoryCacheCount As Integer)
        SyncLock Me.lockObject
            Me.innerDictionary = New Dictionary(Of String, Image)(memoryCacheCount + 1)
            Me.sortedKeyList = New List(Of String)(memoryCacheCount + 1)
            Me.memoryCacheCount = memoryCacheCount
        End SyncLock
    End Sub

    Public Sub Add(ByVal item As System.Collections.Generic.KeyValuePair(Of String, Image)) Implements System.Collections.Generic.ICollection(Of System.Collections.Generic.KeyValuePair(Of String, Image)).Add
        Me.Add(item.Key, item.Value)
    End Sub

    Public Sub Add(ByVal key As String, ByVal value As Image) Implements System.Collections.Generic.IDictionary(Of String, Image).Add
        SyncLock Me.lockObject
            Me.innerDictionary.Add(key, value)
            Me.sortedKeyList.Add(key)

            If Me.innerDictionary.Count > Me.memoryCacheCount Then
                Me.DisposeImage(Me.sortedKeyList(Me.sortedKeyList.Count - Me.memoryCacheCount - 1))
            End If

            While Me.fileCacheProcList.Count > 0
                Me.fileCacheProcList.Dequeue().Invoke()
            End While
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
                If Me.innerDictionary(key) IsNot Nothing Then
                    callBack(Me.innerDictionary(key))
                Else
                    Dim imgDlProc As Threading.ThreadStart = Nothing
                    imgDlProc = Sub()
                                    Dim hv As New HttpVarious()
                                    Dim dlImage As Image = hv.GetImage(key, 10000)
                                    Me.innerDictionary(key) = dlImage
                                    callBack(dlImage)
                                End Sub
                End If
            End SyncLock

            Return Me.innerDictionary(key)
        End Get
    End Property

    Default Public Property Item(ByVal key As String) As Image Implements System.Collections.Generic.IDictionary(Of String, Image).Item
        Get
            SyncLock Me.lockObject
                Me.sortedKeyList.Remove(key)
                Me.sortedKeyList.Add(key)
                If Me.sortedKeyList.Count > Me.memoryCacheCount Then
                    Dim disposeKey As String = Me.sortedKeyList(Me.sortedKeyList.Count - Me.memoryCacheCount - 1)
                    Me.fileCacheProcList.Enqueue(Sub()
                                                     If Me.innerDictionary(key) IsNot Nothing Then
                                                         Me.innerDictionary(key).Dispose()
                                                         Me.innerDictionary(key) = Nothing
                                                     End If
                                                 End Sub)
                End If
                Return Me.innerDictionary(key)
            End SyncLock
        End Get
        Set(ByVal value As Image)
            SyncLock Me.lockObject
                Me.sortedKeyList.Remove(key)
                Me.sortedKeyList.Add(key)
                If Me.sortedKeyList.Count > Me.memoryCacheCount Then
                    Me.DisposeImage(Me.sortedKeyList(Me.sortedKeyList.Count - Me.memoryCacheCount - 1))
                End If
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

    Private Sub DisposeImage(ByVal key As String)
        If Me.innerDictionary(key) IsNot Nothing Then
            Me.innerDictionary(key).Dispose()
            Me.innerDictionary(key) = Nothing
        End If
    End Sub

End Class