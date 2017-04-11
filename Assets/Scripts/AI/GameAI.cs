using System;
using UnityEngine;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Configuration;
using System.Threading;

namespace AI
{
	[System.Serializable]
	public class GameAI: MonoBehaviour
	{
		private MapMonitor m_map;
		//means map monitor
		private TowerMonitor m_tower;
		//		private MonsterMonitor m_monster;
		private MoneyMonitor m_money;

		private List<double[]> c_roads;
		// double List<double x, double y> the coordinates
		private ArrayList monsters;
		private ArrayList tower;
		private int money;
        public List<Vector3> positions = new List<Vector3>();

		//tmp value, need delete in the future
		public List<double[]> rank;
		// index of the place to build tower
		public int index = 0;
		public int time = 0;
        public Vector3 pos = new Vector3();
		GameOperater go;

        void Start ()
		{
			m_map = new MapMonitor ();
			m_tower = new TowerMonitor ();
//			m_monster = new MonsterMonitor ();
			m_money = new MoneyMonitor ();
			money = m_money.GetStartingMoney ();
			c_roads = m_map.GetRoadsCoordinates ();
            Time.timeScale = 15;

			go = new GameOperater ();
            
        }
         
		void Update ()
		{
			money = m_money.GetCurrentMoney ();
			int time_n = (int)Time.time;
			if (money > 60 && (time_n % 5 == 0) && time_n != time) {
				DoAnalysis ();
				time = time_n;
			}
		}

		public void DoAnalysis ()
		{
			Debug.Log ("PosListNum:   " + positions.Count);


			rank = FindBestPlaceToBuildTower (c_roads, m_map.GetAllCandidateSpacesAtBeginning ());
            pos.x = (float)rank[index][0];
            pos.y = 0.0f;
            pos.z = (float)rank[index][1];
            if (!positions.Contains(pos)&&(positions.Count!=9))
            {
                if (go.BuildTower(index % 4, rank[index]))
                {
                    index++;
                    index %= rank.Count;
                    positions.Add(pos);
                }
            }
            else if(positions.Count == 9)
            {

				
				if (go.UpgradeTower(index % 4, rank[index]))
                {
					index++;
					index %= rank.Count;
                }


            }
		}

		public Boolean DoOperation (string operation_type, double[] location)
		{
			return false;
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

		//		public Boolean isCloseEnough (object x, object y)
		//		{
		//			if (Math.Sqrt (Math.Pow (x [0] - y [0], 2) + Math.Pow (x [1] - y [1], 2)) < 3)
		//				return true;
		//			return false;
		//		}

		public void ResetPositionsList() {
			positions.Clear ();
		}
	
	}



}

