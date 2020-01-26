Imports System.IO
Imports System.Text
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json

Public Class clsSongCreator
    Public Shared Function CreateSongsFromFolder(strPath As String) As List(Of clsSong)
        Dim objSongs As New List(Of clsSong)

        Dim strTitel As String = String.Empty

        Dim objStrophes As New List(Of clsStrophe)
        Dim allLines As New StringBuilder()

        For Each objFile As String In IO.Directory.GetFiles(strPath, "*.sng", IO.SearchOption.AllDirectories)
            Using reader As StreamReader = My.Computer.FileSystem.OpenTextFileReader(objFile, System.Text.Encoding.ASCII)
                While reader.Peek <> -1
                    Dim line As String = reader.ReadLine()
                    If Not line = "---" Then
                        If Not line = "" AndAlso line.First = "#" Then
                            If line.Contains("Title") And Not line.Contains("TitleFormat") Then
                                strTitel = line.Split(CType("=", Char())).Last
                            End If
                        Else
                            allLines.Append(line)
                            allLines.Append(vbCrLf)
                        End If
                    Else
                        objStrophes.Add(New clsStrophe With {.Strophe = allLines.ToString})
                        allLines.Clear()
                    End If
                End While
                objStrophes.Add(New clsStrophe With {.Strophe = allLines.ToString})
                allLines.Clear()
            End Using
            objStrophes.RemoveAt(0)
            objSongs.Add(New clsSong With {.Title = strTitel, .StrophesText = objStrophes})
            objStrophes = New List(Of clsStrophe)
        Next

        Return objSongs
    End Function

    Public Function JsonDeserializer(strPath As String) As List(Of clsSong)
        Dim strstreamreader As New StreamReader(strPath)
        Return New JavaScriptSerializer().Deserialize(Of List(Of clsSong))(strstreamreader.ReadToEnd)
    End Function

    Public Sub JsonSerializer(strPath As String, objSongs As List(Of clsSong))
        Using objFileStream As New StreamWriter(strPath, FileMode.OpenOrCreate)
            Dim objXmlSerializer As New JsonSerializer()
            objXmlSerializer.Serialize(objFileStream, objSongs)
        End Using
    End Sub

End Class
