Imports System.Collections.Concurrent ' ← キュー(待ち行列)を使うために追加
Imports System.Diagnostics
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Threading.Tasks
Imports DirectShowLib ' ← NuGetでインストールしたパッケージ
Imports OpenCvSharp
Imports OpenCvSharp.Dnn

Public Class FrmMain
    ' --- クラスレベル変数 ---
    Private _currentCamera As ICameraProvider
    Private _detector As New ClsMeteorDetector()

    ' FPS計算用
    Private _stopwatch As New Stopwatch()
    Private _frameCount As Integer = 0
    Private _currentFps As Double = 0

    ' --- バックグラウンド検知用の変数 ---
    Private _frameQueue As BlockingCollection(Of Mat) ' 映像を貯めておくキュー
    Private _cts As CancellationTokenSource ' スレッド停止用のシグナル

    ' 【変更】Rect のリストから、LineSegmentPoint のリストへ変更
    Private _latestLines As New List(Of LineSegmentPoint)()
    Private _linesLock As New Object() ' スレッド間の衝突を防ぐための鍵

    ' ==========================================================
    ' ↓ 追加：検知を開始してよい時刻を記録する変数
    ' ==========================================================
    Private _detectionStartTime As DateTime

    Private _frmSub As FrmSub
    Private _isDebugEnabled As Boolean = False ' スレッド安全のための変数

    ' スライダーの最大値(10秒)＋余裕を見て、常に15秒分の巨大バッファを確保しておく
    Private _imageBuffer As New ClsImageBuffer(15.0)

    Private _isDetectingActive As Boolean = False
    Private _firstDetectTime As DateTime
    Private _lastDetectTime As DateTime

    ' --- クラス変数に追加 ---
    Private _isFileMode As Boolean = False
    Private _videoFilePath As String = ""

    ' 【追加】スナップショット要求フラグ
    Private _requestSnapshot As Boolean = False

    '「動画の読み込み自体は完了した」ことを示すフラグを追加します。
    Private _videoReadCompleted As Boolean = False

    ' ==========================================================
    ' 【追加】裏の録画スレッドが安全に読み取れる「デュアル録画ON/OFF変数」
    ' デザイナー画面に配置した [ChkDualRecord] と連動します
    ' ==========================================================
    Private _enableDualRecord As Boolean = False

    ' ==========================================================
    ' 【追加】検知音ミュート用の変数
    ' ==========================================================
    Private _isSoundMuted As Boolean = False


    ' ==========================================================
    ' ZWOカメラ制御用の変数群
    ' ==========================================================
    Public Shared Property IsAsiMode As Boolean = False ' True=ASIカメラ, False=Webカメラ
    Public Shared Property AsiCam As New ClsZwoCamera()
    Public Shared Property CamID As Integer = -1
    Public Shared Property IsCapturing As Boolean = False
    Public Shared Property MaxBin As Integer = 1
    Public Shared Property HasCooler As Boolean = False
    Public _isUpdatingUI As Boolean = False

    ' --- 表示・録画用 画質調整変数 ---
    Private _displayAlpha As Double = 1.0
    Private _displayBeta As Integer = 0
    Private _displayGamma As Double = 1.2 ' ★ここに追加

    ' ★ ここに追加（使い回すLUTと、安全な入れ替えのためのロック）
    Private _gammaLut As Mat = Nothing
    Private ReadOnly _lutLock As New Object()

    ' ==========================================================
    ' 常時録画（デバッグ用）変数
    ' ==========================================================
    Private _continuousWriter As VideoWriter = Nothing
    Private _lastRecordedMinute As String = ""

    ' ★追加：ゼロフレームドロップ用のキューと裏方スレッド
    Private _continuousQueue As Concurrent.BlockingCollection(Of Mat)
    Private _continuousCts As Threading.CancellationTokenSource

    ' ==========================================================
    ' --- 検知チューニング用 変数 ---
    ' ==========================================================
    Private _enableDetect As Boolean
    Private _detectThreshold As Integer
    Private _minLineLength As Double
    Private _accSeconds As Double
    Private _maxLineGap As Double
    Private _houghThreshold As Integer
    Private _preRollSec As Double
    Private _postRollSec As Double



    ' ★追加：外部（設定画面）で設定変更中であることを示すフラグ
    Public Shared Property IsExternalChanging As Boolean = False
    Private _externalChangeTimer As Integer = 0

    ' ==========================================================
    ' USB機器の抜き差しを自動検知するWindowsメッセージ処理
    ' ==========================================================
    Protected Overrides Sub WndProc(ByRef m As Message)
        Const WM_DEVICECHANGE As Integer = &H219

        ' デバイスの構成に変更があった場合（USBを挿した、抜いた等）
        If m.Msg = WM_DEVICECHANGE Then
            ' カメラが動いていない時だけリストを自動更新する
            If _currentCamera Is Nothing OrElse Not _currentCamera.IsRunning Then
                RefreshDeviceList()
                AddLog("デバイスの変更を検知し、カメラリストを自動更新しました。")
            End If
        End If
        MyBase.WndProc(m)
    End Sub

    Private Sub FrmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' ==========================================================
            ' 【重要】各パネルをベースパネルの中に強制移動し、位置を固定する
            ' ==========================================================
            ' ※ PnlBasePanel はデザイナであらかじめ配置しておいた親パネルの名前です
            Dim configPanels As Panel() = {PnlUsbSettings, PnlZwoSettings, PnlRtspSettings, PnlVideoSettings}

            For Each p In configPanels
                ' 【重要】一度親を Nothing にしてから PnlBasePanel に入れ直す
                p.Parent = Nothing
                p.Parent = PnlSettingsBase

                ' サイズをベースパネルいっぱいに広げ、位置を (0,0) に固定
                p.Dock = DockStyle.Fill
                p.Visible = False
            Next

            ' 1. 現在のラジオボタン状態に合わせて表示を切り替える
            UpdateSettingsPanel()

            ' 2. 各デバイスリストの取得 (ZWO [cite: 193] や USBカメラなど)
            RefreshDeviceList()

            ' ==========================================================
            ' 【既存】USBカメラ設定のコンボボックス初期化
            ' ==========================================================
            CmbUsbResolution.Items.Clear()
            CmbUsbResolution.Items.Add("1920x1080")
            CmbUsbResolution.Items.Add("1280x720")
            CmbUsbResolution.Items.Add("640x480")
            CmbUsbResolution.SelectedIndex = 0

            ' 起動時に My.Settings から変数をロード
            ReloadDetectSettings()

            ' メイン画面にあるUIの初期化
            ChkEnableDetect.Checked = My.Settings.SysEnableDetect
            TrcGamma.Value = CInt(My.Settings.SysGamma * 10)
            LblGamma.Text = $"ガンマ(Def:1.2): {My.Settings.SysGamma:F1}"

            ' ==========================================================
            ' ★ここに追加：保存されているRTSPのURLをテキストボックスにセットする
            ' ==========================================================
            TxtRtspUrl.Text = My.Settings.RtspUrl

            ' ガンマ補正用LUTの初期化
            UpdateGammaLut()

            BtnStop.Enabled = False
            LblStatusMessage.Text = "準備完了"

            AddLog($"{BasConst.APP_NAME} を起動しました。")

        Catch ex As Exception
            AddLog("起動時の初期化エラー: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As EventArgs) Handles BtnStart.Click

        Dim selectedIndex As Integer = -1
        Dim selectedName As String = ""

        ' --- 1. ソースの選択とインスタンスの準備 ---
        If RdoUsb.Checked Then
            selectedIndex = CmbUsbDeviceList.SelectedIndex
            If selectedIndex < 0 OrElse CmbUsbDeviceList.SelectedItem.ToString().Contains("見つかりません") Then
                MessageBox.Show("有効なUSBカメラを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            selectedName = CmbUsbDeviceList.SelectedItem.ToString()

            Dim usbCam As New ClsUsbCamera()
            If CmbUsbResolution.SelectedItem IsNot Nothing Then
                Dim resStr = CmbUsbResolution.SelectedItem.ToString()
                If resStr = "1920x1080" Then
                    usbCam.ReqWidth = 1920 : usbCam.ReqHeight = 1080
                ElseIf resStr = "1280x720" Then
                    usbCam.ReqWidth = 1280 : usbCam.ReqHeight = 720
                ElseIf resStr = "640x480" Then
                    usbCam.ReqWidth = 640 : usbCam.ReqHeight = 480
                End If
            End If
            _currentCamera = usbCam

        ElseIf RdoZwo.Checked Then
            ' 【重要】ZWOは「接続」ボタンですでに初期化済みである前提
            If CamID = -1 OrElse AsiCam Is Nothing Then
                MessageBox.Show("まず「接続」ボタンでZWOカメラに接続してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            selectedName = CmbZwoDeviceList.SelectedItem.ToString()

            ' すでに接続・保持しているインスタンス(_asiCam)を共通インターフェース変数にセット
            ' ※もし ClsZwoCamera というクラス名にしている場合は適宜読み替えてください
            _currentCamera = AsiCam

            ' ==========================================================
            ' ZWO特有の開始前処理 (ビニング適用とUIロック)
            ' ==========================================================
            BtnConnect.Enabled = False ' 撮影中は切断不可
            CmbBinning.Enabled = False ' 撮影中はビニング変更不可

            Dim bin As Integer = 1
            If CmbBinning.SelectedItem IsNot Nothing Then
                ' "2 (Bin2x2)" から先頭の数字を抽出
                Dim binStr As String = CmbBinning.SelectedItem.ToString().Split(" "c)(0)
                Dim dummy = Integer.TryParse(binStr, bin)
            End If

            ' ビニングの設定をASIカメラに反映
            AsiCam.SetBinning(bin)

            Dim resText = $"Resolution: {AsiCam.Width} x {AsiCam.Height} (Bin{bin})"
            AddLog($"ZWOカメラ設定反映 - {resText}")
            ' ==========================================================

        ElseIf RdoRtsp.Checked Then
            Dim url As String = TxtRtspUrl.Text.Trim()
            If String.IsNullOrEmpty(url) Then
                MessageBox.Show("RTSPのURLを入力してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            selectedName = "RTSP Stream"
            Dim rtspCam As New ClsRtspCamera With {
                .ConnectionUrl = url
            }
            _currentCamera = rtspCam
            ' ==========================================================

        ElseIf RdoVideo.Checked Then
            If String.IsNullOrEmpty(_videoFilePath) Then
                MessageBox.Show("再生する動画ファイルを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            selectedName = Path.GetFileName(_videoFilePath)
            _currentCamera = New ClsVideoFile()
        End If

        ' --- 2. イベントの紐付け ---
        ' どのソース（USB/ZWO/Video）でも同じ OnFrameArrived で受ける
        AddHandler _currentCamera.FrameArrived, AddressOf OnFrameArrived

        If TypeOf _currentCamera Is ClsVideoFile Then
            AddHandler DirectCast(_currentCamera, ClsVideoFile).VideoFinished, AddressOf OnVideoFinished
        End If

        ' --- 3. カメラ/ファイルの起動 ---
        Dim isStarted As Boolean = False
        If RdoVideo.Checked Then
            Using tempCap As New VideoCapture(_videoFilePath)
                _currentFps = tempCap.Fps
            End Using
            isStarted = DirectCast(_currentCamera, ClsVideoFile).StartVideo(_videoFilePath, _currentFps)
        Else
            ' USBもZWOも共通の StartCamera(StartCapture) を呼ぶ
            ' ※ZWOは targetID ではなく _camID を使うなどの調整が必要であれば、クラス内で処理してください
            isStarted = _currentCamera.StartCamera(selectedIndex)
        End If

        ' --- 4. 起動成功時の共通処理 ---
        If isStarted Then
            _detector.Reset()

            Dim maskPath As String = Path.Combine(Application.StartupPath, "mask.jpg")
            _detector.LoadMask(maskPath)

            _frameQueue = New BlockingCollection(Of Mat)(10)
            _cts = New CancellationTokenSource()

            ' ==========================================================
            ' ★追加：常時録画用のキューを準備し、裏方書き込みスレッドを起動
            ' （最大50フレーム＝約3秒分までディスクの遅延を吸収できます）
            ' ==========================================================
            _continuousQueue = New BlockingCollection(Of Mat)(50)
            _continuousCts = New CancellationTokenSource()
            Task.Run(AddressOf ContinuousRecordLoop, _continuousCts.Token)

            _detectionStartTime = DateTime.Now.AddSeconds(5)
            _videoReadCompleted = False

            Task.Run(AddressOf DetectionLoop, _cts.Token)

            ' --- UIの切り替え ---
            BtnStart.Enabled = False
            BtnStop.Enabled = True
            GrpSource.Enabled = False
            BtnRefreshUsb.Enabled = False
            BtnRefreshZwo.Enabled = False

            _stopwatch.Restart()
            _frameCount = 0

            AddLog($"[{selectedName}] の解析を開始しました。")
            LblStatusMessage.Text = "解析実行中..."
        Else
            MessageBox.Show("初期化に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
            AddLog("初期化に失敗しました。")

            ' ZWOの場合、失敗したらロックしたUIを戻す
            If RdoZwo.Checked Then
                BtnConnect.Enabled = True
                CmbBinning.Enabled = True
            End If

            RemoveHandler _currentCamera.FrameArrived, AddressOf OnFrameArrived

            ' ZWOカメラは _currentCamera = Nothing にすると切断されてしまう可能性があるため除外
            If Not RdoZwo.Checked Then
                _currentCamera = Nothing
            End If
        End If
    End Sub

    Private Sub BtnStop_Click(sender As Object, e As EventArgs) Handles BtnStop.Click
        ' ==========================================================
        ' ★追加：常時録画の裏方スレッドとキューを安全に停止
        ' ==========================================================
        If _continuousCts IsNot Nothing Then
            _continuousCts.Cancel()
            _continuousCts.Dispose()
            _continuousCts = Nothing
        End If

        If _continuousQueue IsNot Nothing Then
            _continuousQueue.CompleteAdding()
            Dim remainingMat As Mat = Nothing
            While _continuousQueue.TryTake(remainingMat)
                If remainingMat IsNot Nothing Then remainingMat.Dispose()
            End While
            _continuousQueue.Dispose()
            _continuousQueue = Nothing
        End If

        ' --- 1. バックグラウンド検知スレッドとキューの安全な停止 ---
        If _cts IsNot Nothing Then
            Try
                _cts.Cancel()
                _cts.Dispose()
            Catch ex As ObjectDisposedException
            End Try
            _cts = Nothing
        End If

        If _frameQueue IsNot Nothing Then
            _frameQueue.CompleteAdding()
            Dim remainingFrame As Mat = Nothing
            While _frameQueue.TryTake(remainingFrame)
                If remainingFrame IsNot Nothing Then remainingFrame.Dispose()
            End While
            _frameQueue.Dispose()
            _frameQueue = Nothing
        End If

        ' --- 2. カメラ本体（または動画ファイル）の停止処理 ---
        If _currentCamera IsNot Nothing Then
            If TypeOf _currentCamera Is ClsVideoFile Then
                Dim videoFile = DirectCast(_currentCamera, ClsVideoFile)
                RemoveHandler videoFile.VideoFinished, AddressOf OnVideoFinished
                videoFile.StopVideo()
            ElseIf _currentCamera.IsRunning Then
                ' USBもZWOも共通の StopCamera() を呼び出してループを止める
                _currentCamera.StopCamera()
            End If

            ' フレーム受信イベントを解除
            RemoveHandler _currentCamera.FrameArrived, AddressOf OnFrameArrived

            ' インターフェースの参照をクリア（ZWOの本体インスタンス自体は保持されます）
            _currentCamera = Nothing
            _stopwatch.Stop()
        End If

        ' --- 3. UI（ボタン等）の有効化・ステータス更新 ---
        BtnStart.Enabled = True
        BtnRefreshUsb.Enabled = True
        BtnRefreshZwo.Enabled = True
        GrpSource.Enabled = True
        BtnStop.Enabled = False

        ' ==========================================================
        ' 【追加】ZWOカメラ特有のUIロック解除とフラグ操作
        ' ==========================================================
        If RdoZwo.Checked Then
            ' キャプチャが止まったので、設定変更や切断を許可する
            BtnConnect.Enabled = True
            CmbBinning.Enabled = True

            ' もしフォーム側で _isCapturing フラグを管理している場合は下ろす
            IsCapturing = False
        End If
        ' ==========================================================

        AddLog("映像取得を停止しました。")
        LblStatusMessage.Text = "停止中"
        LblStatusFps.Text = "FPS: 0.0"
        LblStatusResolution.Text = "解像度: ---"

        _isDetectingActive = False

        ' --- 4. 画面のクリア ---
        SyncLock _linesLock
            _latestLines.Clear()
        End SyncLock

        If PicMain.Image IsNot Nothing Then
            Dim oldImg = PicMain.Image
            PicMain.Image = Nothing
            oldImg.Dispose()
        End If
    End Sub

    ' ==========================================================
    ' 【追加】デュアル録画ON/OFFチェックボックスの処理
    ' ==========================================================
    Private Sub ChkDualRecord_CheckedChanged(sender As Object, e As EventArgs) Handles ChkDualRecord.CheckedChanged
        _enableDualRecord = ChkDualRecord.Checked
        If _enableDualRecord Then
            AddLog("デュアル録画（綺麗な動画＋枠付き動画）をONにしました。")
        Else
            AddLog("デュアル録画をOFFにしました（綺麗な動画のみ保存）。")
        End If
    End Sub

    ' ==========================================================
    ' 映像フレーム到着イベント (UIへの表示とキューへの追加)
    ' ==========================================================
    Private Sub OnFrameArrived(ByVal frame As Mat)
        If frame Is Nothing OrElse frame.IsDisposed Then Return

        If Me.InvokeRequired Then
            Me.Invoke(New Action(Of Mat)(AddressOf OnFrameArrived), frame)
            Return
        End If

        ' 表示・録画・検知用の「化粧」画像を格納する変数
        Dim displayMat As Mat = Nothing

        Try
            ' ==========================================================
            ' 1. 最初に、表示・録画・検知の「共通」となる明るい画像を作る
            ' ==========================================================
            displayMat = New Mat()
            '' ZWOの暗い映像を、AtomCamのように明るくコントラストの強い状態にする
            'Cv2.ConvertScaleAbs(frame, displayMat, alpha:=_displayAlpha, beta:=_displayBeta)

            ' 【ステップA】まずは従来の Alpha / Beta を適用
            Dim tempMat As New Mat()
            Cv2.ConvertScaleAbs(frame, tempMat, alpha:=_displayAlpha, beta:=_displayBeta)

            ' 【ステップB】ガンマ補正で背景（暗部）だけをストンと落とし、星（明部）を残す
            ' 毎回計算するのではなく、作成済みの _gammaLut を使い回す
            SyncLock _lutLock
                If _gammaLut IsNot Nothing AndAlso Not _gammaLut.IsDisposed Then
                    ' 爆速で適用（CPU負荷ほぼゼロ）
                    Cv2.LUT(tempMat, _gammaLut, displayMat)
                Else
                    ' 万が一LUTの準備が間に合っていない時の安全対策
                    tempMat.CopyTo(displayMat)
                End If
            End SyncLock

            ' 使い終わった中間画像を忘れずに破棄（メモリリーク防止）
            tempMat.Dispose()


            ' ==========================================================
            ' 2. 明るくした画像(displayMat)のコピーを検知エンジンに送る
            ' ==========================================================
            Dim detectClone As Mat = Nothing
            If _enableDetect AndAlso _frameQueue IsNot Nothing AndAlso DateTime.Now >= _detectionStartTime Then
                ' 生のframeではなく、明るいdisplayMatを渡す
                detectClone = displayMat.Clone()
                If Not _frameQueue.TryAdd(detectClone, 0) Then
                    detectClone.Dispose() ' キューが一杯なら破棄
                End If
            Else
                SyncLock _linesLock
                    _latestLines.Clear()
                End SyncLock
            End If

            ' ==========================================================
            ' 3. 日時を描画する (化粧後の明るい displayMat の方に描く)
            ' ==========================================================
            If Not RdoVideo.Checked Then
                Dim timeStr As String = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                Dim scale As Double = 1.0 * (displayMat.Height / 1080.0)
                Dim fontThick As Integer = If(displayMat.Height > 2000, 4, 2)
                Dim outlineThick As Integer = fontThick + 2

                Dim textSize = Cv2.GetTextSize(timeStr, HersheyFonts.HersheySimplex, scale, fontThick, Nothing)
                Dim pt As New OpenCvSharp.Point(displayMat.Width - textSize.Width - 20, displayMat.Height - 20)

                Cv2.PutText(displayMat, timeStr, pt, HersheyFonts.HersheySimplex, scale, Scalar.Black, outlineThick, LineTypes.AntiAlias)
                Cv2.PutText(displayMat, timeStr, pt, HersheyFonts.HersheySimplex, scale, Scalar.White, fontThick, LineTypes.AntiAlias)
            End If

            ' ==========================================================
            ' 4. 常時録画処理（ATOM Cam互換：1分ごとにファイルを自動分割）
            ' ==========================================================
            If ChkContinuousRecord.Checked AndAlso _continuousQueue IsNot Nothing Then
                ' 裏方スレッドに画像を渡すだけ（ファイル処理を待たないので爆速）
                Dim recordClone = displayMat.Clone()
                If Not _continuousQueue.TryAdd(recordClone, 0) Then
                    recordClone.Dispose() ' 万が一PCが激重でキューが満杯なら破棄
                End If
            End If

            ' ==========================================================
            ' 5. イベント録画バッファには「化粧済み画像のクローン」と「枠情報」を渡す
            ' ==========================================================
            Dim currentLines As New List(Of LineSegmentPoint)()
            If _detector IsNot Nothing AndAlso _detector.DetectedLines IsNot Nothing Then
                currentLines.AddRange(_detector.DetectedLines)
            End If

            _imageBuffer.AddFrame(displayMat.Clone(), currentLines)

            ' スナップショット要求があれば保存
            If _requestSnapshot Then
                _requestSnapshot = False
                Using snapClone = displayMat.Clone()
                    SaveSnapshotInternal(snapClone)
                End Using
            End If

            ' FPS・解像度表示の更新
            _frameCount += 1
            If _stopwatch.ElapsedMilliseconds >= 1000 Then
                _currentFps = _frameCount / (_stopwatch.ElapsedMilliseconds / 1000.0)
                LblStatusFps.Text = $"FPS: {_currentFps:F1}"

                Dim resText As String = $"解像度: {frame.Width}x{frame.Height}"
                If RdoZwo.Checked AndAlso CmbBinning.SelectedItem IsNot Nothing Then
                    Dim binStr = CmbBinning.SelectedItem.ToString().Split(" "c)(0)
                    resText &= $" (Bin{binStr})"
                End If
                LblStatusResolution.Text = resText
                _frameCount = 0
                _stopwatch.Restart()
            End If

            ' 画面へのプレビュー描画 (表示用リサイズ)
            Using previewFrame = displayMat.Clone()
                SyncLock _linesLock
                    For Each line In _latestLines
                        DrawDetectionBoxes(previewFrame, _latestLines)
                    Next
                End SyncLock

                If PicMain.Width > 0 AndAlso PicMain.Height > 0 Then
                    Dim scale As Double = Math.Min(PicMain.Width / previewFrame.Width, PicMain.Height / previewFrame.Height)
                    If scale > 1.0 Then scale = 1.0

                    Dim optimalW As Integer = CInt(previewFrame.Width * scale)
                    Dim optimalH As Integer = CInt(previewFrame.Height * scale)

                    Using optimalDisplay As New Mat()
                        Cv2.Resize(previewFrame, optimalDisplay, New Size(optimalW, optimalH), 0, 0, InterpolationFlags.Area)

                        Dim oldImage = PicMain.Image
                        PicMain.Image = BasFunction.MatToBitmap(optimalDisplay)
                        If oldImage IsNot Nothing Then oldImage.Dispose()
                    End Using
                End If
            End Using

        Catch ex As Exception
            AddLog($"描画エラー: {ex.Message}")
        Finally
            ' メモリリーク防止（非常に重要）
            If frame IsNot Nothing AndAlso Not frame.IsDisposed Then
                frame.Dispose()
            End If
            ' 新しく作った化粧用Matもここで確実に解放
            If displayMat IsNot Nothing AndAlso Not displayMat.IsDisposed Then
                displayMat.Dispose()
            End If
        End Try
    End Sub

    Private Sub SaveSnapshotInternal(ByVal frame As Mat)
        Try
            Dim snapDir As String = IO.Path.Combine(GetMeteorDetectBaseDir(), "SnapShot")
            If Not IO.Directory.Exists(snapDir) Then IO.Directory.CreateDirectory(snapDir)
            Dim savePath As String = IO.Path.Combine(snapDir, $"snapshot_{DateTime.Now:yyyyMMdd_HHmmss}.jpg")

            Using snapClone = frame.Clone()
                Cv2.ImWrite(savePath, snapClone)
            End Using
            AddLog($"スナップショット保存完了: {IO.Path.GetFileName(savePath)}")
        Catch ex As Exception
            AddLog($"スナップショット保存エラー: {ex.Message}")
        End Try
    End Sub

    Private Sub FrmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        BtnStop.PerformClick()

        ' アプリを閉じる瞬間に、メイン画面が持っている検知ON/OFFとガンマを保存
        My.Settings.SysEnableDetect = _enableDetect
        My.Settings.SysGamma = _displayGamma

        ' ==========================================================
        ' ★ここに追加：入力されているRTSPのURLを記憶させる
        ' ==========================================================
        My.Settings.RtspUrl = TxtRtspUrl.Text

        My.Settings.Save()
    End Sub

    Private Sub RdoUsb_CheckedChanged(sender As Object, e As EventArgs) Handles RdoUsb.CheckedChanged
        If RdoUsb.Checked Then
            IsAsiMode = False
            UpdateSettingsPanel() ' パネルを最前面へ [cite: 15]
            RefreshDeviceList()   ' USBカメラのリストを更新
        End If
    End Sub

    Private Sub RdoZwo_CheckedChanged(sender As Object, e As EventArgs) Handles RdoZwo.CheckedChanged
        If RdoZwo.Checked Then
            IsAsiMode = True
            UpdateSettingsPanel()   ' パネルを最前面へ [cite: 15]
            RefreshZwoCameraList()  ' ZWOカメラのリストを更新 [cite: 191, 196]
        End If
    End Sub



    Private Sub RdoVideo_CheckedChanged(sender As Object, e As EventArgs) Handles RdoVideo.CheckedChanged
        If RdoVideo.Checked Then
            IsAsiMode = False
            UpdateSettingsPanel() ' パネルを最前面へ [cite: 15]
            ' 必要に応じて動画ファイル選択の初期化など
        End If
    End Sub

    Private Sub UpdateSettingsPanel()
        ' 一旦すべて非表示にする
        PnlUsbSettings.Visible = False
        PnlZwoSettings.Visible = False
        PnlRtspSettings.Visible = False
        PnlVideoSettings.Visible = False

        ' 選択されたパネルだけを表示し、最前面に持ってくる
        If RdoUsb.Checked Then
            PnlUsbSettings.Visible = True
            PnlUsbSettings.BringToFront() ' [重要] これで重なり順のトップに移動します
            AddLog("入力ソースを [USBカメラ] に変更しました。")

        ElseIf RdoZwo.Checked Then
            PnlZwoSettings.Visible = True
            PnlZwoSettings.BringToFront()
            AddLog("入力ソースを [ZWO ASIカメラ] に変更しました。")

        ElseIf RdoRtsp.Checked Then ' ★ここを追加
            PnlRtspSettings.Visible = True
            PnlRtspSettings.BringToFront()
            AddLog("入力ソースを [RTSP (ATOM Cam)] に変更しました。")

        ElseIf RdoVideo.Checked Then
            PnlVideoSettings.Visible = True
            PnlVideoSettings.BringToFront()
            AddLog("入力ソースを [動画ファイル] に変更しました。")
        End If
    End Sub

    Private Sub AddLog(ByVal message As String)
        Dim timeStr As String = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
        Dim logText As String = $"[{timeStr}] {message}"

        If LstLogs.InvokeRequired Then
            LstLogs.BeginInvoke(New Action(Of String)(AddressOf AddLog), message)
        Else
            LstLogs.Items.Insert(0, logText)
            While LstLogs.Items.Count > BasConst.MAX_LOG_LINES
                LstLogs.Items.RemoveAt(LstLogs.Items.Count - 1)
            End While
        End If
    End Sub

    Private Sub BtnRefreshUsb_Click(sender As Object, e As EventArgs) Handles BtnRefreshUsb.Click
        RefreshUsbDeviceList()
        AddLog("USBカメラリストを手動で更新しました。")
    End Sub


    Private Sub RefreshDeviceList()
        RefreshUsbDeviceList()
        RefreshZwoDeviceList()
    End Sub

    Private Sub RefreshUsbDeviceList()
        CmbUsbDeviceList.Items.Clear()
        Try
            Dim videoDevices As DsDevice() = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice)
            For Each device As DsDevice In videoDevices
                CmbUsbDeviceList.Items.Add(device.Name)
            Next
        Catch ex As Exception
            AddLog("USBカメラリストの取得に失敗しました。")
        End Try

        If CmbUsbDeviceList.Items.Count = 0 Then
            CmbUsbDeviceList.Items.Add("カメラが見つかりません")
        End If
        CmbUsbDeviceList.SelectedIndex = 0
    End Sub

    Private Sub RefreshZwoDeviceList()
        CmbZwoDeviceList.Items.Clear()
        Try
            Dim num = ASI_SDK.ASIGetNumOfConnectedCameras()
            For i As Integer = 0 To num - 1
                Dim info As New ASI_SDK.ASI_CAMERA_INFO()
                Dim ret = ASI_SDK.ASIGetCameraProperty(info, i)
                If ret <> 0 Then
                    AddLog($"ZWOカメラのプロパティ取得に失敗しました (エラーコード: {ret})")
                End If

                CmbZwoDeviceList.Items.Add(info.Name)
            Next
        Catch ex As Exception
            AddLog("ZWOカメラの取得エラー: " & ex.Message)
        End Try

        If CmbZwoDeviceList.Items.Count = 0 Then
            CmbZwoDeviceList.Items.Add("ZWOカメラが見つかりません")
        End If
        CmbZwoDeviceList.SelectedIndex = 0
    End Sub

    Private Sub DetectionLoop()
        Try
            While _cts IsNot Nothing AndAlso Not _cts.IsCancellationRequested
                Dim frameToProcess As Mat = Nothing

                If _frameQueue.TryTake(frameToProcess, 100, _cts.Token) Then
                    Try
                        Dim debugImg As Mat = _detector.ProcessFrame(frameToProcess,
                                                                    _detectThreshold,
                                                                    _minLineLength,
                                                                    _currentFps,
                                                                    _isDebugEnabled)

                        SyncLock _linesLock
                            _latestLines.Clear()
                            _latestLines.AddRange(_detector.DetectedLines)
                        End SyncLock

                        If debugImg IsNot Nothing Then
                            If _isDebugEnabled AndAlso _frmSub IsNot Nothing AndAlso _frmSub.Visible Then
                                _frmSub.UpdateImage(debugImg)
                            Else
                                debugImg.Dispose()
                            End If
                        End If

                        If _detector.DetectedLines.Count > 0 Then
                            If Not _isDetectingActive Then
                                _isDetectingActive = True

                                ' ==========================================================
                                ' 検知チャイムを鳴らす（ミュート判定 ＆ WAV専用）
                                ' ==========================================================
                                If Not _isSoundMuted Then
                                    Try
                                        Dim soundPath As String = My.Settings.AlertSoundPath

                                        If Not String.IsNullOrEmpty(soundPath) AndAlso IO.File.Exists(soundPath) Then
                                            ' WAVファイルをラグなしで再生！
                                            My.Computer.Audio.Play(soundPath, AudioPlayMode.Background)
                                        Else
                                            System.Media.SystemSounds.Asterisk.Play()
                                        End If
                                    Catch ex As Exception
                                        AddLog("警告音の再生エラー: " & ex.Message)
                                    End Try
                                End If

                                _firstDetectTime = DateTime.Now
                                AddLog("流星検知！ 保存予約を開始しました...")
                                'SaveDetectionSnapshot(frameToProcess, _detector.DetectedLines)
                            End If
                            _lastDetectTime = DateTime.Now
                        Else
                            If _isDetectingActive AndAlso (DateTime.Now - _lastDetectTime).TotalSeconds > _postRollSec Then
                                SaveMeteorEvent(_firstDetectTime, _lastDetectTime)
                                _isDetectingActive = False
                            End If
                        End If

                    Finally
                        If frameToProcess IsNot Nothing Then frameToProcess.Dispose()
                    End Try

                Else
                    If _videoReadCompleted Then
                        If _isDetectingActive Then
                            AddLog("動画の終端に達したため、最後の検知イベントを強制保存します。")
                            SaveMeteorEvent(_firstDetectTime, _lastDetectTime)
                            _isDetectingActive = False
                        End If
                        Me.BeginInvoke(Sub() BtnStop.PerformClick())
                        Exit While
                    End If
                End If
            End While

        Catch ex As OperationCanceledException
        Catch ex As ObjectDisposedException
        Catch ex As Exception
            AddLog($"検知ループエラー: {ex.Message}")
        End Try
    End Sub

    ' ==========================================================
    ' 【完成版】デュアル録画対応 ＋ 流星軌跡の比較明合成
    ' ==========================================================
    Private Sub SaveMeteorEvent(ByVal startTime As DateTime, ByVal endTime As DateTime)
        ' ==========================================================
        ' 【追加】容量チェック
        ' ==========================================================
        If Not IsDiskSpaceEnough() Then Return

        ' 1. 保存対象のフレームを取得
        Dim saveStart = startTime.AddSeconds(-_preRollSec)
        Dim saveEnd = endTime.AddSeconds(_postRollSec)
        Dim framesToSave = _imageBuffer.GetFrames(saveStart, saveEnd)

        If framesToSave.Count = 0 Then Return

        Dim baseDir As String = GetMeteorDetectBaseDir()
        Dim dateDir As String = Path.Combine(baseDir, startTime.ToString("yyyy-MM-dd"))

        ' ファイル名を3種類用意する（Clean, Boxed, そして Image）
        Dim fileNameClean = startTime.ToString("HHmmss") & BasConst.VIDEO_EXTENSION
        Dim fileNameBoxed = startTime.ToString("HHmmss") & "_Boxed" & BasConst.VIDEO_EXTENSION
        Dim fileNameImage = "Composite_" & startTime.ToString("HHmmss") & ".jpg" ' ★追加：合成写真

        Dim fullPathClean = Path.Combine(dateDir, fileNameClean)
        Dim fullPathBoxed = Path.Combine(dateDir, fileNameBoxed)
        Dim fullPathImage = Path.Combine(dateDir, fileNameImage) ' ★追加

        ' 非同期で動画・画像生成を実行
        Task.Run(Sub()
                     Try
                         If Not Directory.Exists(dateDir) Then Directory.CreateDirectory(dateDir)

                         Dim firstFrame = framesToSave(0).Frame
                         Dim inputSize As New OpenCvSharp.Size(firstFrame.Width, firstFrame.Height)
                         Dim videoFps As Double = If(_currentFps > 0, _currentFps, 15.0)

                         ' -----------------------------------------------------------
                         ' ★追加：比較明合成のキャンバスと、検知枠を全部入れるリスト
                         ' -----------------------------------------------------------
                         Dim compositeMat As Mat = Nothing
                         Dim allLines As New List(Of LineSegmentPoint)()

                         ' -----------------------------------------------------------
                         ' 1. 動画を書き込みながら、同時に比較明合成を行うブロック
                         ' -----------------------------------------------------------
                         Using writerClean As New VideoWriter(fullPathClean, FourCC.FromString(BasConst.VIDEO_CODEC), videoFps, inputSize)
                             Dim writerBoxed As VideoWriter = Nothing
                             Try
                                 If _enableDualRecord Then
                                     writerBoxed = New VideoWriter(fullPathBoxed, FourCC.FromString(BasConst.VIDEO_CODEC), videoFps, inputSize)
                                 End If

                                 For Each f In framesToSave
                                     ' ==================================================
                                     ' ★比較明合成の処理（今までの画像と新しい画像を比べて、明るい方を残す）
                                     ' ==================================================
                                     If compositeMat Is Nothing Then
                                         compositeMat = f.Frame.Clone()
                                     Else
                                         Cv2.Max(compositeMat, f.Frame, compositeMat)
                                     End If

                                     ' 全フレームの検知枠の座標を蓄積する
                                     If f.Lines IsNot Nothing AndAlso f.Lines.Count > 0 Then
                                         allLines.AddRange(f.Lines)
                                     End If

                                     ' --------------------------------------------------
                                     ' 動画の書き込み
                                     writerClean.Write(f.Frame)
                                     If writerBoxed IsNot Nothing Then
                                         Using boxedFrame = f.Frame.Clone()
                                             DrawDetectionBoxes(boxedFrame, f.Lines)
                                             writerBoxed.Write(boxedFrame)
                                         End Using
                                     End If

                                     ' メモリ解放はここで行う
                                     f.Frame.Dispose()
                                 Next

                                 ' 強制終了宣言
                                 writerClean.Release()
                                 If writerBoxed IsNot Nothing Then writerBoxed.Release()

                             Finally
                                 If writerBoxed IsNot Nothing Then writerBoxed.Dispose()
                             End Try
                         End Using ' <--- ここでOSに対してファイルが「完全にクローズ」される

                         ' -----------------------------------------------------------
                         ' 2. ★追加：動画保存完了後、合成した写真に「全体の枠」を描いて保存
                         ' -----------------------------------------------------------
                         If compositeMat IsNot Nothing Then
                             Try
                                 ' 流星が通った軌跡の全体を覆うように大きな枠を描画
                                 DrawDetectionBoxes(compositeMat, allLines)
                                 Cv2.ImWrite(fullPathImage, compositeMat)
                                 AddLog($"流星軌跡の合成写真を保存しました: {fileNameImage}")
                             Finally
                                 compositeMat.Dispose()
                             End Try
                         End If

                         ' -----------------------------------------------------------
                         ' 3. ファイルが閉じた後にログ出力
                         ' -----------------------------------------------------------
                         AddLog($"動画保存完了: {dateDir} に保存しました。")

                     Catch ex As Exception
                         AddLog("動画/画像保存失敗: " & ex.Message)
                     Finally
                         ' framesToSave の残骸を掃除
                         For Each f In framesToSave
                             If f.Frame IsNot Nothing AndAlso Not f.Frame.IsDisposed Then f.Frame.Dispose()
                         Next
                     End Try
                 End Sub)
    End Sub

    Private Sub BtnOpenFile_Click(sender As Object, e As EventArgs) Handles BtnOpenFile.Click
        If _cts IsNot Nothing Then Return

        Using ofd As New OpenFileDialog()
            ofd.Filter = "動画ファイル|*.mp4;*.avi;*.mkv;*.mov;*.wmv|すべてのファイル|*.*"
            ofd.Title = "テスト用動画を選択してください"

            If ofd.ShowDialog() = DialogResult.OK Then
                _videoFilePath = ofd.FileName
                TxtFileName.Text = Path.GetFileName(_videoFilePath)
                RdoVideo.Checked = True
                AddLog($"ファイル選択: {TxtFileName.Text}")
            End If
        End Using
    End Sub

    Private Sub ChkShowSub_CheckedChanged(sender As Object, e As EventArgs) Handles ChkShowSub.CheckedChanged
        If ChkShowSub.Checked Then
            If _frmSub Is Nothing OrElse _frmSub.IsDisposed Then
                _frmSub = New FrmSub()
            End If
            _frmSub.Owner = Me
            _frmSub.Show()
            _isDebugEnabled = True
        Else
            If _frmSub IsNot Nothing AndAlso Not _frmSub.IsDisposed Then
                _frmSub.Hide()
            End If
            _isDebugEnabled = False
        End If
    End Sub

    Private Sub OnVideoFinished()
        If Me.InvokeRequired Then
            Me.Invoke(New Action(AddressOf OnVideoFinished))
            Return
        End If
        _videoReadCompleted = True
        AddLog("動画の読み込みが完了しました。残りのフレームを解析中です...")
    End Sub

    Private Sub BtnSnapshot_Click(sender As Object, e As EventArgs) Handles BtnSnapshot.Click
        If _currentCamera IsNot Nothing AndAlso _currentCamera.IsRunning Then
            _requestSnapshot = True
            AddLog("スナップショットの保存をリクエストしました...")
        Else
            MessageBox.Show("保存する映像がありません。カメラを開始してください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub BtnSettings_Click(sender As Object, e As EventArgs) Handles BtnSettings.Click
        Using frm As New FrmSettings()
            ' 設定画面をダイアログとして開く
            If frm.ShowDialog(Me) = DialogResult.OK Then
                ' ★ 設定画面で「OK」が押されたら、最新の設定を変数とエンジンに再読み込み！
                ReloadDetectSettings()

                ' OKボタンで閉じられたら、設定変更のログだけを出す
                AddLog($"設定を更新しました。保存先: [{My.Settings.SaveDirectoryBase}]")

                ' ※検知音や保存先の設定は、次に検知・保存が行われる際に
                ' 自動的に新しい My.Settings が読み込まれるため、ここでのリセットは不要です。
            End If
        End Using
    End Sub

    Private Function GetMeteorDetectBaseDir() As String
        Dim basePath As String = My.Settings.SaveDirectoryBase
        If String.IsNullOrEmpty(basePath) Then
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
        End If
        Return Path.Combine(basePath, "MeteorDetect")
    End Function

    ''' <summary>
    ''' 常時録画用のベースフォルダを取得します。
    ''' </summary>
    Private Function GetContinuousRecordBaseDir() As String
        ' 設定画面で指定されたパスを取得
        Dim basePath As String = My.Settings.ContinuousDirectory

        ' もし設定画面で何も指定されていない場合（空欄の場合）は、
        ' 安全のため検知動画と同じ場所（Cドライブなど）をデフォルトとして返す
        If String.IsNullOrEmpty(basePath) Then
            basePath = GetMeteorDetectBaseDir()
        End If

        Return basePath
    End Function

    Private Sub DrawDetectionBoxes(ByVal frame As Mat, ByVal lines As List(Of LineSegmentPoint))
        If lines Is Nothing OrElse lines.Count = 0 Then Return

        Dim padding As Integer = 60
        Dim minX As Integer = Integer.MaxValue
        Dim minY As Integer = Integer.MaxValue
        Dim maxX As Integer = Integer.MinValue
        Dim maxY As Integer = Integer.MinValue

        For Each line In lines
            minX = Math.Min(minX, Math.Min(line.P1.X, line.P2.X))
            minY = Math.Min(minY, Math.Min(line.P1.Y, line.P2.Y))
            maxX = Math.Max(maxX, Math.Max(line.P1.X, line.P2.X))
            maxY = Math.Max(maxY, Math.Max(line.P1.Y, line.P2.Y))
        Next

        minX -= padding
        minY -= padding
        maxX += padding
        maxY += padding

        minX = Math.Max(0, minX)
        minY = Math.Max(0, minY)
        maxX = Math.Min(frame.Width - 1, maxX)
        maxY = Math.Min(frame.Height - 1, maxY)

        Dim pt1 As New OpenCvSharp.Point(minX, minY)
        Dim pt2 As New OpenCvSharp.Point(maxX, maxY)
        Cv2.Rectangle(frame, pt1, pt2, Scalar.Green, 2, LineTypes.AntiAlias)
    End Sub

    ' ==========================================================
    ' 【追加】検知枠付きの証拠写真（JPEG）を保存する処理
    ' ==========================================================
    Private Sub SaveDetectionSnapshot(ByVal baseFrame As Mat, ByVal lines As List(Of LineSegmentPoint))
        Try
            ' ベースとなる映像のコピーを作成（元の映像を汚さないため）
            Using snapClone = baseFrame.Clone()

                ' 枠を描画する関数を呼び出して、緑の枠を焼き付ける
                DrawDetectionBoxes(snapClone, lines)

                ' 保存先フォルダの準備
                Dim baseDir As String = GetMeteorDetectBaseDir()
                Dim dateDir As String = IO.Path.Combine(baseDir, DateTime.Now.ToString("yyyy-MM-dd"))
                If Not IO.Directory.Exists(dateDir) Then IO.Directory.CreateDirectory(dateDir)

                ' ファイル名（例：Detect_235959.jpg）
                Dim fileName As String = "Detect_" & DateTime.Now.ToString("HHmmss") & ".jpg"
                Dim fullPath As String = IO.Path.Combine(dateDir, fileName)

                ' 画像として書き込む
                Cv2.ImWrite(fullPath, snapClone)
                AddLog($"検知枠付きの証拠写真を保存しました: {fileName}")
            End Using
        Catch ex As Exception
            AddLog($"証拠写真の保存エラー: {ex.Message}")
        End Try
    End Sub

    ' ==========================================================
    ' 【メイン画面から直接呼び出し】マスク編集ボタンの処理
    ' ==========================================================
    Private Sub BtnEditMaskDirect_Click(sender As Object, e As EventArgs) Handles BtnEditMaskDirect.Click
        ' カメラ映像が映っていないと下敷きが作れないのでチェック
        If PicMain.Image Is Nothing Then
            MessageBox.Show("カメラを開始して、映像が映っている状態で設定してください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' 現在の映像のコピー（スナップショット）を下敷きとして渡して、マスク設定画面を開く
        Using frm As New FrmMask()
            ' PicMainの現在のImageを新しいBitmapとして複製（Dispose対策）
            Using bgImage = New Bitmap(PicMain.Image)
                frm.Setup(bgImage)

                ' モーダルダイアログとして開く
                If frm.ShowDialog(Me) = DialogResult.OK Then
                    ' 保存して閉じられた場合、リアルタイムでマスクを再読み込み
                    Dim maskPath As String = IO.Path.Combine(Application.StartupPath, "mask.jpg")
                    _detector.LoadMask(maskPath)

                    ' 急激な変化による誤検知を防ぐため、検知エンジンをリセット
                    _detector.Reset()

                    AddLog("マスク設定を更新し、エンジンをリセットしました。")
                End If
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' 保存先ドライブの空き容量が十分にあるかチェックする
    ''' </summary>
    Private Function IsDiskSpaceEnough() As Boolean
        Try
            Dim saveDir As String = GetMeteorDetectBaseDir()
            ' まだフォルダがない場合は作成を試みて、親パスを取得
            If Not Directory.Exists(saveDir) Then Directory.CreateDirectory(saveDir)

            Dim driveInfo As New IO.DriveInfo(IO.Path.GetPathRoot(saveDir))
            Dim freeMB As Long = driveInfo.AvailableFreeSpace / 1024 / 1024

            If freeMB < BasConst.MIN_FREE_SPACE_MB Then
                AddLog($"【警告】空き容量不足です (残り {freeMB}MB)。録画をスキップします。")
                Return False
            End If
            Return True
        Catch ex As Exception
            AddLog($"容量チェックエラー: {ex.Message}")
            Return False ' エラー時は安全のため録画しない
        End Try
    End Function

    ' ==========================================================
    ' 【追加】ミュートチェックボックスのON/OFF処理
    ' ==========================================================
    Private Sub ChkMuteSound_CheckedChanged(sender As Object, e As EventArgs) Handles ChkMuteSound.CheckedChanged
        _isSoundMuted = ChkMuteSound.Checked
        If _isSoundMuted Then
            AddLog("検知音をミュート（消音）にしました。")
        Else
            AddLog("検知音のミュートを解除しました。")
        End If
    End Sub

    ' ==========================================================
    ' 接続 / 切断ボタン (BtnConnect) の処理
    ' ==========================================================
    Private Sub BtnConnect_Click(sender As Object, e As EventArgs) Handles BtnConnect.Click
        ' --- A. 切断処理（すでに繋がっている場合） ---
        If CamID <> -1 Then
            Try
                ' ★切断時は即座にタイマーを止める
                TmrUIUpdate.Enabled = False

                ' キャプチャ中なら止めるフラグを立てる
                IsCapturing = False

                ' AsiCameraクラスに切断とメモリ解放をすべて任せる
                AsiCam.Disconnect()
                CamID = -1

                ' UIを初期状態に戻す
                BtnConnect.Text = "接続"
                BtnConnect.BackColor = Color.FromKnownColor(KnownColor.Control)
                BtnStart.Enabled = False
                BtnStop.Enabled = False
                BtnRefreshZwo.Enabled = True
                CmbZwoDeviceList.Enabled = True
                CmbBinning.Enabled = False ' 切断時は選択不可

                AddLog("ZWOカメラを切断しました。")

            Catch ex As Exception
                MessageBox.Show("切断処理中にエラーが発生しました: " & ex.Message)
            End Try
            Return
        End If

        ' --- B. 接続処理（まだ繋がっていない場合） ---
        Try
            ' 1. バリデーション：カメラが選択されているか
            Dim camIndex As Integer = CmbZwoDeviceList.SelectedIndex
            If camIndex = -1 Then
                MessageBox.Show("カメラを選択してください。")
                Return
            End If

            ' ==========================================================
            ' ★抜本的修正：接続する前に「Index」を使って情報を取得する
            ' ==========================================================
            Dim info As New ASI_SDK.ASI_CAMERA_INFO()
            If ASI_SDK.ASIGetCameraProperty(info, camIndex) <> 0 Then ' 
                Throw New Exception("カメラ情報の取得に失敗しました。")
            End If

            ' SDKから返ってきた「本当の操作用ID」と「冷却の有無」を記憶
            Dim realCamID As Integer = info.CameraID ' 
            HasCooler = (info.IsCoolerCam = 1) ' 
            CamID = realCamID

            ' 2. AsiCameraクラスを通じて接続実行（本当のIDを渡す）
            ' (内部で ASIOpenCamera / ASIInitCamera が走ります)
            If Not AsiCam.Connect(CamID) Then
                Throw New Exception("カメラのオープンまたは初期化に失敗しました。")
            End If

            ' 3. ビニングリストの構築 (カメラの能力に合わせてCmbBinningを更新)
            CmbBinning.Items.Clear()
            Dim bins = AsiCam.GetSupportedBins(CamID)
            For Each b In bins
                CmbBinning.Items.Add($"{b} (Bin{b}x{b})")
            Next

            ' 初期選択を Bin1 にセット
            If CmbBinning.Items.Count > 0 Then
                _isUpdatingUI = True
                CmbBinning.SelectedIndex = 0
                _isUpdatingUI = False
            End If

            ' 【ここを追加】カメラのスペックに合わせてトラックバーの範囲を自動設定
            BasFunction.InitializeCameraTrackBars()

            ' ==========================================================
            ' 4. 接続時の初期パラメータ送信 (露出 60ms, Gain Auto, WB Auto)
            ' ==========================================================

            ' UI操作によって勝手にイベント（カメラへの送信）が走るのを防ぐ
            _isUpdatingUI = True

            ' 画面のチェックボックスを希望の状態にセット
            Me.ChkExpAuto.Checked = False ' 露出は固定
            Me.ChkGainAuto.Checked = True ' ゲインはオート
            Me.ChkWBAuto.Checked = True   ' WBはオート

            ' トラックバーの露出を60msにセット（範囲外エラー防止付き）
            If 33 >= Me.TrkbExp.Minimum AndAlso 60 <= Me.TrkbExp.Maximum Then
                Me.TrkbExp.Value = 60
            End If

            ' ★追加：トラックバーのゲインを 0 にセット
            If 0 >= Me.TrkbGain.Minimum AndAlso 0 <= Me.TrkbGain.Maximum Then
                Me.TrkbGain.Value = 0
            End If

            ' ★追加：トラックバーのホワイトバランス(R/B)を 50 にセット
            If 50 >= Me.TrkbWbR.Minimum AndAlso 50 <= Me.TrkbWbR.Maximum Then Me.TrkbWbR.Value = 50
            If 50 >= Me.TrkbWbB.Minimum AndAlso 50 <= Me.TrkbWbB.Maximum Then Me.TrkbWbB.Value = 50

            ' カメラ本体へ設定値を直接送信
            ' 露出: 60ms (60000μs), オートOFF(0)
            Dim ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_EXPOSURE, 60000, 0)

            ' ★変更：ゲインは 0 を渡しつつ、オートON(1) にする
            ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_GAIN, 0, 1)

            ' ★変更：ホワイトバランス(赤・青): 50 を渡しつつ、オートON(1) にする
            ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_WB_R, 50, 1)
            ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_WB_B, 50, 1)

            ' ==========================================================
            ' ★修正：固定値(3)をやめて、My.Settings から読み込む
            ' ==========================================================
            ' 変更後: ↓ 常に「0」を送って、カメラ本体の反転を封印します
            ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_FLIP, 0, 0)
            ' ==========================================================

            ' UI更新ロックを解除
            _isUpdatingUI = False

            ' ==========================================================
            ' 5. USB速度設定の反映（My.Settingsからロード）
            ' ==========================================================
            Dim hsVal As Integer = If(My.Settings.ZwoHighSpeed, 1, 0)
            ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_HIGH_SPEED_MODE, hsVal, 0)
            ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_BANDWIDTHOVERLOAD, My.Settings.ZwoUsbBandwidth, 0)

            ' ==========================================================
            ' 6. 冷却機能の確認と反映
            ' ==========================================================
            ' ※ infoの取得と _hasCooler の判定は一番上に移動させたので、ここでは送るだけでOKです
            If HasCooler Then
                AddLog("冷却対応カメラを検知しました。")

                ' 初期パラメータの送信
                'ASI_SDK.ASISetControlValue(_camID, ASI_SDK.ASI_CONTROL_TYPE.ASI_TARGET_TEMP, My.Settings.ZwoTargetTemp, 0)
                Dim coolerVal As Integer = If(My.Settings.ZwoCoolerOn, 1, 0)
                ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_COOLER_ON, coolerVal, 0)
                If ret <> 0 Then AddLog($"冷却ON失敗。エラーコード: {ret} (12V電源を確認してください)")
                ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_TARGET_TEMP, My.Settings.ZwoTargetTemp, 0)


                AddLog($"冷却設定を反映しました: {If(My.Settings.ZwoCoolerOn, "ON", "OFF")} (目標 {My.Settings.ZwoTargetTemp}℃)")
            Else
                AddLog("冷却非対応カメラです。")
            End If

            ' 7. UIスライダーやラベルを現在のカメラ状態に同期
            ' (BasFunction内の共通関数を呼び出し)
            BasFunction.UpdateUIFromCamera()

            ' 8. UI表示の最終更新
            BtnConnect.Text = "切断"
            BtnConnect.BackColor = Color.MistyRose
            BtnStart.Enabled = True
            BtnRefreshZwo.Enabled = False
            CmbZwoDeviceList.Enabled = False
            CmbBinning.Enabled = True ' 接続完了したのでビニング変更可能

            AddLog($"ZWOカメラ [{info.Name}] に接続しました。解像度: {info.MaxWidth}x{info.MaxHeight}")

            ' ★タイマーを起動してUI監視をスタート
            TmrUIUpdate.Enabled = True

        Catch ex As Exception
            ' 失敗時は念のため切断処理を呼んでリセット
            AsiCam.Disconnect()
            CamID = -1
            MessageBox.Show("接続に失敗しました: " & ex.Message)
            AddLog("接続失敗: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnRefreshZwo_Click(sender As Object, e As EventArgs) Handles BtnRefreshZwo.Click
        RefreshZwoCameraList()
    End Sub

    ' ==========================================================
    ' ZWO ASIカメラのリストを更新してコンボボックスにセット
    ' ==========================================================
    Private Sub RefreshZwoCameraList()
        ' すでに接続中の場合はリストを更新しない（誤操作防止）
        If CamID <> -1 Then Return

        CmbZwoDeviceList.Items.Clear()

        Try
            ' 1. SDKを使ってPCに繋がっているZWOカメラの台数を取得
            Dim numCameras As Integer = ASI_SDK.ASIGetNumOfConnectedCameras()

            If numCameras > 0 Then
                ' 2. 台数分ループして、カメラの名前を取得する
                For i As Integer = 0 To numCameras - 1
                    Dim info As New ASI_SDK.ASI_CAMERA_INFO()
                    Dim ret As Integer = ASI_SDK.ASIGetCameraProperty(info, i)
                    If ret <> 0 Then
                        AddLog($"ZWOカメラのプロパティ取得に失敗しました (エラーコード: {ret})")
                    End If

                    CmbZwoDeviceList.Items.Add(info.Name)
                Next

                ' 初期選択を1番目のカメラにする
                CmbZwoDeviceList.SelectedIndex = 0
                BtnConnect.Enabled = True

                ' ★ラベルの代わりにログへ出力
                AddLog($"{numCameras}台のZWOカメラが見つかりました。")
            Else
                BtnConnect.Enabled = False

                ' ★ラベルの代わりにログへ出力
                AddLog("ZWOカメラが見つかりません。")
            End If

        Catch ex As Exception
            MessageBox.Show("ZWOカメラリストの取得に失敗しました: " & ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    ' --- ゲイン変更 ---
    Private Sub TrkbGain_Scroll(sender As Object, e As EventArgs) Handles TrkbGain.Scroll
        If CamID = -1 Then Return
        Dim ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_GAIN, TrkbGain.Value, 0)
        If ret <> 0 Then
            AddLog($"カメラへの設定送信に失敗しました (エラーコード: {ret})")
        End If

        LblGain.Text = $"Gain: {TrkbGain.Value}"

        ' カメラ未接続、またはオートON時、プログラムからの自動更新中は無視
        If CamID = -1 OrElse ChkGainAuto.Checked OrElse _isUpdatingUI Then Return

        ' 手動モードの時のみ、カメラにスライダーの値を送る
        ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_GAIN, TrkbGain.Value, 0)
        If ret <> 0 Then
            AddLog($"カメラへの設定送信に失敗しました (エラーコード: {ret})")
        End If

        LblGain.Text = $"Gain: {TrkbGain.Value}"
    End Sub

    ' --- 露出変更 (msをμsに変換して送信) ---
    Private Sub TrkbExp_Scroll(sender As Object, e As EventArgs) Handles TrkbExp.Scroll
        ' ★ここに OrElse _isUpdatingUI を追加
        If CamID = -1 OrElse ChkExpAuto.Checked OrElse _isUpdatingUI Then Return

        ' ミリ秒単位の値を1000倍してマイクロ秒として送る
        Dim exposureUs As Long = CLng(TrkbExp.Value) * 1000
        Dim ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_EXPOSURE, exposureUs, 0)
        If ret <> 0 Then
            AddLog($"カメラへの設定送信に失敗しました (エラーコード: {ret})")
        End If

        LblExp.Text = $"Exp: {TrkbExp.Value} ms"
    End Sub

    ' ==========================================================
    ' ゲイン Auto 切り替え
    ' ==========================================================
    Private Sub ChkGainAuto_CheckedChanged(sender As Object, e As EventArgs) Handles ChkGainAuto.CheckedChanged
        ' ★ 最優先でスライダーのロック状態を切り替える
        TrkbGain.Enabled = Not ChkGainAuto.Checked

        ' その後で通信ブロック判定を行う
        If CamID = -1 OrElse _isUpdatingUI Then Return

        Dim isAuto As Integer = If(ChkGainAuto.Checked, 1, 0)

        If isAuto = 0 Then
            Dim currentVal As Integer = 0
            Dim tempAuto As Integer = 0
            Dim ret = ASI_SDK.ASIGetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_GAIN, currentVal, tempAuto)

            ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_GAIN, currentVal, 0)

            _isUpdatingUI = True
            If currentVal >= TrkbGain.Minimum AndAlso currentVal <= TrkbGain.Maximum Then
                TrkbGain.Value = currentVal
            End If
            _isUpdatingUI = False

            LblGain.Text = $"Gain: {TrkbGain.Value}"
        Else
            Dim ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_GAIN, TrkbGain.Value, 1)
            LblGain.Text = $"Gain: {TrkbGain.Value} (Auto)"
        End If
    End Sub

    ' ==========================================================
    ' 露出 Auto 切り替え
    ' ==========================================================
    Private Sub ChkExpAuto_CheckedChanged(sender As Object, e As EventArgs) Handles ChkExpAuto.CheckedChanged
        ' ★ 最優先でスライダーのロック状態を切り替える
        TrkbExp.Enabled = Not ChkExpAuto.Checked

        ' その後で通信ブロック判定を行う
        If CamID = -1 OrElse _isUpdatingUI Then Return

        Dim isAuto As Integer = If(ChkExpAuto.Checked, 1, 0)

        If isAuto = 0 Then
            Dim currentUs As Integer = 0
            Dim tempAuto As Integer = 0
            Dim ret = ASI_SDK.ASIGetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_EXPOSURE, currentUs, tempAuto)

            ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_EXPOSURE, currentUs, 0)

            Dim currentMs As Integer = CInt(currentUs / 1000)
            _isUpdatingUI = True
            If currentMs >= TrkbExp.Minimum AndAlso currentMs <= TrkbExp.Maximum Then
                TrkbExp.Value = currentMs
            End If
            _isUpdatingUI = False

            LblExp.Text = $"Exp: {TrkbExp.Value} ms"
        Else
            Dim currentUs As Integer = TrkbExp.Value * 1000
            Dim ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_EXPOSURE, currentUs, 1)
            LblExp.Text = $"Exp: {TrkbExp.Value} ms (Auto)"
        End If
    End Sub
    ' ==========================================================
    ' ホワイトバランス Auto 切り替え
    ' ==========================================================
    Private Sub ChkWBAuto_CheckedChanged(sender As Object, e As EventArgs) Handles ChkWBAuto.CheckedChanged
        ' ★ 最優先でスライダーのロック状態を切り替える
        TrkbWbR.Enabled = Not ChkWBAuto.Checked
        TrkbWbB.Enabled = Not ChkWBAuto.Checked

        ' その後で通信ブロック判定を行う
        If CamID = -1 OrElse _isUpdatingUI Then Return

        Dim isAuto As Integer = If(ChkWBAuto.Checked, 1, 0)

        If isAuto = 0 Then
            Dim valR As Integer = 0, valB As Integer = 0
            Dim tempAuto As Integer = 0
            Dim Getret = ASI_SDK.ASIGetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_WB_R, valR, tempAuto)
            Getret = ASI_SDK.ASIGetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_WB_B, valB, tempAuto)

            Dim ret As Integer = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_WB_R, valR, 0)
            If ret <> 0 Then
                AddLog($"カメラへの白色バランスR設定送信に失敗しました (エラーコード: {ret})")
            End If

            ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_WB_B, valB, 0)
            If ret <> 0 Then
                AddLog($"カメラへの白色バランスB設定送信に失敗しました (エラーコード: {ret})")
            End If

            _isUpdatingUI = True
            If valR >= TrkbWbR.Minimum AndAlso valR <= TrkbWbR.Maximum Then TrkbWbR.Value = valR
            If valB >= TrkbWbB.Minimum AndAlso valB <= TrkbWbB.Maximum Then TrkbWbB.Value = valB
            _isUpdatingUI = False

            LblWb.Text = $"WB(R:B): {TrkbWbR.Value}:{TrkbWbB.Value}"
        Else
            Dim ret1 = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_WB_R, TrkbWbR.Value, 1)
            Dim ret2 = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_WB_B, TrkbWbB.Value, 1)
            If ret1 <> 0 OrElse ret2 <> 0 Then
                AddLog($"カメラへの白色バランス設定送信に失敗しました (エラーコード: {Math.Max(ret1, ret2)})")
            End If
            LblWb.Text = $"WB(R:B): {TrkbWbR.Value}:{TrkbWbB.Value} (Auto)"
        End If

    End Sub

    Private Sub TrkbWbR_Scroll(sender As Object, e As EventArgs) Handles TrkbWbR.Scroll
        ' ★ OrElse _isUpdatingUI を追加
        If CamID = -1 OrElse ChkWBAuto.Checked OrElse _isUpdatingUI Then Return

        ' 手動モードの時のみカメラに値を送信
        Dim ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_WB_R, TrkbWbR.Value, 0)
        If ret <> 0 Then
            AddLog($"カメラへの設定送信に失敗しました (エラーコード: {ret})")
        End If

        ' ★ スライダーを動かした瞬間にラベルも更新する
        LblWb.Text = $"WB(R:B): {TrkbWbR.Value}:{TrkbWbB.Value}"
    End Sub

    Private Sub TrkbWbB_Scroll(sender As Object, e As EventArgs) Handles TrkbWbB.Scroll
        ' ★ OrElse _isUpdatingUI を追加
        If CamID = -1 OrElse ChkWBAuto.Checked OrElse _isUpdatingUI Then Return

        ' 手動モードの時のみカメラに値を送信
        Dim ret = ASI_SDK.ASISetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_WB_B, TrkbWbB.Value, 0)
        If ret <> 0 Then
            AddLog($"カメラへの設定送信に失敗しました (エラーコード: {ret})")
        End If

        ' ★ スライダーを動かした瞬間にラベルも更新する
        LblWb.Text = $"WB(R:B): {TrkbWbR.Value}:{TrkbWbB.Value}"
    End Sub

    ' ==========================================================
    ' UI監視・FPS更新タイマー (Interval: 1000ms に設定してください)
    ' ==========================================================
    Private Sub TmrUIUpdate_Tick(sender As Object, e As EventArgs) Handles TmrUIUpdate.Tick
        If CamID = -1 Then Return

        ' ★修正：外部で変更中の場合は、UIの引き戻しを防ぐために更新をスキップ
        If IsExternalChanging Then
            _externalChangeTimer += 1
            ' 5秒経過したら自動的に監視を再開
            If _externalChangeTimer > 5 Then
                IsExternalChanging = False
                _externalChangeTimer = 0
            End If
        Else
            ' オート設定がONの時だけ、カメラに変更後の値を問い合わせる
            If ChkExpAuto.Checked OrElse ChkGainAuto.Checked OrElse ChkWBAuto.Checked Then
                BasFunction.UpdateUIFromCamera()
            End If
        End If

        ' --- センサー温度と冷却パワーの表示（ここは常に動かしてOK） ---
        Dim tempX10 As Integer = 0
        Dim isAuto As Integer = 0
        Dim coolerPower As Integer = 0 ' 冷却パワー(0-255)

        ' 現在の温度取得
        Dim ret = ASI_SDK.ASIGetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_TEMPERATURE, tempX10, isAuto)
        If ret <> 0 Then
            AddLog($"カメラからの温度取得に失敗しました (エラーコード: {ret})")
        End If

        ' 現在の冷却パワー取得（これが0なら冷却が動いていない証拠）
        ret = ASI_SDK.ASIGetControlValue(CamID, ASI_SDK.ASI_CONTROL_TYPE.ASI_COOLER_POWER, coolerPower, isAuto)
        If ret <> 0 Then
            AddLog($"カメラからの冷却パワー取得に失敗しました (エラーコード: {ret})")
        End If

        LblStatusTemp.Text = $"Sensor: {tempX10 / 10.0:F1} ℃ (Power: {coolerPower})"
    End Sub

    ' ==========================================================
    ' 表示・録画用 画質調整スライダー
    ' ==========================================================
    Private Sub TrkbDisplayAlpha_Scroll(sender As Object, e As EventArgs) Handles TrkbDisplayAlpha.Scroll
        ' 10～50の値を、1.0～5.0のDouble型に変換する
        _displayAlpha = TrkbDisplayAlpha.Value / 10.0
        LblDisplayAlpha.Text = $"表示コントラスト: {_displayAlpha:F1}"
    End Sub

    Private Sub TrkbDisplayBeta_Scroll(sender As Object, e As EventArgs) Handles TrkbDisplayBeta.Scroll
        ' 0～100の値をそのまま使用
        _displayBeta = TrkbDisplayBeta.Value
        LblDisplayBeta.Text = $"表示明るさ: {_displayBeta}"
    End Sub

    ' ==========================================================
    ' 常時録画専用の裏方スレッド（ディスク書き込みを非同期で行う）
    ' ==========================================================
    Private Sub ContinuousRecordLoop()
        Try
            While _continuousCts IsNot Nothing AndAlso Not _continuousCts.IsCancellationRequested
                Dim frameToRecord As Mat = Nothing

                ' キューから画像を取り出す（最大100ms待機）
                If _continuousQueue.TryTake(frameToRecord, 100, _continuousCts.Token) Then
                    Try
                        ' チェックボックスがOFFならファイルを閉じて何もしない
                        If Not ChkContinuousRecord.Checked Then
                            If _continuousWriter IsNot Nothing Then
                                _continuousWriter.Dispose()
                                _continuousWriter = Nothing
                                _lastRecordedMinute = ""
                                AddLog("常時録画を停止しました。")
                            End If
                            Continue While
                        End If

                        ' --- ファイル切り替え＆書き込みロジック ---
                        Dim nowTime As DateTime = DateTime.Now
                        Dim currentMinuteStr As String = nowTime.ToString("mm")

                        If _continuousWriter Is Nothing OrElse _lastRecordedMinute <> currentMinuteStr Then
                            If _continuousWriter IsNot Nothing Then _continuousWriter.Dispose()

                            Dim dateDir As String = nowTime.ToString("yyyyMMdd")
                            Dim hourDir As String = nowTime.ToString("HH")
                            Dim contDir As String = IO.Path.Combine(GetContinuousRecordBaseDir(), "record", dateDir, hourDir)

                            If Not IO.Directory.Exists(contDir) Then IO.Directory.CreateDirectory(contDir)

                            Dim contPath As String = IO.Path.Combine(contDir, $"{currentMinuteStr}.mp4")
                            Dim recFps As Double = If(_currentFps > 0, _currentFps, 13.0)

                            _continuousWriter = New VideoWriter(contPath, FourCC.FromString(BasConst.VIDEO_CODEC), recFps, New Size(frameToRecord.Width, frameToRecord.Height))
                            _lastRecordedMinute = currentMinuteStr

                            If currentMinuteStr = "00" Then
                                AddLog($"常時録画: {dateDir}/{hourDir} フォルダの録画を開始しました。")
                            End If
                        End If

                        If _continuousWriter IsNot Nothing AndAlso _continuousWriter.IsOpened() Then
                            _continuousWriter.Write(frameToRecord)
                        End If

                    Finally
                        ' 書き込み終わったMatは確実にメモリ解放
                        If frameToRecord IsNot Nothing Then frameToRecord.Dispose()
                    End Try
                End If
            End While

        Catch ex As OperationCanceledException
            ' スレッド終了時の正常な例外
        Catch ex As Exception
            AddLog($"常時録画ループエラー: {ex.Message}")
        Finally
            ' ループ終了時にファイルを確実に閉じる
            If _continuousWriter IsNot Nothing Then
                _continuousWriter.Dispose()
                _continuousWriter = Nothing
                _lastRecordedMinute = ""
                AddLog("常時録画ファイルを安全に閉じました。")
            End If
        End Try
    End Sub

    ' ガンマスライダーを動かした時の処理
    Private Sub TrcGamma_Scroll(sender As Object, e As EventArgs) Handles TrcGamma.Scroll
        ' スライダーの値(10～30)を、1.0～3.0の少数に変換する
        _displayGamma = TrcGamma.Value / 10.0

        ' ラベルの文字も更新する
        LblGamma.Text = $"ガンマ(Def:1.2): {_displayGamma:F1}"

        ' ★ スライダーを動かした時だけLUTを作り直す
        UpdateGammaLut()
    End Sub

    ' ==========================================================
    ' ガンマ補正用LUTを作り直すメソッド（スライダー操作時のみ実行）
    ' ==========================================================
    Private Sub UpdateGammaLut()
        ' 新しいテーブルを作成
        Dim newLut As New Mat(1, 256, MatType.CV_8UC1)
        Dim indexer = newLut.GetGenericIndexer(Of Byte)()

        For i As Integer = 0 To 255
            Dim val As Double = Math.Pow(i / 255.0, _displayGamma) * 255.0
            indexer(0, i) = CByte(Math.Max(0, Math.Min(255, val)))
        Next

        ' 古いLUTを破棄して、新しいLUTに差し替える（安全のためにロックをかける）
        SyncLock _lutLock
            If _gammaLut IsNot Nothing AndAlso Not _gammaLut.IsDisposed Then
                _gammaLut.Dispose()
            End If
            _gammaLut = newLut
        End SyncLock
    End Sub


    ' 設定を変数に読み込み、検知エンジンにセットする専用メソッド
    Private Sub ReloadDetectSettings()
        _enableDetect = My.Settings.SysEnableDetect
        _detectThreshold = My.Settings.SysThreshold
        _minLineLength = My.Settings.SysMinLength
        _accSeconds = My.Settings.SysAccSec
        _maxLineGap = My.Settings.SysLineGap
        _houghThreshold = My.Settings.SysHough
        _preRollSec = My.Settings.SysPreRoll
        _postRollSec = My.Settings.SysPostRoll

        ' もし検知器が動いていれば即座に反映
        If _detector IsNot Nothing Then
            _detector.AccumulateSeconds = _accSeconds
            _detector.MaxLineGap = _maxLineGap
            _detector.HoughThreshold = _houghThreshold
        End If
        _detector.EnableClahe = My.Settings.SysEnableClahe
        _detector.ClaheClipLimit = My.Settings.SysClaheClip
    End Sub

    Private Sub ChkEnableDetect_CheckedChanged(sender As Object, e As EventArgs) Handles ChkEnableDetect.CheckedChanged
        _enableDetect = ChkEnableDetect.Checked
    End Sub

    Private Sub BtnZwoSettings_Click(sender As Object, e As EventArgs) Handles BtnZwoSettings.Click
        Using frm As New FrmZWO_Settings()
            If frm.ShowDialog(Me) = DialogResult.OK Then
                AddLog("ZWOカメラの詳細設定を更新しました。")
            End If
        End Using
    End Sub


    ' ==========================================================
    ' ファイルの書き込みが完全に終わってロックが解除されるまで待機する関数
    ' ==========================================================
    Private Function WaitForFileReady(ByVal filePath As String, ByVal timeoutMs As Integer) As Boolean
        Dim sw As New Stopwatch()
        sw.Start()

        While sw.ElapsedMilliseconds < timeoutMs
            Try
                ' ファイルを「独占状態（他から書き込まれていない状態）」で開けるかテストする
                Using fs As New IO.FileStream(filePath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.None)
                    ' エラーにならずに開けたら、ファイルは完全に完成している！
                    Return True
                End Using
            Catch ex As IO.IOException
                ' まだロックされている（書き込み中やスキャン中）場合は、0.1秒だけ待って再挑戦
                System.Threading.Thread.Sleep(100)
            End Try
        End While

        ' 指定した時間を過ぎても開けなかった場合はタイムアウト
        Return False
    End Function

    Private Sub RdoRtsp_CheckedChanged(sender As Object, e As EventArgs) Handles RdoRtsp.CheckedChanged
        If RdoRtsp.Checked Then
            IsAsiMode = False
            UpdateSettingsPanel() ' パネルを最前面へ切り替え
        End If
    End Sub
End Class