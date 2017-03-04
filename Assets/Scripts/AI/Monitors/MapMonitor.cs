using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI.Monitors
{
	public class MapMonitor: Monitor
	{
		private GameData gameData;
		public List<int[]> mapGrid;

		public MapMonitor ()
		{
			gameData = new GameData ();
			gameData.Load ();
			this.mapGrid = gameData.GetCurrentLevel ().grid;
		}

		public List<double[]> GetRoadsCoordinates ()
		{
			List<double[]> res = new List<double[]> ();
			for (int i = 0; i < mapGrid.Count; i++) {
				for (int j = 0; j < mapGrid [i].Length; j++) {
					if (mapGrid [i] [j] == 40 || mapGrid [i] [j] == 47) {
						res.Add (new double[]{ i, j });
					}
				}
			}
			return res;
		}

		public List <double[]> GetAllCandidateSpacesAtBeginning ()
		{
			List <double[]> res = new List<double[]> ();
			for (int i = 0; i < mapGrid.Count; i++) {
				for (int j = 0; j < mapGrid [i].Length; j++) {
					if (mapGrid [i] [j] == 33) {
						res.Add (new double[]{ i, j });
					}
				}
			}
			return res;	
		}
	}
}

