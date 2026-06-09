<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmSub
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
        PicSub = New PictureBox()
        CType(PicSub, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' PicSub
        ' 
        PicSub.BackColor = Color.Black
        PicSub.Location = New Point(0, 0)
        PicSub.Margin = New Padding(2)
        PicSub.Name = "PicSub"
        PicSub.Size = New Size(640, 360)
        PicSub.SizeMode = PictureBoxSizeMode.Zoom
        PicSub.TabIndex = 0
        PicSub.TabStop = False
        ' 
        ' FrmSub
        ' 
        AutoScaleDimensions = New SizeF(96F, 96F)
        AutoScaleMode = AutoScaleMode.Dpi
        ClientSize = New Size(656, 374)
        Controls.Add(PicSub)
        FormBorderStyle = FormBorderStyle.FixedToolWindow
        Margin = New Padding(2)
        Name = "FrmSub"
        ShowInTaskbar = False
        Text = "解析モニタ (リアルタイム検知ビュー)"
        CType(PicSub, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents PicSub As PictureBox
End Class
