Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Runtime.Serialization.Json
Imports System.Web
Public Class Form1

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Button1.Enabled = True
        Button2.Enabled = True

        TextBox1.Text = EncodeText(GetOnCC("5"))






    End Sub
    Private Function EncodeText(ByVal sText As String) As String
        Return WebUtility.HtmlEncode(sText)
    End Function

    ' Dim js As New System.Web.Script.Serialization.JavaScriptSerializer
    'Dim rawdata = js.DeserializeObject(textAreaJson)
    'Dim lstTextAreas As List(Of jsTextArea) = CType(rawdata, List(Of jsTextArea))
    Function Convert5(ByVal stock) As String
        Dim temp = ""
        If Len(stock) < 5 Then
            For padzero = 1 To 5 - Len(stock)
                temp = temp + "0"
                Debug.Print(temp)
            Next
        End If
        ' Response.Write temp & stock
        Convert5 = temp & stock
    End Function
    Function GetOnCC(ByVal SCTYCode) As String
        Dim stocktime As String = "real"
        Dim stocktype As String = "r"
        Dim sLine As String
        Dim nStart, nEnd As Integer
        Dim stkprice As String
        sLine = DownloadHTMLPage("http://money18.on.cc/js/" & stocktime & "/quote/" & Convert5(SCTYCode) & "_" & stocktype & ".js?t=1319009536902", "http://money18.on.cc/")

        Debug.Print(sLine)

        GetOnCC = sLine

    End Function






    Public Function DownloadHTMLPage(ByVal _URL As String, ByVal refer As String) As String
        Dim _PageContent As String = Nothing
        Try
            ' Open a connection
            Dim _HttpWebRequest As System.Net.HttpWebRequest = CType(System.Net.HttpWebRequest.Create(_URL), System.Net.HttpWebRequest)

            ' You can also specify additional header values like the user agent or the referer: (Optional)
            _HttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)"
            _HttpWebRequest.Referer = refer
            ' "http://money18.on.cc/"

            ' set timeout for 10 seconds (Optional)
            _HttpWebRequest.Timeout = 10000

            ' Request response:
            Dim _WebResponse As System.Net.WebResponse = _HttpWebRequest.GetResponse()

            ' Open data stream:
            Dim _WebStream As System.IO.Stream = _WebResponse.GetResponseStream()

            ' Create reader object:
            Dim _StreamReader As New System.IO.StreamReader(_WebStream)

            ' Read the entire stream content:
            _PageContent = _StreamReader.ReadToEnd()

            ' Cleanup
            _StreamReader.Close()
            _WebStream.Close()
            _WebResponse.Close()
        Catch _Exception As Exception
            ' Error
            Console.WriteLine("Exception caught in process: {0}", _Exception.ToString())
            Return Nothing
        End Try

        Return _PageContent
    End Function

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
  TextBox1.Text = EncodeText(GetOnCC("5"))


    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        TextBox1.Text = EncodeText(GetOnCC("358"))

    End Sub
End Class




