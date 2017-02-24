using System;
using AI.Monitors;
using System.Collections.Generic;

namespace AI.DTO
{
	public class Money
	{
		public int currentMoney;
		public int startingMoney;
		public List<int> money_sequence;
		// when money change, add a past item

		private MoneyMonitor m_monitor;

		public Money ()
		{
			this.currentMoney = m_monitor.GetCurrentMoney ();
			this.startingMoney = m_monitor.GetStartingMoney ();
		}

		public void UpdateMoneySeq (int money)
		{
			this.money_sequence.Add (money);
		}

		public void UpdateMoneySeq (int money, int index)
		{
			//todo
		}

		public void RemoveMoneySeq (int index)
		{
			//todo
		}
	}
}

