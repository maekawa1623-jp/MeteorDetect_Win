<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmMask
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        PicMask = New PictureBox()
        BtnCancel = New Button()
        BtnSave = New Button()
        BtnClear = New Button()
        CType(PicMask, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' PicMask
        ' 
        PicMask.BackColor = Color.Black
        PicMask.Location = New Point(4, 4)
        PicMask.Margin = New Padding(2)
        PicMask.Name = "PicMask"
        PicMask.Size = New Size(640, 360)
        PicMask.SizeMode = PictureBoxSizeMode.Zoom
        PicMask.TabIndex = 0
        PicMask.TabStop = False
        ' 
        ' BtnCancel
        ' 
        BtnCancel.Location = New Point(430, 368)
        BtnCancel.Margin = New Padding(2)
        BtnCancel.Name = "BtnCancel"
        BtnCancel.Size = New Size(105, 30)
        BtnCancel.TabIndex = 1
        BtnCancel.Text = "キャンセル"
        BtnCancel.UseVisualStyleBackColor = True
        ' 
        ' BtnSave
        ' 
        BtnSave.Location = New Point(539, 368)
        BtnSave.Margin = New Padding(2)
        BtnSave.Name = "BtnSave"
        BtnSave.Size = New Size(105, 30)
        BtnSave.TabIndex = 2
        BtnSave.Text = "保存して閉じる"
        BtnSave.UseVisualStyleBackColor = True
        ' 
        ' BtnClear
        ' 
        BtnClear.Location = New Point(4, 368)
        BtnClear.Margin = New Padding(2)
        BtnClear.Name = "BtnClear"
        BtnClear.Size = New Size(105, 30)
        BtnClear.TabIndex = 3
        BtnClear.Text = "全クリア"
        BtnClear.UseVisualStyleBackColor = True
        ' 
        ' FrmMask
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(650, 403)
        Controls.Add(BtnClear)
        Controls.Add(BtnSave)
        Controls.Add(BtnCancel)
        Controls.Add(PicMask)
        FormBorderStyle = FormBorderStyle.FixedDialog
        Margin = New Padding(2)
        MaximizeBox = False
        MinimizeBox = False
        Name = "FrmMask"
        StartPosition = FormStartPosition.CenterParent
        Text = "FrmMask"
        CType(PicMask, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents PicMask As PictureBox
    Friend WithEvents BtnCancel As Button
    Friend WithEvents BtnSave As Button
    Friend WithEvents BtnClear As Button
End Class
