Imports System.Threading
Imports OpenCvSharp

Public Class ClsRtspCamera
    Implements ICameraProvider

    Public Event FrameArrived(ByVal frame As Mat) Implements ICameraProvider.FrameArrived

    Private _capture As VideoCapture
    Private _isRunning As Boolean

    Private _captureThread As Thread
    Private _processThread As Thread

    ' ★変更点：キューを廃止し、「最新の1枚」だけを保管する変数とロック用オブジェクトを用意
    Private _latestFrame As Mat = Nothing
    Private ReadOnly _frameLock As New Object()

    Public Property ConnectionUrl As String = ""

    Public ReadOnly Property IsRunning As Boolean Implements ICameraProvider.IsRunning
        Get
            Return _isRunning
        End Get
    End Property

    Public Function StartCamera(deviceIndex As Integer) As Boolean Implements ICameraProvider.StartCamera
        If String.IsNullOrEmpty(ConnectionUrl) Then Return False

        Try
            ' 古いフレームが残っていれば掃除しておく
            SyncLock _frameLock
                If _latestFrame IsNot Nothing Then
                    _latestFrame.Dispose()
                    _latestFrame = Nothing
                End If
            End SyncLock

            ' FFMPEGを指定してRTSPストリームを開く
            _capture = New VideoCapture(ConnectionUrl, VideoCaptureAPIs.FFMPEG)

            ' OpenCV側の内部バッファも極限まで減らす
            _capture.Set(VideoCaptureProperties.BufferSize, 1)

            If Not _capture.IsOpened() Then Return False

            _isRunning = True

            ' 1. 受信専用スレッド（生産者）をスタート
            _captureThread = New Thread(AddressOf CaptureLoop) With {.IsBackground = True}
            _captureThread.Start()

            ' 2. 処理専用スレッド（消費者）をスタート
            _processThread = New Thread(AddressOf ProcessLoop) With {.IsBackground = True}
            _processThread.Start()

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ' ==========================================================
    ' 【生産者】カメラから最速で映像を受け取り、最新の1枚だけを上書きする
    ' ==========================================================
    Private Sub CaptureLoop()
        While _isRunning AndAlso _capture IsNot Nothing
            Try
                ' 最速で受信と解凍を行う
                If _capture.Grab() Then
                    Dim frame As New Mat()
                    If _capture.Retrieve(frame) AndAlso Not frame.Empty() Then

                        SyncLock _frameLock
                            ' ★重要：もし未処理の古いフレームが残っていたら、容赦なく破棄！
                            If _latestFrame IsNot Nothing Then
                                _latestFrame.Dispose()
                            End If

                            ' 常に「今届いた最新のフレーム」だけをセットする
                            _latestFrame = frame

                            ' 処理スレッドに「新しい画像が来たよ！」と合図を送る
                            Monitor.Pulse(_frameLock)
                        End SyncLock

                    Else
                        frame.Dispose()
                    End If
                Else
                    Thread.Sleep(5)
                End If
            Catch ex As Exception
                Thread.Sleep(100)
            End Try
        End While
    End Sub

    ' ==========================================================
    ' 【消費者】最新の1枚を受け取り、メインへ渡す
    ' ==========================================================
    Private Sub ProcessLoop()
        While _isRunning
            Dim frameToProcess As Mat = Nothing

            SyncLock _frameLock
                ' 最新の画像が届くまで待機する（CPUの無駄遣いを完全に防ぐ）
                While _latestFrame Is Nothing AndAlso _isRunning
                    Monitor.Wait(_frameLock, 100)
                End While

                ' 最新の画像を取り出し、変数を空（Nothing）にする
                If _latestFrame IsNot Nothing Then
                    frameToProcess = _latestFrame
                    _latestFrame = Nothing
                End If
            End SyncLock

            ' キューの順番待ちが発生しないため、遅延ゼロの最新画像だけが送られる
            If frameToProcess IsNot Nothing Then
                RaiseEvent FrameArrived(frameToProcess)
            End If
        End While
    End Sub

    Public Sub StopCamera() Implements ICameraProvider.StopCamera
        _isRunning = False

        ' 両方のスレッドの終了を待つ
        _captureThread?.Join(1000)
        _processThread?.Join(1000)

        ' ロックして残っているフレームを掃除
        SyncLock _frameLock
            If _latestFrame IsNot Nothing Then
                _latestFrame.Dispose()
                _latestFrame = Nothing
            End If
        End SyncLock

        _capture?.Release()
        _capture?.Dispose()
        _capture = Nothing
    End Sub
End Class