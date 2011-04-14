Public Class PlayerFunctions
    Inherits SpelObject

    Public Sub New(ByVal nParent As Player)
        MyBase.New(nParent)
        Me.DisplayName = "geen"
    End Sub


    Private mDisplayName As String
    Public Overridable Property DisplayName() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mDisplayName
        End Get
        <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As String)
            Me.mDisplayName = value
        End Set
    End Property


    Public Sub Update(ByVal BR As ByteReader)
        Me.DisplayName = BR.ReadString
        'Me.Lamp.Enabled = BR.ReadBoolean
    End Sub

    Public Sub GetData(ByVal BW As ByteWriter)
        BW.Write(Me.DisplayName)
        'BW.Write(Me.Lamp.Enabled)
    End Sub

    Public Sub WriteSerializeData(ByVal nDS As DataSerializer)
        nDS.SetData("DisplayName", Me.DisplayName)
    End Sub

    Public Sub ReadSerializedData(ByVal nDS As DataSerializer)
        Me.DisplayName = nDS.GetDataString("DisplayName", "New Player")
    End Sub

End Class
