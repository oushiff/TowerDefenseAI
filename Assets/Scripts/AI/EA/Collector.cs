using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

public class Collector: MonoBehaviour
{
	public static Collector instance;

	public int spendMoney;
	public int enemyDeadAmount;
	public int enemyArrivedAmount;
	public int upgradeTowerNum;
	public int monsterCount;
	public double enemyLeftDistance;

	//public GameObject gameObject;

	public static Collector Instance
	{
		// Here we use the ?? operator, to return 'instance' if 'instance' does not equal null
		// otherwise we assign instance to a new component and return that
		get { return instance ?? (instance = new GameObject("Collector").AddComponent<Collector>()); }
	}

	public void init ()
	{
		this.spendMoney = 0; // total used money, default is negative
		this.enemyDeadAmount = 0;
		this.enemyArrivedAmount = 0;
		this.upgradeTowerNum = 0;
		this.monsterCount = 0;
		this.enemyLeftDistance = 0; // its the sum of total enemy left distance  
	}
}

