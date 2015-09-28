//namespace TheVandals
//{
//	using UnityEditor;
//	using UnityEngine;
//	using System.Collections;
//	
//	[CustomEditor(typeof(TileObject))]
//	[CanEditMultipleObjects]
//	public class TileEditor : Editor {	
//		
//		SerializedProperty heightProp;
//		SerializedProperty isActiveProp;
//		
//		void OnEnable () {
//			heightProp = serializedObject.FindProperty ("height");
//			isActiveProp = serializedObject.FindProperty ("isActive");
//		}
//		
//		public override void OnInspectorGUI() {
//			// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
//			serializedObject.Update ();
//			
//			// Show the custom GUI controls.
//			EditorGUILayout.Slider (heightProp, 0, 100, new GUIContent ("heightProp"));
//			EditorGUILayout.Toggle(new GUIContent ("isActiveProp"),isActiveProp.boolValue);
//			serializedObject.ApplyModifiedProperties ();
//		}
//		
//		// Custom GUILayout progress bar.
//		void ProgressBar (float value, string label) 
//		{
//			// Get a rect for the progress bar using the same margins as a textfield:
//			Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");
//			EditorGUI.ProgressBar (rect, value, label);
//			EditorGUILayout.Space ();
//		}
//	}
//}