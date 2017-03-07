using System;

namespace AI.DTO
{
	public class Tile
	{
		int[] pos;
		int wayPoint;

		public Tile (int[] pos, int wayPoint)
		{
			this.pos = pos;
			this.wayPoint = wayPoint;
		}
	}
}

