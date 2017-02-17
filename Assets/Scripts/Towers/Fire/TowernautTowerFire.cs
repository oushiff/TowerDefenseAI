using UnityEngine;
using System.Collections;

public class TowernautTowerFire : SplashTowerFire {
	protected override void Awake()
	{
		type = "Towernaut";
		base.Awake();
	}
}
