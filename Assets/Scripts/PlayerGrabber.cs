using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for use with moving platforms (currently unused)
public class PlayerGrabber : MonoBehaviour {

	void OnCollisionStay2D(Collision2D c) {
		if (c.gameObject.tag == "Player") {
			c.transform.parent = this.transform;
		}
	}

	void OnCollisionLeave2D(Collision2D c) {
		if (c.gameObject.tag == "Player") {
			c.transform.parent = null;
		}
	}
}
