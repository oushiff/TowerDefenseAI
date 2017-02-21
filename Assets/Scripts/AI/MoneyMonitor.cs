using System;

namespace AI
{
	public class MoneyMonitor: Monitor
	{
		public int GetStartingMoney ()
		{
			return GameData.instance.GetCurrentLevel ().startingMoney;
		}

		public int GetCurrentMoney ()
		{
			return Currency.instance.coins;
		}
	}
}

