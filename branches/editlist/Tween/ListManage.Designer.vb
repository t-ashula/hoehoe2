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
        Me.SubscriberCountLabel = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.MemberCountLabel = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.StatusLabel = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.NameLabel = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.UsernameLabel = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ユーザーの一覧 = New System.Windows.Forms.ListBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'ListsList
        '
        Me.ListsList.FormattingEnabled = True
        Me.ListsList.ItemHeight = 12
        Me.ListsList.Location = New System.Drawing.Point(12, 26)
        Me.ListsList.Name = "ListsList"
        Me.ListsList.Size = New System.Drawing.Size(215, 232)
        Me.ListsList.TabIndex = 17
        '
        'DescriptionText
        '
        Me.DescriptionText.Location = New System.Drawing.Point(28, 448)
        Me.DescriptionText.Multiline = True
        Me.DescriptionText.Name = "DescriptionText"
        Me.DescriptionText.ReadOnly = True
        Me.DescriptionText.Size = New System.Drawing.Size(174, 56)
        Me.DescriptionText.TabIndex = 29
        Me.DescriptionText.Text = "Description"
        '
        'SubscriberCountLabel
        '
        Me.SubscriberCountLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SubscriberCountLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.SubscriberCountLabel.Location = New System.Drawing.Point(28, 419)
        Me.SubscriberCountLabel.Name = "SubscriberCountLabel"
        Me.SubscriberCountLabel.Size = New System.Drawing.Size(46, 14)
        Me.SubscriberCountLabel.TabIndex = 28
        Me.SubscriberCountLabel.Text = "Label11"
        Me.SubscriberCountLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label12.Location = New System.Drawing.Point(12, 407)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(53, 12)
        Me.Label12.TabIndex = 27
        Me.Label12.Text = "購読者数"
        '
        'MemberCountLabel
        '
        Me.MemberCountLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.MemberCountLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.MemberCountLabel.Location = New System.Drawing.Point(28, 391)
        Me.MemberCountLabel.Name = "MemberCountLabel"
        Me.MemberCountLabel.Size = New System.Drawing.Size(46, 14)
        Me.MemberCountLabel.TabIndex = 26
        Me.MemberCountLabel.Text = "Label9"
        Me.MemberCountLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label10.Location = New System.Drawing.Point(12, 379)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(53, 12)
        Me.Label10.TabIndex = 25
        Me.Label10.Text = "登録者数"
        '
        'StatusLabel
        '
        Me.StatusLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.StatusLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.StatusLabel.Location = New System.Drawing.Point(28, 365)
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusLabel.Size = New System.Drawing.Size(174, 14)
        Me.StatusLabel.TabIndex = 24
        Me.StatusLabel.Text = "Label7"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label8.Location = New System.Drawing.Point(12, 353)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(29, 12)
        Me.Label8.TabIndex = 23
        Me.Label8.Text = "種別"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label6.Location = New System.Drawing.Point(12, 433)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(29, 12)
        Me.Label6.TabIndex = 22
        Me.Label6.Text = "説明"
        '
        'NameLabel
        '
        Me.NameLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.NameLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.NameLabel.Location = New System.Drawing.Point(28, 339)
        Me.NameLabel.Name = "NameLabel"
        Me.NameLabel.Size = New System.Drawing.Size(174, 14)
        Me.NameLabel.TabIndex = 21
        Me.NameLabel.Text = "Label3"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(12, 327)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(41, 12)
        Me.Label4.TabIndex = 20
        Me.Label4.Text = "リスト名"
        '
        'UsernameLabel
        '
        Me.UsernameLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.UsernameLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.UsernameLabel.Location = New System.Drawing.Point(28, 313)
        Me.UsernameLabel.Name = "UsernameLabel"
        Me.UsernameLabel.Size = New System.Drawing.Size(174, 14)
        Me.UsernameLabel.TabIndex = 19
        Me.UsernameLabel.Text = "Label2"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(12, 301)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 12)
        Me.Label1.TabIndex = 18
        Me.Label1.Text = "作成者"
        '
        'ユーザーの一覧
        '
        Me.ユーザーの一覧.FormattingEnabled = True
        Me.ユーザーの一覧.ItemHeight = 12
        Me.ユーザーの一覧.Location = New System.Drawing.Point(233, 26)
        Me.ユーザーの一覧.Name = "ユーザーの一覧"
        Me.ユーザーの一覧.Size = New System.Drawing.Size(224, 316)
        Me.ユーザーの一覧.TabIndex = 30
        '
        'GroupBox1
        '
        Me.GroupBox1.Location = New System.Drawing.Point(463, 26)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(200, 220)
        Me.GroupBox1.TabIndex = 31
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "選択したユーザー情報"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(12, 264)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(53, 23)
        Me.Button1.TabIndex = 32
        Me.Button1.Text = "追加"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(71, 264)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(53, 23)
        Me.Button2.TabIndex = 33
        Me.Button2.Text = "編集"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(174, 264)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(53, 23)
        Me.Button3.TabIndex = 34
        Me.Button3.Text = "削除"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Button4
        '
        Me.Button4.Location = New System.Drawing.Point(233, 348)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(133, 23)
        Me.Button4.TabIndex = 35
        Me.Button4.Text = "ユーザー一覧更新"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'Button5
        '
        Me.Button5.Location = New System.Drawing.Point(463, 252)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(114, 23)
        Me.Button5.TabIndex = 36
        Me.Button5.Text = "リストから削除"
        Me.Button5.UseVisualStyleBackColor = True
        '
        'ListManage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(683, 516)
        Me.Controls.Add(Me.Button5)
        Me.Controls.Add(Me.Button4)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.ユーザーの一覧)
        Me.Controls.Add(Me.DescriptionText)
        Me.Controls.Add(Me.SubscriberCountLabel)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.MemberCountLabel)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.StatusLabel)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.NameLabel)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.UsernameLabel)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ListsList)
        Me.Name = "ListManage"
        Me.Text = "ListManage"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ListsList As System.Windows.Forms.ListBox
    Friend WithEvents DescriptionText As System.Windows.Forms.TextBox
    Friend WithEvents SubscriberCountLabel As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents MemberCountLabel As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents StatusLabel As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents NameLabel As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents UsernameLabel As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ユーザーの一覧 As System.Windows.Forms.ListBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents Button5 As System.Windows.Forms.Button
End Class
