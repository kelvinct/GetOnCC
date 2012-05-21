Imports System.Xml


Public Class GetOnCCConfig
    Inherits Config
    Private _TimeIntervial As String

    Public Property TimeIntervial() As String
        Get
            Dim n As XmlNode

            n = m_node.SelectSingleNode("TimeIntervial")
            If Not n Is Nothing Then
                TimeIntervial = n.InnerText
            Else
                TimeIntervial = Nothing
            End If
        End Get

        Set(ByVal value As String)
     
            _TimeIntervial = value
        End Set
    End Property
    Public Function SaveXML() As Boolean
        Try
            SaveXML = False
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

            SaveXML = True
            writer.Close()
        Catch _Exception As Exception
        End Try

    End Function
End Class
