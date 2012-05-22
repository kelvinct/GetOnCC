Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Runtime.Serialization.Json
Imports System.Web
Imports System.Windows.Forms
Imports System.Data.OleDb
Public Class Commn
    Function EncodeText(ByVal sText As String) As String
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
    Function AAstocks(ByVal SCTYCode) As String
        Dim result = ""
        Dim sLine As String
        Try
            'http://www.etnet.com.hk/www/tc/stocks/realtime/quote.php?code=5
            sLine = DownloadHTMLPage(" http://www.aastocks.com/tc/ltp/rtquote.aspx?symbol=00005", "http://www.aastocks.com", System.Text.Encoding.UTF8)
            Debug.Print(sLine)

        Catch ex As Exception
            AAstocks = result
        Finally
            AAstocks = result
        End Try
    End Function
    Sub UpdateDataSetting(ByVal DataSet1 As DataSet, ByVal dr As DataRow)
        'http://money18.on.cc/js/real/topStock_stock.js?t=1337670103979
        Dim sLine As String
        Dim nStart, nEnd As Integer
        Dim SearchChar As String = "M18.r_"

        Dim dt As DataTable
        Dim row As DataRow
        Dim ModuleColumn As DataColumn
        Dim DescriptionCoulumn As DataColumn
        Dim nameCoulumn As DataColumn
        Dim CheckCoulumn As DataColumn
        Dim UpperCoulumn As DataColumn
        Dim BelowCoulumn As DataColumn
        '  Dim dsStatus As New DataSet
        dt = New DataTable("Quote")
        ModuleColumn = New DataColumn("StockCode", Type.GetType("System.String"))
        DescriptionCoulumn = New DataColumn("stockName", Type.GetType("System.String"))
        nameCoulumn = New DataColumn("Price", Type.GetType("System.String"))
        CheckCoulumn = New DataColumn("Change", Type.GetType("System.String"))

        dt.Columns.Add(ModuleColumn)
        dt.Columns.Add(DescriptionCoulumn)
        dt.Columns.Add(nameCoulumn)
        dt.Columns.Add(CheckCoulumn)

        'GetOnTop.Tables.

        sLine = DownloadHTMLPage("http://money18.on.cc/js/real/topStock_stock.js?t=1337670103979", "http://money18.on.cc/", System.Text.Encoding.GetEncoding("Big5"))
        '  Debug.Print(sLine)
        SearchChar = "up"":"
        nStart = InStr(1, sLine, SearchChar) + Len(SearchChar)
        SearchChar = "["
        nStart = InStr(nStart, sLine, SearchChar) + Len(SearchChar)
        nEnd = InStr(nStart, sLine, "]")
        Dim up = Mid(sLine, nStart, nEnd - nStart)
        ' Debug.Print(up)
        Dim b = up.Split("},{")
        Dim ArrayTop = b.Count
        Dim Stock As Array

        Dim StockCode = GetString(b(0), """s"":""")
        Dim StockName = GetString(b(0), "cn")
        Dim Stockprice = GetString(b(0), "price")
        Dim StockChg = GetString(b(0), "chg")
        Dim StockChgP = GetString(b(0), "chgP") & "%"


        '  For i As Integer = 0 To ary1.Length - 1
        dr = DataSet1.Tables(0).NewRow
        dr(0) = StockCode
        dr(1) = StockName
        dr(2) = Stockprice
        DataSet1.Tables(0).Rows.Add(dr)
        '   Next

    End Sub

    Function GetString(ByVal val As String, ByVal key As String) As String

        Dim nStart, nEnd As Integer
        Dim SearchChar As String = ""
        SearchChar = """" & key & """:"""
        nStart = InStr(1, val, SearchChar) + Len(SearchChar)
        nEnd = InStr(nStart, val, """,")
        GetString = Mid(val, nStart, nEnd - nStart)
    End Function



    Function GetOnCCHSI() As String

        Dim sLine As String
        Dim nStart, nEnd As Integer
        Dim SearchChar As String = "M18.r_"

        sLine = DownloadHTMLPage("http://money18.on.cc/js/real/index/HSI_r.js?t=1319009536902", "http://money18.on.cc/", System.Text.Encoding.GetEncoding("Big5"))
        'HSi Preclosingprice'
        SearchChar = "pc: '"
        nStart = InStr(1, sLine, SearchChar) + Len(SearchChar)
        nEnd = InStr(nStart, sLine, "'")
        Dim HSIPreclosingprice = Mid(sLine, nStart, nEnd - nStart)
        Debug.Print(HSIPreclosingprice)

        'HSi index'
        SearchChar = "value: '"
        nStart = InStr(1, sLine, SearchChar) + Len(SearchChar)
        nEnd = InStr(nStart, sLine, "'")
        Dim HsiValue = Mid(sLine, nStart, nEnd - nStart)
        Debug.Print(HsiValue)

        SearchChar = "difference: '"
        nStart = InStr(1, sLine, SearchChar) + Len(SearchChar)
        nEnd = InStr(nStart, sLine, "'")
        Dim difference = Mid(sLine, nStart, nEnd - nStart)
        Debug.Print(difference)

        GetOnCCHSI = HsiValue & "," & difference & "," & HSIPreclosingprice
    End Function

    Function GetOnCC(ByVal SCTYCode) As String
        Dim result = ""
        Try
            GetOnCC = ""
            Dim stocktime As String = "real"
            Dim stocktype As String = "r"
            Dim sLine As String
            Dim nStart, nEnd As Integer
            Dim StockName
            Dim stkprice As String = 0

            stocktype = "r"
            sLine = DownloadHTMLPage("http://money18.on.cc/js/" & stocktime & "/quote/" & Convert5(SCTYCode) & "_" & stocktype & ".js?t=1319009536902", "http://money18.on.cc/", System.Text.Encoding.GetEncoding("Big5"))

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

                ' http://money18.on.cc/js/real/index/HSI_r.js?t=1337658441937

                sLine = DownloadHTMLPage("http://money18.on.cc/js/daily/quote/" & Convert5(SCTYCode) & "_d.js?t=1319009536902", "http://money18.on.cc/", System.Text.Encoding.GetEncoding("Big5"))
                SearchChar = "preCPrice:"""
                nStart = InStr(1, sLine, SearchChar) + Len(SearchChar)
                nEnd = InStr(nStart, sLine, """")
                Dim StkpreCPrice = Mid(sLine, nStart, nEnd - nStart)


                SearchChar = "nameChi:"""
                nStart = InStr(1, sLine, SearchChar) + Len(SearchChar)
                nEnd = InStr(nStart, sLine, """")
                Dim nameChi = (Mid(sLine, nStart, nEnd - nStart))
                Dim Change
                Change = ((stkprice - StkpreCPrice) / StkpreCPrice) * 100
                Change = FormatNumber((stkprice - StkpreCPrice), 3)
                result = StockName + "," + nameChi + "," + stkprice + "," + Change



                result = StockName + "," + nameChi + "," + stkprice + "," + Change
            Else

                result = "null,null,null,null"
            End If
        Catch _Exception As Exception
            result = "null,null,null,null"
            'DgStatus.Rows.RemoveAt(e.RowIndex)
        Finally
            GetOnCC = result
        End Try
    End Function
    Public Function DownloadHTMLPage(ByVal _URL As String, ByVal refer As String, ByVal ecode As System.Text.Encoding) As String
        Dim _PageContent As String = Nothing
        Try
            ' Open a connection
            Dim _HttpWebRequest As System.Net.HttpWebRequest = CType(System.Net.HttpWebRequest.Create(_URL), System.Net.HttpWebRequest)

            ' You can also specify additional header values like the user agent or the referer: (Optional)
            _HttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)"
            _HttpWebRequest.Referer = refer


            ' set timeout for 10 seconds (Optional)
            _HttpWebRequest.Timeout = 10000

            ' Request response:
            Dim _WebResponse As System.Net.WebResponse = _HttpWebRequest.GetResponse()

            ' Open data stream:
            Dim _WebStream As System.IO.Stream = _WebResponse.GetResponseStream()
            '  Dim encode As Encoding = System.Text.Encoding.GetEncoding(ecode)
            ' Create reader object:
            Dim _StreamReader As New System.IO.StreamReader(_WebStream, ecode)

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
    Sub GetStatus(ByVal Dg As DataGridView, ByVal ds As DataSet)
        Try
            Dim dt As DataTable
            Dim ModuleColumn As DataColumn
            Dim DescriptionCoulumn As DataColumn
            Dim nameCoulumn As DataColumn
            Dim CheckCoulumn As DataColumn
            Dim UpperCoulumn As DataColumn
            Dim BelowCoulumn As DataColumn
            '  Dim dsStatus As New DataSet
            dt = New DataTable("Quote")
            ModuleColumn = New DataColumn("StockCode", Type.GetType("System.String"))
            DescriptionCoulumn = New DataColumn("stockName", Type.GetType("System.String"))
            nameCoulumn = New DataColumn("Price", Type.GetType("System.String"))
            CheckCoulumn = New DataColumn("Change", Type.GetType("System.String"))
            BelowCoulumn = New DataColumn("Below", Type.GetType("System.String"))
            UpperCoulumn = New DataColumn("Upper", Type.GetType("System.String"))
            dt.Columns.Add(ModuleColumn)
            dt.Columns.Add(DescriptionCoulumn)
            dt.Columns.Add(nameCoulumn)
            dt.Columns.Add(CheckCoulumn)
            dt.Columns.Add(BelowCoulumn)
            dt.Columns.Add(UpperCoulumn)

            ds.Tables.Add(dt)

            Dg.DataSource = ds.Tables(0)
            ds.Tables(0).Clear()
            Dg.Columns("StockCode").Width = 70
            Dg.Columns("StockName").Width = 100
            Dg.Columns("Price").Width = 50
            Dg.Columns("Change").Width = 50
            Dg.Columns("Below").Width = 50
            Dg.Columns("Upper").Width = 50


            Dim a As Array
            a = EncodeText(GetOnCC(Dg.Item(0, 0).Value)).Split(",")
            If a(0) = "null" Then

                Return
            End If
            Debug.Print(a(0))
            'dsStatus.Tables(0).Rows.Add(dr)

        Catch _Exception As Exception

            'DgStatus.Rows.RemoveAt(e.RowIndex)
        Finally

        End Try
    End Sub
End Class
