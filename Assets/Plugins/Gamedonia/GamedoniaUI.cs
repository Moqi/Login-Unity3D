using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class GamedoniaUI 
{
	
	#if UNITY_EDITOR	
		public static void showAlertDialog (String parameters) {}	
	
	#elif UNITY_IPHONE
		[DllImport ("__Internal")]
		public static extern void showAlertDialog (String parameters);
	
	#elif UNITY_ANDROID
		public static void showAlertDialog (String parameters) { 
		
			AndroidJNI.AttachCurrentThread(); 
	
			AndroidJavaClass uiClass = new AndroidJavaClass("com.gamedonia.utilities.GamedoniaUI");						
			uiClass.CallStatic("showAlertDialog",new object [] {parameters});
	
		}	
	#endif
	
	public GamedoniaUI () {}
	
	
}
