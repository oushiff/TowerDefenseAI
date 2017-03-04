using System;
using System.Collections.Generic;

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

		public Map ()
		{
		}
	}
}

