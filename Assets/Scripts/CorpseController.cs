﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseController : MonoBehaviour {

	public Transform chunk;

	public int numChunks;

	//destroy corpses on fire collision	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "fire") {
			Destroy(this.gameObject);
		}
	}

	//spawn a bunch of smaller, self-destructing corpse prefabs in random upward directions
	public void Burst() {
		for (int i=0; i<numChunks; i++) {
			Transform newChunk = Instantiate(chunk, this.transform.position, Quaternion.identity);
			Rigidbody2D cRB = newChunk.GetComponent<Rigidbody2D>();
			cRB.velocity = new Vector2(Random.Range(-4, 4), Random.Range(4, 6));
		}
		Destroy(this.gameObject);
	}
}
