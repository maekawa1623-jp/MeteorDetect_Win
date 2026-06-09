Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

Public Class FrmMask
    Private _bgBitmap As Bitmap      ' 下敷きになるカメラ映像
    Private _maskBitmap As Bitmap    ' 保存する白黒のマスク画像
    Private _isDrawing As Boolean = False
    Private _prevPt As Point
    Private _brushSize As Integer = 15 ' 筆の太さ

    ''' <summary>
    ''' メイン画面から現在のカメラ映像を受け取って初期化する
    ''' </summary>
    Public Sub Setup(ByVal currentFrame As Image)
        If currentFrame Is Nothing Then Return

        ' --- 1. 640x360 の枠に対して、縦横比を保った縮小率を計算 ---
        Dim scale As Double = Math.Min(PicMask.Width / currentFrame.Width, PicMask.Height / currentFrame.Height)
        If scale > 1.0 Then scale = 1.0

        Dim optW As Integer = CInt(currentFrame.Width * scale)
        Dim optH As Integer = CInt(currentFrame.Height * scale)

        ' 2. 背景（カメラ映像）を「計算した縮小サイズ」で生成する
        _bgBitmap = New Bitmap(currentFrame, optW, optH)

        ' ★修正：PicMaskのサイズは変更しません（640x360のまま固定）！
        ' その代わり、Paintイベントで中央に描画します。

        ' 3. マスク画像（白黒）のキャンバスも同じサイズで作成
        _maskBitmap = New Bitmap(optW, optH)
        Dim maskPath As String = IO.Path.Combine(Application.StartupPath, "mask.jpg")

        Using g As Graphics = Graphics.FromImage(_maskBitmap)
            If IO.File.Exists(maskPath) Then
                ' 既にマスクファイルがあれば、今のサイズに合わせて読み込む
                Using existingMask = Image.FromFile(maskPath)
                    g.DrawImage(existingMask, 0, 0, optW, optH)
                End Using
            Else
                ' なければ全体を白（検知ON）で塗りつぶす
                g.Clear(Color.White)
            End If
        End Using

        PicMask.Invalidate() ' 描画の更新
    End Sub

    Private Sub FrmMask_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = $"マスク編集 (現在のペンの太さ: {_brushSize})"
    End Sub

    Private Sub PicMask_Click(sender As Object, e As EventArgs) Handles PicMask.Click

    End Sub

    ' ==========================================================
    ' マウスでなぞって黒く塗る処理
    ' ==========================================================
    Private Sub PicMask_MouseDown(sender As Object, e As MouseEventArgs) Handles PicMask.MouseDown
        If e.Button = MouseButtons.Left Then
            _isDrawing = True
            _prevPt = GetImagePoint(e.Location)
            DrawMaskLine(_prevPt, _prevPt) ' クリックしただけでも点が描けるようにする
        End If
    End Sub

    Private Sub PicMask_MouseMove(sender As Object, e As MouseEventArgs) Handles PicMask.MouseMove
        If _isDrawing Then
            Dim currentPt As Point = GetImagePoint(e.Location)
            ' 画面外にはみ出た時のエラー防止
            If currentPt.X >= 0 AndAlso currentPt.Y >= 0 Then
                DrawMaskLine(_prevPt, currentPt)
                _prevPt = currentPt
            End If
        End If
    End Sub

    Private Sub PicMask_MouseUp(sender As Object, e As MouseEventArgs) Handles PicMask.MouseUp
        If e.Button = MouseButtons.Left Then
            _isDrawing = False
        End If
    End Sub

    ''' <summary>
    ''' マスク画像に黒い太線を引く
    ''' </summary>
    Private Sub DrawMaskLine(ByVal p1 As Point, ByVal p2 As Point)
        If _maskBitmap Is Nothing Then Return
        Using g As Graphics = Graphics.FromImage(_maskBitmap)
            ' なめらかな丸い筆を設定（色は黒＝検知除外）
            Using pen As New Pen(Color.Black, _brushSize)
                pen.StartCap = LineCap.Round
                pen.EndCap = LineCap.Round
                pen.LineJoin = LineJoin.Round
                g.DrawLine(pen, p1, p2)
            End Using
        End Using
        PicMask.Invalidate() ' 画面に反映
    End Sub

    ' ==========================================================
    ' 画面への描画（中央揃えで描画する）
    ' ==========================================================
    Private Sub PicMask_Paint(sender As Object, e As PaintEventArgs) Handles PicMask.Paint
        If _bgBitmap Is Nothing Then Return

        ' 中央に寄せるためのオフセット（余白）を計算
        Dim offsetX As Integer = (PicMask.Width - _bgBitmap.Width) \ 2
        Dim offsetY As Integer = (PicMask.Height - _bgBitmap.Height) \ 2

        ' 背景（カメラ映像）を描画
        e.Graphics.DrawImage(_bgBitmap, offsetX, offsetY)

        ' マスクを半透明で重ねる
        If _maskBitmap IsNot Nothing Then
            Dim cm As New ColorMatrix() With {.Matrix33 = 0.5F}
            Dim ia As New ImageAttributes()
            ia.SetColorMatrix(cm)

            Dim destRect As New Rectangle(offsetX, offsetY, _maskBitmap.Width, _maskBitmap.Height)
            e.Graphics.DrawImage(_maskBitmap, destRect, 0, 0, _maskBitmap.Width, _maskBitmap.Height, GraphicsUnit.Pixel, ia)
        End If
    End Sub

    ' ==========================================================
    ' マウス座標の計算（中央揃えの余白分を差し引く）
    ' ==========================================================
    Private Function GetImagePoint(ByVal mousePt As Point) As Point
        If _maskBitmap Is Nothing Then Return mousePt

        ' 中央描画の余白（オフセット）を計算
        Dim offsetX As Integer = (PicMask.Width - _maskBitmap.Width) \ 2
        Dim offsetY As Integer = (PicMask.Height - _maskBitmap.Height) \ 2

        ' マウスの座標から余白を引いて、画像上の本当の座標にする
        Dim imgX As Integer = mousePt.X - offsetX
        Dim imgY As Integer = mousePt.Y - offsetY

        ' 黒い余白部分をクリックした時のエラー防止（画像の端っこに制限する）
        If imgX < 0 Then imgX = 0
        If imgY < 0 Then imgY = 0
        If imgX >= _maskBitmap.Width Then imgX = _maskBitmap.Width - 1
        If imgY >= _maskBitmap.Height Then imgY = _maskBitmap.Height - 1

        Return New Point(imgX, imgY)
    End Function

    Private Sub BtnClear_Click(sender As Object, e As EventArgs) Handles BtnClear.Click
        If _maskBitmap Is Nothing Then Return
        Using g As Graphics = Graphics.FromImage(_maskBitmap)
            g.Clear(Color.White) ' 全て白（検知対象）に戻す
        End Using
        PicMask.Invalidate()
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        If _maskBitmap IsNot Nothing Then
            Dim maskPath As String = IO.Path.Combine(Application.StartupPath, "mask.jpg")
            _maskBitmap.Save(maskPath, ImageFormat.Jpeg)
            MessageBox.Show("マスク画像を保存しました。次回の検知から適用されます。", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    ' ==========================================================
    ' 【追加機能】マウスのホイールでペンの太さを変える
    ' ==========================================================
    Private Sub FrmMask_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
        ' ホイールを上に回すと太く、下に回すと細くする（5ずつ増減）
        If e.Delta > 0 Then
            _brushSize += 5
            If _brushSize > 100 Then _brushSize = 100 ' 最大値
        ElseIf e.Delta < 0 Then
            _brushSize -= 5
            If _brushSize < 5 Then _brushSize = 5     ' 最小値
        End If

        ' 現在のペンの太さをウィンドウのタイトルバーに表示してあげる
        Me.Text = $"マスク編集 (現在のペンの太さ: {_brushSize})"
    End Sub
End Class