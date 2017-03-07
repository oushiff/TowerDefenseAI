using System;
using System.Collections.Generic;
<<<<<<< HEAD
using System.Collections;
using UnityEngine;
using AI.DTO;
using AI.Operator;

namespace AI.Strategys
{
	/**
	 * This class is a stategy human designed
	 * for playing the TowerDefence game
     */
	public class HumanSimulator : MonoBehaviour
	{
		/**
		 * Construct a HumanSimulator Object
		 * 
		 * @param delta the mount of data
		 * @return void
		 */
		public Map map;
		public Money money;
		public Monster monster;
		public Tower tower;
		public List<double[]> rank;

		public int index = 0;  // index of the place to build tower


		public int test (int a)
=======
using AI.DTO;

namespace AI.Strategys
{
	/**   * This class is a stategy human designed   * for playing the TowerDefence game      */
	public class HumanSimulator
	{
		/**    * Construct a HumanSimulator Object    *     * @param delta the mount of data    * @return void    */
		//  public int test (int a)
		//  {
		//   return a;
		//  }

		public Map map;
		public Money money;
		public List<Monster> monsters;
		public List<Tower> towers;


//		public class Position_Tower : IComparable {
//			double[] pos;
//			double maxDamage;
//			//Position_Tower nextPT;
//			public Position_Tower(double[] pos, double maxDamage) {
//				this.pos = pos;
//				this.maxDamage = maxDamage;
//				//nextPT = null;
//			}
//
//			public int CompareTo(object obj) {
//				if (obj == null) return 1;
//
//				Position_Tower otherPosition_Tower = obj as Position_Tower;
//				if (otherPosition_Tower != null) 
//					return this.maxDamage - otherPosition_Tower.maxDamage;
//				else
//					throw new ArgumentException("Object is not a Position_Tower");
//			}
//
//		}
//
//
//		public class Tile {
//			double[] pos;
//			int remainDistane;
//			public Tile(double[] pos, int remainDistane) {
//				this.pos = pos;
//				this.remainDistane = remainDistane;
//			}
//		}

		public HumanSimulator ()
>>>>>>> a0649d434e485f6f22b68a035ef4a2b0bf2c8595
		{
			map = new Map();
			money = new Money();
			//monsters = new Monster();
			//towers = new Tower(); 
		}

//
//		public List<Tower> getTowersInLevel1() {
//		
//		}
//
//
//
//		public List<Tile> getTilesInRange(double[] pos, int range){
//		}

//		public void calDistance(double[] pos, int range, List<int> inRangeDistance, List<int> extraDistance) {
//			List<Tile> tiles = getTilesInRange (pos, range);
//			IDictionary<int, int> pathDic = 
//				new Dictionary<int, int>();
//			foreach (Tile tile  in tiles) {
//				int key = tile.remainDistane;
//				if (pathDic.ContainsKey (key)) {
//					pathDic.Add (key, pathDic [key] + 1);
//				} else {
//					pathDic.Add (key, 1);
//				}
//			}
//			foreach(KeyValuePair<int, int> entry in pathDic)
//			{
//				inRangeDistance.Add (entry.Value);
//				extraDistance.Add (entry.Key);
//			}
//		}

//		public void initPriorityQueue(SortedList priorityQueue) {
//			List<double[]> vacant_pos = map.vacant_pos;
//			List<Tower> towers = getTowersInLevel1 ();
//			foreach (double[] pos in vacant_pos) {
//				double maxDamage = 0;
//				Tower bestTower = null;
//				foreach (Tower tower in towers) {
//					List<int> inRangeDistances = new List<> ();
//					List<int> extraDistances = new List<> ();
//					calDistance (pos, tower.range, inRangeDistances, extraDistances);
//					double damge = tower.getMaxDamage (inRangeDistances, extraDistances);
//					if (damge > maxDamage) {
//						maxDamage = damage;
//						bestTower = tower;
//					}
//				}
//				priorityQueue.add (new Position_Tower (pos, bestTower));
//			}
//		}

		public void humanSimulator ()
		{
<<<<<<< HEAD
			map = new Map();
			money = new Money();
			monster = new Monster();
			tower = new Tower();	

		}


		public void humanSimulator ()
		{
			int moneyCount = money.currentMoney;
			if (moneyCount > 60) {
				
				List<Double[]> c_roads = map.roads;
				List<Double[]> candidateTower = map.tower_pos;
				GameOperator go = new GameOperator();
				//rank 
				rank = FindBestPlaceToBuildTower(c_roads,candidateTower);
				  
				for (int i = 0; i < rank.Count; i++) {
					Debug.Log ("rank hahahha zhaoni" + rank [i][0]);
				}
				if (go.BuildTower (index % 4, rank [index])) {
					index++;
					index %= rank.Count;
				}
			}

=======
			List<Double[]> c_roads = map.roads;

		//	SortedList priorityQueue = new SortedList ();

		//	initPriorityQueue (priorityQueue);


			//GameOperater go = new GameOperater ();
			List<double[]> rank = FindBestPlaceToBuildTower (c_roads, map.vacant_pos);
			//          if (go.BuildTower (index % 4, rank [index])) {
			//              index++;
			//              index %= rank.Count;
			//          }
>>>>>>> a0649d434e485f6f22b68a035ef4a2b0bf2c8595
		}


		public List<double[]> FindBestPlaceToBuildTower (List<double[]> roads, List<double[]> candidates)
		{
			Dictionary<double[], int> dict = new Dictionary<double[], int> ();

			for (int i = 0; i < candidates.Count; i++) {
				int value = 0;
				for (int j = 0; j < roads.Count; j++) {
<<<<<<< HEAD
					//					if (isCloseEnough (candidates [i], roads [j])) {
=======
					//     if (isCloseEnough (candidates [i], roads [j])) {
>>>>>>> a0649d434e485f6f22b68a035ef4a2b0bf2c8595
					if (Math.Sqrt (Math.Pow (candidates [i] [0] - roads [j] [0], 2) + Math.Pow (candidates [i] [1] - roads [j] [1], 2)) < 3) {
						value++;
					}
				}
<<<<<<< HEAD
				//				Debug.Log (value + " " + candidates [i]);
=======
				//    Debug.Log (value + " " + candidates [i]);
>>>>>>> a0649d434e485f6f22b68a035ef4a2b0bf2c8595
				dict.Add (candidates [i], value);
			}
			List<double[]> res = new List<double[]> ();
			while (dict.Count != 0) {
				int max_value = 0;
				double[] max_key = new double[]{ 0, 0 };
				foreach (var pair in dict) {
					if (pair.Value >= max_value) {
						max_value = pair.Value;
						max_key = pair.Key;
					}
				}
				res.Add (max_key);
				dict.Remove (max_key);
			}
			return res;
		}


	}
}
