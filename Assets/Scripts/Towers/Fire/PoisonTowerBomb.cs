using UnityEngine;
using System.Collections;

public class PoisonTowerBomb : MonoBehaviour {

	public void Initialize (float range, float reduction, float duration)
	{
		StartCoroutine (SearchTarget (range, reduction, duration));
	}

	IEnumerator SearchTarget(float range, float reduction, float duration)
	{
		int repeats = 3;
		do {
			Collider[] enemies = Physics.OverlapSphere(transform.position, range);
			int startingIndex = Random.Range(0, enemies.Length-1);
			for (int i = 0 ; i < enemies.Length ; i++)
			{
				int index = (i + startingIndex) % enemies.Length;
				if (enemies[index].CompareTag("Enemy"))
				{
					Enemy closeEnemy = enemies[index].GetComponent<Enemy>();
					SlowDownEnemy(closeEnemy, reduction, duration);
					break;
				}
			}
			yield return new WaitForSeconds(0.5f);
		} while(repeats-- > 0);
		Destroy (gameObject);
	}
	protected void SlowDownEnemy(Enemy enemy, float reduction, float duration)
	{
		Enemy.EnemyProperties slowEnemy = new Enemy.EnemyProperties();
		slowEnemy.speed = -reduction * slowEnemy.originalSpeed;
		enemy.properties.AddProperties(slowEnemy, duration);
	}
}
