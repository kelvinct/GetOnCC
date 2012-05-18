﻿Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Runtime.Serialization.Json
Imports System.Web
Imports System.Windows.Forms
Public Class Form1
    Dim dsStatus As New DataSet
    Dim ServerCount As Integer
    Dim dsXML As New DataSet
    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load


        '  TextBox1.Text = EncodeText(GetOnCC("5")) + vbNewLine
        ' Dim SLINE As String = "123"
        GetStatus()
    End Sub
    Private Function EncodeText(ByVal sText As String) As String
        Return WebUtility.HtmlEncode(sText)
    End Function
    Private Sub CallCurrentCellChanged()
        AddHandler DgStatus.CurrentCellChanged, AddressOf Grid_CurCellChange
    End Sub 'CallCurrentCellChanged

    ' Raise the event when focus on DataGrid cell changes.
    Private Sub Grid_CurCellChange(ByVal sender As Object, ByVal e As EventArgs)
        ' String variable used to show message.
        Dim myString As String = "CurrentCellChanged event raised, cell focus is at "
        ' Get the co-ordinates of the focussed cell.
        '   Dim myPoint As String = DgStatus.CurrentCell.ColumnNumber + "," + DgStatus.CurrentCell.RowNumber
        ' Create the alert message.
        '   myString = myString + "(" + myPoint + ")"
        ' Show Co-ordinates when CurrentCellChanged event is raised.
        MessageBox.Show(myString, "Current cell co-ordinates")
    End Sub 'Grid_CurCellChange

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
        Dim StockName
        Dim stkprice As String = 0

        stocktype = "r"
        sLine = DownloadHTMLPage("http://money18.on.cc/js/" & stocktime & "/quote/" & Convert5(SCTYCode) & "_" & stocktype & ".js?t=1319009536902", "http://money18.on.cc/")
        Dim SearchChar As String = "M18.r_"
        If InStr(sLine, SearchChar) > 0 Then


            nStart = InStr(1, sLine, SearchChar) + Len(SearchChar)
            Dim EndSearchChar As String = "="
            nEnd = InStr(1, sLine, EndSearchChar)
            StockName = Mid(sLine, nStart, nEnd - nStart)

            SearchChar = "np: '"
            nStart = InStr(nStart, sLine, SearchChar) + Len(SearchChar)
            nEnd = InStr(nStart, sLine, "'")
            stkprice = Mid(sLine, nStart, nEnd - nStart)


            sLine = DownloadHTMLPage("http://money18.on.cc/js/daily/quote/" & Convert5(SCTYCode) & "_d.js?t=1319009536902", "http://money18.on.cc/")
            SearchChar = "preCPrice:"""
            nStart = InStr(1, sLine, SearchChar) + Len(SearchChar)
            nEnd = InStr(nStart, sLine, """")
            Dim StkpreCPrice = Mid(sLine, nStart, nEnd - nStart)
            Debug.Print(StkpreCPrice)

            SearchChar = "nameChi:"""
            nStart = InStr(1, sLine, SearchChar) + Len(SearchChar)
            nEnd = InStr(nStart, sLine, """")
            Dim nameChi = (Mid(sLine, nStart, nEnd - nStart))
            Dim Change
            Change = ((stkprice - StkpreCPrice) / StkpreCPrice) * 100
            Change = FormatNumber((stkprice - StkpreCPrice), 3)
            GetOnCC = StockName + "," + nameChi + "," + stkprice + "," + Change


            Debug.Print(GetOnCC)
        Else

            GetOnCC = "null,null,null,null"
        End If
    End Function

    Public Function DownloadDiableStatus() As String
        Dim _PageContent As String = Nothing
        Try
            ' Open a connection
            Dim _HttpWebRequest As System.Net.HttpWebRequest = CType(System.Net.HttpWebRequest.Create("http://us.battle.net/d3/en/status"), System.Net.HttpWebRequest)

            _HttpWebRequest.ContentType = "application/x-www-form-urlencoded"
            ' You can also specify additional header values like the user agent or the referer: (Optional)
            _HttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)"
            _HttpWebRequest.Referer = "http://us.battle.net/d3/en/status"

            ' set timeout for 10 seconds (Optional)
            _HttpWebRequest.Timeout = 10000

            ' Request response:
            Dim _WebResponse As System.Net.WebResponse = _HttpWebRequest.GetResponse()

            ' Open data stream:
            Dim _WebStream As System.IO.Stream = _WebResponse.GetResponseStream()
            Dim encode As Encoding = System.Text.Encoding.GetEncoding("Big-5")
            ' Create reader object:
            Dim _StreamReader As New System.IO.StreamReader(_WebStream)
            Dim readStream As New StreamReader(_WebStream, encode)
            ' Read the entire stream content:
            _PageContent = readStream.ReadToEnd()

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
            Dim encode As Encoding = System.Text.Encoding.GetEncoding("Big5")
            ' Create reader object:
            Dim _StreamReader As New System.IO.StreamReader(_WebStream, encode)

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

    '  Private Sub Button1_Click(sender As System.Object, e As System.EventArgs)
    '     TextBox2.Text = ""
    '      TextBox1.Text = ""

    ' Dim SLINE As String = "123"
    ' End Sub

    ' Private Sub TextBox2_TextChanged(sender As System.Object, e As System.EventArgs)


    '  End Sub

    '  Private Sub TextBox2KeyPress(sender As System.Object, e As System.Windows.Forms.KeyPressEventArgs)
    '     If Asc(e.KeyChar) = 32 Or Asc(e.KeyChar) = 13 Then
    '         If Len(TextBox2.Text) > 0 Then
    '             TextBox1.AppendText(EncodeText(GetOnCC(TextBox2.Text)) + vbNewLine)
    '         End If

    '     End If

    ' End Sub






    Sub GetStatus()
        Dim dt As DataTable
        Dim ModuleColumn As DataColumn
        Dim DescriptionCoulumn As DataColumn
        Dim nameCoulumn As DataColumn
        Dim CheckCoulumn As DataColumn
        Dim dr As DataRow

        dt = New DataTable()
        ModuleColumn = New DataColumn("Stock Code", Type.GetType("System.String"))
        DescriptionCoulumn = New DataColumn("stock Name", Type.GetType("System.String"))
        nameCoulumn = New DataColumn("Price", Type.GetType("System.String"))
        CheckCoulumn = New DataColumn("Change", Type.GetType("System.String"))

        dt.Columns.Add(ModuleColumn)
        dt.Columns.Add(DescriptionCoulumn)
        dt.Columns.Add(nameCoulumn)
        dt.Columns.Add(CheckCoulumn)
        dsStatus.Tables.Add(dt)

        DgStatus.DataSource = dsStatus.Tables(0)
        dsStatus.Tables(0).Clear()
        DgStatus.Columns("Stock Code").Width = 100
        DgStatus.Columns("Stock Name").Width = 100
        DgStatus.Columns("Price").Width = 100
        DgStatus.Columns("Change").Width = 100
        '   DgStatus.Columns("Check").HeaderText = "Checking"


        dsStatus.Tables(0).Rows.Add(dr)
    End Sub

    Private Sub DgStatus_CellContentClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgStatus.CellContentClick
        Dim i, j As Integer
        i = DgStatus.CurrentRow.Index
        Debug.Print(DgStatus.Item(0, i).Value)
        Debug.Print(DgStatus.Item(1, i).Value)
        Debug.Print(DgStatus.Item(2, i).Value)
        'Debug.Print(tem(3, i).Value
    End Sub

    Private Sub GridKeyPress(sender As System.Object, e As System.Windows.Forms.KeyPressEventArgs) Handles DgStatus.KeyPress


    End Sub

    Private Sub DgStatus__RowEnter(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgStatus.RowEnter
        Dim i As Integer
        For i = 0 To DgStatus.Rows(e.RowIndex).Cells.Count - 1
            DgStatus(i, e.RowIndex).Style _
                .BackColor = Color.Yellow
        Next i
    End Sub

    Private Sub DgStatus_RowLeave(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgStatus.RowLeave
        Dim i As Integer
        For i = 0 To DgStatus.Rows(e.RowIndex).Cells.Count - 1
            DgStatus(i, e.RowIndex).Style _
                .BackColor = Color.Empty
        Next i
    End Sub

    Private Sub DgStatus_CellValidated(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgStatus.CellValidated
        Dim headerText As String = DgStatus.Columns(e.ColumnIndex).HeaderText

        ' Abort validation if cell is not in the CompanyName column.
        If Not headerText.Equals("Stock Code") Then Return
        Dim a As Array
        Dim StockCode = (DgStatus.Item(e.ColumnIndex, e.RowIndex).Value.ToString())
        If IsNumeric(StockCode) Then
            Debug.Print(e.ColumnIndex)
            Debug.Print(e.RowIndex)


            a = EncodeText(GetOnCC(StockCode)).Split(",")
            If a(0) = "null" Then


                Return
            End If
            Debug.Print(a(0))
            DgStatus.Item(e.ColumnIndex + 1, e.RowIndex).Value = (a(1))
            DgStatus.Item(e.ColumnIndex + 2, e.RowIndex).Value = (a(2))
            DgStatus.Item(e.ColumnIndex + 3, e.RowIndex).Value = (a(3))
        Else
            Dim i As Integer = (DgStatus.Rows.Count - 1)
            Do While (i >= 0)
                i = (i - 1)
                For Each dr As DataGridViewRow In DgStatus.Rows
                    If dr.Cells("Price").Value Is Nothing Then
                        DgStatus.Rows.Remove(dr)
                    End If
                Next
            Loop
         
        End If



    End Sub

    Private Sub DgStatus_CellValidating(sender As System.Object, e As System.Windows.Forms.DataGridViewCellValidatingEventArgs) Handles DgStatus.CellValidating

    End Sub

    Private Sub DgStatus_RowsRemoved(sender As System.Object, e As System.Windows.Forms.DataGridViewRowsRemovedEventArgs) Handles DgStatus.RowsRemoved

    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        DgStatus.Rows.RemoveAt(0)
    End Sub
End Class




