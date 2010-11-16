<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ApiConsole
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
        Me.ComboBoxMethod = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ComboBoxQuery = New System.Windows.Forms.ComboBox()
        Me.ComboBoxHttpMethod = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.GroupBoxFormat = New System.Windows.Forms.GroupBox()
        Me.RadioButtonAtom = New System.Windows.Forms.RadioButton()
        Me.RadioButtonRss = New System.Windows.Forms.RadioButton()
        Me.RadioButtonJson = New System.Windows.Forms.RadioButton()
        Me.RadioButtonXml = New System.Windows.Forms.RadioButton()
        Me.ButtonApiCall = New System.Windows.Forms.Button()
        Me.TextBoxResult = New System.Windows.Forms.TextBox()
        Me.GroupBoxFormat.SuspendLayout()
        Me.SuspendLayout()
        '
        'ComboBoxMethod
        '
        Me.ComboBoxMethod.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboBoxMethod.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.ComboBoxMethod.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.ComboBoxMethod.FormattingEnabled = True
        Me.ComboBoxMethod.Items.AddRange(New Object() {"http://api.twitter.com/1/statuses/public_timeline.format", "http://api.twitter.com/1/statuses/home_timeline.format", "http://api.twitter.com/1/statuses/friends_timeline.format", "http://api.twitter.com/1/statuses/user_timeline.format", "http://api.twitter.com/1/statuses/replies.format", "http://api.twitter.com/1/statuses/mentions.format", "http://api.twitter.com/1/statuses/retweeted_by_me.format", "http://api.twitter.com/1/statuses/retweeted_to_me.format", "http://api.twitter.com/1/statuses/retweets_of_me.format", "http://api.twitter.com/1/statuses/show/id.format", "http://api.twitter.com/1/statuses/update.format", "http://api.twitter.com/1/statuses/destroy/id.format", "http://api.twitter.com/1/statuses/retweet/id.format", "http://api.twitter.com/1/statuses/retweets/id.format", "http://api.twitter.com/1/statuses/id/retweeted_by.format", "http://api.twitter.com/1/statuses/id/retweeted_by/ids.format", "http://api.twitter.com/1/statuses/friends.format", "http://api.twitter.com/1/statuses/followers.format", "http://api.twitter.com/1/users/show/id.format", "http://api.twitter.com/1/users/lookup.format", "http://api.twitter.com/1/users/search.format", "http://api.twitter.com/1/users/suggestions.format", "http://api.twitter.com/1/users/suggestions/category.format", "http://api.twitter.com/1/direct_messages.format", "http://api.twitter.com/1/direct_messages/sent.format", "http://api.twitter.com/1/direct_messages/new.format", "http://api.twitter.com/1/direct_messages/destroy/id.format", "http://api.twitter.com/1/friendships/create/id.format", "http://api.twitter.com/1/friendships/destroy/id.format", "http://api.twitter.com/1/friendships/exists.format", "http://api.twitter.com/1/friendships/show.format", "http://api.twitter.com/1/friendships/incoming.format", "http://api.twitter.com/1/friendships/outgoing.format", "http://api.twitter.com/1/friends/ids.format", "http://api.twitter.com/1/followers/ids.format", "http://api.twitter.com/1/account/verify_credentials.format", "http://api.twitter.com/1/account/end_session.format", "http://api.twitter.com/1/account/update_location.format", "http://api.twitter.com/1/account/update_delivery_device.format", "http://api.twitter.com/1/account/update_profile_colors.format", "http://api.twitter.com/1/account/update_profile_image.format", "http://api.twitter.com/1/account/update_profile_background_image.format", "http://api.twitter.com/1/account/rate_limit_status.format", "http://api.twitter.com/1/account/update_profile.format", "http://api.twitter.com/1/favorites.format", "http://api.twitter.com/1/favorites/create/id.format", "http://api.twitter.com/1/favorites/destroy/id.format", "http://api.twitter.com/1/notifications/follow/id.format", "http://api.twitter.com/1/notifications/leave/id.format", "http://api.twitter.com/1/blocks/create/id.format", "http://api.twitter.com/1/blocks/destroy/id.format", "http://api.twitter.com/1/blocks/exists/id.format", "http://api.twitter.com/1/blocks/blocking.format", "http://api.twitter.com/1/blocks/blocking/ids.format", "http://api.twitter.com/1/help/test.format", "http://api.twitter.com/1/report_spam.format", "http://api.twitter.com/1/user/lists.format", "http://api.twitter.com/1/user/lists/id.format", "http://api.twitter.com/1/user/lists.format", "http://api.twitter.com/1/user/lists/id.format", "http://api.twitter.com/1/user/lists/id.format", "http://api.twitter.com/1/user/lists/list_id/statuses.format", "http://api.twitter.com/1/user/lists/memberships.format", "http://api.twitter.com/1/user/lists/subscriptions.format", "http://api.twitter.com/1/user/list_id/members.format", "http://api.twitter.com/1/user/list_id/members.format", "http://api.twitter.com/1/user/list_id/members.format", "http://api.twitter.com/1/user/list_id/members/id.format", "http://api.twitter.com/1/user/list_id/subscribers.format", "http://api.twitter.com/1/user/list_id/subscribers.format", "http://api.twitter.com/1/user/list_id/subscribers.format", "http://api.twitter.com/1/user/list_id/subscribers/id.format", "http://api.twitter.com/oauth/request_token", "http://api.twitter.com/oauth/authorize", "http://api.twitter.com/oauth/authenticate", "http://api.twitter.com/oauth/access_token", "http://api.twitter.com/1/trends/available.format", "http://api.twitter.com/1/trends/woeid.format", "http://api.twitter.com/1/geo/nearby_places.json", "http://api.twitter.com/1/geo/reverse_geocode.json", "http://api.twitter.com/1/geo/id/ID.json", "http://api.twitter.com/1/saved_searches.format", "http://api.twitter.com/1/saved_searches/show/id.format", "http://api.twitter.com/1/saved_searches/create.format", "http://api.twitter.com/1/saved_searches/destroy.format"})
        Me.ComboBoxMethod.Location = New System.Drawing.Point(90, 22)
        Me.ComboBoxMethod.Name = "ComboBoxMethod"
        Me.ComboBoxMethod.Size = New System.Drawing.Size(443, 20)
        Me.ComboBoxMethod.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 25)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(59, 12)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "ApiMethod"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 55)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(35, 12)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Query"
        '
        'ComboBoxQuery
        '
        Me.ComboBoxQuery.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboBoxQuery.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.ComboBoxQuery.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.ComboBoxQuery.FormattingEnabled = True
        Me.ComboBoxQuery.Location = New System.Drawing.Point(90, 52)
        Me.ComboBoxQuery.Name = "ComboBoxQuery"
        Me.ComboBoxQuery.Size = New System.Drawing.Size(443, 20)
        Me.ComboBoxQuery.TabIndex = 3
        '
        'ComboBoxHttpMethod
        '
        Me.ComboBoxHttpMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxHttpMethod.FormattingEnabled = True
        Me.ComboBoxHttpMethod.Items.AddRange(New Object() {"GET", "POST", "DELETE"})
        Me.ComboBoxHttpMethod.Location = New System.Drawing.Point(90, 91)
        Me.ComboBoxHttpMethod.Name = "ComboBoxHttpMethod"
        Me.ComboBoxHttpMethod.Size = New System.Drawing.Size(101, 20)
        Me.ComboBoxHttpMethod.TabIndex = 4
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 94)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(64, 12)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "HttpMethod"
        '
        'GroupBoxFormat
        '
        Me.GroupBoxFormat.Controls.Add(Me.RadioButtonAtom)
        Me.GroupBoxFormat.Controls.Add(Me.RadioButtonRss)
        Me.GroupBoxFormat.Controls.Add(Me.RadioButtonJson)
        Me.GroupBoxFormat.Controls.Add(Me.RadioButtonXml)
        Me.GroupBoxFormat.Location = New System.Drawing.Point(197, 78)
        Me.GroupBoxFormat.Name = "GroupBoxFormat"
        Me.GroupBoxFormat.Size = New System.Drawing.Size(215, 44)
        Me.GroupBoxFormat.TabIndex = 6
        Me.GroupBoxFormat.TabStop = False
        Me.GroupBoxFormat.Tag = ".xml"
        Me.GroupBoxFormat.Text = "Format"
        '
        'RadioButtonAtom
        '
        Me.RadioButtonAtom.AutoSize = True
        Me.RadioButtonAtom.Location = New System.Drawing.Point(159, 14)
        Me.RadioButtonAtom.Name = "RadioButtonAtom"
        Me.RadioButtonAtom.Size = New System.Drawing.Size(50, 16)
        Me.RadioButtonAtom.TabIndex = 3
        Me.RadioButtonAtom.Tag = ".atom"
        Me.RadioButtonAtom.Text = "Atom"
        Me.RadioButtonAtom.UseVisualStyleBackColor = True
        '
        'RadioButtonRss
        '
        Me.RadioButtonRss.AutoSize = True
        Me.RadioButtonRss.Location = New System.Drawing.Point(107, 14)
        Me.RadioButtonRss.Name = "RadioButtonRss"
        Me.RadioButtonRss.Size = New System.Drawing.Size(45, 16)
        Me.RadioButtonRss.TabIndex = 2
        Me.RadioButtonRss.Tag = ".rss"
        Me.RadioButtonRss.Text = "RSS"
        Me.RadioButtonRss.UseVisualStyleBackColor = True
        '
        'RadioButtonJson
        '
        Me.RadioButtonJson.AutoSize = True
        Me.RadioButtonJson.Location = New System.Drawing.Point(56, 14)
        Me.RadioButtonJson.Name = "RadioButtonJson"
        Me.RadioButtonJson.Size = New System.Drawing.Size(44, 16)
        Me.RadioButtonJson.TabIndex = 1
        Me.RadioButtonJson.Tag = ".json"
        Me.RadioButtonJson.Text = "json"
        Me.RadioButtonJson.UseVisualStyleBackColor = True
        '
        'RadioButtonXml
        '
        Me.RadioButtonXml.AutoSize = True
        Me.RadioButtonXml.Checked = True
        Me.RadioButtonXml.Location = New System.Drawing.Point(7, 14)
        Me.RadioButtonXml.Name = "RadioButtonXml"
        Me.RadioButtonXml.Size = New System.Drawing.Size(42, 16)
        Me.RadioButtonXml.TabIndex = 0
        Me.RadioButtonXml.TabStop = True
        Me.RadioButtonXml.Tag = ".xml"
        Me.RadioButtonXml.Text = "Xml"
        Me.RadioButtonXml.UseVisualStyleBackColor = True
        '
        'ButtonApiCall
        '
        Me.ButtonApiCall.Location = New System.Drawing.Point(439, 91)
        Me.ButtonApiCall.Name = "ButtonApiCall"
        Me.ButtonApiCall.Size = New System.Drawing.Size(75, 23)
        Me.ButtonApiCall.TabIndex = 7
        Me.ButtonApiCall.Text = "ApiCall"
        Me.ButtonApiCall.UseVisualStyleBackColor = True
        '
        'TextBoxResult
        '
        Me.TextBoxResult.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxResult.Location = New System.Drawing.Point(14, 128)
        Me.TextBoxResult.Multiline = True
        Me.TextBoxResult.Name = "TextBoxResult"
        Me.TextBoxResult.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TextBoxResult.Size = New System.Drawing.Size(519, 153)
        Me.TextBoxResult.TabIndex = 8
        Me.TextBoxResult.WordWrap = False
        '
        'ApiConsole
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(545, 293)
        Me.Controls.Add(Me.TextBoxResult)
        Me.Controls.Add(Me.ButtonApiCall)
        Me.Controls.Add(Me.GroupBoxFormat)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.ComboBoxHttpMethod)
        Me.Controls.Add(Me.ComboBoxQuery)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ComboBoxMethod)
        Me.Name = "ApiConsole"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "ApiConsole"
        Me.GroupBoxFormat.ResumeLayout(False)
        Me.GroupBoxFormat.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ComboBoxMethod As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxQuery As System.Windows.Forms.ComboBox
    Friend WithEvents ComboBoxHttpMethod As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents GroupBoxFormat As System.Windows.Forms.GroupBox
    Friend WithEvents RadioButtonAtom As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButtonRss As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButtonJson As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButtonXml As System.Windows.Forms.RadioButton
    Friend WithEvents ButtonApiCall As System.Windows.Forms.Button
    Friend WithEvents TextBoxResult As System.Windows.Forms.TextBox
End Class
