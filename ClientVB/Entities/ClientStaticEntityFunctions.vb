Public Class ClientStaticEntityFunctions
    Inherits StaEntFunctions
    Public Sub New(ByVal nParent As StaticEntity)
        MyBase.New(nParent)
        Me.setXModel(New XModel(Me))

        Me.mStaticEntity = Me.StaticEntity
    End Sub

    Private WithEvents mStaticEntity As StaticEntity

    Private mXModel As XModel
    Public ReadOnly Property XModel() As XModel
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mXModel
        End Get
    End Property
    Private Sub setXModel(ByVal value As XModel)
        Me.mXModel = value
    End Sub
    Public Overrides Sub LaadModel(ByVal nMD As Common.ModelData)
        MyBase.LaadModel(nMD)
        If nMD Is Nothing Then
            Me.XModel.ModelPad = ""
        Else
            Me.XModel.ModelPad = nMD.FullPath
        End If
    End Sub


    Public Overrides Sub ReloadModel()
        MyBase.ReloadModel()
        Dim MD As ModelData = Me.StaticEntity.BaseMain.ModelManager.ModelData(Me.ModelID)

        If MD Is Nothing OrElse MD.Versie < Me.ModelVersie Then
            Me.LaadModel(Nothing)
            CLMain.ServerComm.GetModelAsync(Me.ModelID)
        Else
            Me.LaadModel(MD)
        End If

    End Sub

    'Public Overrides Sub Update(ByVal BR As Common.ByteReader)
    '    MyBase.Update(BR)


    '    Dim MD As ModelData = CLMain.ModelManager.ModelData(Me.ModelID)
    '    If MD Is Nothing OrElse MD.Versie < Me.LastStaticEntityDataPacket.ModelVersie Then
    '        CLMain.ServerComm.GetModelAsync(Me.ModelID)
    '    End If


    'End Sub

    Protected Overrides Sub DisposeObject()
        MyBase.DisposeObject()

    End Sub

    Private Sub mStaticEntity_PositieVeranderd(ByVal sender As Object, ByVal e As Common.PositieVeranderdEventArgs) Handles mStaticEntity.PositieVeranderd
        Me.XModel.Positie = Me.StaticEntity.Functions.Positie
    End Sub

    Private Sub mStaticEntity_RotatieVeranderd(ByVal sender As Object, ByVal e As Common.RotatieVeranderdEventArgs) Handles mStaticEntity.RotatieVeranderd
        Me.XModel.Rotatie = Me.StaticEntity.Functions.RotatieMatrix
    End Sub

    Private Sub mStaticEntity_ScaleVeranderd(ByVal sender As Object, ByVal e As Common.ScaleVeranderdEventArgs) Handles mStaticEntity.ScaleVeranderd
        Me.XModel.Scale = Me.StaticEntity.Functions.Scale
    End Sub
End Class
