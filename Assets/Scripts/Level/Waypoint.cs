using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {
	public int number;

	void OnDrawGizmos () {
		Gizmos.DrawIcon (transform.position, "waypoint.png", false);
	}
}
