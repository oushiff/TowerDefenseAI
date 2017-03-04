using System;
using System.Collections.Generic;
using AI;
using Monitor

namespace AI.DTO
{
	public class Map
	{
		List<double[]> roads;
		List<double[]> vacant_pos;
		// couble build tower
		List<double[]> tower_pos;
		// has build tower
		List<Tower> towers;
		int life;

		MapMonitor m_monitor;

		public Map ()
		{
		}
	}
}

