using System;
using AI.Monitors;
using System.Collections.Generic;
using System.Collections;

namespace AI.DTO
{
	public class Money
	{
		public int currentMoney;
		public int startingMoney;
		public ArrayList money_sequence;

		private MoneyMonitor m_monitor = new MoneyMonitor ();

		public Money ()
		{
			this.currentMoney = m_monitor.GetCurrentMoney ();
			this.startingMoney = m_monitor.GetStartingMoney ();
			this.money_sequence = new ArrayList ();
		}

		public void UpdateMoneySeq (int money)
		{
			this.money_sequence.Add (money);
		}

		public void RemoveMoneySeq (int index)
		{
			this.money_sequence.RemoveAt (index);
		}
	}
}

