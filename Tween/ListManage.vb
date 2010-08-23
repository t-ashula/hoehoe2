﻿Imports System.ComponentModel

Public Class ListManage
    Private tw As Twitter

    Public Sub New(ByVal tw As Twitter)
        Me.InitializeComponent()

        Me.tw = tw
    End Sub


    Private Sub ListManage_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.UserList_SelectedIndexChanged(Nothing, EventArgs.Empty)
        If TabInformations.GetInstance().SubscribableLists.Count = 0 Then Me.RefreshLists()
        For Each listItem As ListElement In TabInformations.GetInstance().SubscribableLists.FindAll(Function(i) i.Username = Me.tw.Username)
            Me.ListsList.Items.Add(listItem)
        Next
        If Me.ListsList.Items.Count > 0 Then
            Me.ListsList.SelectedIndex = 0
        End If
        Me.ListsList.Focus()
    End Sub

    Private Sub ListsList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListsList.SelectedIndexChanged
        If Me.ListsList.SelectedItem Is Nothing Then Return

        Dim list As ListElement = CType(Me.ListsList.SelectedItem, ListElement)
        Me.UsernameTextBox.Text = list.Username
        Me.NameTextBox.Text = list.Name
        Me.PublicRadioButton.Checked = list.IsPublic
        Me.PrivateRadioButton.Checked = Not list.IsPublic
        Me.MemberCountTextBox.Text = list.MemberCount.ToString()
        Me.SubscriberCountTextBox.Text = list.SubscriberCount.ToString()
        Me.DescriptionText.Text = list.Description

        Me.UserList.Items.Clear()
        For Each user As UserInfo In list.Members
            Me.UserList.Items.Add(user)
        Next
    End Sub

    Private Sub EditCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditCheckBox.CheckedChanged
        Me.NameTextBox.ReadOnly = Not Me.EditCheckBox.Checked
        Me.PublicRadioButton.Enabled = Me.EditCheckBox.Checked
        Me.PrivateRadioButton.Enabled = Me.EditCheckBox.Checked
        Me.DescriptionText.ReadOnly = Not Me.EditCheckBox.Checked
        Me.ListsList.Enabled = Not Me.EditCheckBox.Checked

        Me.OKEditButton.Enabled = Me.EditCheckBox.Enabled
        Me.CancelEditButton.Enabled = Me.EditCheckBox.Enabled
        Me.EditCheckBox.AutoCheck = Not Me.EditCheckBox.Checked
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKEditButton.Click
        If Me.ListsList.SelectedItem Is Nothing Then Return
        Dim listItem As ListElement = DirectCast(Me.ListsList.SelectedItem, ListElement)

        If Me.NameTextBox.Text = "" Then
            MessageBox.Show("リスト名を入力して下さい。")
            Return
        End If

        listItem.Name = Me.NameTextBox.Text
        listItem.IsPublic = Me.PublicRadioButton.Checked
        listItem.Description = Me.DescriptionText.Text

        Dim rslt As String = listItem.Refresh()

        If rslt <> "" Then
            MessageBox.Show("通信エラー (" + rslt + ")")
            'Me.ListsList.Items.Clear()
            'Me.ListManage_Load(Nothing, EventArgs.Empty)
            Return
        End If

        Me.ListsList.Items.Clear()
        Me.ListManage_Load(Nothing, EventArgs.Empty)

        Me.EditCheckBox.AutoCheck = True
        Me.EditCheckBox.Checked = False

        Me.OKEditButton.Enabled = False
        Me.CancelEditButton.Enabled = False

        Me.ListsList.Refresh()
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelEditButton.Click
        Me.EditCheckBox.AutoCheck = True
        Me.EditCheckBox.Checked = False

        Me.OKEditButton.Enabled = False
        Me.CancelEditButton.Enabled = False

        For i As Integer = 0 To Me.ListsList.Items.Count - 1
            If TypeOf Me.ListsList.Items(i) Is NewListElement Then
                Me.ListsList.Items.RemoveAt(i)
            End If
        Next

        Me.ListsList_SelectedIndexChanged(Me.ListsList, EventArgs.Empty)
    End Sub

    Private Sub RefreshUsersButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshUsersButton.Click
        If Me.ListsList.SelectedItem Is Nothing Then Return
        Me.UserList.Items.Clear()
        Dim dlgt As New Action(Of ListElement)(Sub(lElement)
                                                   Me.Invoke(New Action(Of String)(AddressOf GetListMembersCallback), lElement.RefreshMembers())
                                               End Sub)
        dlgt.BeginInvoke(DirectCast(Me.ListsList.SelectedItem, ListElement), Nothing, Nothing)
    End Sub

    Private Sub GetMoreUsersButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetMoreUsersButton.Click
        If Me.ListsList.SelectedItem Is Nothing Then Return
        Dim dlgt As New Action(Of ListElement)(Sub(lElement)
                                                   Me.Invoke(New Action(Of String)(AddressOf GetListMembersCallback), lElement.GetMoreMembers())
                                               End Sub)
        dlgt.BeginInvoke(DirectCast(Me.ListsList.SelectedItem, ListElement), Nothing, Nothing)
    End Sub

    Private Sub GetListMembersCallback(ByVal result As String)
        If result = Me.ListsList.SelectedItem.ToString Then
            Me.ListsList_SelectedIndexChanged(Me.ListsList, EventArgs.Empty)
        Else
            MessageBox.Show(result)
        End If
    End Sub

    Private Sub DeleteUserButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteUserButton.Click
        If Me.ListsList.SelectedItem Is Nothing OrElse Me.UserList.SelectedItem Is Nothing Then
            Exit Sub
        End If

        Dim list As ListElement = CType(Me.ListsList.SelectedItem, ListElement)
        Dim user As UserInfo = CType(Me.UserList.SelectedItem, UserInfo)
        If MessageBox.Show(list.Name + "から" + user.ScreenName + "を削除します。", "Tween", MessageBoxButtons.OKCancel) = DialogResult.OK Then
            Dim rslt As String = Me.tw.RemoveUserToList(list.Id.ToString(), user.Id.ToString())

            If rslt <> "" Then
                MessageBox.Show("通信エラー (" + rslt + ")")
            End If

            Me.GetMoreUsersButton.PerformClick()

        End If
    End Sub

    Private Sub DeleteListButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteListButton.Click
        If Me.ListsList.SelectedItem Is Nothing Then Return
        Dim list As ListElement = CType(Me.ListsList.SelectedItem, ListElement)

        If MessageBox.Show(list.Name + "リストを削除します。", "Tween", MessageBoxButtons.OKCancel) = DialogResult.OK Then
            Dim rslt As String = ""

            rslt = Me.tw.DeleteList(list.Id.ToString())

            If rslt <> "" Then
                MessageBox.Show("通信エラー (" + rslt + ")")
                Return
            End If

            rslt = Me.tw.GetListsApi()

            If rslt <> "" Then
                MessageBox.Show("通信エラー (" + rslt + ")")
                Return
            End If

            Me.ListsList.Items.Clear()
            Me.ListManage_Load(Me, EventArgs.Empty)
        End If
    End Sub

    Private Sub AddListButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddListButton.Click
        Dim newList As New NewListElement(Me.tw)
        Me.ListsList.Items.Add(newList)
        Me.ListsList.SelectedItem = newList
        Me.EditCheckBox.Checked = True
        Me.EditCheckBox_CheckedChanged(Me.EditCheckBox, EventArgs.Empty)
    End Sub

    Private Sub UserList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserList.SelectedIndexChanged
        If UserList.SelectedItem Is Nothing Then
            If Me.UserIcon.Image IsNot Nothing Then
                Me.UserIcon.Image.Dispose()
                Me.UserIcon.Image = Nothing
            End If
            Me.UserLocation.Text = ""
            Me.UserWeb.Text = ""
            Me.UserFollowNum.Text = "0"
            Me.UserFollowerNum.Text = "0"
            Me.UserPostsNum.Text = "0"
            Me.UserProfile.Text = ""
            Me.UserTweet.Text = ""
            Me.DeleteUserButton.Enabled = False
        Else
            Dim user As UserInfo = DirectCast(Me.UserList.SelectedItem, UserInfo)
            Me.UserLocation.Text = user.Location
            Me.UserWeb.Text = user.Url
            Me.UserFollowNum.Text = user.FriendsCount.ToString("#,###,##0")
            Me.UserFollowerNum.Text = user.FollowersCount.ToString("#,###,##0")
            Me.UserPostsNum.Text = user.StatusesCount.ToString("#,###,##0")
            Me.UserProfile.Text = user.Description
            Me.UserTweet.Text = user.RecentPost
            Me.DeleteUserButton.Enabled = True

            Dim a As New Action(Of Uri)(Sub(url)
                                            Me.Invoke(New Action(Of Image)(AddressOf DisplayIcon), (New HttpVarious).GetImage(url))
                                        End Sub)
            a.BeginInvoke(user.ImageUrl, Nothing, Nothing)
        End If
    End Sub

    Private Sub DisplayIcon(ByVal img As Image)
        If img Is Nothing OrElse Me.UserList.SelectedItem Is Nothing Then Exit Sub
        If DirectCast(Me.UserList.SelectedItem, UserInfo).ImageUrl.ToString = DirectCast(img.Tag, String) Then
            Me.UserIcon.Image = img
        End If
    End Sub
    Private Sub RefreshListsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshListsButton.Click
        Me.RefreshLists()
        Me.ListsList.Items.Clear()
        Me.ListManage_Load(Nothing, EventArgs.Empty)
    End Sub

    Private Sub RefreshLists()
        Using dlg As New FormInfo("Getting Lists...", AddressOf RefreshLists_Dowork)
            dlg.ShowDialog()
            If Not String.IsNullOrEmpty(DirectCast(dlg.Result, String)) Then
                MessageBox.Show("Failed to get lists. (" + DirectCast(dlg.Result, String) + ")")
                Exit Sub
            End If
        End Using
    End Sub

    Private Sub RefreshLists_Dowork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        e.Result = tw.GetListsApi()
    End Sub

    Private Sub UserWeb_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles UserWeb.LinkClicked
        If Me.Owner IsNot Nothing Then
            DirectCast(Me.Owner, TweenMain).OpenUriAsync(UserWeb.Text)
        End If
    End Sub

    Private Class NewListElement
        Inherits ListElement

        Private _isCreated As Boolean = False

        Public Sub New(ByVal tw As Twitter)
            Me._tw = tw
        End Sub

        Public Overrides Function Refresh() As String
            If Me.IsCreated Then
                Return MyBase.Refresh()
            Else
                Dim rslt As String = Me._tw.CreateListApi(Me.Name, Not Me.IsPublic, Me.Description)
                Me._isCreated = (rslt = "")
                Return rslt
            End If
        End Function

        Public ReadOnly Property IsCreated As Boolean
            Get
                Return Me._isCreated
            End Get
        End Property

        Public Overrides Function ToString() As String
            If IsCreated Then
                Return MyBase.ToString()
            Else
                Return "NewList"
            End If
        End Function
    End Class

End Class