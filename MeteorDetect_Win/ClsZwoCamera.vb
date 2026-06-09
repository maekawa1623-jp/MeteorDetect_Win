Imports System.Runtime.InteropServices
Imports System.Threading
Imports OpenCvSharp

Public Class ClsZwoCamera
    Implements ICameraProvider ' ← これを追加

    ' ==========================================================
    ' ★ 共通インターフェースとしてのイベント
    ' ==========================================================
    'Public Event FrameArrived(ByVal frame As Mat)
    'Private Event ICameraProvider_FrameArrived(frame As Mat) Implements ICameraProvider.FrameArrived
    Public Event FrameArrived(ByVal frame As Mat) Implements ICameraProvider.FrameArrived

    ' --- クラス内部の変数 ---
    Private _cameraId As Integer = -1
    Private _camWidth As Integer  ' ← _width から名前を変更して衝突を回避
    Private _camHeight As Integer ' ← _height から名前を変更して衝突を回避
    Private _bufferSize As Integer
    Private _bufferPtr As IntPtr = IntPtr.Zero
    Private _isCapturing As Boolean = False

    ' --- プロパティ (VB.NETの正式なブロック構文に修正) ---
    Public ReadOnly Property Width As Integer
        Get
            Return _camWidth
        End Get
    End Property

    Public ReadOnly Property Height As Integer
        Get
            Return _camHeight
        End Get
    End Property

    Public ReadOnly Property IsRunning As Boolean
        Get
            Return _isCapturing
        End Get
    End Property

    Private ReadOnly Property ICameraProvider_IsRunning As Boolean Implements ICameraProvider.IsRunning
        Get
            Return IsRunning
        End Get
    End Property

    ' ==========================================================
    ' 接続と初期化
    ' ==========================================================
    Public Function Connect(cameraId As Integer) As Boolean
        If ASI_SDK.ASIOpenCamera(cameraId) <> 0 Then Return False
        If ASI_SDK.ASIInitCamera(cameraId) <> 0 Then Return False

        _cameraId = cameraId

        Dim info As New ASI_SDK.ASI_CAMERA_INFO()
        ASI_SDK.ASIGetCameraProperty(info, _cameraId)

        ' 新しい変数名を使用
        _camWidth = info.MaxWidth
        _camHeight = info.MaxHeight

        ' 初期状態としてBin1, RGB24を設定
        If ASI_SDK.ASISetROIFormat(_cameraId, _camWidth, _camHeight, 1, ASI_SDK.ASI_IMG_TYPE.ASI_IMG_RGB24) <> 0 Then
            Return False
        End If

        ' メモリバッファの確保
        AllocateBuffer(_camWidth, _camHeight)

        Return True
    End Function

    ' ==========================================================
    ' ビニング設定
    ' ==========================================================
    Public Function SetBinning(bin As Integer) As Boolean
        If _cameraId = -1 Then Return False

        Dim info As New ASI_SDK.ASI_CAMERA_INFO()
        ASI_SDK.ASIGetCameraProperty(info, _cameraId)

        Dim newWidth As Integer = info.MaxWidth \ bin
        Dim newHeight As Integer = info.MaxHeight \ bin

        Dim res = ASI_SDK.ASISetROIFormat(_cameraId, newWidth, newHeight, bin, ASI_SDK.ASI_IMG_TYPE.ASI_IMG_RGB24)

        If res = 0 Then
            _camWidth = newWidth
            _camHeight = newHeight
            AllocateBuffer(_camWidth, _camHeight)
            Return True
        End If
        Return False
    End Function

    Private Sub AllocateBuffer(w As Integer, h As Integer)
        If _bufferPtr <> IntPtr.Zero Then
            Marshal.FreeHGlobal(_bufferPtr)
            _bufferPtr = IntPtr.Zero
        End If
        _bufferSize = w * h * 3
        _bufferPtr = Marshal.AllocHGlobal(_bufferSize)
    End Sub

    ' ==========================================================
    ' 撮影（キャプチャループ）の開始・停止
    ' ==========================================================
    Public Function StartCamera(Optional dummyId As Integer = 0) As Boolean
        If _cameraId < 0 Then Return False

        If ASI_SDK.ASIStartVideoCapture(_cameraId) = 0 Then
            _isCapturing = True
            Task.Run(AddressOf CaptureLoopWorker)
            Return True
        End If
        Return False
    End Function

    Public Sub StopCamera()
        If _cameraId >= 0 AndAlso _isCapturing Then
            _isCapturing = False
            Thread.Sleep(200) ' ループが安全に終了するのを待機
            ASI_SDK.ASIStopVideoCapture(_cameraId)
        End If
    End Sub

    ' ==========================================================
    ' ★ 映像取得専用ループ
    ' ==========================================================
    Private Sub CaptureLoopWorker()
        Dim val As Integer = 0
        Dim isAuto As Integer = 0
        Dim waitMs As Integer = 1000

        ' 現在の露出時間を取得して、タイムアウト時間を安全に設定する
        If ASI_SDK.ASIGetControlValue(_cameraId, ASI_SDK.ASI_CONTROL_TYPE.ASI_EXPOSURE, val, isAuto) = 0 Then
            waitMs = CInt((val / 1000) * 2 + 500)
        End If

        While _isCapturing
            If _bufferPtr = IntPtr.Zero Then Continue While

            ' カメラから1フレーム分の生データを取得
            Dim status = ASI_SDK.ASIGetVideoData(_cameraId, _bufferPtr, _bufferSize, waitMs)

            If status = 0 Then ' 成功
                Using currentMat As Mat = Mat.FromPixelData(_camHeight, _camWidth, MatType.CV_8UC3, _bufferPtr)

                    ' ★重要：カメラの生データ（currentMat）を直接いじらず、クローンを作る
                    Dim outFrame As Mat = currentMat.Clone()

                    ' ★OpenCVの超高速処理でソフトウェア反転を行う（色は絶対に壊れません！）
                    Dim flipVal As Integer = My.Settings.ZwoFlip
                    If flipVal = 1 Then
                        Cv2.Flip(outFrame, outFrame, FlipMode.Y)  ' 左右反転
                    ElseIf flipVal = 2 Then
                        Cv2.Flip(outFrame, outFrame, FlipMode.X)  ' 上下反転
                    ElseIf flipVal = 3 Then
                        Cv2.Flip(outFrame, outFrame, FlipMode.XY) ' 180度回転 (上下左右)
                    End If

                    RaiseEvent FrameArrived(outFrame)
                End Using
            Else
                Debug.WriteLine($"Capture Error Code: {status}")
            End If
        End While
    End Sub

    ' ==========================================================
    ' 切断処理とメモリ解放
    ' ==========================================================
    Public Sub Disconnect()
        If _cameraId >= 0 Then
            StopCamera()
            ASI_SDK.ASICloseCamera(_cameraId)
            _cameraId = -1

            If _bufferPtr <> IntPtr.Zero Then
                Marshal.FreeHGlobal(_bufferPtr)
                _bufferPtr = IntPtr.Zero
                _bufferSize = 0
            End If
        End If
    End Sub

    Public Function GetSupportedBins(targetID As Integer) As List(Of Integer)
        Dim binList As New List(Of Integer)
        Try
            Dim info As New ASI_SDK.ASI_CAMERA_INFO()
            ASI_SDK.ASIGetCameraProperty(info, targetID)
            For Each b As Integer In info.SupportedBins
                If b = 0 Then Exit For
                binList.Add(b)
            Next
        Catch ex As Exception
        End Try
        Return binList
    End Function

    Private Function ICameraProvider_StartCamera(deviceIndex As Integer) As Boolean Implements ICameraProvider.StartCamera
        Return StartCamera(deviceIndex)
    End Function

    Private Sub ICameraProvider_StopCamera() Implements ICameraProvider.StopCamera
        StopCamera()
    End Sub

    ' ==========================================================
    ' デストラクタ（メモリ解放の最終防衛ライン）
    ' ==========================================================
    Protected Overrides Sub Finalize()
        If _bufferPtr <> IntPtr.Zero Then
            Marshal.FreeHGlobal(_bufferPtr)
            _bufferPtr = IntPtr.Zero
        End If
        MyBase.Finalize()
    End Sub
End Class