Imports OpenCvSharp

Public Class ClsMeteorDetector
    ' 過去の差分画像を保持するキュー
    Private ReadOnly _diffQueue As New Queue(Of Mat)()
    Private _prevFrame As Mat
    Private _queueSize As Integer = 15 ' 15fpsで1秒分（環境に合わせて調整可）

    ' 検知した「直線」のリスト (RectではなくLineSegmentPointになります)
    Public Property DetectedLines As New List(Of LineSegmentPoint)

    ' ==========================================================
    ' 外部（UI設定画面）から調整できるパラメータの受け皿
    ' ==========================================================
    Public Property AccumulateSeconds As Double = 1.0  ' 蓄積時間(秒)
    Public Property MaxLineGap As Double = 5.0         ' 途切れ許容値
    Public Property HoughThreshold As Integer = 25     ' ハフ変換検知しきい値

    ' (オプション: 今後いじりたくなった時用)
    Public Property BlurSize As Integer = 5            ' ぼかしサイズ
    Public Property CannyThreshold1 As Double = 100.0  ' エッジ感度(下限)
    Public Property CannyThreshold2 As Double = 200.0  ' エッジ感度(上限)

    ' ==========================================================
    ' ★追加：CLAHE（明暗差補正）のチューニング用パラメータ
    ' ==========================================================
    Public Property EnableClahe As Boolean = True      ' CLAHE機能のON/OFF
    Public Property ClaheClipLimit As Double = 3.0     ' コントラスト強調度 (低ノイズ機は3.0〜4.0推奨)
    Public Property ClaheGridSize As Integer = 8       ' 分割グリッドサイズ (標準は8)
    ' ==========================================================

    ' ==========================================================
    ' マスク画像を保持する変数と読み込みメソッド
    ' ==========================================================
    Private _maskMat As Mat
    Public Sub LoadMask(ByVal filePath As String)
        If IO.File.Exists(filePath) Then
            ' 白黒（グレースケール）として画像を読み込む
            Dim tempMask = Cv2.ImRead(filePath, ImreadModes.Grayscale)
            If Not tempMask.Empty() Then
                ' 古いマスクがあれば破棄して入れ替える
                If _maskMat IsNot Nothing Then _maskMat.Dispose()
                _maskMat = tempMask
            End If
        End If
    End Sub
    ' ==========================================================

    Public Function ProcessFrame(ByVal src As Mat, ByVal threshold As Integer, ByVal baseMinLineLength As Double, ByVal currentFps As Double, ByVal needDebugImage As Boolean) As Mat
        ' 1. 基本チェック
        If src Is Nothing OrElse src.IsDisposed OrElse src.Empty Then Return Nothing

        ' 2. 動的パラメータ計算
        Dim activeFps As Double = If(currentFps <= 0, 15.0, currentFps)

        ' 1.0 固定から AccumulateSeconds プロパティを使用
        _queueSize = CInt(Math.Round(activeFps * AccumulateSeconds))
        If _queueSize < 2 Then _queueSize = 2

        Dim scaleFactor As Double = src.Height / 1080.0
        Dim dynamicMinLineLength As Double = baseMinLineLength * scaleFactor

        ' 5.0 固定から MaxLineGap プロパティを使用
        Dim dynamicMaxLineGap As Double = MaxLineGap * scaleFactor

        ' 3. 前処理 (グレースケール)
        Dim gray As New Mat()
        Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY)

        ' ==========================================================
        ' ★ここに追加！：CLAHE（適応的ヒストグラム平滑化）の適用
        ' 地平線の明るいカブリを抑えつつ、天頂の暗い星を浮き上がらせる
        ' ※必ずマスク処理の「前」に実行し、正確な空の明るさを計算させる
        ' ==========================================================
        If EnableClahe Then
            Using clahe = Cv2.CreateCLAHE(clipLimit:=ClaheClipLimit, tileGridSize:=New OpenCvSharp.Size(ClaheGridSize, ClaheGridSize))
                clahe.Apply(gray, gray)
            End Using
        End If
        ' ==========================================================

        ' ==========================================================
        ' マスクの適用（BitwiseAndで不要領域を真っ黒にする）
        ' ==========================================================
        If _maskMat IsNot Nothing AndAlso Not _maskMat.Empty() Then
            Using resizedMask As New Mat()
                ' カメラの解像度に合わせてマスクのサイズを自動調整
                Cv2.Resize(_maskMat, resizedMask, gray.Size(), 0, 0, InterpolationFlags.Nearest)

                Dim maskedGray As New Mat()
                ' gray と resizedMask を重ね合わせ、白の部分だけを残す
                Cv2.BitwiseAnd(gray, resizedMask, maskedGray)

                gray.Dispose()
                gray = maskedGray ' マスク処理された画像を gray として引き継ぐ
            End Using
        End If
        ' ==========================================================

        ' ==========================================================
        ' ★修正・追加ここから：ROI変更（サイズ不一致）によるクラッシュを完全に防ぐバリア
        ' ==========================================================
        ' 過去の画像が残っているが、現在の画像とサイズが違う場合（ROI変更時など）
        If _prevFrame IsNot Nothing AndAlso Not _prevFrame.IsDisposed Then
            If _prevFrame.Size() <> gray.Size() Then
                ' サイズが違う古い過去画像は使えないので破棄する
                _prevFrame.Dispose()
                _prevFrame = Nothing

                ' キューに溜まっている過去の差分画像もサイズが違うので全クリア
                While _diffQueue.Count > 0
                    Dim old = _diffQueue.Dequeue()
                    If old IsNot Nothing Then old.Dispose()
                End While

                Debug.WriteLine("解像度(ROI)の変更を検知したため、検知エンジンをリセットしました。")
            End If
        End If

        ' 比較対象の過去フレームがない場合は、現在のフレームを記憶して今回は終了
        If _prevFrame Is Nothing OrElse _prevFrame.IsDisposed Then
            _prevFrame = gray.Clone()
            gray.Dispose()
            Return Nothing
        End If
        ' ==========================================================
        ' ★修正・追加ここまで
        ' ==========================================================

        ' 4. 差分抽出
        Dim diff As New Mat()
        Cv2.Absdiff(gray, _prevFrame, diff)
        Cv2.Threshold(diff, diff, threshold, 255, ThresholdTypes.Tozero)

        ' 前フレーム更新
        _prevFrame.Dispose()
        _prevFrame = gray.Clone()
        gray.Dispose()

        ' 5. キューへの蓄積
        _diffQueue.Enqueue(diff)
        While _diffQueue.Count > _queueSize
            Dim old = _diffQueue.Dequeue()
            If old IsNot Nothing Then old.Dispose()
        End While

        ' フレームが溜まるまで待機
        If _diffQueue.Count < _queueSize Then Return Nothing

        ' 6. 比較明合成 (ここがメモリリークしやすいので注意)
        ' 最初のフレームをコピー
        Dim composite As Mat = _diffQueue.Peek().Clone()
        For Each d In _diffQueue
            ' Cv2.Max(A, B, A) は避け、一時的な Mat を作らないようにする
            Cv2.Max(composite, d, composite)
        Next

        ' 7. 直線抽出 (Usingを使って中間Matを自動解放)
        'Dim lines() As LineSegmentPoint = Nothing
        Using blur As New Mat(), canny As New Mat()
            ' ガウシアンぼかし
            Dim bSize As Integer = CInt(Math.Round(BlurSize * scaleFactor))
            If bSize Mod 2 = 0 Then bSize += 1
            Cv2.GaussianBlur(composite, blur, New Size(bSize, bSize), 0)

            ' エッジ検出
            Cv2.Canny(blur, canny, CannyThreshold1, CannyThreshold2, 3)

            ' ハフ変換
            Dim lines = Cv2.HoughLinesP(canny, 1, Math.PI / 180, HoughThreshold, dynamicMinLineLength, dynamicMaxLineGap)

            ' デバッグ画像の生成（Using内で行うことでCannyを安全に参照）
            Dim debugMat As Mat = Nothing
            If needDebugImage Then
                debugMat = New Mat()
                Cv2.Resize(canny, debugMat, New Size(640, 360), 0, 0, InterpolationFlags.Nearest)
            End If

            ' 結果の保存
            DetectedLines.Clear()
            If lines IsNot Nothing Then DetectedLines.AddRange(lines)

            ' 合成画像の解放
            composite.Dispose()

            ' デバッグ画像（またはNothing）を返して、blur/cannyはUsingで自動解放
            Return debugMat
        End Using
    End Function

    Public Sub Reset()
        ' ★ここを追加：過去のフレーム記憶を完全に消去する
        If _prevFrame IsNot Nothing Then
            If Not _prevFrame.IsDisposed Then _prevFrame.Dispose()
            _prevFrame = Nothing
        End If

        ' ★ここを追加：過去の差分キューのメモリもすべて解放して空にする
        If _diffQueue IsNot Nothing Then
            While _diffQueue.Count > 0
                Dim old = _diffQueue.Dequeue()
                If old IsNot Nothing Then old.Dispose()
            End While
        End If

        ' （その他、既存のカウンタやリストのリセット処理など）
        DetectedLines.Clear()
    End Sub
End Class