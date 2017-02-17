#region Copyright
// <copyright file="SplashFireArea.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/08/2016, 11:02 PM </date>
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplashFireArea : MonoBehaviour {
	public List<GameObject> enemiesInRange;
	public string type;
	public int level;
	private double splashArea;

	void Awake(){
		enemiesInRange = new List<GameObject>();
	}

	// Use this for initialization
	void Start ()
	{
	    TowerData.Level tower = GameData.instance.GetTower(type, level);
	    splashArea = tower.GetSpecialProperty(GameData.GameProperies.EFFECT);
		this.GetComponent<SphereCollider> ().radius = (float) splashArea;
	}
	
	void OnTriggerEnter(Collider intruder)
	{
		if (intruder.CompareTag ("Enemy")) {
			Enemy enemy = intruder.GetComponent<Enemy> ();
			if (enemy == null && intruder.transform.parent != null)
				enemy = intruder.transform.parent.GetComponent<Enemy> ();

			if (enemy != null) {
				enemiesInRange.Add (enemy.gameObject);
			}
		}
	}
	
	void OnTriggerExit(Collider intruder)
	{
		if (intruder.CompareTag ("Enemy")) {
			Enemy enemy = intruder.GetComponent<Enemy> ();
			if (enemy == null && intruder.transform.parent != null)
				enemy = intruder.transform.parent.GetComponent<Enemy> ();
		
			if (enemy != null) {
				if (enemiesInRange.Contains (enemy.gameObject))
					enemiesInRange.Remove (enemy.gameObject);
			}
		}
	}
}
