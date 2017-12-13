using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	public bool grounded = false;
	public bool frozen = false;

	//Just Death Things
	public GameObject respawnPoint;
	public Transform corpse;

	public int deaths = 0;

	public GameObject graveyard;

	//collectibles
	public int coins = 0;

	//set to false after getting flung off a jump pad
	public bool speedClamped = true;

	//ui elements
	public Text deathCount;
	//e.g. "deaths: "
	string deathText;

	public CanvasGroup cg;
	private bool flash = false;
	private bool ended = false;

	int MAX_CORPSES = 100;

	void Start () {
		this.rb2d = GetComponent<Rigidbody2D>();
		this.transform.position = new Vector3(respawnPoint.transform.position.x,
										respawnPoint.transform.position.y,
										this.transform.position.z);
		deathText = deathCount.text;
	}

	void UpdateUI() {
		deathCount.text = deathText + this.deaths;

		//screen flashing white on death
		if (flash) {
			//make the flash fade quickly, but not instantly
			cg.alpha -= Time.deltaTime * 5;
			if (cg.alpha <= 0) {
				flash = false;
			}
		}

		if (ended) {
			cg.alpha += Time.deltaTime * 2;
		}
	}
	
	void Update () {
		Jump();
		Move();

		//call UpdateUI at the end so it happens on the same frame as a gameworld update
		UpdateUI();
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

		//since we're applying force instead of directly changing velocity
		ClampSpeed();

		//remove suicide ability for release
		/* 
		if (Input.GetKeyDown(KeyCode.Z)) {
			Suicide();
		} 
		if (Input.GetKeyDown(KeyCode.X)) {
			RemoveCorpses();
		}
		*/
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
			this.speedClamped = true;
			//stop the !grounded timeout on landing
			if (groundCoroutine != null) {
				StopCoroutine(groundCoroutine);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "shock" 
			|| other.gameObject.tag == "saw") {
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
		//flash the screen white for a frame or two
		FlashWhite();

		deaths++;

		//store the current rotation and position
		Vector3 lastPos = rb2d.transform.position;
		Vector2 lastV = rb2d.velocity;
		float lastR = rb2d.rotation;
		float lastAngV = rb2d.angularVelocity;

		Respawn();	

		//create a corpse in the same spot, and add the last attributes
		Transform newcorpse = Instantiate(corpse, lastPos, Quaternion.identity);
		Rigidbody2D cRB = newcorpse.GetComponent<Rigidbody2D>();
		cRB.velocity = lastV;
		cRB.rotation = lastR;
		cRB.angularVelocity = lastAngV;

		//then put the corpse in the container for possible removal later on
		newcorpse.transform.parent = graveyard.transform;
	}

	public void Respawn() {
		//move back to the respawn point, reset movement
		this.transform.position = new Vector3(respawnPoint.transform.position.x,
												respawnPoint.transform.position.y,
												this.transform.position.z);
		rb2d.angularVelocity = 0;
		rb2d.velocity = new Vector2(0, 0);
		rb2d.rotation = 0;
	}

	void RemoveCorpses() {
		//don't destroy the physics engine if there are a ton of corpses
		if (graveyard.transform.childCount > MAX_CORPSES) {
			foreach (Transform corpse in graveyard.transform) {
				Destroy(corpse.gameObject);
			}
		} else {
			foreach (Transform corpse in graveyard.transform) {
				corpse.gameObject.GetComponent<CorpseController>().Burst();
			}
		}
	}

	void HitCheckpoint(GameObject cp) {
		if (cp.name == "END") {
			EndGame();
		}

		Animator cpAnim = cp.GetComponent<Animator>();		
		//only play the animation, set respawn and remove corpses if the checkpoint hasn't been hit yet
		if (!cpAnim.GetBool("active")) {
			cpAnim.SetBool("active", true);
			RemoveCorpses();
			this.respawnPoint = cp;	
		}
	}

	void CollectCoin(GameObject coin) {
		this.deaths--;
		coin.GetComponent<Animator>().SetTrigger("get");
	}

	void Suicide() {
		deaths += 9;
		Die();
	}

	void ClampSpeed() {
		if (!speedClamped) return;

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
	}

	void FlashWhite() {
		cg.alpha = 1;
		flash = true;
	}

	void EndGame() {
		rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
		ended = true;
	}

	public IEnumerator FreezeFor(float seconds) {
		frozen = true;
		yield return new WaitForSeconds(seconds);
		frozen = false;
	}
}
