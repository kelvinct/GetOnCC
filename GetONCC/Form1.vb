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
        BtnOn.Enabled = False
        BtnOff.Enabled = False
        GetStatus()
        Tableupdate()
        Label3.Text = DgStatus.Rows.Count
    End Sub
    Private Function EncodeText(ByVal sText As String) As String
        Return WebUtility.HtmlEncode(sText)
    End Function

    Function Convert5(ByVal stock) As String
        Dim temp = ""
        If Len(stock) < 5 Then
            For padzero = 1 To 5 - Len(stock)
                temp = temp + "0"
                '  Debug.Print(temp)
            Next
        End If
        ' Response.Write temp & stock
        Convert5 = temp & stock
    End Function
    Function GetOnCC(ByVal SCTYCode) As String
        Try
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
                ' Debug.Print(StkpreCPrice)

                SearchChar = "nameChi:"""
                nStart = InStr(1, sLine, SearchChar) + Len(SearchChar)
                nEnd = InStr(nStart, sLine, """")
                Dim nameChi = (Mid(sLine, nStart, nEnd - nStart))
                Dim Change
                Change = ((stkprice - StkpreCPrice) / StkpreCPrice) * 100
                Change = FormatNumber((stkprice - StkpreCPrice), 3)
                GetOnCC = StockName + "," + nameChi + "," + stkprice + "," + Change


                'Debug.Print(GetOnCC)
            Else

                GetOnCC = "null,null,null,null"
            End If
        Catch _Exception As Exception

            'DgStatus.Rows.RemoveAt(e.RowIndex)
        Finally

        End Try
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

    Sub GetStatus()
        Try
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
            dr = dsStatus.Tables(0).NewRow
            dr(0) = 5

            DgStatus.Item(0, 0).Value = (5)
            Dim a As Array
            a = EncodeText(GetOnCC(DgStatus.Item(0, 0).Value)).Split(",")
            If a(0) = "null" Then

                Return
            End If
            'Debug.Print(a(0))
            dsStatus.Tables(0).Rows.Add(dr)
        Catch _Exception As Exception

            'DgStatus.Rows.RemoveAt(e.RowIndex)
        Finally

        End Try
    End Sub

    Private Sub DgStatus_CellContentClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgStatus.CellContentClick
        Try
            Dim i, j As Integer
            i = DgStatus.CurrentRow.Index
            'Debug.Print(DgStatus.Item(0, i).Value)
            'Debug.Print(DgStatus.Item(1, i).Value)
            'Debug.Print(DgStatus.Item(2, i).Value)
            'Debug.Print(tem(3, i).Value
        Catch _Exception As Exception

            'DgStatus.Rows.RemoveAt(e.RowIndex)
        Finally

        End Try
    End Sub



    Private Sub DgStatus__RowEnter(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgStatus.RowEnter
        'Dim i As Integer
        'For i = 0 To DgStatus.Rows(e.RowIndex).Cells.Count - 1
        'DgStatus(i, e.RowIndex).Style _
        '.BackColor = Color.Yellow
        ' Next i
    End Sub

    Private Sub DgStatus_RowLeave(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgStatus.RowLeave
        ' Dim i As Integer
        ''  For i = 0 To DgStatus.Rows(e.RowIndex).Cells.Count - 1
        'DgStatus(i, e.RowIndex).Style _
        '.BackColor = Color.Empty
        '  Next i
    End Sub

    Private Sub DgStatus_CellValidated(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgStatus.CellValidated
        Dim result = False
        Try
            Dim headerText As String = DgStatus.Columns(e.ColumnIndex).HeaderText

            ' Abort validation if cell is not in the CompanyName column.
            If Not headerText.Equals("Stock Code") Then Return
            Dim a As Array
            Dim StockCode = (DgStatus.Item(e.ColumnIndex, e.RowIndex).Value.ToString())
            If IsNumeric(StockCode) Then
                a = EncodeText(GetOnCC(StockCode)).Split(",")
                If a(0) = "null" Then
                    Return
                End If
                'Debug.Print(a(0))
                DgStatus.Item(e.ColumnIndex, e.RowIndex).Value = Val(a(0))
                DgStatus.Item(e.ColumnIndex + 1, e.RowIndex).Value = (a(1))
                DgStatus.Item(e.ColumnIndex + 2, e.RowIndex).Value = (a(2))
                DgStatus.Item(e.ColumnIndex + 3, e.RowIndex).Value = (a(3))
                If a(3) < 0 Then

                    DgStatus(e.ColumnIndex + 3, e.RowIndex).Style.BackColor = Color.Red
                    DgStatus(e.ColumnIndex + 3, e.RowIndex).Style.ForeColor = Color.White
                    DgStatus(e.ColumnIndex + 3, e.RowIndex).Style.Format = Font.Bold
                Else
                    DgStatus(e.ColumnIndex + 3, e.RowIndex).Style.BackColor = Color.Green
                    DgStatus(e.ColumnIndex + 3, e.RowIndex).Style.ForeColor = Color.White

                End If

            Else
                Dim i As Integer = (DgStatus.Rows.Count - 1)
                Do While (i >= 0)
                    i = (i - 1)
                    For Each dr As DataGridViewRow In DgStatus.Rows
                        If dr.Cells("Price").Value Is Nothing Then
                            DgStatus.Rows.Remove(dr)
                        End If
                    Next

                    For Each dr As DataGridViewRow In DgStatus.Rows
                        If dr.Cells("stockname").Value Is Nothing Then
                            DgStatus.Rows.Remove(dr)
                        End If
                    Next
                Loop
            End If


        Catch _Exception As Exception

            'DgStatus.Rows.RemoveAt(e.RowIndex)
        Finally

        End Try



    End Sub
    Private Sub Tableupdate()
        Try
            Dim i As Integer = (DgStatus.Rows.Count - 1)
            Dim a As Array
            Do While (i >= 0)
                If IsNumeric(DgStatus.Item(0, i).Value) = True Then
                    Dim stockName = ""
                    a = EncodeText(GetOnCC(DgStatus.Item(0, i).Value)).Split(",")
                    For Each dr As DataGridViewRow In DgStatus.Rows
                        stockName = DgStatus.Item(0, i).Value
                        DgStatus.Item(0, i).Value = a(0)
                        DgStatus.Item(1, i).Value = a(1)
                        DgStatus.Item(2, i).Value = a(2)
                        DgStatus.Item(3, i).Value = a(3)
                        ' lblToolStrip.Text = "last update" + "(" + stockName + ")" + "========>" + DateTime.Now
                    Next
                End If
                If Timer1.Enabled = True Then
                    ToolStripProgressBar1.Increment(5)
                    If ToolStripProgressBar1.Value = 100 Then
                        ToolStripProgressBar1.Value = 0
                    End If
                Else
                    ToolStripProgressBar1.Value = False
                End If
                ShowTime.TextAlign = ContentAlignment.BottomRight
                ShowTime.Text = DateTime.Now.ToString("hh:mm")

                i = (i - 1)
            Loop
        Catch _Exception As InvalidExpressionException


        Finally

        End Try
    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        Tableupdate()

    End Sub

    Private Sub BtnOff_Click(sender As System.Object, e As System.EventArgs) Handles BtnOff.Click
        Timer1.Enabled = False
        BtnOn.Enabled = False
        BtnOff.Enabled = True
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        Try
            If ComboBox1.SelectedIndex >= 0 Then
                BtnOn.Enabled = True
                BtnOff.Enabled = False
            End If
        Catch _Exception As Exception

            'DgStatus.Rows.RemoveAt(e.RowIndex)
        Finally

        End Try

    End Sub

    Private Sub BtnON_Click(sender As System.Object, e As System.EventArgs) Handles BtnOn.Click
        Try
            If ComboBox1.SelectedIndex >= 0 Then
                Timer1.Interval = 1000 * ComboBox1.SelectedItem.ToString
                Timer1.Enabled = True
                BtnOn.Enabled = False
                BtnOff.Enabled = True
            End If
        Catch _Exception As Exception

            'DgStatus.Rows.RemoveAt(e.RowIndex)
        Finally

        End Try
    End Sub

    Private Sub StatusStrip1_ItemClicked(sender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles StatusStrip1.ItemClicked

    End Sub

    Private Sub DgStatus_CellEndEdit(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgStatus.CellEndEdit
        If Len(DgStatus.Item(0, e.RowIndex).Value.ToString) > 0 Then
            If DgStatus.Item(0, e.RowIndex).Value = "null" Then
                DgStatus.Rows.RemoveAt(e.RowIndex)
            ElseIf IsNumeric(DgStatus.Item(0, e.RowIndex).Value) = False Or IsNumeric(DgStatus.Item(3, e.RowIndex).Value) = False Then

                DgStatus.Rows.RemoveAt(e.RowIndex)
            End If
        Else

            DgStatus.Rows.RemoveAt(e.RowIndex)


        End If
        Dim i = 0
        Dim a As Array
        If DgStatus.Rows.Count - 1 > 1 Then
            Do While (i < DgStatus.Rows.Count - 1)

                If i = e.RowIndex Then
                    Debug.Print(DgStatus.Item(0, e.RowIndex).Value)
                ElseIf DgStatus.Item(0, i).Value = DgStatus.Item(0, i + 1).Value Then
                    Debug.Print("same")
                End If
                i = i + 1
            Loop
        End If


    End Sub
End Class




