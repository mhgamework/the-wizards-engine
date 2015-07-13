Public Class DesignTextList2D


    Protected Overrides Function CreatePanelPart(ByVal nParent As MHGameWork.Game3DPlay.Panel) As MHGameWork.Game3DPlay.PanelPart
        Dim P As New TextList2D(nParent)

        PanelDesign.SetBasicInfo(P, Me)
        P.BackgroundColor = Me.BackColor

        Return P

    End Function

    Public Function GetTextList2D() As TextList2D
        Return CType(Me.PanelPart, TextList2D)
    End Function

End Class
