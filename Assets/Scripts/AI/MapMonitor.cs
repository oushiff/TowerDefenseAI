using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
	public class MapMonitor: Monitor
	{
		private GameData gameData;
		public List<int[]> mapGrid;

		public static MapMonitor instance;

		public static MapMonitor Instance {
			get { return instance ?? (instance = new GameObject("MapMonitor").AddComponent<MapMonitor>()); }
		}

		public void init()
		{
			gameData = new GameData ();
			gameData.Load ();
			this.mapGrid = gameData.GetCurrentLevel ().grid;
		}

		public MapMonitor ()
		{
//			gameData = new GameData ();
//			gameData.Load ();
//			this.mapGrid = gameData.GetCurrentLevel ().grid;
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

		public LevelData GetPlaceHaveTower ()
		{
			// todo
			return null;
		}

		public LevelData GetPlaceCouldBuildTower ()
		{
			// todo
			return null;
		}
	}
}

