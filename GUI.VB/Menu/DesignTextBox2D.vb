Public Class DesignTextBox2D

    Public Sub New()
        Me.mTextBoxText = "(geen)"
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private mTextBoxText As String
    <ComponentModel.DefaultValue("(geen)")> Public Property TextBoxText() As String
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mTextBoxText
        End Get
        Set(ByVal value As String)
            If value = Nothing Then value = ""
            Me.mTextBoxText = value
            Me.TextBox1.Text = value
        End Set
    End Property


    Protected Overrides Function CreatePanelPart(ByVal nParent As MHGameWork.Game3DPlay.Panel) As MHGameWork.Game3DPlay.PanelPart
        Dim P As New TextBox2D(nParent)

        PanelDesign.SetBasicInfo(P, Me)
        P.Text = Me.TextBoxText
        P.BackgroundColor = Me.BackColor

        Return P

    End Function

    Public Function GetTextBox2D() As TextBox2D
        Return CType(Me.PanelPart, TextBox2D)
    End Function

    Private Sub DesignTextBox2D_BackColorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.BackColorChanged
        Me.TextBox1.BackColor = Me.BackColor
    End Sub
End Class
