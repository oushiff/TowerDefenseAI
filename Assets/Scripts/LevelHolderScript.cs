using UnityEngine;
using System.Collections;

public class LevelHolderScript : MonoBehaviour {
	void Awake()
	{	
		DontDestroyOnLoad(this);
	}
}
