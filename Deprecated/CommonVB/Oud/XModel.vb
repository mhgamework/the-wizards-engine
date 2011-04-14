'Public Class XModel
'    Inherits SpelObject3D

'    Public Sub New(ByVal nParent As SpelObject)
'        MyBase.New(nParent)

'        'Me.KiesModel()

'        'Me.mModel.ComputeBoundingVolume()
'        'Me.mSphereStraal = Me.mModel.SphereRadius



'        Me.Scaling = New Vector3(1, 1, 1)
'        Me.StartRootMatrix = Matrix.Identity
'        Me.Rotatie = Matrix.RotationYawPitchRoll(0, 0, 0)
'        'Me.UpdateRootMatrix()          'optioneel, zie scaling

'        'Me.ScheduleAction(AddressOf Me.KiesModel, 4000)
'    End Sub






'    Dim mModel As Artificial.Heart.AHStaticModel
'    Private Shared OFD As OpenFileDialog
'    Private mModelPad As String
'    Private mMesh As Direct3D.Mesh
'    Private mMeshMaterial As Direct3D.Material

'    'Private mRelativeSphereCenter As Vector3



'    Private mScaling As Vector3
'    Public Property Scaling() As Vector3
'        <System.Diagnostics.DebuggerStepThrough()> Get
'            Return Me.mScaling
'        End Get
'        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Vector3)
'            Me.mScaling = value
'            Me.UpdateRootMatrix()
'        End Set
'    End Property


'    'Public Overrides ReadOnly Property SphereCenter() As Microsoft.DirectX.Vector3
'    '    Get
'    '        Return MyBase.SphereCenter + Me.mRelativeSphereCenter
'    '    End Get
'    'End Property
'    Public Property ModelPad() As String
'        Get
'            Return Me.mModelPad
'        End Get
'        Set(ByVal value As String)

'            Me.mModelPad = value

'            If Me.HoofdObj.DeviceReady Then Me.CreateModel()

'            'Me.mSphereStraal = Me.mModel.SphereRadius
'            'Me.mRelativeSphereCenter = Me.mModel.SphereCenter

'            'Me.mMesh = Direct3D.Mesh.Sphere(Spel.GetInstance.DX.Device, Me.SphereStraal, 100, 100)
'            'Me.mMesh.ComputeNormals()
'            'Me.mMeshMaterial = New Direct3D.Material
'            'Me.mMeshMaterial.Ambient = Color.FromArgb(128, 255, 0, 255)
'            'Me.mMeshMaterial.Diffuse = Me.mMeshMaterial.Ambient
'        End Set
'    End Property

'    Public Overrides Sub OnDeviceReset(ByVal sender As Object, ByVal e As spelobject.deviceeventargs)
'        MyBase.OnDeviceReset(sender, e)
'        Me.CreateModel()
'        Me.UpdateRootMatrix()
'    End Sub

'    Public Sub CreateModel()
'        If System.IO.File.Exists(Me.ModelPad) = False Then
'            MsgBox("Kan " & Me.ModelPad & " niet vinden!")
'            Exit Sub
'        End If

'        If Me.mModel IsNot Nothing Then Me.mModel.Dispose()
'        Me.mModel = New AHStaticModel(Me.HoofdObj.DevContainer.DX, Me.mModelPad, True, Direct3D.MeshFlags.Managed)
'        Me.mModel.ComputeBoundingVolume()
'    End Sub

'    Public Sub KiesModel()
'        If OFD Is Nothing Then OFD = New OpenFileDialog
'        If Not (OFD.ShowDialog = DialogResult.OK) Then Exit Sub
'        If System.IO.File.Exists(OFD.FileName) = False Then Exit Sub

'        Me.ModelPad = OFD.FileName

'        'If Me.mModel Is Nothing Then
'        '    Me.mModel = New AHStaticModel(Spel.GetInstance.DX, Application.StartupPath & "\Data\Models\Bol001.x", True, Direct3D.MeshFlags.Managed)
'        'End If
'    End Sub




'    Public Sub UpdateRootMatrix()
'        If Me.mModel IsNot Nothing Then Me.mModel.RootMatrix = Me.StartRootMatrix * Matrix.Scaling(Me.Scaling) * Me.Rotatie * Matrix.Translation(Me.Positie)

'    End Sub

'    Protected Overrides Sub DisposeObject()
'        MyBase.DisposeObject()
'        If Me.mModel IsNot Nothing Then
'            Me.mModel.Dispose()
'            Me.mModel = Nothing
'        End If

'    End Sub




'    Private mRotatie As Matrix
'    Public Property Rotatie() As Matrix
'        <System.Diagnostics.DebuggerStepThrough()> Get
'            Return Me.mRotatie
'        End Get
'        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Matrix)
'            Me.mRotatie = value
'            Me.UpdateRootMatrix()
'        End Set
'    End Property





'    Private mPositie As Vector3
'    Public Property Positie() As Vector3
'        <System.Diagnostics.DebuggerStepThrough()> Get
'            Return Me.mPositie
'        End Get
'        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Vector3)
'            Me.mPositie = value
'            Me.UpdateRootMatrix()
'        End Set
'    End Property







'    Private mStartRootMatrix As Matrix
'    Public Property StartRootMatrix() As Matrix
'        <System.Diagnostics.DebuggerStepThrough()> Get
'            Return Me.mStartRootMatrix
'        End Get
'        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As Matrix)
'            Me.mStartRootMatrix = value
'        End Set
'    End Property








'    Private Sub XModel_Render3D(ByVal ev As MHGameWork.Game3DPlay.Render3DEvent) Handles Me.Render3D
'        If Me.mModel Is Nothing Then Exit Sub

'        Me.mModel.RenderDirect()
'        'If Spel.GetInstance.DX.Input.KeyPressed(DirectInput.Key.B) Then
'        '    Spel.GetInstance.DX.SetTexture(0, Nothing)
'        '    Spel.GetInstance.DX.SetMaterial(Me.mMeshMaterial)
'        '    Spel.GetInstance.DX.Device.Transform.World = Matrix.Translation(Me.SphereCenter)
'        '    Me.mMesh.DrawSubset(0)
'        'End If
'    End Sub
'End Class
