using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {

	public float timeout = 0;
	public bool randomTimeout = false;

	void Start() {
		if (timeout > 0) {
			StartCoroutine(KillTimer(timeout));
		}
	}

	public void Kill() {
		Destroy(this.gameObject);
	}

	IEnumerator KillTimer(float seconds) {
		seconds = randomTimeout ? Random.Range(2f, 4f) : seconds;
		yield return new WaitForSeconds(seconds);
		Kill();
	}
}
