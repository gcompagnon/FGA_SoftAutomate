' ==========================================================
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT 
'  WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,    
' INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES   
' OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR   
' PURPOSE.								
' ==========================================================
Imports System.IO
Namespace Bloomberglp.Blpapi.Examples
    Public Class FormBulkData
        Public Sub New(ByVal dataSource As DataTable)
            InitializeComponent()
            dataGridViewRDBulkData.DataSource = dataSource
            dataGridViewRDBulkData.Columns("Id").Visible = False
        End Sub

        ''' <summary>
        ''' Save data to file
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub toolStripButtonSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles toolStripButtonSave.Click
            Dim fileName As String = String.Empty
            Dim delim As String = ","
            Dim data As DataTable = Nothing
            Dim save As SaveFileDialog = New SaveFileDialog()

            save.Title = "Save Data"
            save.Filter = "Text|*.txt"
            If (save.ShowDialog() = DialogResult.OK) Then
                If (save.FileName.Trim().Length > 0) Then
                    ' check for existing file
                    If File.Exists(save.FileName.Trim()) Then
                        ' delete file
                        File.Delete(save.FileName.Trim())
                    End If
                    ' get data table from grid 
                    data = (CType(dataGridViewRDBulkData.DataSource, DataTable))
                    Dim writer As StreamWriter = New StreamWriter(save.FileName)
                    Dim columnCount As Integer = data.Columns.Count
                    Dim output As String = String.Empty
                    ' create header
                    For Each column As DataColumn In data.Columns
                        output = String.Concat(output, column.ColumnName, delim)
                    Next
                    writer.WriteLine(output.Substring(0, output.Length - 1))
                    ' output data
                    For Each row As DataRow In data.Rows
                        output = String.Empty
                        For index As Integer = 0 To columnCount - 1
                            output = String.Concat(output, row(index), delim)
                        Next
                        writer.WriteLine(output.Substring(0, output.Length - 1))
                    Next
                    writer.Flush()
                    writer.Close()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Close bulk data
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub toolStripButtonBDClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles toolStripButtonBDClose.Click
            Me.Close()
        End Sub

    End Class
End Namespace