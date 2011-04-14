Public Class SnelheidVeranderdEventArgs
    Inherits EventArgs

    Public Sub New(Optional ByVal nDirect As Boolean = True)
        Me.setDirect(nDirect)
    End Sub

    Private mDirect As Boolean
    ''' <summary>
    ''' Direct is true als de snelheid lett. is ingesteld, niet door de snelheid is veranderd
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Direct() As Boolean
        <System.Diagnostics.DebuggerStepThrough()> Get
            Return Me.mDirect
        End Get
    End Property
    Private Sub setDirect(ByVal value As Boolean)
        Me.mDirect = value
    End Sub

End Class
