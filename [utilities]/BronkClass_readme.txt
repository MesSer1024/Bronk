Intention:
	Template for easily creating classes inside Visual Studio
	Place in following place: C:\Users\<your users name>\Documents\Visual Studio 2010\Templates\ItemTemplates


Also possible to do the following by creating a macro in VS2010:
	Create new Class (Temp or some random name)
	Tools/Macro/Record Macro
	Ctrl + home (move to first item)
	Remove all using-declaratives not being used (ending up with only having "using System;")
	Navigate down to namespace-line, remove every text there and replace with Bronk so it ends up with "namespace Bronk"
	Tools/Macro/Stop Recording
	Tools/Macro/Save Macro

Alternatively just record macro/save macro then edit the macro and replace it with following code:

Option Strict Off
Option Explicit Off
Imports System
Imports EnvDTE
Imports EnvDTE80
Imports EnvDTE90
Imports EnvDTE90a
Imports EnvDTE100
Imports System.Diagnostics

Public Module RecordingModule


    Sub to_bronk_class()
        DTE.ActiveDocument.Selection.StartOfDocument()
        DTE.ActiveDocument.Selection.LineDown()
        DTE.ActiveDocument.Selection.LineDown(True, 3)
        DTE.ActiveDocument.Selection.Delete()
        DTE.ActiveDocument.Selection.LineDown()
        DTE.ActiveDocument.Selection.EndOfLine()
        DTE.ActiveDocument.Selection.WordLeft(True, 7)
        DTE.ActiveDocument.Selection.WordRight(True)
        DTE.ActiveDocument.Selection.Text = "Bronk"
        DTE.ActiveDocument.Save()
    End Sub
End Module
