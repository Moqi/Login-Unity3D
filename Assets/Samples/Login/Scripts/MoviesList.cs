using UnityEngine;
using System.Collections;
using System.Text;

public class MoviesList : MonoBehaviour {
	
	public Texture2D backgroundImg;
	public GUISkin skin;
	
	public IList movies;
	private string errorMsg = "";
	
	
	// Use this for initialization
	void Start () {	
		GamedoniaData.Search("movies","{}",OnSearchMovies);
	}
	
	void OnGUI () {
		
		GUI.skin = skin;
		
		GUI.DrawTexture(UtilResize.ResizeGUI(new Rect(0,0,320,480)),backgroundImg);
		
		GUI.Label(new Rect(80,10, 220,25),"Movies List:","LabelBold");
		
			
		GUILayout.BeginArea(new Rect(80,40,260,400));
		
		//Draw table header
		GUILayout.BeginHorizontal();
		GUILayout.Label("Title", "LabelVerySmallBold",GUILayout.Width((int)(220*0.40)));
		GUILayout.Label("Director", "LabelVerySmallBold",GUILayout.Width((int)(220*0.40)));
		GUILayout.Label("Rating", "LabelVerySmallBold",GUILayout.Width((int)(220*0.20)));
		GUILayout.EndHorizontal();
					
		if (movies != null && movies.Count > 0) {
			foreach (IDictionary movie in movies) {
								
				GUILayout.BeginHorizontal();
				
				GUILayout.Label(movie["title"] as string, "LabelVerySmall",GUILayout.Width((int)(220*0.40)));
				GUILayout.Label(movie["director"] as string, "LabelVerySmall",GUILayout.Width((int)(220*0.40)));
				GUILayout.Label(movie["rating"].ToString(), "LabelVerySmall",GUILayout.Width((int)(220*0.20)));												
				
				GUILayout.EndHorizontal();
												
			}
		}else {
			
			//No results line
			GUILayout.BeginHorizontal();
			GUILayout.Label("No movies found!.",GUILayout.Width(220));
			GUILayout.EndHorizontal();
		}
		
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("",GUILayout.Height(25));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Movie",GUILayout.Width(220), GUILayout.Height(50))) {
			Application.LoadLevel("AddMovieScene");
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Back",GUILayout.Width(220), GUILayout.Height(50))) {
			Application.LoadLevel("UserDetailsScene");
		}
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();	
		
		if (errorMsg != "") {
			GUI.Box (new Rect ((Screen.width - (UtilResize.resMultiplier() * 260)),(Screen.height - (UtilResize.resMultiplier() * 50)),(UtilResize.resMultiplier() * 260),(UtilResize.resMultiplier() * 50)), errorMsg);
			if(GUI.Button(new Rect (Screen.width - 20,Screen.height - UtilResize.resMultiplier() * 45,16,16), "x","ButtonSmall")) {
				errorMsg = "";
			}
		}
				
	}
	
	void OnSearchMovies(bool success, IList movies) {
		
		if (success) {
			this.movies = movies;
		}else {
			errorMsg = Gamedonia.getLastError().ToString();
			Debug.Log(errorMsg);
		}
		
	}
}
