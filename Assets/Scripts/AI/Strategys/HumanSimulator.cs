using System;
using System.Collections.Generic;
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
		{
			return a;
		}

		public HumanSimulator ()
		{
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

		}


		public List<double[]> FindBestPlaceToBuildTower (List<double[]> roads, List<double[]> candidates)
		{
			Dictionary<double[], int> dict = new Dictionary<double[], int> ();

			for (int i = 0; i < candidates.Count; i++) {
				int value = 0;
				for (int j = 0; j < roads.Count; j++) {
					//					if (isCloseEnough (candidates [i], roads [j])) {
					if (Math.Sqrt (Math.Pow (candidates [i] [0] - roads [j] [0], 2) + Math.Pow (candidates [i] [1] - roads [j] [1], 2)) < 3) {
						value++;
					}
				}
				//				Debug.Log (value + " " + candidates [i]);
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

