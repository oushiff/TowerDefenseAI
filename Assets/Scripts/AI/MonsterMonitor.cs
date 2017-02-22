using System;

// Moster category, health, amounts, location
using System.Collections;


namespace AI.Monitors
{
	public class MonsterMonitor : Monitor
	{
		public MonsterMonitor ()
		{
			//todo in the future
		}

		public ArrayList getMonsterID ()
		{
			// return monster id
			return new ArrayList ();
		}

		public MonsterData getMonsterInfo (int id)
		{
			// return the info of one monster
			// need to understand the meaning of this two parameters
			return new MonsterData ("", 1);
		}
	}
}

