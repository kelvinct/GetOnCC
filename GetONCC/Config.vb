Imports System.IO
Imports System.Xml

Public Class Config
    Const ROOT_NAME As String = "Config"
    Protected m_node As XmlNode
    Dim xTxtReader As XmlTextReader
    Public Function loadXML(ByVal bstrXML As String) As Boolean
        Dim xTxtReader As XmlTextReader = New XmlTextReader(bstrXML)
        loadXML = False
        Try
            Dim xd As XmlDocument = New XmlDocument()
            xd.Load(xTxtReader)
            m_node = xd.DocumentElement()
            xTxtReader.Close()
            loadXML = True
        Catch ex As Exception
            loadXML = False
            xTxtReader.Close()
        End Try

    End Function
    Public Function SaveXML(ByVal bstrXML As String) As Boolean
        SaveXML = False
        Dim xd As XmlDocument = New XmlDocument()
        Dim xTxtWriter As XmlTextWriter = New XmlTextWriter(bstrXML, Nothing)
        m_node.WriteTo(xTxtWriter)

        xTxtWriter.Close()

    End Function
End Class