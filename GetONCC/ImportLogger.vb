Imports System.IO

Public Class ImportLogger
    Private mLogWriter As StreamWriter
    Private mIsClosed As Boolean = True
    Public Sub New(ByVal logFilename As String)
        mLogWriter = File.CreateText(logFilename)
        mIsClosed = False
    End Sub
    Public Sub WriteLog(ByVal content As String)
        mLogWriter.WriteLine(content)
    End Sub
    Public Sub WriteLogWithTimeStamp(ByVal content As String)
        mLogWriter.WriteLine(Format(Now, "yyyy/MM/dd hh:mm:ss ") + content)
    End Sub
    Public Sub Close()
        If (mIsClosed = False) Then
            mLogWriter.Flush()
            mLogWriter.Close()
            mIsClosed = True
        End If
    End Sub
End Class
