/**
 *  Gamedonia Build PostProcessor
 *  Copyright 2012 Gamedonia Team (www.gamedonia.com)
 */ 
	

using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System;
 
public static class PostBuildTrigger
{
        	
	private static string projectPath;
	
 
    /// Processbuild Function
    [PostProcessBuild] // <- this is where the magic happens
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        Debug.Log("Starting Gamedonia Build PostProcess");
 
		projectPath = path;	
		
		ProcessPlugins();
		
    }
	
	private static void ProcessPlugins() {
						
		ExecuteCommandSync(Application.dataPath + Path.DirectorySeparatorChar + "Editor" + Path.DirectorySeparatorChar + "GamedoniaBuildPostprocessor.pyc " + projectPath);
		
	}
 
	
	
	private static void ExecuteCommandSync(string command)
	{
	     try
	     {
	     
		    System.Diagnostics.ProcessStartInfo procStartInfo =
		        new System.Diagnostics.ProcessStartInfo("python");
			procStartInfo.Arguments = command;
		
		    procStartInfo.RedirectStandardOutput = true;
		    procStartInfo.UseShellExecute = false;
		    
		    procStartInfo.CreateNoWindow = true;
		    
		    System.Diagnostics.Process proc = new System.Diagnostics.Process();
		    proc.StartInfo = procStartInfo;
		    proc.Start();
		    
	     }
	     catch (Exception objException) {
	     	Debug.Log(objException);
	     }
	}
	
}
