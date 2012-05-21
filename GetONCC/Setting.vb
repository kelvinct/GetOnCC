Imports System.IO
Imports System.Xml
Public Class Settings
    Public Function IsExits() As Boolean
        If System.IO.File.Exists(Application.StartupPath & "\" & "Settings.xml") Then
            IsExits = True
        Else
            IsExits = False
        End If
    End Function
    Public Function initSettings() As Boolean

        initSettings = False
        Dim writer As New XmlTextWriter(Application.StartupPath & "\" & "Settings.xml", System.Text.Encoding.UTF8)
        writer.WriteStartDocument(True)
        writer.Formatting = Formatting.Indented
        writer.Indentation = 2

        writer.WriteStartElement("NewDataSet")
        writer.WriteStartElement("Settings")

        writer.WriteStartElement("Key")
        writer.WriteString("Time")
        writer.WriteEndElement()

        writer.WriteStartElement("Value")
        writer.WriteString("0")
        writer.WriteEndElement()

        writer.WriteEndElement()
        writer.WriteEndElement()

        initSettings = True
        writer.Close()
  

    End Function
    Public Function GetSetting(ByVal Key As String) As String
        Dim sReturn As String = String.Empty
        Dim dsSettings As New DataSet
        If System.IO.File.Exists(Application.StartupPath & "\" & "Settings.xml") Then
            dsSettings.ReadXml(Application.StartupPath & "\" & "Settings.xml")
        Else
            dsSettings.Tables.Add("Settings")
            dsSettings.Tables(0).Columns.Add("Key", GetType(String))
            dsSettings.Tables(0).Columns.Add("Value", GetType(String))
        End If

        Dim dr() As DataRow = dsSettings.Tables("Settings").Select("Key = '" & Key & "'")
        If dr.Length = 1 Then sReturn = dr(0)("Value").ToString

        Return sReturn
    End Function

    Public Sub SetSetting(ByVal Key As String, ByVal Value As String)
        Dim dsSettings As New DataSet
        If System.IO.File.Exists(Application.StartupPath & "\" & "Settings.xml") Then
            dsSettings.ReadXml(Application.StartupPath & "\" & "Settings.xml")
        Else
            dsSettings.Tables.Add("Settings")
            dsSettings.Tables(0).Columns.Add("Key", GetType(String))
            dsSettings.Tables(0).Columns.Add("Value", GetType(String))
        End If

        Dim dr() As DataRow = dsSettings.Tables(0).Select("Key = '" & Key & "'")
        If dr.Length = 1 Then
            dr(0)("Value") = Value
        Else
            Dim drSetting As DataRow = dsSettings.Tables("Settings").NewRow
            drSetting("Key") = Key
            drSetting("Value") = Value
            dsSettings.Tables("Settings").Rows.Add(drSetting)
        End If
        dsSettings.WriteXml(Application.StartupPath & "\" & "Settings.xml")
    End Sub
End Class