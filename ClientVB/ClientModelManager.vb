Public Class ClientModelManager
    Inherits ModelManager
    Public Sub New(ByVal nParent As SpelObject)
        MyBase.New(nParent)
        Me.setModels(New Dictionary(Of Integer, ModelData))
        Me.ReadModelFile()
    End Sub

    Public Sub ReadModelFile()
        Dim FileName As String = Forms.Application.StartupPath & "\Models.TWF"
        If IO.File.Exists(FileName) = False Then Exit Sub
        Dim FS As IO.FileStream = Nothing
        Dim BR As ByteReader = Nothing
        Dim MD As ModelData

        Try
            FS = New IO.FileStream(FileName, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Delete)
            BR = New ByteReader(FS)


            For I As Integer = 0 To BR.ReadInt32 - 1
                md = Common.ModelData.FromNetworkBytes(BR)
                Me.Models.Add(MD.ModelID, MD)


            Next
        Finally
            If BR IsNot Nothing Then BR.Close()
            If FS IsNot Nothing Then FS.Close()
        End Try
    End Sub

    Public Sub SaveModelFile()
        Dim FS As IO.FileStream = Nothing
        Dim BW As ByteWriter = Nothing
        Try
            FS = New IO.FileStream(Forms.Application.StartupPath & "\Models.TWF", IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.Delete)
            BW = New ByteWriter(FS)


            BW.Write(Me.Models.Count)
            For Each M As ModelData In Me.Models.Values
                BW.Write(M)

            Next
        Finally
            If BW IsNot Nothing Then BW.Close()
            If FS IsNot Nothing Then FS.Close()
        End Try
    End Sub


    Private mModels As Dictionary(Of Integer, ModelData)
    Protected ReadOnly Property Models() As Dictionary(Of Integer, ModelData)
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mModels
        End Get
    End Property
    Private Sub setModels(ByVal value As Dictionary(Of Integer, ModelData))
        Me.mModels = value
    End Sub

    Public Overrides ReadOnly Property ModelData(ByVal nModelID As Integer) As Common.ModelData
        Get
            If Me.Models.ContainsKey(nModelID) = False Then Return Nothing

            Return Me.Models(nModelID)
        End Get
    End Property


    Protected Overrides Sub DisposeObject()
        Me.SaveModelFile()
        MyBase.DisposeObject()

    End Sub

    Public Sub UpdateModel(ByVal nMD As ModelData)
        If Me.Models.ContainsKey(nMD.ModelID) Then
            If Me.Models(nMD.ModelID).Versie < nMD.Versie Then
                Me.Models(nMD.ModelID) = nMD
                Me.ReloadStaticEntities()
            End If
        Else
            Me.Models.Add(nMD.ModelID, nMD)
            Me.ReloadStaticEntities()
        End If
    End Sub



    Protected Overrides ReadOnly Property StaticEntityFunctionsEnumerable() As System.Collections.IEnumerable
        Get
            Return Me.StaticEntityFunctions
        End Get
    End Property

    Private mStaticEntityFunctions As New List(Of ClientStaticEntityFunctions)
    Private ReadOnly Property StaticEntityFunctions() As List(Of ClientStaticEntityFunctions)
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mStaticEntityFunctions
        End Get
    End Property
    Private Sub setStaticEntityFunctions(ByVal value As List(Of ClientStaticEntityFunctions))
        Me.mStaticEntityFunctions = value
    End Sub

    Public Overrides Sub AddStaticEntityFunctions(ByVal nSFunc As Common.StaEntFunctions)
        Me.StaticEntityFunctions.Add(DirectCast(nSFunc, ClientStaticEntityFunctions))

    End Sub

    Public Overrides Sub RemoveStaticEntityFunctions(ByVal nSFunc As Common.StaEntFunctions)
        Me.StaticEntityFunctions.Remove(DirectCast(nSFunc, ClientStaticEntityFunctions))
    End Sub



End Class
