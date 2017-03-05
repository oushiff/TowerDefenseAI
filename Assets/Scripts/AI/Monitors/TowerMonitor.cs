using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Application = UnityEngine.Application;

// tower location, rank, attack, defend... properities

namespace AI.Monitors
{
	public class TowerMonitor : Monitor
	{
		String[] type = { "Gun", "Cannon", "Poison", "Electrical", "Scythe",  "Towernaut" };

		public TowerMonitor ()
		{
			for (int i = 0; i < type.Length; i++) {
				Debug.Log (type [i]);
			}
//			for (counter = 0; counter < loadLevel.towers.Count; counter++) {
//				towers.Add (GameData.instance.GetTower (loadLevel.towers [counter], 0));
//			}
		}
	}
}

