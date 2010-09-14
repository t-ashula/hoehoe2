Public Class ImageListViewItem
    Inherits ListViewItem

    Private img As Image = Nothing

    Public Sub New(ByVal items() As String, ByVal imageKey As String)

    End Sub

    Public Sub New(ByVal items() As String, ByVal imageDictionary As ImageCacheDictionary, ByVal imageKey As String)
        MyBase.New(items, ImageKey)

        Dim dummy As Image = imageDictionary.Item(imageKey, Sub(getImg)
                                                                Dim bmp As New Bitmap(getImg.Width, getImg.Height)
                                                                Using g As Graphics = Graphics.FromImage(bmp)
                                                                    g.InterpolationMode = Drawing2D.InterpolationMode.High
                                                                    g.DrawImage(getImg, New Rectangle(0, 0, bmp.Width, bmp.Height))
                                                                    Me.img = bmp

                                                                    Me.ListView.Invoke(Sub()
                                                                                           Me.ListView.RedrawItems(Me.Index, Me.Index, False)
                                                                                       End Sub)
                                                                End Using
                                                            End Sub)
    End Sub

    Public ReadOnly Property Image As Image
        Get
            Return Me.img
        End Get
    End Property

    Protected Overrides Sub Finalize()
        If Me.img IsNot Nothing Then
            Me.img.Dispose()
            Me.img = Nothing
        End If
        MyBase.Finalize()
    End Sub
End Class
