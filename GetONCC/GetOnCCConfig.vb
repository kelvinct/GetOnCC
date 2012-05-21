Imports System.Xml


Public Class GetOnCCConfig
    Inherits Config

    Public Property ApplicationName() As String
        Get
            Dim n As XmlNode

            n = m_node.SelectSingleNode("ApplicationName")
            If Not n Is Nothing Then
                ApplicationName = n.InnerText
            Else
                ApplicationName = Nothing
            End If
        End Get

        Set(ByVal value As String)

        End Set
    End Property
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

        End Set
    End Property

End Class
