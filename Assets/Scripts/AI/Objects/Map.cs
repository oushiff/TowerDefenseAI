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
			this.tower_pos = new List<double[]> ();
		}

		public void UpdatePos (double x, double y)
		{
			double[] pos = { x, y };
			this.tower_pos.Add (pos);
			this.vacant_pos.Remove (pos);
		}

	}
}

