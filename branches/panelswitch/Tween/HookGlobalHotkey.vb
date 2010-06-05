'�t�H�[���̃R���X�g���N�^�ł��̃N���X�̃C���X�^���X�쐬���邱��
'�C���X�^���X�ϐ���withevents�Ő錾���AHotkeyPressed�C�x���g���󂯎�邱��
'�O���[�o���z�b�g�L�[��RegisterOriginalHotkey�œo�^�B������ޓo�^�ł��邪�A�d���`�F�b�N�͂��Ă��Ȃ��̂Œ��ӁB
'�Đݒ�O�ɂ�UnregisterAllOriginalHotkey���ĂԂ���

Public Class HookGlobalHotkey
    Inherits NativeWindow
    Implements IDisposable

    Private _targetForm As Form
    Private _hotkeyID As Dictionary(Of Integer, KeyEventArgs)

    <FlagsAttribute()> _
    Public Enum ModKeys As Integer
        None = 0
        Alt = &H1
        Ctrl = &H2
        Shift = &H4
        Win = &H8
    End Enum

    Public Event HotkeyPressed As KeyEventHandler

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        Const WM_HOTKEY As Integer = &H312
        If m.Msg = WM_HOTKEY Then
            If _hotkeyID.ContainsKey(m.WParam.ToInt32) Then RaiseEvent HotkeyPressed(Me, _hotkeyID(m.WParam.ToInt32))
            Exit Sub
        End If
        MyBase.WndProc(m)
    End Sub

    Public Sub New(ByVal targetForm As Form)
        _targetForm = targetForm
        _hotkeyID = New Dictionary(Of Integer, KeyEventArgs)

        AddHandler _targetForm.HandleCreated, AddressOf Me.OnHandleCreated
        AddHandler _targetForm.HandleDestroyed, AddressOf Me.OnHandleDestroyed
    End Sub

    Public Sub OnHandleCreated(ByVal sender As Object, ByVal e As EventArgs)
        Me.AssignHandle(_targetForm.Handle)
    End Sub

    Public Sub OnHandleDestroyed(ByVal sender As Object, ByVal e As EventArgs)
        Me.ReleaseHandle()
    End Sub

    Public Function RegisterOriginalHotkey(ByVal hotkey As Keys, ByVal modifiers As ModKeys) As Boolean
        Dim modKey As Keys = Keys.None
        If (modifiers And ModKeys.Alt) = ModKeys.Alt Then modKey = modKey Or Keys.Alt
        If (modifiers And ModKeys.Ctrl) = ModKeys.Ctrl Then modKey = modKey Or Keys.Control
        If (modifiers And ModKeys.Shift) = ModKeys.Shift Then modKey = modKey Or Keys.Shift
        If (modifiers And ModKeys.Win) = ModKeys.Win Then modKey = modKey Or Keys.LWin
        Dim key As New KeyEventArgs(hotkey Or modKey)
        For Each kvp As KeyValuePair(Of Integer, KeyEventArgs) In Me._hotkeyID
            If kvp.Value.KeyData = key.KeyData Then Return True '�o�^�ς݂Ȃ琳��I��
        Next
        Dim hotkeyId As Integer = RegisterGlobalHotKey(hotkey, modifiers, Me._targetForm)
        If hotkeyId > 0 Then
            Me._hotkeyID.Add(hotkeyId, key)
            Return True
        End If
        Return False
    End Function

    Public Sub UnregisterAllOriginalHotkey()
        For Each hotkeyId As Integer In Me._hotkeyID.Keys
            UnregisterGlobalHotKey(hotkeyId, Me._targetForm)
        Next
        Me._hotkeyID.Clear()
    End Sub

    Private disposedValue As Boolean = False        ' �d������Ăяo�������o����ɂ�

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: �����I�ɌĂяo���ꂽ�Ƃ��Ƀ}�l�[�W ���\�[�X��������܂�
            End If

            ' TODO: ���L�̃A���}�l�[�W ���\�[�X��������܂�
            If Me._targetForm IsNot Nothing AndAlso Not Me._targetForm.IsDisposed Then
                Me.UnregisterAllOriginalHotkey()
                RemoveHandler _targetForm.HandleCreated, AddressOf Me.OnHandleCreated
                RemoveHandler _targetForm.HandleDestroyed, AddressOf Me.OnHandleDestroyed
            End If
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' ���̃R�[�h�́A�j���\�ȃp�^�[���𐳂��������ł���悤�� Visual Basic �ɂ���Ēǉ�����܂����B
    Public Sub Dispose() Implements IDisposable.Dispose
        ' ���̃R�[�h��ύX���Ȃ��ł��������B�N���[���A�b�v �R�[�h����� Dispose(ByVal disposing As Boolean) �ɋL�q���܂��B
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    Protected Overrides Sub Finalize()
        Me.Dispose(False)
        MyBase.Finalize()
    End Sub
End Class
