Public Class ListElement
    Public Id As Long = 0
    Public Name As String = ""
    Public Description As String = ""
    Public Slug As String = ""
    Public IsPublic As Boolean = True
    Public SubscriberCount As Integer = 0   'çwì«é“êî
    Public MemberCount As Integer = 0   'ÉäÉXÉgÉÅÉìÉoêî
    Public UserId As Long = 0
    Public Username As String = ""
    Public Nickname As String = ""

    Public Sub New()

    End Sub

    Public Sub New(ByVal xmlNode As Xml.XmlNode)
        Me.Description = xmlNode.Item("description").InnerText
        Me.Id = Long.Parse(xmlNode.Item("id").InnerText)
        Me.IsPublic = (xmlNode.Item("mode").InnerText = "public")
        Me.MemberCount = Integer.Parse(xmlNode.Item("member_count").InnerText)
        Me.Name = xmlNode.Item("name").InnerText
        Me.SubscriberCount = Integer.Parse(xmlNode.Item("subscriber_count").InnerText)
        Me.Slug = xmlNode.Item("slug").InnerText
        Dim xUserEntry As Xml.XmlElement = CType(xmlNode.SelectSingleNode("./user"), Xml.XmlElement)
        Me.Nickname = xUserEntry.Item("name").InnerText
        Me.Username = xUserEntry.Item("screen_name").InnerText
        Me.UserId = Long.Parse(xUserEntry.Item("id").InnerText)
    End Sub

    Public Overrides Function ToString() As String
        Return "@" + Username + "/" + Name
    End Function
End Class
