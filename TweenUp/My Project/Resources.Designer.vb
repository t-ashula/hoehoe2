﻿'------------------------------------------------------------------------------
' <auto-generated>
'     このコードはツールによって生成されました。
'     ランタイム バージョン:4.0.30319.235
'
'     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
'     コードが再生成されるときに損失したりします。
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'このクラスは StronglyTypedResourceBuilder クラスが ResGen
    'または Visual Studio のようなツールを使用して自動生成されました。
    'メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    'ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    '''<summary>
    '''  ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Friend Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '''<summary>
        '''  このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("TweenUp.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  厳密に型指定されたこのリソース クラスを使用して、すべての検索リソースに対し、
        '''  現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Exception was occured in updateing proccess. Error log is TweenUp.log. に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property ExceptionMessage() As String
            Get
                Return ResourceManager.GetString("ExceptionMessage", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Update Error に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property ExceptionTitle() As String
            Get
                Return ResourceManager.GetString("ExceptionTitle", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Tween.XmlSerializers.dll に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property FilenameDll() As String
            Get
                Return ResourceManager.GetString("FilenameDll", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  TweenNew.XmlSerializers.dll に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property FilenameDllNew() As String
            Get
                Return ResourceManager.GetString("FilenameDllNew", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  TweenNew.exe に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property FilenameNew() As String
            Get
                Return ResourceManager.GetString("FilenameNew", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Tween.resources.dll に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property FilenameResourceDll() As String
            Get
                Return ResourceManager.GetString("FilenameResourceDll", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Tween.resourcesNew.dll に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property FilenameResourceNew() As String
            Get
                Return ResourceManager.GetString("FilenameResourceNew", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Tween.exe に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property FilenameTweenExe() As String
            Get
                Return ResourceManager.GetString("FilenameTweenExe", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Tween Update に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property FormTitle() As String
            Get
                Return ResourceManager.GetString("FormTitle", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Please wait for a while... に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property PleaseWait() As String
            Get
                Return ResourceManager.GetString("PleaseWait", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Backup setting files... に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property ProgressBackup() As String
            Get
                Return ResourceManager.GetString("ProgressBackup", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Copying new files... に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property ProgressCopying() As String
            Get
                Return ResourceManager.GetString("ProgressCopying", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Attempting force termination Tween instances... に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property ProgressDetectMultipleInstance() As String
            Get
                Return ResourceManager.GetString("ProgressDetectMultipleInstance", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Detected Tween termination... に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property ProgressDetectTweenExit() As String
            Get
                Return ResourceManager.GetString("ProgressDetectTweenExit", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Executing new Tween... に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property ProgressExecuteTween() As String
            Get
                Return ResourceManager.GetString("ProgressExecuteTween", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Attempting force termination Tween instance... に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property ProgressProcessKill() As String
            Get
                Return ResourceManager.GetString("ProgressProcessKill", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Waiting Tween termination... に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property ProgressWaitForTweenExit() As String
            Get
                Return ResourceManager.GetString("ProgressWaitForTweenExit", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Failed to terminate Tween. Please try kill Tween process in TaskManager, or Reboot this computer. に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property TimeOutException() As String
            Get
                Return ResourceManager.GetString("TimeOutException", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Updating Tween... に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property TweenUpdating() As String
            Get
                Return ResourceManager.GetString("TweenUpdating", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Tween に類似しているローカライズされた文字列を検索します。
        '''</summary>
        Friend ReadOnly Property WaitProcessName() As String
            Get
                Return ResourceManager.GetString("WaitProcessName", resourceCulture)
            End Get
        End Property
    End Module
End Namespace
