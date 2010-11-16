Public Class ApiConsole

    Private _tw As Twitter


    Public Sub New(ByVal tw As Twitter)

        ' この呼び出しはデザイナーで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

        _tw = tw

    End Sub

    Private Sub ButtonApiCall_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonApiCall.Click
        Dim content As String = ""
        Dim url As String = ""

        If ComboBoxMethod.Text.Contains(".format") Then
            url = ComboBoxMethod.Text.Replace(".format", GroupBoxFormat.Tag.ToString)
        Else
            url = ComboBoxMethod.Text
        End If

        If Not String.IsNullOrEmpty(ComboBoxQuery.Text) Then
            url += "?" + ComboBoxQuery.Text
        End If


        Dim res As String = _tw.ApiCall(url, ComboBoxHttpMethod.Text, content)
        TextBoxResult.Text += ("----------------------" + url + vbCrLf + res + vbCrLf + content.Replace(vbLf, vbCrLf) + "----------------------" + vbCrLf)

    End Sub

    Private Sub RadioButtonFormat_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButtonXml.CheckedChanged, RadioButtonRss.CheckedChanged, RadioButtonJson.CheckedChanged, RadioButtonAtom.CheckedChanged
        GroupBoxFormat.Tag = DirectCast(sender, RadioButton).Tag
    End Sub

    Private Sub ApiConsole_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        Me.Hide()
        e.Cancel = True
    End Sub

    Private Sub ApiConsole_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ComboBoxHttpMethod.SelectedIndex = 0
    End Sub
End Class