Imports MHGameWork.Game3DPlay.Gui
Imports MHGameWork.Game3DPlay.Gui.Design

Public Class DesignKnop001
    'Implements IDesignPanelPart

    Public Sub New()
        Me.mKnopText = "(geen)"
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private mKnopText As String
    <ComponentModel.Browsable(True), ComponentModel.Category("Content")> _
    <ComponentModel.DefaultValue("(geen)")> Public Property KnopText() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return mKnopText
        End Get
        Set(ByVal value As String)
            If value = Nothing Then value = ""
            Me.mKnopText = value
            Me.Label1.Text = value
        End Set
    End Property

    Protected Overrides Function CreatePanelPart(ByVal nParent As MHGameWork.Game3DPlay.Gui.Panel) As MHGameWork.Game3DPlay.Gui.PanelPart
        Dim K As New Knop001(nParent)
        PanelDesign.SetBasicInfo(K, Me)

        'If Me.KnopText IsNot Nothing Then
        K.Text = Me.KnopText
        'End If

        Return K

    End Function

    Public Function GetKnop001() As Knop001
        Return CType(Me.PanelPart, Knop001)
    End Function

End Class
