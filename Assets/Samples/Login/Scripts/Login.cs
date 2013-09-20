using UnityEngine;
using System.Collections;

public class Login : MonoBehaviour {

	/*void OnGUI () {
		// Make a background box
		GUI.Box(new Rect(10,10,100,90), "Loader Menu");

		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.Button(new Rect(20,40,80,20), "Level 1")) {
			Application.LoadLevel(1);
		}

		// Make the second button.
		if(GUI.Button(new Rect(20,70,80,20), "Level 2")) {
			Application.LoadLevel(2);
		}
	}*/
	
	public Texture2D backgroundImg;
	public GUISkin skin;
	
	private string email = "";
	private string password = "";
	private string errorMsg = "";
	

	void OnGUI () {
		
		GUI.skin = skin;
		// Make a text field that modifies stringToEdit.
		//GUI.backgroundColor = Color.black;
		GUI.DrawTexture(UtilResize.ResizeGUI(new Rect(0,0,320,480)),backgroundImg);
		
		GUI.Label(UtilResize.ResizeGUI(new Rect(80,10,220,20)),"eMail","LabelBold");
		email = GUI.TextField (UtilResize.ResizeGUI(new Rect (80, 30, 220, 40)), email, 25);
		
		GUI.Label(UtilResize.ResizeGUI(new Rect(80,75,220,20)),"Password", "LabelBold");
		password = GUI.PasswordField (UtilResize.ResizeGUI(new Rect (80, 100, 220, 40)), password,'*');
		

		if (GUI.Button (UtilResize.ResizeGUI(new Rect (80,150, 220, 50)), "Login")) {
			//print ("you clicked the text button");
			//Application.LoadLevel ("UserDetailsScene");
			GamedoniaUsers.LoginUserWithEmail(email,password,OnLogin);
		}
		
		if (GUI.Button (UtilResize.ResizeGUI(new Rect (80,205, 220, 50)), "Create Account")) {
			//print ("you clicked the text button");
			Application.LoadLevel("CreateAccountScene");
		}
		
		if (GUI.Button (UtilResize.ResizeGUI(new Rect (80,260, 220, 50)), "Remember Password")) {
			
			Application.LoadLevel("ResetPasswordScene");
		}
		
		if (errorMsg != "") {
			GUI.Box (new Rect ((Screen.width - (UtilResize.resMultiplier() * 260)),(Screen.height - (UtilResize.resMultiplier() * 50)),(UtilResize.resMultiplier() * 260),(UtilResize.resMultiplier() * 50)), errorMsg);
			if(GUI.Button(new Rect (Screen.width - 20,Screen.height - UtilResize.resMultiplier() * 45,16,16), "x","ButtonSmall")) {
				errorMsg = "";
			}
		}
				
	}
	
	void OnLogin (bool success) {
		
		if (success) {			
			Application.LoadLevel("UserDetailsScene");			
		}else {
			errorMsg = Gamedonia.getLastError().ToString();
			Debug.Log(errorMsg);
		}
		
	}
}
