<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DesignWorldManager
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
        Me.txtGridY = New MHGameWork.TheWizards.Common.DesignTextBox2D
        Me.txtGridX = New MHGameWork.TheWizards.Common.DesignTextBox2D
        Me.DesignLabel2 = New MHGameWork.TheWizards.Common.DesignLabel
        Me.DesignLabel1 = New MHGameWork.TheWizards.Common.DesignLabel
        Me.knpSnap = New MHGameWork.TheWizards.Common.DesignKnop001
        Me.renWorldViewport = New MHGameWork.TheWizards.Common.DesignRendererTo2D
        Me.knpHoofdmenu = New MHGameWork.TheWizards.Common.DesignKnop001
        Me.knpGetModelList = New MHGameWork.TheWizards.Common.DesignKnop001
        Me.lstModels = New MHGameWork.TheWizards.Common.DesignListBox2D
        Me.SuspendLayout()
        '
        'txtGridY
        '
        Me.txtGridY.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.txtGridY.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.txtGridY.Location = New System.Drawing.Point(244, 662)
        Me.txtGridY.Name = "txtGridY"
        Me.txtGridY.Size = New System.Drawing.Size(89, 21)
        Me.txtGridY.TabIndex = 30
        '
        'txtGridX
        '
        Me.txtGridX.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.txtGridX.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.txtGridX.Location = New System.Drawing.Point(244, 635)
        Me.txtGridX.Name = "txtGridX"
        Me.txtGridX.Size = New System.Drawing.Size(89, 21)
        Me.txtGridX.TabIndex = 29
        '
        'DesignLabel2
        '
        Me.DesignLabel2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DesignLabel2.LabelText = "GridY:"
        Me.DesignLabel2.Location = New System.Drawing.Point(190, 662)
        Me.DesignLabel2.Name = "DesignLabel2"
        Me.DesignLabel2.Size = New System.Drawing.Size(48, 21)
        Me.DesignLabel2.TabIndex = 28
        Me.DesignLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'DesignLabel1
        '
        Me.DesignLabel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DesignLabel1.LabelText = "GridX:"
        Me.DesignLabel1.Location = New System.Drawing.Point(190, 635)
        Me.DesignLabel1.Name = "DesignLabel1"
        Me.DesignLabel1.Size = New System.Drawing.Size(48, 21)
        Me.DesignLabel1.TabIndex = 27
        Me.DesignLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'knpSnap
        '
        Me.knpSnap.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.knpSnap.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpSnap.KnopText = "Disable Snap"
        Me.knpSnap.Location = New System.Drawing.Point(190, 595)
        Me.knpSnap.Name = "knpSnap"
        Me.knpSnap.Size = New System.Drawing.Size(143, 34)
        Me.knpSnap.TabIndex = 26
        '
        'renWorldViewport
        '
        Me.renWorldViewport.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.renWorldViewport.BackColor = System.Drawing.Color.Goldenrod
        Me.renWorldViewport.Location = New System.Drawing.Point(190, 3)
        Me.renWorldViewport.Name = "renWorldViewport"
        Me.renWorldViewport.Size = New System.Drawing.Size(639, 573)
        Me.renWorldViewport.TabIndex = 0
        '
        'knpHoofdmenu
        '
        Me.knpHoofdmenu.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.knpHoofdmenu.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpHoofdmenu.KnopText = "Terug naar Hoofdmenu"
        Me.knpHoofdmenu.Location = New System.Drawing.Point(600, 648)
        Me.knpHoofdmenu.Name = "knpHoofdmenu"
        Me.knpHoofdmenu.Size = New System.Drawing.Size(203, 35)
        Me.knpHoofdmenu.TabIndex = 25
        '
        'knpGetModelList
        '
        Me.knpGetModelList.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpGetModelList.KnopText = "Get Model List"
        Me.knpGetModelList.Location = New System.Drawing.Point(15, 13)
        Me.knpGetModelList.Name = "knpGetModelList"
        Me.knpGetModelList.Size = New System.Drawing.Size(169, 49)
        Me.knpGetModelList.TabIndex = 24
        '
        'lstModels
        '
        Me.lstModels.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lstModels.BackColor = System.Drawing.Color.PowderBlue
        Me.lstModels.Location = New System.Drawing.Point(15, 68)
        Me.lstModels.Name = "lstModels"
        Me.lstModels.Size = New System.Drawing.Size(169, 605)
        Me.lstModels.TabIndex = 23
        '
        'DesignWorldManager
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.Controls.Add(Me.txtGridY)
        Me.Controls.Add(Me.txtGridX)
        Me.Controls.Add(Me.DesignLabel2)
        Me.Controls.Add(Me.DesignLabel1)
        Me.Controls.Add(Me.knpSnap)
        Me.Controls.Add(Me.renWorldViewport)
        Me.Controls.Add(Me.knpHoofdmenu)
        Me.Controls.Add(Me.knpGetModelList)
        Me.Controls.Add(Me.lstModels)
        Me.Name = "DesignWorldManager"
        Me.Size = New System.Drawing.Size(832, 699)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents renWorldViewport As MHGameWork.TheWizards.Common.DesignRendererTo2D
    Friend WithEvents knpHoofdmenu As MHGameWork.TheWizards.Common.DesignKnop001
    Friend WithEvents knpGetModelList As MHGameWork.TheWizards.Common.DesignKnop001
    Friend WithEvents lstModels As MHGameWork.TheWizards.Common.DesignListBox2D
    Friend WithEvents knpSnap As MHGameWork.TheWizards.Common.DesignKnop001
    Friend WithEvents DesignLabel1 As MHGameWork.TheWizards.Common.DesignLabel
    Friend WithEvents DesignLabel2 As MHGameWork.TheWizards.Common.DesignLabel
    Friend WithEvents txtGridX As MHGameWork.TheWizards.Common.DesignTextBox2D
    Friend WithEvents txtGridY As MHGameWork.TheWizards.Common.DesignTextBox2D


End Class
