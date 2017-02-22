using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace AI
{
	public class Monitor : MonoBehaviour
	{
		// need to declare some interfaces
		public static Monitor _instance = null;

		public static Monitor instance {
			set {
				_instance = value;
				if (_instance != null) {
					_instance.Initialize ();
				}
			}
			get {
				if (_instance == null) {
					_instance = (Monitor)FindObjectOfType (typeof(Monitor));

					if (_instance == null) {
						GameObject newInstance = new GameObject ("Monitor");
						_instance = newInstance.AddComponent <Monitor> ();
						DontDestroyOnLoad (newInstance); 						// may change in the future
					}
				}
				return _instance;
			}
		}

		public void Initialize ()
		{
//			_instance.LoadMonsters ();
//			_instance.LoadTowers ();
//			_instance.LoadCards ();
		}
	}
}

