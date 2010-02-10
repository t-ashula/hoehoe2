' Tween - Client of Twitter
' Copyright (c) 2007-2010 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
'           (c) 2008-2010 Moz (@syo68k) <http://iddy.jp/profile/moz/>
'           (c) 2008-2010 takeshik (@takeshik) <http://www.takeshik.org/>
' All rights reserved.
' 
' This file is part of Tween.
' 
' This program is free software; you can redistribute it and/or modify it
' under the terms of the GNU General Public License as published by the Free
' Software Foundation; either version 3 of the License, or (at your option)
' any later version.
' 
' This program is distributed in the hope that it will be useful, but
' WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
' or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
' for more details. 
' 
' You should have received a copy of the GNU General Public License along
' with this program. If not, see <http://www.gnu.org/licenses/>, or write to
' the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
' Boston, MA 02110-1301, USA.

Imports System.Runtime.InteropServices
Imports System.Diagnostics

Module Win32Api
#Region "��s�N���v���Z�X���A�N�e�B�u�ɂ���"
    ' �O���v���Z�X�̃E�B���h�E���N������
    Public Sub WakeupWindow(ByVal hWnd As IntPtr)
        ' ���C���E�E�B���h�E���ŏ�������Ă���Ό��ɖ߂�
        If IsIconic(hWnd) Then
            ShowWindowAsync(hWnd, SW_RESTORE)
        End If

        ' ���C���E�E�B���h�E���őO�ʂɕ\������
        SetForegroundWindow(hWnd)
    End Sub

    ' �O���v���Z�X�̃��C���E�E�B���h�E���N�����邽�߂�Win32 API
    <DllImport("user32.dll")> _
    Private Function SetForegroundWindow( _
        ByVal hWnd As IntPtr) As Boolean
    End Function
    ' �E�B���h�E�̕\����Ԃ�ݒ�
    <DllImport("user32.dll")> _
    Private Function ShowWindowAsync( _
        ByVal hWnd As IntPtr, _
        ByVal nCmdShow As Integer) As Boolean
    End Function
    ' �w�肳�ꂽ�E�B���h�E���ŏ����i �A�C�R�����j����Ă��邩�ǂ����𒲂ׂ�
    <DllImport("user32.dll")> _
    Private Function IsIconic( _
        ByVal hWnd As IntPtr) As Boolean
    End Function
    ' ShowWindowAsync�֐��̃p�����[�^�ɓn����`�l
    Private Const SW_RESTORE As Integer = 9 ' ��ʂ����̑傫���ɖ߂�

    ' ���s���̓����A�v���P�[�V�����̃v���Z�X���擾����
    Public Function GetPreviousProcess() As Process
        Dim curProcess As Process = Process.GetCurrentProcess()
        Dim allProcesses() As Process = Process.GetProcessesByName(curProcess.ProcessName)

        Dim checkProcess As Process
        For Each checkProcess In allProcesses
            ' �������g�̃v���Z�XID�͖�������
            If checkProcess.Id <> curProcess.Id Then
                ' �v���Z�X�̃t���p�X�����r���ē����A�v���P�[�V����������
                If String.Compare( _
                        checkProcess.MainModule.FileName, _
                        curProcess.MainModule.FileName, True) = 0 Then
                    ' �����t���p�X���̃v���Z�X���擾
                    Return checkProcess
                End If
            End If
        Next

        ' �����A�v���P�[�V�����̃v���Z�X��������Ȃ��I  
        Return Nothing
    End Function
#End Region
#Region "�^�X�N�g���C�A�C�R���̃N���b�N"
    ' �w�肳�ꂽ�N���X������уE�B���h�E���ƈ�v����g�b�v���x���E�B���h�E�̃n���h�����擾���܂�
    <DllImport("user32.dll")> _
    Private Function FindWindow( _
        ByVal lpClassName As String, _
        ByVal lpWindowName As String) As IntPtr
    End Function
    ' �w�肳�ꂽ������ƈ�v����N���X���ƃE�B���h�E������������E�B���h�E�̃n���h����Ԃ��܂�
    <DllImport("user32.dll")> _
    Private Function FindWindowEx( _
        ByVal hWnd1 As IntPtr, _
        ByVal hWnd2 As IntPtr, _
        ByVal lpsz1 As String, _
        ByVal lpsz2 As String) As IntPtr
    End Function
    ' �w�肳�ꂽ�E�B���h�E�ցA�w�肳�ꂽ���b�Z�[�W�𑗐M���܂�
    <DllImport("user32.dll")> _
    Private Function SendMessage( _
        ByVal hwnd As IntPtr, _
        ByVal wMsg As Integer, _
        ByVal wParam As IntPtr, _
        ByVal lParam As IntPtr) As Integer
    End Function
    ' SendMessage�ő��M���郁�b�Z�[�W
    Private Enum Sm_Message As Integer
        WM_USER = &H400                     '���[�U�[��`���b�Z�[�W
        TB_GETBUTTON = WM_USER + 23         '�c�[���o�[�̃{�^���擾
        TB_BUTTONCOUNT = WM_USER + 24       '�c�[���o�[�̃{�^���i�A�C�R���j���擾
        TB_GETBUTTONINFO = WM_USER + 65     '�c�[���o�[�̃{�^���ڍ׏��擾
    End Enum
    ' �c�[���o�[�{�^���\����
    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Private Structure TBBUTTON
        Public iBitmap As Integer
        Public idCommand As IntPtr
        Public fsState As Byte
        Public fsStyle As Byte
        Public bReserved0 As Byte
        Public bReserved1 As Byte
        Public dwData As Integer
        Public iString As Integer
    End Structure
    ' �c�[���o�[�{�^���ڍ׏��\����
    <StructLayout(LayoutKind.Sequential)> _
    Private Structure TBBUTTONINFO
        Public cbSize As Int32
        Public dwMask As Int32
        Public idCommand As Int32
        Public iImage As Int32
        Public fsState As Byte
        Public fsStyle As Byte
        Public cx As Short
        Public lParam As IntPtr
        Public pszText As IntPtr
        Public cchText As Int32
    End Structure
    ' TBBUTTONINFO��lParam�Ń|�C���g�����A�C�R�����iPostMessage�Ŏg�p�j
    <StructLayout(LayoutKind.Sequential)> _
    Private Structure TRAYNOTIFY
        Public hWnd As IntPtr
        Public uID As UInt32
        Public uCallbackMessage As UInt32
        Public dwDummy1 As UInt32
        Public dwDummy2 As UInt32
        Public hIcon As IntPtr
    End Structure
    ' TBBUTTONINFO�Ɏw�肷��}�X�N���
    <Flags()> _
    Private Enum ToolbarButtonMask As Int32
        TBIF_COMMAND = &H20
        TBIF_LPARAM = &H10
        TBIF_TEXT = &H2
    End Enum
    ' �w�肳�ꂽ�E�B���h�E���쐬�����X���b�h�� ID ���擾���܂�
    <DllImport("user32.dll", SetLastError:=True)> _
    Private Function GetWindowThreadProcessId( _
        ByVal hwnd As IntPtr, _
        ByRef lpdwProcessId As Integer) As Integer
    End Function
    ' �w�肵���v���Z�XID�ɑ΂���v���Z�X�n���h�����擾���܂�
    <DllImport("kernel32.dll")> _
    Private Function OpenProcess( _
        ByVal dwDesiredAccess As ProcessAccess, _
        <MarshalAs(UnmanagedType.Bool)> ByVal bInheritHandle As Boolean, _
        ByVal dwProcessId As Integer) As IntPtr
    End Function
    ' OpenProcess�Ŏw�肷��A�N�Z�X��
    <Flags()> _
    Private Enum ProcessAccess As Integer
        ''' <summary>Specifies all possible access flags for the process object.</summary>
        AllAccess = CreateThread Or DuplicateHandle Or QueryInformation Or SetInformation Or Terminate Or VMOperation Or VMRead Or VMWrite Or Synchronize
        ''' <summary>Enables usage of the process handle in the CreateRemoteThread function to create a thread in the process.</summary>
        CreateThread = &H2
        ''' <summary>Enables usage of the process handle as either the source or target process in the DuplicateHandle function to duplicate a handle.</summary>
        DuplicateHandle = &H40
        ''' <summary>Enables usage of the process handle in the GetExitCodeProcess and GetPriorityClass functions to read information from the process object.</summary>
        QueryInformation = &H400
        ''' <summary>Enables usage of the process handle in the SetPriorityClass function to set the priority class of the process.</summary>
        SetInformation = &H200
        ''' <summary>Enables usage of the process handle in the TerminateProcess function to terminate the process.</summary>
        Terminate = &H1
        ''' <summary>Enables usage of the process handle in the VirtualProtectEx and WriteProcessMemory functions to modify the virtual memory of the process.</summary>
        VMOperation = &H8
        ''' <summary>Enables usage of the process handle in the ReadProcessMemory function to' read from the virtual memory of the process.</summary>
        VMRead = &H10
        ''' <summary>Enables usage of the process handle in the WriteProcessMemory function to write to the virtual memory of the process.</summary>
        VMWrite = &H20
        ''' <summary>Enables usage of the process handle in any of the wait functions to wait for the process to terminate.</summary>
        Synchronize = &H100000
    End Enum
    ' �w�肵���v���Z�X�̉��z�A�h���X��ԂɃ������̈���m��
    <DllImport("kernel32.dll", SetLastError:=True, ExactSpelling:=True)> _
    Private Function VirtualAllocEx( _
        ByVal hProcess As IntPtr, _
        ByVal lpAddress As IntPtr, _
        ByVal dwSize As Integer, _
        ByVal flAllocationType As AllocationTypes, _
        ByVal flProtect As MemoryProtectionTypes) As IntPtr
    End Function
    ' �A���P�[�g���
    <Flags()> _
    Private Enum AllocationTypes As UInteger
        Commit = &H1000
        Reserve = &H2000
        Decommit = &H4000
        Release = &H8000
        Reset = &H80000
        Physical = &H400000
        TopDown = &H100000
        WriteWatch = &H200000
        LargePages = &H20000000
    End Enum
    ' �A���P�[�g�����������ɑ΂���ی샌�x��
    <Flags()> _
    Private Enum MemoryProtectionTypes As UInteger
        Execute = &H10
        ExecuteRead = &H20
        ExecuteReadWrite = &H40
        ExecuteWriteCopy = &H80
        NoAccess = &H1
        [ReadOnly] = &H2
        ReadWrite = &H4
        WriteCopy = &H8
        GuardModifierflag = &H100
        NoCacheModifierflag = &H200
        WriteCombineModifierflag = &H400
    End Enum
    ' �I�[�v�����Ă���J�[�l���I�u�W�F�N�g�̃n���h�����N���[�Y���܂�
    <DllImport("kernel32.dll", SetLastError:=True)> _
    Private Function CloseHandle(ByVal hHandle As IntPtr) As Boolean
    End Function
    ' �w�肳�ꂽ�v���Z�X�̉��z�A�h���X��ԓ��̃������̈������܂��̓R�~�b�g�������܂�
    <DllImport("kernel32.dll")> _
    Private Function VirtualFreeEx( _
        ByVal hProcess As IntPtr, _
        ByVal lpAddress As IntPtr, _
        ByVal dwSize As Integer, _
        ByVal dwFreeType As Integer) As Boolean
    End Function
    ' ������������
    <Flags()> _
    Private Enum MemoryFreeTypes
        Release = &H8000
    End Enum
    '�w�肵���v���Z�X�̃������̈�Ƀf�[�^���R�s�[����
    <DllImport("kernel32.dll", SetLastError:=True)> _
    Private Function WriteProcessMemory( _
        ByVal hProcess As IntPtr, _
        ByVal lpBaseAddress As IntPtr, _
        ByRef lpBuffer As TBBUTTONINFO, _
        ByVal nSize As Integer, _
        <Out()> ByRef lpNumberOfBytesWritten As Integer) As Boolean
    End Function
    '�w�肵���v���Z�X�̃������̈�̃f�[�^���Ăяo�����v���Z�X�̃o�b�t�@�ɃR�s�[����
    <DllImport("kernel32.dll", SetLastError:=True)> _
    Private Function ReadProcessMemory( _
        ByVal hProcess As IntPtr, _
        ByVal lpBaseAddress As IntPtr, _
        ByVal lpBuffer As IntPtr, _
        ByVal iSize As Integer, _
        ByRef lpNumberOfBytesRead As Integer) As Boolean
    End Function
    '���b�Z�[�W���E�B���h�E�̃��b�Z�[�W �L���[�ɒu���A�Ή�����E�B���h�E�����b�Z�[�W����������̂�҂����ɖ߂�܂�
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Function PostMessage( _
        ByVal hWnd As IntPtr, _
        ByVal Msg As UInteger, _
        ByVal wParam As UInt32, _
        ByVal lParam As UInt32) As Boolean
    End Function
    'PostMessage�ő��M���郁�b�Z�[�W
    Private Enum PM_Message As UInt32
        WM_LBUTTONDOWN = &H201      '���}�E�X�{�^����������
        WM_LBUTTONUP = &H202        '���}�E�X�{�^������
    End Enum

    '�^�X�N�g���C�A�C�R���̃N���b�N����
    Public Function ClickTasktrayIcon(ByVal tooltip As String) As Boolean
        Const TRAY_WINDOW As String = "Shell_TrayWnd"
        Const TRAY_NOTIFYWINDOW As String = "TrayNotifyWnd"
        Const TRAY_PAGER As String = "SysPager"
        Const TOOLBAR_CONTROL As String = "ToolbarWindow32"
        '�^�X�N�o�[�̃n���h���擾
        Dim taskbarWin As IntPtr = FindWindow(TRAY_WINDOW, Nothing)
        If taskbarWin.Equals(IntPtr.Zero) Then Return False
        '�ʒm�̈�̃n���h���擾
        Dim trayWin As IntPtr = FindWindowEx(taskbarWin, IntPtr.Zero, TRAY_NOTIFYWINDOW, Nothing)
        If trayWin.Equals(IntPtr.Zero) Then Return False
        'SysPager�̗L���m�F�B�iXP/2000��SysPager����j
        Dim tempWin As IntPtr = FindWindowEx(trayWin, IntPtr.Zero, TRAY_PAGER, Nothing)
        If tempWin.Equals(IntPtr.Zero) Then tempWin = trayWin
        '�^�X�N�g���C���c�[���o�[�ŏo���Ă��邩�m�F
        '�@���@�c�[���o�[�łȂ���ΏI��
        Dim toolWin As IntPtr = FindWindowEx(tempWin, IntPtr.Zero, TOOLBAR_CONTROL, Nothing)
        If toolWin.Equals(IntPtr.Zero) Then Return False
        '�^�X�N�g���C�̃v���Z�X�iExplorer�j���擾���A�O������Q�Ƃ��邽�߂ɊJ��
        Dim expPid As Integer = 0
        GetWindowThreadProcessId(toolWin, expPid)
        Dim hProc As IntPtr = OpenProcess(ProcessAccess.VMOperation Or ProcessAccess.VMRead Or ProcessAccess.VMWrite, False, expPid)
        If hProc.Equals(IntPtr.Zero) Then Return False

        '�v���Z�X����邽�߂�Try-Finally
        Try
            Dim tbButtonLocal As New TBBUTTON   '�{�v���Z�X���̃^�X�N�o�[�{�^�����쐬�i�T�C�Y����ł̂ݎg�p�j
            'Explorer���̃^�X�N�o�[�{�^���i�[�������m��
            Dim ptbSysButton As IntPtr = VirtualAllocEx(hProc, IntPtr.Zero, Marshal.SizeOf(tbButtonLocal), AllocationTypes.Reserve Or AllocationTypes.Commit, MemoryProtectionTypes.ReadWrite)
            If ptbSysButton.Equals(IntPtr.Zero) Then Return False '�������m�ێ��s
            Try
                Dim tbButtonInfoLocal As New TBBUTTONINFO   '�{�v���Z�X���c�[���o�[�{�^���ڍ׏��쐬
                'Explorer���̃^�X�N�o�[�{�^���ڍ׏��i�[�������m��
                Dim ptbSysInfo As IntPtr = VirtualAllocEx(hProc, IntPtr.Zero, Marshal.SizeOf(tbButtonInfoLocal), AllocationTypes.Reserve Or AllocationTypes.Commit, MemoryProtectionTypes.ReadWrite)
                If ptbSysInfo.Equals(IntPtr.Zero) Then Return False '�������m�ێ��s
                Try
                    Const titleSize As Integer = 256    'Tooltip������
                    Dim title As String = ""            'Tooltip������
                    '���L��������Tooltip�Ǎ��������m��
                    Dim pszTitle As IntPtr = Marshal.AllocCoTaskMem(titleSize)
                    If pszTitle.Equals(IntPtr.Zero) Then Return False '�������m�ێ��s
                    Try
                        'Explorer����Tooltip�Ǎ��������m��
                        Dim pszSysTitle As IntPtr = VirtualAllocEx(hProc, IntPtr.Zero, titleSize, AllocationTypes.Reserve Or AllocationTypes.Commit, MemoryProtectionTypes.ReadWrite)
                        If pszSysTitle.Equals(IntPtr.Zero) Then Return False '�������m�ێ��s
                        Try
                            '�ʒm�̈�{�^�����擾
                            Dim iCount As Integer = SendMessage(toolWin, Sm_Message.TB_BUTTONCOUNT, New IntPtr(0), New IntPtr(0))
                            '�����珇�ɏ��擾
                            For i As Integer = 0 To iCount - 1
                                Dim dwBytes As Integer = 0  '�ǂݏ����o�C�g��
                                Dim tbButtonLocal2 As TBBUTTON  '�{�^�����
                                Dim tbButtonInfoLocal2 As TBBUTTONINFO  '�{�^���ڍ׏��
                                '���L�������Ƀ{�^�����Ǎ��������m��
                                Dim ptrLocal As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(tbButtonLocal))
                                If ptrLocal.Equals(IntPtr.Zero) Then Return False '�������m�ێ��s
                                Try
                                    Marshal.StructureToPtr(tbButtonLocal, ptrLocal, True)   '���L������������
                                    '�{�^�����擾�iidCommand���擾���邽�߁j
                                    SendMessage( _
                                        toolWin, _
                                        Sm_Message.TB_GETBUTTON, _
                                        New IntPtr(i), _
                                        ptbSysButton)
                                    'Explorer���̃����������L�������ɓǂݍ���
                                    ReadProcessMemory( _
                                        hProc, _
                                        ptbSysButton, _
                                        ptrLocal, _
                                        Marshal.SizeOf(tbButtonLocal), _
                                        dwBytes)
                                    '���L�������̓��e���\���̂ɕϊ�
                                    tbButtonLocal2 = DirectCast( _
                                                        Marshal.PtrToStructure( _
                                                            ptrLocal, _
                                                            GetType(TBBUTTON)), _
                                                        TBBUTTON)
                                Finally
                                    Marshal.FreeCoTaskMem(ptrLocal) '���L���������
                                End Try

                                '�{�^���ڍ׏����擾���邽�߂̃}�X�N����ݒ�
                                tbButtonInfoLocal.cbSize = Marshal.SizeOf(tbButtonInfoLocal)
                                tbButtonInfoLocal.dwMask = ToolbarButtonMask.TBIF_COMMAND Or ToolbarButtonMask.TBIF_LPARAM Or ToolbarButtonMask.TBIF_TEXT
                                tbButtonInfoLocal.pszText = pszSysTitle     'Tooltip�������ݐ�̈�
                                tbButtonInfoLocal.cchText = titleSize
                                '�}�X�N�ݒ蓙��Explorer�̃������֏�������
                                WriteProcessMemory( _
                                    hProc, _
                                    ptbSysInfo, _
                                    tbButtonInfoLocal, _
                                    Marshal.SizeOf(tbButtonInfoLocal), _
                                    dwBytes)
                                '�{�^���ڍ׏��擾
                                SendMessage( _
                                    toolWin, _
                                    Sm_Message.TB_GETBUTTONINFO, _
                                    tbButtonLocal2.idCommand, _
                                    ptbSysInfo)
                                '���L�������Ƀ{�^���ڍ׏���ǂݍ��ޗ̈�m��
                                Dim ptrInfo As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(tbButtonInfoLocal))
                                If ptrInfo.Equals(IntPtr.Zero) Then Return False '���L�������m�ێ��s
                                Try
                                    Marshal.StructureToPtr(tbButtonInfoLocal, ptrInfo, True)    '���L������������
                                    'Explorer���̃����������L�������ɓǂݍ���
                                    ReadProcessMemory( _
                                        hProc, _
                                        ptbSysInfo, _
                                        ptrInfo, _
                                        Marshal.SizeOf(tbButtonInfoLocal), _
                                        dwBytes)
                                    '���L�������̓��e���\���̂ɕϊ�
                                    tbButtonInfoLocal2 = DirectCast( _
                                                            Marshal.PtrToStructure( _
                                                                ptrInfo, _
                                                                GetType(TBBUTTONINFO)), _
                                                            TBBUTTONINFO)
                                Finally
                                    Marshal.FreeCoTaskMem(ptrInfo)  '���L���������
                                End Try
                                'Tooltip�̓��e��Explorer���̃��������狤�L�������֓Ǎ�
                                ReadProcessMemory(hProc, pszSysTitle, pszTitle, titleSize, dwBytes)
                                '���[�J���ϐ��֕ϊ�
                                title = Marshal.PtrToStringAnsi(pszTitle, titleSize)

                                'Tooltip���w�蕶������܂�ł���΃N���b�N
                                If title.Contains(tooltip) Then
                                    'PostMessage�ŃN���b�N�𑗂邽�߂ɁA�{�^���ڍ׏���lParam�Ń|�C���g����Ă���TRAYNOTIFY��񂪕K�v
                                    Dim tNotify As New TRAYNOTIFY
                                    Dim tNotify2 As TRAYNOTIFY
                                    '���L�������m��
                                    Dim ptNotify As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(tNotify))
                                    If ptNotify.Equals(IntPtr.Zero) Then Return False '�������m�ێ��s
                                    Try
                                        Marshal.StructureToPtr(tNotify, ptNotify, True) '������
                                        'lParam�̃�������Ǎ�
                                        ReadProcessMemory( _
                                            hProc, _
                                            tbButtonInfoLocal2.lParam, _
                                            ptNotify, _
                                            Marshal.SizeOf(tNotify), _
                                            dwBytes)
                                        '�\���̂֕ϊ�
                                        tNotify2 = DirectCast( _
                                                        Marshal.PtrToStructure( _
                                                            ptNotify, _
                                                            GetType(TRAYNOTIFY)), _
                                                        TRAYNOTIFY)
                                    Finally
                                        Marshal.FreeCoTaskMem(ptNotify) '���L���������
                                    End Try
                                    '�N���b�N���邽�߂ɂ͒ʒm�̈悪�A�N�e�B�u�łȂ���΂Ȃ�Ȃ�
                                    SetForegroundWindow(tNotify2.hWnd)
                                    '���N���b�N
                                    PostMessage(tNotify2.hWnd, tNotify2.uCallbackMessage, tNotify2.uID, PM_Message.WM_LBUTTONDOWN)
                                    PostMessage(tNotify2.hWnd, tNotify2.uCallbackMessage, tNotify2.uID, PM_Message.WM_LBUTTONUP)
                                    Return True
                                End If
                            Next
                            Return False    '�Y���Ȃ�
                        Finally
                            VirtualFreeEx(hProc, pszSysTitle, titleSize, MemoryFreeTypes.Release)   '���������
                        End Try
                    Finally
                        Marshal.FreeCoTaskMem(pszTitle)     '���L���������
                    End Try
                Finally
                    VirtualFreeEx(hProc, ptbSysInfo, Marshal.SizeOf(tbButtonInfoLocal), MemoryFreeTypes.Release)    '���������
                End Try
            Finally
                VirtualFreeEx(hProc, ptbSysButton, Marshal.SizeOf(tbButtonLocal), MemoryFreeTypes.Release)      '���������
            End Try
        Finally
            CloseHandle(hProc)  'Explorer�̃v���Z�X����
        End Try
    End Function
#End Region

    '��ʂ��u�����N���邽�߂�Win32API�B�N������10�y�[�W�ǂݎ�育�ƂɌp���m�F���b�Z�[�W��\������ۂ̒ʒm�����p
    <DllImport("user32.dll")> _
    Public Function FlashWindow( _
        ByVal hwnd As Integer, _
        ByVal bInvert As Integer) As Integer
    End Function

    <DllImport("user32.dll")> _
    Public Function ValidateRect( _
        ByVal hwnd As IntPtr, _
        ByVal rect As IntPtr) As Boolean
    End Function

#Region "�X�N���[���Z�[�o�[�N����������"
    <DllImport("user32", CharSet:=CharSet.Auto)> _
    Private Function SystemParametersInfo( _
                ByVal intAction As Integer, _
                ByVal intParam As Integer, _
                ByRef bParam As Boolean, _
                ByVal intWinIniFlag As Integer) As Integer
        ' returns non-zero value if function succeeds
    End Function
    '�X�N���[���Z�[�o�[���N���������擾����萔
    Private Const SPI_GETSCREENSAVERRUNNING As Integer = &H61

    Public Function IsScreenSaverRunning() As Boolean
        Dim ret As Integer = 0
        Dim isRunning As Boolean = False

        ret = SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, isRunning, 0)
        Return isRunning
    End Function
#End Region
End Module
