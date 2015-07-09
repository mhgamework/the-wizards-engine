Public Class DesignLabel

    Public Sub New()
        Me.mLabelText = "(geen)"
        Me.mTextAlign = ContentAlignment.MiddleLeft
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private mLabelText As String
    <ComponentModel.Browsable(True), ComponentModel.Category("Content")> _
    <ComponentModel.DefaultValue("(geen)")> Public Property LabelText() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return mLabelText
        End Get
        Set(ByVal value As String)
            If value = Nothing Then value = ""
            Me.mLabelText = value
            Me.Label1.Text = value
        End Set
    End Property


    Private mTextAlign As Drawing.ContentAlignment
    <ComponentModel.Browsable(True), ComponentModel.Category("Content")> _
    Public Property TextAlign() As Drawing.ContentAlignment
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mTextAlign
        End Get
        Set(ByVal value As Drawing.ContentAlignment)
            Me.mTextAlign = value
            Me.Label1.TextAlign = value
        End Set
    End Property



    Protected Overrides Function CreatePanelPart(ByVal nParent As MHGameWork.Game3DPlay.Panel) As MHGameWork.Game3DPlay.PanelPart
        Dim P As New Label(nParent)

        PanelDesign.SetBasicInfo(P, Me)
        P.Text = Me.LabelText
        P.TextAlign = PanelDesign.ConvertDrawTextFormat(Me.Label1.TextAlign)

        Return P

    End Function

    Public Function GetLabel() As Label
        Return CType(Me.PanelPart, Label)
    End Function

End Class
