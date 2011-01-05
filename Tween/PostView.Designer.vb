<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PostView
    Inherits System.Windows.Forms.UserControl

    'UserControl はコンポーネント一覧をクリーンアップするために dispose をオーバーライドします。
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.UserPicture = New System.Windows.Forms.PictureBox()
        Me.NameLabel = New System.Windows.Forms.Label()
        Me.DateTimeLabel = New System.Windows.Forms.Label()
        Me.SourceLinkLabel = New System.Windows.Forms.LinkLabel()
        Me.PostBrowser = New Tween.PostBrowser()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.UserPicture, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 4
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 56.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.UserPicture, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.NameLabel, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.PostBrowser, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.DateTimeLabel, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.SourceLinkLabel, 3, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 17.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(366, 184)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'UserPicture
        '
        Me.UserPicture.BackColor = System.Drawing.Color.White
        Me.UserPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.UserPicture.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.UserPicture.Location = New System.Drawing.Point(3, 3)
        Me.UserPicture.Name = "UserPicture"
        Me.TableLayoutPanel1.SetRowSpan(Me.UserPicture, 2)
        Me.UserPicture.Size = New System.Drawing.Size(50, 50)
        Me.UserPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.UserPicture.TabIndex = 5
        Me.UserPicture.TabStop = False
        '
        'NameLabel
        '
        Me.NameLabel.AutoEllipsis = True
        Me.NameLabel.AutoSize = True
        Me.NameLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NameLabel.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Bold)
        Me.NameLabel.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.NameLabel.Location = New System.Drawing.Point(59, 3)
        Me.NameLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.NameLabel.Name = "NameLabel"
        Me.NameLabel.Size = New System.Drawing.Size(119, 14)
        Me.NameLabel.TabIndex = 0
        Me.NameLabel.Text = "LblName"
        Me.NameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.NameLabel.UseMnemonic = False
        '
        'DateTimeLabel
        '
        Me.DateTimeLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DateTimeLabel.AutoEllipsis = True
        Me.DateTimeLabel.AutoSize = True
        Me.DateTimeLabel.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.DateTimeLabel.Location = New System.Drawing.Point(260, 3)
        Me.DateTimeLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.DateTimeLabel.Name = "DateTimeLabel"
        Me.DateTimeLabel.Size = New System.Drawing.Size(38, 14)
        Me.DateTimeLabel.TabIndex = 1
        Me.DateTimeLabel.Text = "Label1"
        Me.DateTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'SourceLinkLabel
        '
        Me.SourceLinkLabel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SourceLinkLabel.AutoEllipsis = True
        Me.SourceLinkLabel.AutoSize = True
        Me.SourceLinkLabel.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.SourceLinkLabel.Location = New System.Drawing.Point(304, 3)
        Me.SourceLinkLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.SourceLinkLabel.MaximumSize = New System.Drawing.Size(130, 0)
        Me.SourceLinkLabel.Name = "SourceLinkLabel"
        Me.SourceLinkLabel.Size = New System.Drawing.Size(59, 14)
        Me.SourceLinkLabel.TabIndex = 7
        Me.SourceLinkLabel.TabStop = True
        Me.SourceLinkLabel.Text = "LinkLabel1"
        Me.SourceLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'PostBrowser
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.PostBrowser, 3)
        Me.PostBrowser.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PostBrowser.isMonospace = False
        Me.PostBrowser.Location = New System.Drawing.Point(59, 20)
        Me.PostBrowser.MinimumSize = New System.Drawing.Size(20, 20)
        Me.PostBrowser.Name = "PostBrowser"
        Me.PostBrowser.Post = Nothing
        Me.PostBrowser.Size = New System.Drawing.Size(304, 161)
        Me.PostBrowser.TabIndex = 6
        Me.PostBrowser.TabStop = False
        '
        'PostView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "PostView"
        Me.Size = New System.Drawing.Size(366, 184)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        CType(Me.UserPicture, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents UserPicture As System.Windows.Forms.PictureBox
    Friend WithEvents NameLabel As System.Windows.Forms.Label
    Friend WithEvents PostBrowser As PostBrowser
    Friend WithEvents DateTimeLabel As System.Windows.Forms.Label
    Friend WithEvents SourceLinkLabel As System.Windows.Forms.LinkLabel

End Class
