Imports System.Threading
Imports System.Collections.Concurrent ' ★追加：スレッドセーフなキューを使うため
Imports OpenCvSharp

Public Class ClsRtspCamera
    Implements ICameraProvider

    Public Event FrameArrived(ByVal frame As Mat) Implements ICameraProvider.FrameArrived

    Private _capture As VideoCapture
    Private _isRunning As Boolean

    ' ★変更：スレッドを「受信担当」と「処理担当」の2つに分ける
    Private _captureThread As Thread
    Private _processThread As Thread

    ' ★追加：受信した画像を一時保管する「キュー（土管）」
    Private ReadOnly _frameQueue As New ConcurrentQueue(Of Mat)()

    Public Property ConnectionUrl As String = ""

    Public ReadOnly Property IsRunning As Boolean Implements ICameraProvider.IsRunning
        Get
            Return _isRunning
        End Get
    End Property

    Public Function StartCamera(deviceIndex As Integer) As Boolean Implements ICameraProvider.StartCamera
        If String.IsNullOrEmpty(ConnectionUrl) Then Return False

        Try
            ' キューの中身を空にしておく（再スタート時用）
            While Not _frameQueue.IsEmpty
                Dim oldFrame As Mat = Nothing
                If _frameQueue.TryDequeue(oldFrame) Then oldFrame.Dispose()
            End While

            _capture = New VideoCapture(ConnectionUrl, VideoCaptureAPIs.FFMPEG)
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
    ' 【生産者】カメラから最速で映像を受け取り、キューに入れるだけのループ
    ' ==========================================================
    Private Sub CaptureLoop()
        While _isRunning
            Try
                ' 最速で受信と解凍を行う
                If _capture.Grab() Then
                    Dim frame As New Mat()
                    If _capture.Retrieve(frame) AndAlso Not frame.Empty() Then

                        ' ★重要：キューに溜めすぎるとメモリがパンクするので、
                        ' 例えば「3枚」以上溜まっていたら一番古いものを捨てて常に最新を保つ
                        While _frameQueue.Count >= 3
                            Dim oldFrame As Mat = Nothing
                            If _frameQueue.TryDequeue(oldFrame) Then
                                oldFrame.Dispose()
                            End If
                        End While

                        ' 受け取った最新の画像をキューに放り込む
                        _frameQueue.Enqueue(frame)
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
    ' 【消費者】キューから画像を取り出して、メイン側に渡すだけのループ
    ' ==========================================================
    Private Sub ProcessLoop()
        While _isRunning
            Dim frame As Mat = Nothing

            ' キューの中に画像が入っていれば取り出す
            If _frameQueue.TryDequeue(frame) Then
                ' キューから取り出した時点で所有権が移るので、そのままメインへ渡す
                ' ※受け取った側（FrmMain等）で frame.Dispose() が必要なのはこれまで通りです
                RaiseEvent FrameArrived(frame)
            Else
                ' キューが空の場合は、CPUを休ませて少し待つ
                Thread.Sleep(5)
            End If
        End While
    End Sub

    Public Sub StopCamera() Implements ICameraProvider.StopCamera
        _isRunning = False

        ' 両方のスレッドの終了を待つ
        _captureThread?.Join(1000)
        _processThread?.Join(1000)

        _capture?.Release()
        _capture?.Dispose()
        _capture = Nothing

        ' 残ったキューの掃除
        While Not _frameQueue.IsEmpty
            Dim oldFrame As Mat = Nothing
            If _frameQueue.TryDequeue(oldFrame) Then oldFrame.Dispose()
        End While
    End Sub
End Class