' ==========================================================
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT 
'  WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,    
' INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES   
' OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR   
' PURPOSE.								
' ==========================================================
' Purpose of this example:
' - Show how to retrieve service schema
' ==========================================================
Imports System.Collections.Generic
Imports Service = Bloomberglp.Blpapi.Service
Imports Session = Bloomberglp.Blpapi.Session
Imports SessionOptions = Bloomberglp.Blpapi.SessionOptions
Namespace Bloomberglp.Blpapi.Examples
    Public Class Form1
        Private d_sessionOptions As SessionOptions
        Private d_session As Session
        Private d_serviceDictionary As Dictionary(Of String, Service)
        Private d_currentServiceIndex As Integer

        Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim serverHost As String = "localhost"
            Dim serverPort As Integer = 8194

            ' set sesson options
            d_sessionOptions = New SessionOptions()
            d_sessionOptions.ServerHost = serverHost
            d_sessionOptions.ServerPort = serverPort

            d_currentServiceIndex = -1
            ' start session
            If Not createSession() Then
                ' error
                MessageBox.Show("Unable to start session.", "Session Error")
                textBoxService.Enabled = False
                buttonGetService.Enabled = False
            End If
            ' create service dictionary
            d_serviceDictionary = New Dictionary(Of String, Service)()
            ' initialize UI controls
            initUI()
        End Sub

#Region "methods"
        ''' <summary>
        ''' Initialize form controls
        ''' </summary>
        Private Sub initUI()
            textBoxService.Text = "//blp/refdata"
        End Sub

        ''' <summary>
        ''' Create session
        ''' </summary>
        ''' <returns></returns>
        Private Function createSession() As Boolean
            ' create session
            d_session = New Session(d_sessionOptions)
            Return d_session.Start()
        End Function

        ''' <summary>
        ''' Get service schema
        ''' </summary>
        ''' <param name="serviceName">service name</param>
        Private Sub decodeServiceSchema(ByVal serviceName As String)
            Dim service As Service = Nothing
            Dim data As String = String.Empty
            Dim node As TreeNode = Nothing

            ' get service
            service = d_serviceDictionary(serviceName)
            ' clear all child nodes
            treeViewSchema.Nodes.Clear()
            If (service.NumOperations > 0) Then
                ' create Operation node
                node = treeViewSchema.Nodes.Add("Operations", "Operations", 0)
                ' get service operations
                Dim op As Operation
                For Each op In service.Operations
                    If (Not op Is Nothing) Then
                        ' Request name
                        Dim childNodeLvl1 As TreeNode = node.Nodes.Add(op.ToString(), op.Name.ToString())
                        ' Request Type
                        Dim childNodeLvl2 As TreeNode = childNodeLvl1.Nodes.Add(op.RequestDefinition.Name.ToString(), _
                            op.RequestDefinition.Name.ToString() + _
                            " (" + op.RequestDefinition.TypeDefinition.Datatype.ToString() + ")")
                        ' store element definition
                        childNodeLvl2.Tag = op.RequestDefinition
                        ' process definitions
                        processOperation(childNodeLvl2, op.RequestDefinition.TypeDefinition)
                        ' Response
                        If (op.NumResponseDefinition > 0) Then
                            childNodeLvl2 = Nothing
                            Dim def As SchemaElementDefinition
                            For Each def In op.ResponseDefinitions
                                If (Not def Is Nothing) Then
                                    If (childNodeLvl2 Is Nothing) Then
                                        childNodeLvl2 = childNodeLvl1.Nodes.Add("Responses", "Responses", 0)
                                    End If
                                    ' process response definitions
                                    processOperation(childNodeLvl2, def.TypeDefinition)
                                End If
                            Next
                        End If
                    End If
                Next
                If (Not node Is Nothing) Then
                    ' expand top level node
                    node.Expand()
                End If
            End If
            ' get element definition
            If (service.NumEventDefinitions > 0) Then
                ' create Operation node
                node = treeViewSchema.Nodes.Add("Events", "Events", 0)
                ' get service operations
                Dim def As SchemaElementDefinition
                For Each def In service.EventDefinitions
                    If (Not def Is Nothing) Then
                        ' request name
                        Dim childNodeLvl1 As TreeNode = node.Nodes.Add(def.ToString(), def.Name.ToString(), 0)
                        childNodeLvl1.Tag = def
                        Dim childNodeLvl2 As TreeNode = childNodeLvl1.Nodes.Add(def.TypeDefinition.Name.ToString(), _
                            def.TypeDefinition.Name.ToString() + _
                            " (" + def.TypeDefinition.Datatype.ToString() + ")")
                        If (def.TypeDefinition.NumElementDefinitions > 0) Then
                            ' process request definition
                            processOperation(childNodeLvl2, def.TypeDefinition)
                        End If
                    End If
                Next
                If (Not node Is Nothing) Then
                    ' expand top level node
                    node.Expand()
                End If
            End If
            ' sort tree
            treeViewSchema.Sort()
        End Sub

        ''' <summary>
        ''' Process schema operations
        ''' </summary>
        ''' <param name="node"></param>
        ''' <param name="elementDef"></param>
        Private Sub processOperation(ByVal node As TreeNode, ByVal elementDef As SchemaTypeDefinition)
            Dim def As SchemaElementDefinition
            For Each def In elementDef.ElementDefinitions
                ' add node
                Dim nextChild As TreeNode = node.Nodes.Add(def.Name.ToString(), def.Name.ToString() + " (" + _
                    def.TypeDefinition.Datatype.ToString() + ")")
                ' store node element definition
                nextChild.Tag = def
                ' check if definition had child definitions
                If (def.TypeDefinition.NumElementDefinitions > 0) Then
                    ' process child definition
                    processOperation(nextChild, def.TypeDefinition)
                End If
            Next
        End Sub
#End Region

#Region "Control Event"
        ''' <summary>
        ''' Get service
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonGetService_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonGetService.Click
            Dim service As Service
            Dim serviceName As String = textBoxService.Text.Trim()
            Dim itemIndex As Integer = -1
            If (serviceName.Length > 0) Then
                ' check if service exist
                If (Not d_serviceDictionary.ContainsKey(serviceName)) Then
                    ' open services
                    If (d_session.OpenService(serviceName)) Then
                        service = d_session.GetService(serviceName)
                        ' add service to dictionary
                        d_serviceDictionary.Add(serviceName, service)
                        ' create item for service
                        listBoxServices.SelectedIndex = listBoxServices.Items.Add(serviceName)
                    Else
                        ' unable to open service
                        MessageBox.Show("Unable to open service " + serviceName + ".", "Invalid Service")
                    End If
                Else
                    ' select service
                    itemIndex = listBoxServices.Items.IndexOf(serviceName.ToLower())
                    listBoxServices.SelectedIndex = itemIndex
                End If
            End If
        End Sub

        ''' <summary>
        ''' User press enter in service textbox
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub textBoxService_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles textBoxService.KeyDown
            If (e.KeyCode = Keys.Enter) Then
                ' call get service button
                buttonGetService_Click(sender, New EventArgs())
            End If
        End Sub

        ''' <summary>
        ''' Process selected service schema
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub listBoxServices_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles listBoxServices.SelectedIndexChanged
            ' only display service if it is selected and different from previous selection
            If (Not (listBoxServices.SelectedIndex = -1 Or _
                listBoxServices.SelectedIndex = d_currentServiceIndex)) Then
                ' clear properties list
                listViewProperties.Items.Clear()
                richTextBoxDescription.Text = String.Empty
                ' get service name
                Dim service As String = listBoxServices.SelectedItem.ToString()
                ' process service schema
                decodeServiceSchema(service)
                d_currentServiceIndex = listBoxServices.SelectedIndex
            End If
        End Sub

        ''' <summary>
        ''' Display element properties
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub treeViewSchema_AfterSelect(ByVal sender As Object, ByVal e As TreeViewEventArgs) Handles treeViewSchema.AfterSelect
            Dim node As TreeNode = e.Node
            Dim def As SchemaElementDefinition
            ' clear properties list
            listViewProperties.Items.Clear()
            richTextBoxDescription.Text = String.Empty

            If (Not node.Tag Is Nothing) Then
                ' get element definition
                def = CType(node.Tag, SchemaElementDefinition)
                ' get description
                If (Not def.Description Is Nothing) Then
                    richTextBoxDescription.Text = def.Description
                End If
                ' add properties to list
                Dim prop As ListViewItem = listViewProperties.Items.Add("Name:")
                prop.SubItems.Add(def.Name.ToString())
                prop = listViewProperties.Items.Add("Status:")
                prop.SubItems.Add(def.Status.ToString())
                prop = listViewProperties.Items.Add("Type:")
                prop.SubItems.Add(def.TypeDefinition.Datatype.ToString())
                If (def.TypeDefinition.IsEnumerationType) Then
                    Dim item As Constant
                    For Each item In def.TypeDefinition.Enumeration.Values
                        Dim enumDisplay As ListViewItem = listViewProperties.Items.Add("")
                        enumDisplay.SubItems.Add(item.GetValueAsString())
                    Next
                End If
                prop = listViewProperties.Items.Add("Minimal Occurence:")
                prop.SubItems.Add(def.MinValues.ToString())
                prop = listViewProperties.Items.Add("Maximal Occurence:")
                prop.SubItems.Add(def.MaxValues.ToString())
                prop = listViewProperties.Items.Add("Constraints:")
                If (Not def.Constraints Is Nothing) Then
                    Dim item As Constraint
                    For Each item In def.Constraints.Values
                        Dim enumDisplay As ListViewItem = listViewProperties.Items.Add("")
                        enumDisplay.SubItems.Add(item.ConstraintType.ToString())
                    Next
                End If
            End If
        End Sub
#End Region

    End Class
End Namespace
