Imports OpenCvSharp

Public Class FrmSub
    ' メインフォームから非同期で画像を投げ込むためのメソッド
    Public Sub UpdateImage(ByVal img As Mat)
        If Me.InvokeRequired Then
            ' UIスレッドで実行し直す
            Me.BeginInvoke(New Action(Of Mat)(AddressOf UpdateImage), img)
            Return
        End If

        ' PictureBoxに表示
        Dim oldImage = PicSub.Image
        PicSub.Image = BasFunction.MatToBitmap(img)
        If oldImage IsNot Nothing Then oldImage.Dispose()

        ' 表示が終わったら必ず破棄（メモリリーク防止）
        img.Dispose()
    End Sub


    Private Sub FrmSub_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' フォームの枠のスタイルを「サイズ変更不可」にする
        Me.FormBorderStyle = FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.Text = "検知画面"

        ' クライアント領域（内側の描画エリア）を正確に640x360に設定
        Me.ClientSize = New System.Drawing.Size(640, 360)

        ' PictureBoxをフォームいっぱいに広げる
        PicSub.Location = New System.Drawing.Point(0, 0)
        PicSub.Size = New System.Drawing.Size(640, 360)
        PicSub.SizeMode = PictureBoxSizeMode.Zoom ' 念のためアスペクト比維持
        PicSub.Dock = DockStyle.Fill

    End Sub

    Private Sub FrmSub_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            ' 1. フォームが完全に破棄（Dispose）されるのを防ぐ
            e.Cancel = True

            ' 2. メイン画面のチェックボックスを「OFF」にする
            ' （※メイン画面のクラス名が FrmMain である前提です）
            If Me.Owner IsNot Nothing AndAlso TypeOf Me.Owner Is FrmMain Then
                Dim mainForm As FrmMain = CType(Me.Owner, FrmMain)
                mainForm.ChkShowSub.Checked = False
            End If

            ' ※ mainForm.ChkShowSub.Checked = False が実行された瞬間に、
            ' メイン画面側の ChkShowSub_CheckedChanged が自動的に走り、
            ' そこで Me.Hide() と _isDebugEnabled = False が行われるため、
            ' ここで直接 Me.Hide() と書く必要すらなくなります！
        End If
    End Sub

End Class