using UnityEngine;
using System.Collections;

public class GoToPost : MonoBehaviour 
{
	void OnGUI()
	{
		if(GUI.Button(new Rect(70,100,150,20),"Back to 41 Post..."))
		{
			Application.OpenURL ("http://www.41post.com/?p=1935");
		}
	}

}
