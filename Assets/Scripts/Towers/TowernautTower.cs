using UnityEngine;
using System.Collections;

public class TowernautTower : Tower {
	protected override void Start()
	{
		type = "Towernaut";
		base.Start ();
	}
}