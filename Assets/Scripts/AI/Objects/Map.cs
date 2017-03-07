﻿using System;
using System.Collections.Generic;
using AI.Monitors;
using System.Diagnostics;

//
//List<int[]> waypoints = gameData.GetCurrentLevel ().waypoints;
//Debug.Log ("@@@@");
//String logStream = "";
//foreach (int[] line in waypoints) {
//	foreach (int num in line) {
//		logStream += num;
//		logStream += " "; 
//	}
//	logStream += "\n";
//}
//Debug.Log (logStream);


namespace AI.DTO
{
	public class Map
	{
		public List<double[]> roads;
		public List<Tile> tiles;
		public List<double[]> vacant_pos;
		public List<double[]> tower_pos;
		public List<Tower> towers;

		public int[] startPos;
		public int[] destinationPos;

		public int waypointNum;
		public int monster_count;
		public int remain_life;

		private MapMonitor m_monitor = new MapMonitor ();

		public Map ()
		{
			this.roads = m_monitor.GetRoadsCoordinates ();
			this.vacant_pos = m_monitor.GetAllCandidateSpacesAtBeginning ();
			this.waypointNum = m_monitor.GetWayPointNum ();
			this.monster_count = m_monitor.GetMonsterCount ();
			this.remain_life = m_monitor.GetLife ();
			this.tower_pos = new List<double[]> ();

			int[][] startDestination = m_monitor.getStartDistanation ();
			this.startPos = startDestination [0];
			this.destinationPos = startDestination [1];
			buildTiles ();
		}


		public void buildTilesInLine(int[] prevPos, int[] destinationPos) {
			int point = m_monitor.wayPoints [destinationPos [0]] [destinationPos [1]]; 
			int dx = destinationPos [0] - prevPos [0];
			int dy = destinationPos [1] - prevPos [1];
			for (int i = prevPos [0], j = prevPos [1]; i <= destinationPos [0] && j <= destinationPos [1]; i += dx, j += dy) {
				Tile tile = new Tile (new int[2] { i, j }, point);
				this.tiles.Add (tile);
			}
		}

		public void buildTiles() { 
			int[] prevPos = this.startPos;
			int[] curPos;
			int[][] cornerPoints = m_monitor.GetAllCornerPoints ();
			foreach (int[] pos in cornerPoints) {
				curPos = pos;
				buildTilesInLine (prevPos, curPos);
				prevPos = curPos;
			}
			prevPos = curPos;
			curPos = this.destinationPos;
			buildTilesInLine (prevPos, curPos);
		}

		public Boolean UpdatePos (double x, double y)
		{
			double[] pos = { x, y };
			this.tower_pos.Add (pos);
			for (int i = 0; i < this.vacant_pos.Count; i++) {
				if (vacant_pos [i] [0] == x && vacant_pos [i] [1] == y) {
					this.vacant_pos.RemoveAt (i);
					return true;
				}
			}

			return false;
		}

	}
}

