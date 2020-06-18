using System;
using UnityEditor;

public static class GUIDebuggers {
	[MenuItem("Window/Analysis/GUI View Debugger")]
	public static void GUIViewDebuggerWindow() { EditorWindow.GetWindow(Type.GetType("UnityEditor.GUIViewDebuggerWindow,UnityEditor"));}
	
//	[MenuItem("Window/Analysis/UIElements Debugger")]
//	public static void UIElementsDebuggerWindow() { EditorWindow.GetWindow(Type.GetType("UnityEditor.Experimental.UIElements.Debugger.UIElementsDebugger,UnityEditor"));}
}