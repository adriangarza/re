using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawController : MonoBehaviour {

	public float rotateSpeed;

	//time-locked instead of frame-locked
	void Update () {
		this.transform.Rotate(0, 0, rotateSpeed * Time.deltaTime * -1);
	}
}
