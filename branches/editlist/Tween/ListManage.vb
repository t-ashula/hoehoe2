Public Class ListManage
    Private tw As Twitter

    Public Sub New(ByVal tw As Twitter)
        Me.InitializeComponent()

        Me.tw = tw
    End Sub


    Private Sub ListManage_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        For Each listItem As ListElement In TabInformations.GetInstance().SubscribableLists.FindAll(Function(i) i.Username = Me.tw.Username)
            Me.ListsList.Items.Add(listItem)
        Next
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

        Me.OKButton.Enabled = Me.EditCheckBox.Enabled
        Me.CancelButton.Enabled = Me.EditCheckBox.Enabled
        Me.EditCheckBox.AutoCheck = Not Me.EditCheckBox.Checked

        'If Not Me.EditCheckBox.Checked Then
        '    If Me.ListsList.SelectedItem Is Nothing Then Return
        '    Dim listItem As ListElement = DirectCast(Me.ListsList.SelectedItem, ListElement)
        '    Dim list_id As String = listItem.Id.ToString()
        '    Dim newListElement As New ListElement()

        '    Dim rslt As String = tw.EditList(list_id, Me.NameTextBox.Text, Me.PrivateRadioButton.Checked, Me.DescriptionText.Text, newListElement)

        '    If rslt <> "" Then
        '        MessageBox.Show("通信エラー (" + rslt + ")")
        '        Return
        '    End If

        '    Dim oldItem As ListElement = TabInformations.GetInstance().SubscribableLists.Find(Function(i) i.Id = listItem.Id)
        '    oldItem.Name = newListElement.Name
        '    oldItem.IsPublic = newListElement.IsPublic
        '    oldItem.Description = newListElement.Description

        '    Me.ListsList.Items.Clear()
        '    Me.ListManage_Load(Nothing, EventArgs.Empty)
        'End If
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        If Me.ListsList.SelectedItem Is Nothing Then Return
        Dim listItem As ListElement = DirectCast(Me.ListsList.SelectedItem, ListElement)
        Dim list_id As String = listItem.Id.ToString()
        Dim newListElement As New ListElement()

        Dim rslt As String = tw.EditList(list_id, Me.NameTextBox.Text, Me.PrivateRadioButton.Checked, Me.DescriptionText.Text, newListElement)

        If rslt <> "" Then
            MessageBox.Show("通信エラー (" + rslt + ")")
            Return
        End If

        Dim oldItem As ListElement = TabInformations.GetInstance().SubscribableLists.Find(Function(i) i.Id = listItem.Id)
        oldItem.Name = newListElement.Name
        oldItem.IsPublic = newListElement.IsPublic
        oldItem.Description = newListElement.Description

        Me.ListsList.Items.Clear()
        Me.ListManage_Load(Nothing, EventArgs.Empty)

        Me.EditCheckBox.AutoCheck = True
        Me.EditCheckBox.Checked = False

        Me.OKButton.Enabled = False
        Me.CancelButton.Enabled = False
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelButton.Click
        Me.EditCheckBox.AutoCheck = True
        Me.EditCheckBox.Checked = False

        Me.OKButton.Enabled = False
        Me.CancelButton.Enabled = False

        Me.ListsList_SelectedIndexChanged(Me.ListsList, EventArgs.Empty)
    End Sub

    Private Sub RefreshUsersButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshUsersButton.Click
        If Me.ListsList.SelectedItem Is Nothing Then Return
        CType(Me.ListsList.SelectedItem, ListElement).RefreshMembers()
        Me.ListsList_SelectedIndexChanged(Me.ListsList, EventArgs.Empty)
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

            Me.RefreshUsersButton.PerformClick()

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
End Class