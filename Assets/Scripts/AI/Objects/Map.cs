using System;
using System.Collections.Generic;
using AI.Monitors;
using System.Diagnostics;

namespace AI.DTO
{
	public class Map
	{
		public List<double[]> roads;
		public List<double[]> vacant_pos;
		public List<double[]> tower_pos;
		public List<Tower> towers;

		public int monster_count;
		public int remain_life;

		private MapMonitor m_monitor;

		public Map ()
		{
			m_monitor = new MapMonitor ();
			this.roads = m_monitor.GetRoadsCoordinates ();
			this.vacant_pos = m_monitor.GetAllCandidateSpacesAtBeginning ();
			this.monster_count = m_monitor.GetMonsterCount ();
			this.remain_life = m_monitor.GetLife ();
			this.tower_pos = m_monitor.GetAllCandidateSpacesAtBeginning ();
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

