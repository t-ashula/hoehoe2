Public Class ListElement
    Public Id As Long = 0
    Public Name As String = ""
    Public Description As String = ""
    Public Slug As String = ""
    Public IsPublic As Boolean = True
    Public SubscriberCount As Integer = 0   '�w�ǎҐ�
    Public MemberCount As Integer = 0   '���X�g�����o��
    Public UserId As Long = 0
    Public Username As String = ""
    Public Nickname As String = ""

    Public Overrides Function ToString() As String
        Return "@" + Username + "/" + Name
    End Function
End Class
