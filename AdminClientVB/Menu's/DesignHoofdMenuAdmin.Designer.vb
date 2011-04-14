<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DesignHoofdMenuAdmin
    Inherits DesignMainPanel

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.knpQuit = New MHGameWork.TheWizards.Common.DesignKnop001
        Me.knpModelManager = New MHGameWork.TheWizards.Common.DesignKnop001
        Me.knpFileManager = New MHGameWork.TheWizards.Common.DesignKnop001
        Me.knpWorldManager = New MHGameWork.TheWizards.Common.DesignKnop001
        Me.SuspendLayout()
        '
        'knpQuit
        '
        Me.knpQuit.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.knpQuit.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpQuit.KnopText = "Quit game"
        Me.knpQuit.Location = New System.Drawing.Point(57, 104)
        Me.knpQuit.Name = "knpQuit"
        Me.knpQuit.Size = New System.Drawing.Size(182, 55)
        Me.knpQuit.TabIndex = 1
        '
        'knpModelManager
        '
        Me.knpModelManager.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.knpModelManager.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpModelManager.KnopText = "Model Manager"
        Me.knpModelManager.Location = New System.Drawing.Point(345, 186)
        Me.knpModelManager.Name = "knpModelManager"
        Me.knpModelManager.Size = New System.Drawing.Size(182, 55)
        Me.knpModelManager.TabIndex = 2
        '
        'knpFileManager
        '
        Me.knpFileManager.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.knpFileManager.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpFileManager.KnopText = "File Manager"
        Me.knpFileManager.Location = New System.Drawing.Point(345, 104)
        Me.knpFileManager.Name = "knpFileManager"
        Me.knpFileManager.Size = New System.Drawing.Size(182, 55)
        Me.knpFileManager.TabIndex = 3
        '
        'knpWorldManager
        '
        Me.knpWorldManager.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.knpWorldManager.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpWorldManager.KnopText = "World Manager"
        Me.knpWorldManager.Location = New System.Drawing.Point(345, 273)
        Me.knpWorldManager.Name = "knpWorldManager"
        Me.knpWorldManager.Size = New System.Drawing.Size(182, 55)
        Me.knpWorldManager.TabIndex = 4
        '
        'DesignHoofdMenuAdmin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.Controls.Add(Me.knpWorldManager)
        Me.Controls.Add(Me.knpFileManager)
        Me.Controls.Add(Me.knpModelManager)
        Me.Controls.Add(Me.knpQuit)
        Me.Name = "DesignHoofdMenuAdmin"
        Me.Size = New System.Drawing.Size(659, 653)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents knpQuit As Common.DesignKnop001
    Friend WithEvents knpModelManager As Common.DesignKnop001
    Friend WithEvents knpFileManager As Common.DesignKnop001
    Friend WithEvents knpWorldManager As MHGameWork.TheWizards.Common.DesignKnop001


End Class
