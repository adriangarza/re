using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

	public GameObject platform;
	public GameObject pointA;
	public GameObject pointB;
	public float moveSpeed = 1;

	private bool returning = false;

	void Start () {
		if (pointA == null || pointB == null) {
			Debug.LogError("You forgot to assign the movement endpoints!");
		}
		platform.transform.position = pointA.transform.position;
	}

	//moves from point A to point B and then back	
	void Update () {
		Move();
		if (!returning) {
			if (platform.transform.position == pointB.transform.position) {
				returning = true;
			}
		} else {
			if (platform.transform.position == pointA.transform.position) {
				returning = false;
			}
		}
	}

	void Move() {
		Vector3 target = returning ? pointA.transform.position : pointB.transform.position;
		float step = moveSpeed * Time.deltaTime;
        platform.transform.position = Vector3.MoveTowards(platform.transform.position, target, step);
	}
}
