Public Class DesignListBox2D

    Public Sub New()
        Me.mItemHeight = 20
        Me.setLabelList(New List(Of Forms.Label))
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private mItemHeight As Integer
    <ComponentModel.DefaultValue(GetType(Integer), "20")> Public Property ItemHeight() As Integer
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mItemHeight
        End Get
        Set(ByVal value As Integer)
            If Me.mItemHeight = value Then Exit Property
            Me.mItemHeight = value
            Me.UpdateLabels()
        End Set
    End Property


    Protected Overrides Function CreatePanelPart(ByVal nParent As MHGameWork.Game3DPlay.Panel) As MHGameWork.Game3DPlay.PanelPart
        Dim P As New ListBox2D(nParent)

        PanelDesign.SetBasicInfo(P, Me)
        P.ItemHeight = Me.ItemHeight
        P.BackgroundColor = Me.BackColor

        Return P

    End Function

    Public Function GetListBox2D() As ListBox2D
        Return CType(Me.PanelPart, ListBox2D)
    End Function


    Private mLabelList As List(Of Forms.Label)
    <ComponentModel.Browsable(False)> Public ReadOnly Property LabelList() As List(Of Forms.Label)
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mLabelList
        End Get
    End Property
    Private Sub setLabelList(ByVal value As List(Of Forms.Label))
        Me.mLabelList = value
    End Sub


    Private Sub DesignListBox2D_ClientSizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ClientSizeChanged
        Me.UpdateLabels()

    End Sub

    Public Sub UpdateLabels()
        For Each IL As Forms.Label In Me.LabelList
            Me.Controls.Remove(IL)
            IL.Dispose()
        Next

        Me.LabelList.Clear()
        Dim NumItems As Integer = CInt(Math.Floor(Me.Size.Height / Me.ItemHeight))

        Dim L As Forms.Label
        For I As Integer = 0 To NumItems - 1
            L = New Forms.Label
            L.AutoSize = False
            L.Location = New Point(0, Me.ItemHeight * I)
            L.Size = New Size(Me.Size.Width, Me.ItemHeight)
            L.Text = "Item" & I.ToString
            L.TextAlign = ContentAlignment.MiddleLeft
            Me.Controls.Add(L)

            Me.LabelList.Add(L)

        Next
    End Sub
End Class
