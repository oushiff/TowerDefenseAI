using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Application = UnityEngine.Application;

/***
 * there's no properities in GameData, so we cannot monitor the towers
 */

namespace AI.Monitors
{
	public class TowerMonitor : Monitor
	{
		public String[] type = { "Gun", "Cannon", "Poison", "Electrical", "Scythe",  "Towernaut" };

		public TowerMonitor ()
		{
//			for (int i = 0; i < type.Length; i++) {
//				Debug.Log (type [i]);
//			}
//			for (counter = 0; counter < loadLevel.towers.Count; counter++) {
//				towers.Add (GameData.instance.GetTower (loadLevel.towers [counter], 0));
//			}
		}


	}
}

