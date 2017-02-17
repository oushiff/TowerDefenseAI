using UnityEngine;
using System.Collections;

public class ElectricalTower : Tower {
	protected override void Start()
	{
		type = "Electrical";
		base.Start ();
	}
}