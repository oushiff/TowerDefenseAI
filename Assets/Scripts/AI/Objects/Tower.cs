using System;
using AI.Monitors;

namespace AI.DTO
{
	public class Tower
	{
		public int range;
		public int[] money_level;
		public int[] current_level;
		public int armor;
		public int reboot_time;

		//		int kills;

		public double x;
		public double y;
		public double attack;
		public double freq;

		private TowerMonitor t_monitor;


		public Tower ()
		{
			this.t_monitor = new TowerMonitor ();
		}
	}
}

	