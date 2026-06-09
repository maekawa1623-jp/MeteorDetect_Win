Imports OpenCvSharp

Public Class ClsImageBuffer
    ' ==========================================================
    ' 【修正】Frame に加えて、その瞬間の「検知枠の座標(Lines)」も一緒に記憶できるように変更
    ' ==========================================================
    Private _buffer As New LinkedList(Of (Timestamp As DateTime, Frame As Mat, Lines As List(Of LineSegmentPoint)))
    Private _maxSec As Double
    Private _lock As New Object() ' スレッド間の衝突を防ぐための鍵

    Public Sub New(maxSec As Double)
        _maxSec = maxSec
    End Sub

    ' ==========================================================
    ' 【修正】引数に currentLines を追加して、枠情報も受け取る
    ' ==========================================================
    Public Sub AddFrame(ByVal clonedFrame As Mat, ByVal currentLines As List(Of LineSegmentPoint))
        SyncLock _lock
            Dim nowTime = DateTime.Now

            ' 枠情報（線）の独立したコピーを作成（メインループ側で書き換わっても影響を受けないように）
            Dim linesCopy As New List(Of LineSegmentPoint)()
            If currentLines IsNot Nothing Then
                linesCopy.AddRange(currentLines)
            End If

            ' 映像と枠情報をセットにして追加
            _buffer.AddLast((nowTime, clonedFrame, linesCopy))

            ' 指定秒数より古いフレームを削除してメモリを解放
            While _buffer.Count > 0 AndAlso (nowTime - _buffer.First.Value.Timestamp).TotalSeconds > _maxSec
                Dim oldFrame = _buffer.First.Value.Frame
                If oldFrame IsNot Nothing AndAlso Not oldFrame.IsDisposed Then
                    oldFrame.Dispose()
                End If
                _buffer.RemoveFirst()
            End While
        End SyncLock
    End Sub

    ' ==========================================================
    ' 【修正】戻り値にも Lines（枠情報）を含めるように変更
    ' ==========================================================
    Public Function GetFrames(startTime As DateTime, endTime As DateTime) As List(Of (Timestamp As DateTime, Frame As Mat, Lines As List(Of LineSegmentPoint)))
        Dim result As New List(Of (Timestamp As DateTime, Frame As Mat, Lines As List(Of LineSegmentPoint)))
        SyncLock _lock
            ' ==========================================================
            ' 【超重要】保存スレッド用に「さらに専用のコピー(Clone)」を渡す
            ' 枠情報（Lines）も一緒にコピーして渡す
            ' ==========================================================
            For Each item In _buffer
                If item.Timestamp >= startTime AndAlso item.Timestamp <= endTime Then

                    ' 枠情報のコピー
                    Dim linesForSave As New List(Of LineSegmentPoint)()
                    If item.Lines IsNot Nothing Then
                        linesForSave.AddRange(item.Lines)
                    End If

                    ' 映像のコピーと枠情報のコピーをセットにして返す
                    result.Add((item.Timestamp, item.Frame.Clone(), linesForSave))
                End If
            Next
        End SyncLock
        Return result
    End Function

    ' 全クリア
    Public Sub Clear()
        SyncLock _lock
            For Each item In _buffer
                If item.Frame IsNot Nothing AndAlso Not item.Frame.IsDisposed Then
                    item.Frame.Dispose()
                End If
                If item.Lines IsNot Nothing Then
                    item.Lines.Clear()
                End If
            Next
            _buffer.Clear()
        End SyncLock
    End Sub
End Class