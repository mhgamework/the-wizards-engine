<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DesignModelManager
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
        Me.lstModels = New MHGameWork.TheWizards.Common.DesignListBox2D
        Me.knpGetModelList = New MHGameWork.TheWizards.Common.DesignKnop001
        Me.knpHoofdmenu = New MHGameWork.TheWizards.Common.DesignKnop001
        Me.DesignLabel1 = New MHGameWork.TheWizards.Common.DesignLabel
        Me.txtNaam = New MHGameWork.TheWizards.Common.DesignTextBox2D
        Me.lblID = New MHGameWork.TheWizards.Common.DesignLabel
        Me.DesignLabel2 = New MHGameWork.TheWizards.Common.DesignLabel
        Me.DesignLabel3 = New MHGameWork.TheWizards.Common.DesignLabel
        Me.DesignLabel4 = New MHGameWork.TheWizards.Common.DesignLabel
        Me.DesignLabel5 = New MHGameWork.TheWizards.Common.DesignLabel
        Me.DesignLabel6 = New MHGameWork.TheWizards.Common.DesignLabel
        Me.txtGameFile = New MHGameWork.TheWizards.Common.DesignTextBox2D
        Me.txtCollisionFile = New MHGameWork.TheWizards.Common.DesignTextBox2D
        Me.txtCustomData = New MHGameWork.TheWizards.Common.DesignTextBox2D
        Me.txtVersie = New MHGameWork.TheWizards.Common.DesignTextBox2D
        Me.knpUpdate = New MHGameWork.TheWizards.Common.DesignKnop001
        Me.txtLstOutput = New MHGameWork.TheWizards.Common.DesignTextList2D
        Me.SuspendLayout()
        '
        'lstModels
        '
        Me.lstModels.BackColor = System.Drawing.Color.PowderBlue
        Me.lstModels.Location = New System.Drawing.Point(33, 72)
        Me.lstModels.Name = "lstModels"
        Me.lstModels.Size = New System.Drawing.Size(169, 506)
        Me.lstModels.TabIndex = 0
        '
        'knpGetModelList
        '
        Me.knpGetModelList.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpGetModelList.KnopText = "Get Model List"
        Me.knpGetModelList.Location = New System.Drawing.Point(33, 17)
        Me.knpGetModelList.Name = "knpGetModelList"
        Me.knpGetModelList.Size = New System.Drawing.Size(169, 49)
        Me.knpGetModelList.TabIndex = 1
        '
        'knpHoofdmenu
        '
        Me.knpHoofdmenu.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.knpHoofdmenu.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpHoofdmenu.KnopText = "Terug naar Hoofdmenu"
        Me.knpHoofdmenu.Location = New System.Drawing.Point(546, 589)
        Me.knpHoofdmenu.Name = "knpHoofdmenu"
        Me.knpHoofdmenu.Size = New System.Drawing.Size(203, 35)
        Me.knpHoofdmenu.TabIndex = 22
        '
        'DesignLabel1
        '
        Me.DesignLabel1.LabelText = "ID:"
        Me.DesignLabel1.Location = New System.Drawing.Point(208, 72)
        Me.DesignLabel1.Name = "DesignLabel1"
        Me.DesignLabel1.Size = New System.Drawing.Size(142, 36)
        Me.DesignLabel1.TabIndex = 23
        Me.DesignLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtNaam
        '
        Me.txtNaam.BackColor = System.Drawing.Color.SkyBlue
        Me.txtNaam.Location = New System.Drawing.Point(356, 140)
        Me.txtNaam.Name = "txtNaam"
        Me.txtNaam.Size = New System.Drawing.Size(326, 36)
        Me.txtNaam.TabIndex = 24
        '
        'lblID
        '
        Me.lblID.BackColor = System.Drawing.Color.SkyBlue
        Me.lblID.LabelText = ""
        Me.lblID.Location = New System.Drawing.Point(356, 72)
        Me.lblID.Name = "lblID"
        Me.lblID.Size = New System.Drawing.Size(150, 36)
        Me.lblID.TabIndex = 25
        Me.lblID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'DesignLabel2
        '
        Me.DesignLabel2.LabelText = "Naam:"
        Me.DesignLabel2.Location = New System.Drawing.Point(208, 140)
        Me.DesignLabel2.Name = "DesignLabel2"
        Me.DesignLabel2.Size = New System.Drawing.Size(142, 36)
        Me.DesignLabel2.TabIndex = 26
        Me.DesignLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'DesignLabel3
        '
        Me.DesignLabel3.LabelText = "Client File ID:"
        Me.DesignLabel3.Location = New System.Drawing.Point(208, 182)
        Me.DesignLabel3.Name = "DesignLabel3"
        Me.DesignLabel3.Size = New System.Drawing.Size(142, 36)
        Me.DesignLabel3.TabIndex = 27
        Me.DesignLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'DesignLabel4
        '
        Me.DesignLabel4.LabelText = "Collision File ID:"
        Me.DesignLabel4.Location = New System.Drawing.Point(208, 224)
        Me.DesignLabel4.Name = "DesignLabel4"
        Me.DesignLabel4.Size = New System.Drawing.Size(142, 36)
        Me.DesignLabel4.TabIndex = 28
        Me.DesignLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'DesignLabel5
        '
        Me.DesignLabel5.LabelText = "CustomData:"
        Me.DesignLabel5.Location = New System.Drawing.Point(208, 266)
        Me.DesignLabel5.Name = "DesignLabel5"
        Me.DesignLabel5.Size = New System.Drawing.Size(142, 36)
        Me.DesignLabel5.TabIndex = 29
        Me.DesignLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'DesignLabel6
        '
        Me.DesignLabel6.LabelText = "Versie:"
        Me.DesignLabel6.Location = New System.Drawing.Point(208, 308)
        Me.DesignLabel6.Name = "DesignLabel6"
        Me.DesignLabel6.Size = New System.Drawing.Size(142, 36)
        Me.DesignLabel6.TabIndex = 30
        Me.DesignLabel6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtGameFile
        '
        Me.txtGameFile.BackColor = System.Drawing.Color.SkyBlue
        Me.txtGameFile.Location = New System.Drawing.Point(356, 182)
        Me.txtGameFile.Name = "txtGameFile"
        Me.txtGameFile.Size = New System.Drawing.Size(326, 36)
        Me.txtGameFile.TabIndex = 31
        '
        'txtCollisionFile
        '
        Me.txtCollisionFile.BackColor = System.Drawing.Color.SkyBlue
        Me.txtCollisionFile.Location = New System.Drawing.Point(356, 224)
        Me.txtCollisionFile.Name = "txtCollisionFile"
        Me.txtCollisionFile.Size = New System.Drawing.Size(326, 36)
        Me.txtCollisionFile.TabIndex = 32
        '
        'txtCustomData
        '
        Me.txtCustomData.BackColor = System.Drawing.Color.SkyBlue
        Me.txtCustomData.Location = New System.Drawing.Point(356, 266)
        Me.txtCustomData.Name = "txtCustomData"
        Me.txtCustomData.Size = New System.Drawing.Size(326, 36)
        Me.txtCustomData.TabIndex = 33
        '
        'txtVersie
        '
        Me.txtVersie.BackColor = System.Drawing.Color.SkyBlue
        Me.txtVersie.Location = New System.Drawing.Point(356, 308)
        Me.txtVersie.Name = "txtVersie"
        Me.txtVersie.Size = New System.Drawing.Size(326, 36)
        Me.txtVersie.TabIndex = 34
        '
        'knpUpdate
        '
        Me.knpUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.knpUpdate.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.knpUpdate.KnopText = "Update Model Data"
        Me.knpUpdate.Location = New System.Drawing.Point(335, 363)
        Me.knpUpdate.Name = "knpUpdate"
        Me.knpUpdate.Size = New System.Drawing.Size(203, 35)
        Me.knpUpdate.TabIndex = 35
        '
        'txtLstOutput
        '
        Me.txtLstOutput.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtLstOutput.BackColor = System.Drawing.Color.RoyalBlue
        Me.txtLstOutput.Location = New System.Drawing.Point(208, 404)
        Me.txtLstOutput.Name = "txtLstOutput"
        Me.txtLstOutput.Size = New System.Drawing.Size(474, 143)
        Me.txtLstOutput.TabIndex = 36
        '
        'DesignModelManager
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.BackColor = System.Drawing.Color.CornflowerBlue
        Me.Controls.Add(Me.txtLstOutput)
        Me.Controls.Add(Me.knpUpdate)
        Me.Controls.Add(Me.txtVersie)
        Me.Controls.Add(Me.txtCustomData)
        Me.Controls.Add(Me.txtCollisionFile)
        Me.Controls.Add(Me.txtGameFile)
        Me.Controls.Add(Me.DesignLabel6)
        Me.Controls.Add(Me.DesignLabel5)
        Me.Controls.Add(Me.DesignLabel4)
        Me.Controls.Add(Me.DesignLabel3)
        Me.Controls.Add(Me.DesignLabel2)
        Me.Controls.Add(Me.lblID)
        Me.Controls.Add(Me.txtNaam)
        Me.Controls.Add(Me.DesignLabel1)
        Me.Controls.Add(Me.knpHoofdmenu)
        Me.Controls.Add(Me.knpGetModelList)
        Me.Controls.Add(Me.lstModels)
        Me.Name = "DesignModelManager"
        Me.Size = New System.Drawing.Size(770, 648)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lstModels As MHGameWork.TheWizards.Common.DesignListBox2D
    Friend WithEvents knpGetModelList As MHGameWork.TheWizards.Common.DesignKnop001
    Friend WithEvents knpHoofdmenu As MHGameWork.TheWizards.Common.DesignKnop001
    Friend WithEvents DesignLabel1 As MHGameWork.TheWizards.Common.DesignLabel
    Friend WithEvents txtNaam As MHGameWork.TheWizards.Common.DesignTextBox2D
    Friend WithEvents lblID As MHGameWork.TheWizards.Common.DesignLabel
    Friend WithEvents DesignLabel2 As MHGameWork.TheWizards.Common.DesignLabel
    Friend WithEvents DesignLabel3 As MHGameWork.TheWizards.Common.DesignLabel
    Friend WithEvents DesignLabel4 As MHGameWork.TheWizards.Common.DesignLabel
    Friend WithEvents DesignLabel5 As MHGameWork.TheWizards.Common.DesignLabel
    Friend WithEvents DesignLabel6 As MHGameWork.TheWizards.Common.DesignLabel
    Friend WithEvents txtGameFile As MHGameWork.TheWizards.Common.DesignTextBox2D
    Friend WithEvents txtCollisionFile As MHGameWork.TheWizards.Common.DesignTextBox2D
    Friend WithEvents txtCustomData As MHGameWork.TheWizards.Common.DesignTextBox2D
    Friend WithEvents txtVersie As MHGameWork.TheWizards.Common.DesignTextBox2D
    Friend WithEvents knpUpdate As MHGameWork.TheWizards.Common.DesignKnop001
    Friend WithEvents txtLstOutput As MHGameWork.TheWizards.Common.DesignTextList2D


End Class
