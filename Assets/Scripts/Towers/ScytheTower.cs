using UnityEngine;
using System.Collections;

public class ScytheTower : Tower {
	public float extraGain = 0;
	protected override void Start()
	{
		type = "Scythe";
		base.Start ();
	}

	public override TowerFire SpawnProjectile(Vector3 position, Quaternion rotation)
	{
		ScytheTowerFire projectile = base.SpawnProjectile (position, rotation) as ScytheTowerFire;
		if (projectile != null) {
			projectile.tower = this;
		}
		return projectile;
	}

	public void SetExtraGain (float amount, float duration)
	{
		extraGain = amount;
		Invoke ("ResetExtraGain", duration);
	}

	private void ResetExtraGain()
	{
		extraGain = 0;
	}
}