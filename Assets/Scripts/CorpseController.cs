using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseController : MonoBehaviour {

	//destroy corpses on fire collision	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "fire") {
			Destroy(this.gameObject);
		}
	}
}
