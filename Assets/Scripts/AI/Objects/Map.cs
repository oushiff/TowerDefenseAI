using System;
using System.Collections.Generic;
using AI.Monitors;

namespace AI.DTO
{
	public class Map
	{
		public List<double[]> roads;
		public List<double[]> vacant_pos;
		public List<double[]> tower_pos;
		public List<Tower> towers;

		private MapMonitor m_monitor;

		public Map ()
		{
			m_monitor = new MapMonitor ();
			this.roads = m_monitor.GetRoadsCoordinates ();
			this.vacant_pos = m_monitor.GetAllCandidateSpacesAtBeginning ();

			this.tower_pos = new List<double[]> ();
		}

		public int GetMonsterCount ()
		{
			return GamePlay.instance.monsterCount;
		}

		public int GetLife ()
		{
			return GamePlay.instance.remainLife;
		}

		public void UpdatePos (double[] pos)
		{
			this.tower_pos.Add (pos);
			this.vacant_pos.Remove (pos);
		}

	}
}

