'--------------------------------------------------------------------------
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.'
' --------------------------------------------------------------------------

Imports Bloomberglp.Blpapi
Imports System

Namespace Bloomberglp.Blpapi.Examples
    Module SimpleFieldInfoExample

        Private Const APIFLDS_SVC As String = "//blp/apiflds"
        Private Const ID_LEN As Integer = 13
        Private Const MNEMONIC_LEN As Integer = 25
        Private Const DESC_LEN As Integer = 40
        Private Const PADDING As String = "                                            "

        Private ReadOnly FIELD_ID As New Name("id")
        Private ReadOnly FIELD_MNEMONIC As New Name("mnemonic")
        Private ReadOnly FIELD_DATA As New Name("fieldData")
        Private ReadOnly FIELD_DESC As New Name("description")
        Private ReadOnly FIELD_INFO As New Name("fieldInfo")
        Private ReadOnly FIELD_ERROR As New Name("fieldError")
        Private ReadOnly FIELD_MSG As New Name("message")

        Private d_serverHost As String
        Private d_serverPort As Integer

        Sub Main(ByVal args As String())

            d_serverHost = "localhost"
            d_serverPort = 8194

            If Not parseCommandLine(args) Then
                Return
            End If

            Dim sessionOptions As New SessionOptions()

            sessionOptions.ServerHost = d_serverHost
            sessionOptions.ServerPort = d_serverPort

            System.Console.WriteLine("Connecting to " + d_serverHost + _
            ":" + d_serverPort.ToString())

            Dim session As New Session(sessionOptions)
            Dim sessionStarted As Boolean = session.Start()

            If Not sessionStarted Then
                System.Console.WriteLine("Failed to start session.")
                Return
            End If

            If Not session.OpenService(APIFLDS_SVC) Then
                System.Console.WriteLine("Failed to open service: " + APIFLDS_SVC)
                Return
            End If

            Dim fieldInfoService As Service = session.GetService(APIFLDS_SVC)
            Dim request As Request = fieldInfoService.CreateRequest("FieldInfoRequest")
            Dim idList As Element = request.GetElement("id")

            request.Append("id", "LAST_PRICE")
            request.Append("id", "pq005")
            request.Append("id", "zz0002")

            request.Set("returnFieldDocumentation", False)

            System.Console.WriteLine("Sending Request: " + request.AsElement.ToString())
            session.SendRequest(request, Nothing)

            While True
                Try
                    Dim eventObj As [Event] = session.NextEvent()
                    For Each msg As Message In eventObj
                        If eventObj.Type <> [Event].EventType.RESPONSE AndAlso _
                        eventObj.Type <> [Event].EventType.PARTIAL_RESPONSE Then
                            Continue For
                        End If

                        Dim fields As Element = msg.GetElement(FIELD_DATA)
                        Dim numElements As Integer = fields.NumValues

                        printHeader()
                        For i As Integer = 0 To numElements - 1
                            printField(fields.GetValueAsElement(i))
                        Next
                        System.Console.WriteLine()
                    Next
                    If eventObj.Type = [Event].EventType.RESPONSE Then
                        Exit While
                    End If
                Catch ex As Exception
                    System.Console.WriteLine("Got Exception:" + ex.ToString())
                End Try
            End While

            System.Console.WriteLine("Press ENTER to quit")
            System.Console.Read()

        End Sub

        Private Sub printField(ByVal field As Element)

            Dim fldId As String, fldMnemonic As String, fldDesc As String

            fldId = field.GetElementAsString(FIELD_ID)

            If field.HasElement(FIELD_INFO) Then
                Dim fldInfo As Element = field.GetElement(FIELD_INFO)
                fldMnemonic = fldInfo.GetElementAsString(FIELD_MNEMONIC)
                fldDesc = fldInfo.GetElementAsString(FIELD_DESC)

                System.Console.WriteLine(padString(fldId, ID_LEN) + _
                padString(fldMnemonic, MNEMONIC_LEN) + padString(fldDesc, DESC_LEN))
            Else
                Dim fldError As Element = field.GetElement(FIELD_ERROR)
                fldDesc = fldError.GetElementAsString(FIELD_MSG)

                System.Console.WriteLine(vbLf & " ERROR: " + fldId + " - " + fldDesc)
            End If

        End Sub

        Private Function padString(ByVal str As String, ByVal width As Integer) As String

            If str.Length >= width OrElse str.Length >= PADDING.Length Then
                Return str
            Else
                Return str + PADDING.Substring(0, width - str.Length)
            End If

        End Function

        Private Function parseCommandLine(ByVal args As String()) As Boolean

            For i As Integer = 0 To args.Length - 1
                If String.Compare(args(i), "-ip", True) = 0 Then
                    d_serverHost = args(i + 1)
                    i += 1
                ElseIf String.Compare(args(i), "-p", True) = 0 Then
                    Dim outPort As Integer = 0
                    If Integer.TryParse(args(i + 1), outPort) Then
                        d_serverPort = outPort
                    End If
                    i += 1
                ElseIf String.Compare(args(i), "-h", True) = 0 Then
                    printUsage()
                    Return (False)
                Else
                    System.Console.WriteLine("Ignoring unknown option:" + args(i))
                End If
            Next
            Return (True)

        End Function

        Private Sub printUsage()

            System.Console.WriteLine("Usage:")
            System.Console.WriteLine(vbTab & _
            "Retrieve field information in categorized form from ServerApi")
            System.Console.WriteLine(vbTab & vbTab & "[-ip <ipAddress> default = " + _
            d_serverHost + " ]")
            System.Console.WriteLine(vbTab & vbTab & "[-p <tcpPort> default = " + _
            d_serverPort.ToString() + " ]")
            System.Console.WriteLine(vbTab & vbTab & "[-h print this message and quit]" _
            & vbLf)

        End Sub

        Private Sub printHeader()

            System.Console.WriteLine(padString("FIELD ID", ID_LEN) + _
            padString("MNEMONIC", MNEMONIC_LEN) + padString("DESCRIPTION", DESC_LEN))
            System.Console.WriteLine(padString("-----------", ID_LEN) + _
            padString("-----------", MNEMONIC_LEN) + padString("-----------", DESC_LEN))

        End Sub

    End Module
End Namespace
