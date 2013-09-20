using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddMovie : MonoBehaviour {
	
	public Texture2D backgroundImg;
	public GUISkin skin;
	
	public IList movies;
	private string errorMsg = "";
	
	private string title = "";
	private string director = "";
	private string rating = "";
	
	void OnGUI () {
		
		GUI.skin = skin;
		// Make a text field that modifies stringToEdit.
		//GUI.backgroundColor = Color.black;
		GUI.DrawTexture(UtilResize.ResizeGUI(new Rect(0,0,320,480)),backgroundImg);
		
		GUI.Label(UtilResize.ResizeGUI(new Rect(80,10,220,20)),"Title*","LabelBold");
		title = GUI.TextField (UtilResize.ResizeGUI(new Rect (80, 30, 220, 40)), title, 25);
		
		GUI.Label(UtilResize.ResizeGUI(new Rect(80,75,220,20)),"Director*","LabelBold");
		director = GUI.TextField (UtilResize.ResizeGUI(new Rect (80, 100, 220, 40)), director,'*');
		
		GUI.Label(UtilResize.ResizeGUI(new Rect(80,145,200,20)),"Rating*","LabelBold");
		rating = GUI.TextField (UtilResize.ResizeGUI(new Rect (80, 170, 220, 40)), rating,'*');
		

		if (GUI.Button (UtilResize.ResizeGUI(new Rect (80,290, 220, 50)), "Add")) {
			
			if ((title != "")
				&& (director != "")
				&& (rating != "")) {
				
				Dictionary<string,object> movie = new Dictionary<string,object>();
				movie.Add("title",title);
				movie.Add("director",director);
				movie.Add("rating",rating);
				GamedoniaData.Create("movies",movie,OnCreateMovie);
				
			}else {
				errorMsg = "Fill all the fields with (*) correctly";
				Debug.Log(errorMsg);				
			}
		}
		
		if (GUI.Button (UtilResize.ResizeGUI(new Rect (80,345, 220, 50)), "Cancel")) {			
			Application.LoadLevel ("MoviesListScene");			
		}
		
		if (errorMsg != "") {
			GUI.Box (new Rect ((Screen.width - (UtilResize.resMultiplier() * 260)),(Screen.height - (UtilResize.resMultiplier() * 50)),(UtilResize.resMultiplier() * 260),(UtilResize.resMultiplier() * 50)), errorMsg);
			if(GUI.Button(new Rect (Screen.width - 20,Screen.height - UtilResize.resMultiplier() * 45,16,16), "x","ButtonSmall")) {
				errorMsg = "";
			}
		}
				
	}
	
	void OnCreateMovie(bool success, IDictionary movie) {
		
		if (success) {			
			Application.LoadLevel("MoviesListScene");			
		}else {
			errorMsg = Gamedonia.getLastError().ToString();		
			Debug.Log(errorMsg);
		}
	}
}
