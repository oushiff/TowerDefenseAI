using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI.Monitors
{
	public class MapMonitor: Monitor
	{
		public List<int[]> mapGrid;

		public int wayPointsNum;
		public List<int[]> wayPoints;
		public int[] startPos;
		public int[] destinationPos;

		public MapMonitor ()
		{
			this.mapGrid = gameData.GetCurrentLevel ().grid;
			this.wayPointsNum = gameData.GetCurrentLevel ().waypointsNo;
			this.wayPoints = gameData.GetCurrentLevel ().waypoints;
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

		public int[][] GetAllCornerPoints() {
			int[][] res = new int[this.wayPointsNum] []; 
			for (int i = 0; i < wayPoints.Count; i++) {
				for (int j = 0; j < wayPoints [i].Length; j++) {
					if (wayPoints [i] [j] != 0) {
						res[wayPoints [i] [j]] = new int[2]{i, j};
					}
				}
			}
			return res;	
		} 

		public int[][] getStartDistanation() {
			int[][] res = new int[2] []; 
			for (int i = 0; i < mapGrid.Count; i++) {
				for (int j = 0; j < mapGrid [i].Length; j++) {
					if (mapGrid [i] [j] == 47) {
						res[0] = new int[2]{i, j};
					} else if (mapGrid [i] [j] == 14) {
						res[1] = new int[2]{i, j};
					}  
				}
			}
			return res;	
		}

		public int GetMonsterCount ()
		{
			return gamePlay.monsterCount;
		}

		public int GetLife ()
		{
			return gamePlay.remainLife;
		}

		public int GetWayPointNum () {
			return wayPointsNum;
		}
	}
}

