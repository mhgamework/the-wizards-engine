Public Class Knop001
    Inherits Knop



    Public Sub New(ByVal nParent As Panel)
        MyBase.New(nParent, "Gamedata\Knop001")
        Me.Image2D.SourceRectangle = New Microsoft.Xna.Framework.Rectangle(0, 0, 294, 110)
        Me.Image2D.UseSourceRectangle = True

        'Me.Text2D.FontFilename = "Gamedata\ComicSansMS"
        Me.Text2D.FontFilename = "Content\ComicSansMS"
        'Me.Text2D.TextAlign = DrawTextFormat.VerticalCenter Or DrawTextFormat.Center



    End Sub



End Class
