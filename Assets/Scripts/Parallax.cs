using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

	//movement ratio to that of the player, 0 is stationary
    public float ratio;

    public bool snapToPlayer = false;

    GameObject player;
    float playerOrigX;
    float origX;

    //new stuff
    float prevX;
    float currX;

    void Start () {
        player = GameObject.Find("Player");

        if (snapToPlayer) {
            this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, this.transform.position.z);
        }

        currX = player.transform.position.x;
        prevX = player.transform.position.x;
	}
	
	void Update () {
        if (ratio != 0)
        {

            currX = player.transform.position.x;

            this.transform.Translate(new Vector2(ratio * (currX - prevX), 0));

            prevX = player.transform.position.x;

        }
	}
}