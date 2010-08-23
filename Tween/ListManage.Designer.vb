<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ListManage
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ListsList = New System.Windows.Forms.ListBox()
        Me.DescriptionText = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.UserList = New System.Windows.Forms.ListBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.AddListButton = New System.Windows.Forms.Button()
        Me.DeleteListButton = New System.Windows.Forms.Button()
        Me.GetMoreUsersButton = New System.Windows.Forms.Button()
        Me.DeleteUserButton = New System.Windows.Forms.Button()
        Me.NameTextBox = New System.Windows.Forms.TextBox()
        Me.UsernameTextBox = New System.Windows.Forms.TextBox()
        Me.MemberCountTextBox = New System.Windows.Forms.TextBox()
        Me.SubscriberCountTextBox = New System.Windows.Forms.TextBox()
        Me.EditCheckBox = New System.Windows.Forms.CheckBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.PrivateRadioButton = New System.Windows.Forms.RadioButton()
        Me.PublicRadioButton = New System.Windows.Forms.RadioButton()
        Me.OKEditButton = New System.Windows.Forms.Button()
        Me.CancelEditButton = New System.Windows.Forms.Button()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.RefreshUsersButton = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'ListsList
        '
        Me.ListsList.DisplayMember = "Name"
        Me.ListsList.FormattingEnabled = True
        Me.ListsList.ItemHeight = 12
        Me.ListsList.Location = New System.Drawing.Point(12, 12)
        Me.ListsList.Name = "ListsList"
        Me.ListsList.Size = New System.Drawing.Size(215, 196)
        Me.ListsList.TabIndex = 17
        '
        'DescriptionText
        '
        Me.DescriptionText.Location = New System.Drawing.Point(59, 169)
        Me.DescriptionText.Multiline = True
        Me.DescriptionText.Name = "DescriptionText"
        Me.DescriptionText.ReadOnly = True
        Me.DescriptionText.Size = New System.Drawing.Size(146, 56)
        Me.DescriptionText.TabIndex = 29
        Me.DescriptionText.Text = "Description"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label12.Location = New System.Drawing.Point(6, 147)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(53, 12)
        Me.Label12.TabIndex = 27
        Me.Label12.Text = "購読者数"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label10.Location = New System.Drawing.Point(6, 122)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(53, 12)
        Me.Label10.TabIndex = 25
        Me.Label10.Text = "登録者数"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label6.Location = New System.Drawing.Point(6, 172)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(29, 12)
        Me.Label6.TabIndex = 22
        Me.Label6.Text = "説明"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(6, 46)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(41, 12)
        Me.Label4.TabIndex = 20
        Me.Label4.Text = "リスト名"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(6, 21)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 12)
        Me.Label1.TabIndex = 18
        Me.Label1.Text = "作成者"
        '
        'UserList
        '
        Me.UserList.FormattingEnabled = True
        Me.UserList.HorizontalScrollbar = True
        Me.UserList.ItemHeight = 12
        Me.UserList.Location = New System.Drawing.Point(233, 12)
        Me.UserList.Name = "UserList"
        Me.UserList.Size = New System.Drawing.Size(224, 472)
        Me.UserList.TabIndex = 30
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label19)
        Me.GroupBox1.Controls.Add(Me.Label20)
        Me.GroupBox1.Controls.Add(Me.DeleteUserButton)
        Me.GroupBox1.Controls.Add(Me.Label18)
        Me.GroupBox1.Controls.Add(Me.Label17)
        Me.GroupBox1.Controls.Add(Me.Label16)
        Me.GroupBox1.Controls.Add(Me.Label15)
        Me.GroupBox1.Controls.Add(Me.Label14)
        Me.GroupBox1.Controls.Add(Me.Label13)
        Me.GroupBox1.Controls.Add(Me.Label11)
        Me.GroupBox1.Controls.Add(Me.Label9)
        Me.GroupBox1.Controls.Add(Me.LinkLabel1)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Location = New System.Drawing.Point(463, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(208, 472)
        Me.GroupBox1.TabIndex = 31
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "選択したユーザー情報"
        '
        'AddListButton
        '
        Me.AddListButton.Location = New System.Drawing.Point(12, 214)
        Me.AddListButton.Name = "AddListButton"
        Me.AddListButton.Size = New System.Drawing.Size(53, 23)
        Me.AddListButton.TabIndex = 32
        Me.AddListButton.Text = "追加"
        Me.AddListButton.UseVisualStyleBackColor = True
        '
        'DeleteListButton
        '
        Me.DeleteListButton.Location = New System.Drawing.Point(174, 214)
        Me.DeleteListButton.Name = "DeleteListButton"
        Me.DeleteListButton.Size = New System.Drawing.Size(53, 23)
        Me.DeleteListButton.TabIndex = 34
        Me.DeleteListButton.Text = "削除"
        Me.DeleteListButton.UseVisualStyleBackColor = True
        '
        'GetMoreUsersButton
        '
        Me.GetMoreUsersButton.Location = New System.Drawing.Point(304, 487)
        Me.GetMoreUsersButton.Name = "GetMoreUsersButton"
        Me.GetMoreUsersButton.Size = New System.Drawing.Size(153, 23)
        Me.GetMoreUsersButton.TabIndex = 35
        Me.GetMoreUsersButton.Text = "さらに取得"
        Me.GetMoreUsersButton.UseVisualStyleBackColor = True
        '
        'DeleteUserButton
        '
        Me.DeleteUserButton.Location = New System.Drawing.Point(9, 443)
        Me.DeleteUserButton.Name = "DeleteUserButton"
        Me.DeleteUserButton.Size = New System.Drawing.Size(193, 23)
        Me.DeleteUserButton.TabIndex = 36
        Me.DeleteUserButton.Text = "リストから削除"
        Me.DeleteUserButton.UseVisualStyleBackColor = True
        '
        'NameTextBox
        '
        Me.NameTextBox.Location = New System.Drawing.Point(59, 43)
        Me.NameTextBox.Name = "NameTextBox"
        Me.NameTextBox.ReadOnly = True
        Me.NameTextBox.Size = New System.Drawing.Size(146, 19)
        Me.NameTextBox.TabIndex = 37
        '
        'UsernameTextBox
        '
        Me.UsernameTextBox.Location = New System.Drawing.Point(59, 18)
        Me.UsernameTextBox.Name = "UsernameTextBox"
        Me.UsernameTextBox.ReadOnly = True
        Me.UsernameTextBox.Size = New System.Drawing.Size(146, 19)
        Me.UsernameTextBox.TabIndex = 39
        '
        'MemberCountTextBox
        '
        Me.MemberCountTextBox.Location = New System.Drawing.Point(65, 119)
        Me.MemberCountTextBox.Name = "MemberCountTextBox"
        Me.MemberCountTextBox.ReadOnly = True
        Me.MemberCountTextBox.Size = New System.Drawing.Size(46, 19)
        Me.MemberCountTextBox.TabIndex = 40
        '
        'SubscriberCountTextBox
        '
        Me.SubscriberCountTextBox.Location = New System.Drawing.Point(65, 144)
        Me.SubscriberCountTextBox.Name = "SubscriberCountTextBox"
        Me.SubscriberCountTextBox.ReadOnly = True
        Me.SubscriberCountTextBox.Size = New System.Drawing.Size(46, 19)
        Me.SubscriberCountTextBox.TabIndex = 41
        '
        'EditCheckBox
        '
        Me.EditCheckBox.Appearance = System.Windows.Forms.Appearance.Button
        Me.EditCheckBox.Location = New System.Drawing.Point(71, 214)
        Me.EditCheckBox.Name = "EditCheckBox"
        Me.EditCheckBox.Size = New System.Drawing.Size(53, 23)
        Me.EditCheckBox.TabIndex = 42
        Me.EditCheckBox.Text = "編集"
        Me.EditCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.EditCheckBox.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.PrivateRadioButton)
        Me.GroupBox2.Controls.Add(Me.PublicRadioButton)
        Me.GroupBox2.Location = New System.Drawing.Point(27, 68)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(158, 45)
        Me.GroupBox2.TabIndex = 43
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "種別"
        '
        'PrivateRadioButton
        '
        Me.PrivateRadioButton.AutoSize = True
        Me.PrivateRadioButton.Enabled = False
        Me.PrivateRadioButton.Location = New System.Drawing.Point(14, 18)
        Me.PrivateRadioButton.Name = "PrivateRadioButton"
        Me.PrivateRadioButton.Size = New System.Drawing.Size(59, 16)
        Me.PrivateRadioButton.TabIndex = 46
        Me.PrivateRadioButton.TabStop = True
        Me.PrivateRadioButton.Text = "Private"
        Me.PrivateRadioButton.UseVisualStyleBackColor = True
        '
        'PublicRadioButton
        '
        Me.PublicRadioButton.AutoSize = True
        Me.PublicRadioButton.Enabled = False
        Me.PublicRadioButton.Location = New System.Drawing.Point(79, 18)
        Me.PublicRadioButton.Name = "PublicRadioButton"
        Me.PublicRadioButton.Size = New System.Drawing.Size(54, 16)
        Me.PublicRadioButton.TabIndex = 0
        Me.PublicRadioButton.TabStop = True
        Me.PublicRadioButton.Text = "Public"
        Me.PublicRadioButton.UseVisualStyleBackColor = True
        '
        'OKEditButton
        '
        Me.OKEditButton.Enabled = False
        Me.OKEditButton.Location = New System.Drawing.Point(25, 236)
        Me.OKEditButton.Name = "OKEditButton"
        Me.OKEditButton.Size = New System.Drawing.Size(75, 23)
        Me.OKEditButton.TabIndex = 44
        Me.OKEditButton.Text = "OK"
        Me.OKEditButton.UseVisualStyleBackColor = True
        '
        'CancelEditButton
        '
        Me.CancelEditButton.Enabled = False
        Me.CancelEditButton.Location = New System.Drawing.Point(120, 236)
        Me.CancelEditButton.Name = "CancelEditButton"
        Me.CancelEditButton.Size = New System.Drawing.Size(75, 23)
        Me.CancelEditButton.TabIndex = 45
        Me.CancelEditButton.Text = "Cancel"
        Me.CancelEditButton.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Label1)
        Me.GroupBox3.Controls.Add(Me.CancelEditButton)
        Me.GroupBox3.Controls.Add(Me.GroupBox2)
        Me.GroupBox3.Controls.Add(Me.OKEditButton)
        Me.GroupBox3.Controls.Add(Me.SubscriberCountTextBox)
        Me.GroupBox3.Controls.Add(Me.MemberCountTextBox)
        Me.GroupBox3.Controls.Add(Me.UsernameTextBox)
        Me.GroupBox3.Controls.Add(Me.NameTextBox)
        Me.GroupBox3.Controls.Add(Me.Label4)
        Me.GroupBox3.Controls.Add(Me.Label6)
        Me.GroupBox3.Controls.Add(Me.Label10)
        Me.GroupBox3.Controls.Add(Me.DescriptionText)
        Me.GroupBox3.Controls.Add(Me.Label12)
        Me.GroupBox3.Location = New System.Drawing.Point(12, 243)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(215, 268)
        Me.GroupBox3.TabIndex = 46
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "GroupBox3"
        '
        'RefreshUsersButton
        '
        Me.RefreshUsersButton.Location = New System.Drawing.Point(233, 488)
        Me.RefreshUsersButton.Name = "RefreshUsersButton"
        Me.RefreshUsersButton.Size = New System.Drawing.Size(65, 23)
        Me.RefreshUsersButton.TabIndex = 47
        Me.RefreshUsersButton.Text = "再取得"
        Me.RefreshUsersButton.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 19)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(45, 12)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "ユーザー"
        '
        'Label3
        '
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label3.Location = New System.Drawing.Point(76, 18)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(126, 14)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "Label3"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(9, 33)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(41, 12)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "現在地"
        '
        'Label7
        '
        Me.Label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label7.Location = New System.Drawing.Point(76, 32)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(126, 14)
        Me.Label7.TabIndex = 3
        Me.Label7.Text = "Label7"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(9, 47)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(26, 12)
        Me.Label8.TabIndex = 4
        Me.Label8.Text = "Web"
        '
        'LinkLabel1
        '
        Me.LinkLabel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LinkLabel1.Location = New System.Drawing.Point(76, 46)
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.Size = New System.Drawing.Size(126, 14)
        Me.LinkLabel1.TabIndex = 5
        Me.LinkLabel1.TabStop = True
        Me.LinkLabel1.Text = "LinkLabel1"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(9, 69)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(40, 12)
        Me.Label9.TabIndex = 6
        Me.Label9.Text = "フォロー"
        '
        'Label11
        '
        Me.Label11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label11.Location = New System.Drawing.Point(11, 85)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(50, 14)
        Me.Label11.TabIndex = 7
        Me.Label11.Text = "Label11"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(74, 69)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(49, 12)
        Me.Label13.TabIndex = 8
        Me.Label13.Text = "フォロワー"
        '
        'Label14
        '
        Me.Label14.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label14.Location = New System.Drawing.Point(76, 85)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(50, 14)
        Me.Label14.TabIndex = 9
        Me.Label14.Text = "Label14"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(147, 69)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(29, 12)
        Me.Label15.TabIndex = 10
        Me.Label15.Text = "リスト"
        '
        'Label16
        '
        Me.Label16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label16.Location = New System.Drawing.Point(149, 85)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(50, 14)
        Me.Label16.TabIndex = 11
        Me.Label16.Text = "Label16"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(9, 110)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(53, 12)
        Me.Label17.TabIndex = 12
        Me.Label17.Text = "自己紹介"
        '
        'Label18
        '
        Me.Label18.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label18.Location = New System.Drawing.Point(11, 122)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(191, 162)
        Me.Label18.TabIndex = 13
        Me.Label18.Text = "Label18"
        '
        'Label19
        '
        Me.Label19.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label19.Location = New System.Drawing.Point(11, 296)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(191, 141)
        Me.Label19.TabIndex = 15
        Me.Label19.Text = "Label19"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(9, 284)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(75, 12)
        Me.Label20.TabIndex = 14
        Me.Label20.Text = "最近のツイート"
        '
        'ListManage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(683, 522)
        Me.Controls.Add(Me.RefreshUsersButton)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GetMoreUsersButton)
        Me.Controls.Add(Me.DeleteListButton)
        Me.Controls.Add(Me.AddListButton)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.UserList)
        Me.Controls.Add(Me.ListsList)
        Me.Controls.Add(Me.EditCheckBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ListManage"
        Me.ShowInTaskbar = False
        Me.Text = "ListManage"
        Me.TopMost = True
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ListsList As System.Windows.Forms.ListBox
    Friend WithEvents DescriptionText As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents UserList As System.Windows.Forms.ListBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents AddListButton As System.Windows.Forms.Button
    Friend WithEvents DeleteListButton As System.Windows.Forms.Button
    Friend WithEvents GetMoreUsersButton As System.Windows.Forms.Button
    Friend WithEvents DeleteUserButton As System.Windows.Forms.Button
    Friend WithEvents NameTextBox As System.Windows.Forms.TextBox
    Friend WithEvents UsernameTextBox As System.Windows.Forms.TextBox
    Friend WithEvents MemberCountTextBox As System.Windows.Forms.TextBox
    Friend WithEvents SubscriberCountTextBox As System.Windows.Forms.TextBox
    Friend WithEvents EditCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents PrivateRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents PublicRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents OKEditButton As System.Windows.Forms.Button
    Friend WithEvents CancelEditButton As System.Windows.Forms.Button
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents RefreshUsersButton As System.Windows.Forms.Button
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
