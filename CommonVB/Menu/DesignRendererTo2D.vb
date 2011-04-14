Public Class DesignRendererTo2D

    Protected Overrides Function CreatePanelPart(ByVal nParent As MHGameWork.Game3DPlay.Panel) As MHGameWork.Game3DPlay.PanelPart
        Dim P As New RendererTo2D(nParent)

        PanelDesign.SetBasicInfo(P, Me)
        P.BackgroundColor = Me.BackColor

        Return P

    End Function

    Public Function GetRendererTo2D() As RendererTo2D
        Return CType(Me.PanelPart, RendererTo2D)
    End Function

End Class
