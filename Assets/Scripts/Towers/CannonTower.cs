using UnityEngine;
using System.Collections;

public class CannonTower : Tower {
	double colourChangeTimeout;
	Renderer turretRenderer;

	protected override void Awake()
	{
		base.Awake ();
		turretRenderer = turretSphere.GetComponent<Renderer> ();
	}
	protected override void Start()
	{
		type = "Cannon";
		base.Start ();
	}

	protected override void Update()
	{
		if(extradamage > 0)
		{
			turretRenderer.material.color = Color.red;
		}
		else if(colourChangeTimeout <= 0)
		{
			turretRenderer.material.color = Color.yellow;
		}
		else
		{
			turretRenderer.material.color = Color.green;
		}
		
		if(colourChangeTimeout > 0)
		{
			colourChangeTimeout -= Time.deltaTime;
		}

		base.Update ();
	}

	public override void PowerShot(float damage)
	{
		extradamage = properties.active.damage * damage;
	}
}