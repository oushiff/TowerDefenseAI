using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace AI
{
	public class Monitor : MonoBehaviour
	{
		public GameData gameData;
		public GamePlay gamePlay;
		public GameObject gameObject;

		public Monitor ()
		{
			this.gameData = GameData.instance;
			this.gamePlay = GamePlay.instance;
			this.gameObject = new GameObject ();
		}
	}
}
