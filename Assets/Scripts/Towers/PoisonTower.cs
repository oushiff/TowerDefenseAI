using UnityEngine;
using System.Collections;

public class PoisonTower : Tower {
	protected override void Start()
	{
		type = "Poison";
		base.Start ();
	}
}