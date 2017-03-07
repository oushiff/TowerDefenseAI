using System;
using AI.Monitors;
using System.Collections.Generic;

namespace AI.DTO
{
	public class Tower
	{
		public int range;
		public int[] money_level;
		public int[] current_level;
		public int armor;
		public int reboot_time;
		public int kills;

		public int[] pos;  
		public double attack;
		public double freq;
		public double effect;

		public TowerMonitor t_monitor = new TowerMonitor ();


		public Tower ()
		{
			this.kills = 0;
		}


		public double getMaxDamage(List<int> inRangeDistance, List<int> effectDistance) {
			double sum = 0;
			double dps = (double) attack / freq;
			foreach (int distance in inRangeDistance) {
				sum += dps * distance;
			}

			foreach (int distance in effectDistance) {
				sum += effect * distance;
			}
			return sum;
		}
	}
}

	