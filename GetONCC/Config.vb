Imports System.IO
Imports System.Xml

Public Class Config
    Const ROOT_NAME As String = "Config"
    Protected m_node As XmlNode
    Public Function loadXML(ByVal bstrXML As String) As Boolean
        loadXML = False
        Dim xd As XmlDocument = New XmlDocument()
        Dim xTxtReader As XmlTextReader = New XmlTextReader(bstrXML)
        xd.Load(xTxtReader)
        m_node = xd.DocumentElement()
    End Function
End Class