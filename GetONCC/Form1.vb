Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Runtime.Serialization.Json
Imports System.Web
Imports System.Windows.Forms
Imports System.Data.OleDb

Imports GetONCC
Public Class Form1

    Dim dsStatus As New DataSet("StockMarket")
    Dim dsUpStatus As New DataSet("StockMarket")
    Dim ServerCount As Integer
    Dim Common As New GetONCC.Commn
    Dim SetConfig As New Settings
    Private ready As Boolean = False
    Dim dr As DataRow
    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        BtnOn.Enabled = False
        BtnOff.Enabled = False

        Common.GetStatus(DgStatus, dsStatus)

        ' Common.GetOnTop()
        ' Common.GetStatus(DataGridView2, Common.GetOnTop())
        '  SetBalloonTip()
        Timer2.Enabled = True
        Timer2.Start()
        Try
            dsStatus.ReadXml(Application.StartupPath & "\" & "data.xml")


            ' dsUpStatus.ReadXml(Application.StartupPath & "\" & "Topdata.xml")
            Tableupdate()
            ' SetBalloonTip()
            If SetConfig.IsExits = True Then
                ComboBox1.SelectedIndex = SetConfig.GetSetting("time")
                Label3.Text = "Your select :" + ComboBox1.SelectedItem.ToString + "sec"
            Else
                Label3.Text = "Settings.xml is not find"
                SetConfig.initSettings()
            End If
        Catch ex As Exception
            Label3.Text = "data.xml is not find"
            Me.Show()
        End Try
    End Sub



    Private Sub DgStatus__RowEnter(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgStatus.RowEnter

    End Sub

    Private Sub DgStatus_RowLeave(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgStatus.RowLeave

    End Sub

    Private Sub DgStatus_CellValidated(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgStatus.CellValidated
        Dim result = False
        Try
            Dim headerText As String = DgStatus.Columns(e.ColumnIndex).HeaderText

            ' Abort validation if cell is not in the CompanyName column.
            If Not headerText.Equals("StockCode") Then Return
            Dim a As Array
            Dim StockCode = (DgStatus.Item(e.ColumnIndex, e.RowIndex).Value.ToString())
            If IsNumeric(StockCode) Then
                a = Common.EncodeText(Common.GetOnCC(StockCode)).Split(",")
                If a(0) = "null" Then
                    Return
                End If
                DgStatus.Item(e.ColumnIndex + 1, e.RowIndex).Value = (a(1))
                DgStatus.Item(e.ColumnIndex + 2, e.RowIndex).Value = (a(2))
                DgStatus.Item(e.ColumnIndex + 3, e.RowIndex).Value = (a(3))
                If DgStatus.Item(e.ColumnIndex + 2, e.RowIndex).Value < DgStatus.Item(e.ColumnIndex + 4, e.RowIndex).Value Then
                    DgStatus.Item(e.ColumnIndex + 1, e.RowIndex).Style.ForeColor = Color.Red
                Else
                    DgStatus.Item(e.ColumnIndex + 1, e.RowIndex).Style.ForeColor = Color.Green
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
 
            Dim B = Common.GetOnCCHSI.Split(",")
            HsiValue.Text = B(0)
            difference.Text = B(1)
            HSIPreClosingPrice.Text = B(2)

            If HsiValue.Text > HSIPreClosingPrice.Text Then
                HsiValue.ForeColor = Color.White
                HsiValue.BackColor = Color.Green
            Else
                HsiValue.ForeColor = Color.White
                HsiValue.BackColor = Color.Red
            End If




            Debug.Print("tableupdate")
            Dim i As Integer = (DgStatus.Rows.Count - 1)
            Dim a As Array
            Dim hsi As Array = Common.GetOnCCHSI.Split(",")
            Dim hsiType As String = ""
            If hsi(1) > 0 Then
                hsiType = "+"
            End If
            NotifyIcon1.BalloonTipText = "HSI: " & hsi(0) & " , " & hsiType & hsi(1) & vbNewLine
            '  NotifyIcon1.BalloonTipText = ""
            Do While (i >= 0)
                If IsNumeric(DgStatus.Item(0, i).Value) = True Then
                    Dim stockName = ""
                    Dim stockUpDown = ""
                    a = Common.EncodeText(Common.GetOnCC(DgStatus.Item(0, i).Value)).Split(",")
                    If a(3) > 0 Then
                        stockUpDown = "+"
                    End If
                    NotifyIcon1.BalloonTipText += a(0) & " , " & a(1) & " , " & a(2) & " , " & stockUpDown & a(3) & vbNewLine
                    For Each dr As DataGridViewRow In DgStatus.Rows
                        stockName = DgStatus.Item(0, i).Value
                        DgStatus.Item(1, i).Value = a(1)
                        DgStatus.Item(2, i).Value = a(2)
                        DgStatus.Item(3, i).Value = a(3)
                        '  If a(3) = "null" Then
                        'DgStatus.Rows.RemoveAt(i)

                        '  End If
                        If DgStatus.Item(3, i).Value < 0 Then
                            DgStatus.Item(3, i).Style.BackColor = Color.Red
                            DgStatus.Item(3, i).Style.ForeColor = Color.White
                            DgStatus.Item(3, i).Style.Format = Font.Bold
                        Else
                            DgStatus.Item(3, i).Style.BackColor = Color.Green
                            DgStatus.Item(3, i).Style.ForeColor = Color.White
                        End If

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
        If Timer1.Enabled = True Then
            Tableupdate()

        End If

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
                Label3.Text = "Your select :" + ComboBox1.SelectedItem.ToString + "sec"
            End If
        Catch ex As Exception

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
        Finally
            Label3.Text = "Your select :" + ComboBox1.SelectedItem.ToString + "sec"
        End Try
    End Sub

    Private Sub DgStatus_CellEndEdit(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DgStatus.CellEndEdit
        Try
            Dim a As Array
            a = Common.EncodeText(Common.GetOnCC(DgStatus.Item(0, e.RowIndex).Value)).Split(",")
            DgStatus.Item(1, e.RowIndex).Value = a(1)
            DgStatus.Item(2, e.RowIndex).Value = a(2)
            DgStatus.Item(3, e.RowIndex).Value = a(3)
            If DgStatus.Item(3, e.RowIndex).Value = "null" Then
                DgStatus.Rows.RemoveAt(e.RowIndex)
                Return
            End If
            If DgStatus.Item(0, e.RowIndex).Value Is Nothing Then
                Return
            End If

            If Len(DgStatus.Item(0, e.RowIndex).Value.ToString) > 0 Then
                If DgStatus.Item(0, e.RowIndex).Value = "null" Then
                    DgStatus.Rows.RemoveAt(e.RowIndex)
                ElseIf IsNumeric(DgStatus.Item(0, e.RowIndex).Value) = False Or IsNumeric(DgStatus.Item(3, e.RowIndex).Value) = False Then

                    DgStatus.Rows.RemoveAt(e.RowIndex)
                End If
            Else
                DgStatus.Rows.RemoveAt(e.RowIndex)
            End If
            Dim count = DgStatus.Rows.Count - 2
            Do While count >= -1
                If count <> e.RowIndex Then
                    If DgStatus.Item(0, count).Value = DgStatus.Item(0, e.RowIndex).Value Then
                        DgStatus.Rows.RemoveAt(e.RowIndex)
                    End If

                End If
                count = count - 1
            Loop
            DgStatus.Item(1, e.RowIndex).Style.ForeColor = Color.Green
        Catch _Exception As Exception
            ' DgStatus.Rows.RemoveAt(e.RowIndex)
        Finally
            Tableupdate()
        End Try

    End Sub
    Private Sub DgStatus_ColumnHeaderMouseClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles DgStatus.ColumnHeaderMouseClick
        Tableupdate()
    End Sub

    Private Sub Form1_FormClosed(sender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
       
        dsStatus.WriteXml(Application.StartupPath & "\" & "data.xml")
        dsUpStatus.WriteXml(Application.StartupPath & "\" & "Updata.xml")
        SetConfig.SetSetting("time", ComboBox1.SelectedIndex.ToString)
        Application.Exit()
    End Sub


    Private Sub Button2_Click_1(sender As System.Object, e As System.EventArgs)
        Dim setConfig As New Settings
        setConfig.SetSetting("time", ComboBox1.SelectedIndex.ToString)
    End Sub


    Private Sub DgStatus_CellValidating(sender As System.Object, e As System.Windows.Forms.DataGridViewCellValidatingEventArgs) Handles DgStatus.CellValidating

        DgStatus.Item(1, e.RowIndex).Style.ForeColor = Color.Red

    End Sub




    Private Sub ExitToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ExitToolStripMenuItem.Click

        Application.Exit()


    End Sub


    Private Sub NotifyIcon1_MouseClick(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseClick
        If e.Button = MouseButtons.Left Then
            NotifyIcon1.ShowBalloonTip(1000)
        End If

    End Sub

    Private Sub ConfigToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ConfigToolStripMenuItem.Click
        Me.Show()

        Me.WindowState = FormWindowState.Normal
      


    End Sub

    Private Sub Form1_Resize(sender As System.Object, e As System.EventArgs) Handles MyBase.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Me.Hide()

        End If
    End Sub

    Private Sub Timer2_Tick(sender As System.Object, e As System.EventArgs) Handles Timer2.Tick
        If Timer2.Enabled = True Then
            NotifyIcon1.ShowBalloonTip(1000)

        End If
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles TabControl1.SelectedIndexChanged

    End Sub

    Private Sub TabControl1_Selected(sender As System.Object, e As System.Windows.Forms.TabControlEventArgs) Handles TabControl1.Selected


    End Sub
End Class




