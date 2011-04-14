Public Class Lamp001
    Inherits Entity

    Shared Sub New()
        LightNumber = 1
    End Sub

    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)

        Me.setModel(New XModel(Me))
        Me.Model.ModelPad = Application.StartupPath & "\GameData\Models\DraakHoofd001.x"
        Me.Model.StartRootMatrix = Matrix.Scaling(0.01, 0.01, 0.01) * Matrix.RotationYawPitchRoll(0, -Math.PI / 2, 0)
        Me.setDXLightNumber(LightNumber)
        LightNumber += 1
    End Sub

    Public Shared LightNumber As Integer


    Private mDXLightNumber As Integer
    Public ReadOnly Property DXLightNumber() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mDXLightNumber
        End Get
    End Property
    Private Sub setDXLightNumber(ByVal value As Integer)
        Me.mDXLightNumber = value
    End Sub


    Private mModel As XModel
    Public ReadOnly Property Model() As XModel
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mModel
        End Get
    End Property
    Private Sub setModel(ByVal value As XModel)
        Me.mModel = value
    End Sub


    Public Overrides Property Enabled() As Boolean
        Get
            Return MyBase.Enabled
        End Get
        Set(ByVal value As Boolean)
            MyBase.Enabled = value
        End Set
    End Property


    Public Sub UpdateLightPositie()
        If Me.Enabled And Me.HoofdObj.DevContainer.DX IsNot Nothing Then
            With Me.HoofdObj.DevContainer.DX.Device.Lights(Me.DXLightNumber)
                .Position = Me.Positie
            End With
        End If

    End Sub

    Private Sub Lamp001_Process(ByVal ev As MHGameWork.Game3DPlay.ProcessEvent) Handles Me.Process

        'Me.HoofdObj.DevContainer.DX.Device.RenderState.FillMode = FillMode.WireFrame
        'Me.HoofdObj.DevContainer.DX.Device.RenderState.Lighting = True
        If Me.Enabled <> Me.HoofdObj.DevContainer.DX.Device.Lights(Me.DXLightNumber).Enabled Then
            With Me.HoofdObj.DevContainer.DX.Device.Lights(Me.DXLightNumber)
                .Enabled = Me.Enabled
                If Me.Enabled Then
                    .Type = LightType.Point
                    '.Ambient = Color.White
                    .Diffuse = Color.Green
                    '.Specular = Color.Yellow
                    .Range = 100
                    .Attenuation0 = 0.02
                    Me.UpdateLightPositie()
                End If

            End With
        End If

        Me.HoofdObj.DevContainer.DX.Device.Lights(Me.DXLightNumber).Update()
    End Sub


    Public Overrides Sub OnPositieVeranderd()
        MyBase.OnPositieVeranderd()
        Me.Model.Positie = Me.Positie
        Me.UpdateLightPositie()
    End Sub
End Class
