﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float moveForce = 50f;
	public float maxSpeedX = 5f;
	
	public float jumpSpeed = .0005f;

	public float torque = .01f;
	public float maxAngularVelocity = .5f;

	//time to wait, in seconds, before the player collider stops being grounded
	public float groundTimeout = .2f;
	private IEnumerator groundCoroutine = null;

	private Rigidbody2D rb2d;
	private BoxCollider2D bc;

	public bool grounded = false;
	public bool frozen = false;

	//Just Death Things
	public GameObject respawnPoint;
	public Transform corpse;

	public int deaths = 0;

	public GameObject graveyard;

	//collectibles
	public int coins = 0;


	void Start () {
		this.rb2d = GetComponent<Rigidbody2D>();
		this.transform.position = new Vector3(respawnPoint.transform.position.x,
										respawnPoint.transform.position.y,
										this.transform.position.z);
	}
	
	void Update () {
		Jump();
		Move();
	}

	void Move() {
		if (HorizontalInput() && !frozen) {
			if (Input.GetKey(KeyCode.RightArrow))
            {
                rb2d.AddForce(new Vector2(moveForce, 0));
				rb2d.AddTorque(torque * -1);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                rb2d.AddForce(new Vector2(-moveForce, 0));
				rb2d.AddTorque(torque);
            }
		}

		//clamp speed, angular velocity, etc
		float speedX = this.rb2d.velocity.x;
		float speedY = this.rb2d.velocity.y;
		float angV = this.rb2d.angularVelocity;

		if (speedX < maxSpeedX * -1) {
			this.rb2d.velocity = new Vector2(maxSpeedX * -1, speedY);
		} else if (speedX > maxSpeedX) {
			this.rb2d.velocity = new Vector2(maxSpeedX, speedY);
		}

		if (angV > maxAngularVelocity) {
			this.rb2d.angularVelocity = maxAngularVelocity;
		} else if (angV < maxAngularVelocity * -1) {
			this.rb2d.angularVelocity = maxAngularVelocity * -1;
		}

		if (Input.GetKeyDown(KeyCode.Z)) {
			Die();
		}
	}

	void Jump() {
		if (Input.GetKeyDown(KeyCode.UpArrow) && grounded) {
			rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
			this.grounded = false;
		}
	}

	bool HorizontalInput()
    {
        return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow);
    }

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "platform" || other.gameObject.tag == "corpse") {
			this.grounded = true;
			
			//stop the !grounded timeout on landing
			if (groundCoroutine != null) {
				StopCoroutine(groundCoroutine);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "shock") {
			Die();
		}
		//on hitting a checkpoint
		else if (other.gameObject.tag == "Respawn") {
			HitCheckpoint(other.gameObject);
		}
		else if (other.gameObject.tag == "coin") {
			CollectCoin(other.gameObject);
		}
	}

	//wait a moment and THEN count as leaving the ground, since this thing is never touching the ground for long
	void OnCollisionExit2D(Collision2D other) {
		if (other.gameObject.tag == "platform") {
			groundCoroutine = LeaveGround();
			StartCoroutine(groundCoroutine);
		}
	}

	IEnumerator LeaveGround() {
		yield return new WaitForSeconds(groundTimeout);
		this.grounded = false;
		groundCoroutine = null;
	}


	public void Die() {
		//ow oof ouch, bone hurting environment hazards
		deaths++;

		//store the current rotation and position
		Vector3 lastPos = rb2d.transform.position;
		Vector2 lastV = rb2d.velocity;
		float lastR = rb2d.rotation;
		float lastAngV = rb2d.angularVelocity;

		//move back to the respawn point, reset movement
		this.transform.position = new Vector3(respawnPoint.transform.position.x,
												respawnPoint.transform.position.y,
												this.transform.position.z);
		rb2d.angularVelocity = 0;
		rb2d.velocity = new Vector2(0, 0);
		rb2d.rotation = 0;	

		//create a corpse in the same spot, and add the last attributes
		Transform newcorpse = Instantiate(corpse, lastPos, Quaternion.identity);
		Rigidbody2D cRB = newcorpse.GetComponent<Rigidbody2D>();
		cRB.velocity = lastV;
		cRB.rotation = lastR;
		cRB.angularVelocity = lastAngV;

		//then put the corpse in the container for possible removal later on
		newcorpse.transform.parent = graveyard.transform;
	}

	void RemoveCorpses() {
		foreach (Transform corpse in graveyard.transform) {
			Destroy(corpse.gameObject);
		}
	}

	void HitCheckpoint(GameObject cp) {
		this.respawnPoint = cp;
		Animator cpAnim = cp.GetComponent<Animator>();

		//only play the animation and remove corpses if the checkpoint hasn't been hit yet
		if (!cpAnim.GetBool("active")) {
			cpAnim.SetBool("active", true);
			RemoveCorpses();
		}
	}

	void CollectCoin(GameObject coin) {
		this.coins++;
		coin.GetComponent<Animator>().SetTrigger("get");
	}
}
