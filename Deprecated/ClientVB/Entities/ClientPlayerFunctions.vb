Public Class ClientPlayerFunctions
    Inherits PlayerFunctions

    Public Sub New(ByVal nParent As Player)
        MyBase.New(nParent)
        Me.setBol1(New Bol3D(Me))
        Me.setBol2(New Bol3D(Me))
        Me.Bol1.Scale = New Vector3(2, 2, 2)
        Me.Bol2.Scale = New Vector3(2, 2, 2)


        Me.setNaamText(New Text3D(Me))
        Me.NaamText.Size = New Vector2(400, 40)
        Me.NaamText.Text2D.FontHeight = 40
        Me.NaamText.Scale = New Vector2(4, 0.4)
        Me.NaamText.Text2D.TextAlign = DrawTextFormat.Center Or DrawTextFormat.VerticalCenter
        Me.NaamText.Text2D.FontColor = Color.Yellow
        Me.NaamText.BackgroundColor = Color.FromArgb(150, 0, 0, 0)

        Me.Player = nParent

        'Me.setXModel(New XModel(Me, Forms.Application.StartupPath & "\GameData\Models\WallTower14.X"))
        'Me.XModel.StartRootMatrix = Matrix.RotationYawPitchRoll(0, -Math.PI / 2, 0)


    End Sub


    'Private mXModel As XModel
    'Public ReadOnly Property XModel() As XModel
    '    <System.Diagnostics.DebuggerStepThrough()> Get
    '        Return Me.mXModel
    '    End Get
    'End Property
    'Private Sub setXModel(ByVal value As XModel)
    '    Me.mXModel = value
    'End Sub


    Private WithEvents Player As Player

    Public Overrides Sub Initialize()
        MyBase.Initialize()

        Me.Player_PositieVeranderd(Nothing, Nothing)
        Me.Player_RotatieVeranderd(Nothing, Nothing)
    End Sub

    Private mBol1 As Bol3D
    Public ReadOnly Property Bol1() As Bol3D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBol1
        End Get
    End Property
    Private Sub setBol1(ByVal value As Bol3D)
        Me.mBol1 = value
    End Sub


    Private mBol2 As Bol3D
    Public ReadOnly Property Bol2() As Bol3D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mBol2
        End Get
    End Property
    Private Sub setBol2(ByVal value As Bol3D)
        Me.mBol2 = value
    End Sub



    Private mDisplayName As String
    Public Overrides Property DisplayName() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mDisplayName
        End Get
        Set(ByVal value As String)
            Me.mDisplayName = value
            If Me.NaamText IsNot Nothing Then Me.NaamText.Text2D.Text = value
        End Set
    End Property



    Private mNaamText As Text3D
    Public ReadOnly Property NaamText() As Text3D
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mNaamText
        End Get
    End Property
    Private Sub setNaamText(ByVal value As Text3D)
        Me.mNaamText = value
        Me.NaamText.Text2D.Text = Me.DisplayName
    End Sub

    Private Sub Player_PositieVeranderd(ByVal sender As Object, ByVal e As Common.PositieVeranderdEventArgs) Handles Player.PositieVeranderd
        Me.Bol1.Positie = Me.Player.Functions.Positie + New Vector3(0, 0.5, 0)
        Me.Bol2.Positie = Me.Player.Functions.Positie - New Vector3(0, 0.5, 0)

        Me.NaamText.Positie = Me.Player.Functions.Positie + New Vector3(0, 4, 0)

        'Me.XModel.Positie = Me.Player.Functions.Positie
    End Sub

    Private Sub Player_RotatieVeranderd(ByVal sender As Object, ByVal e As Common.RotatieVeranderdEventArgs) Handles Player.RotatieVeranderd
        Me.Bol1.Rotatie = Me.Player.Functions.RotatieMatrix
        Me.Bol2.Positie = Me.Player.Functions.Positie
    End Sub
End Class
