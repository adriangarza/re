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

    //leave undefined for none
    public float ratioY = 0;
    float prevY;
    float currY;

    void Start () {
        player = GameObject.Find("Player");

        if (snapToPlayer) {
            this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, this.transform.position.z);
        }

        currX = player.transform.position.x;
        prevX = player.transform.position.x;

        currY = player.transform.position.y;
        prevY = player.transform.position.y;
	}
	
	void Update () {
        if (ratio != 0)
        {
            currX = player.transform.position.x;
            this.transform.Translate(new Vector2(ratio * (currX - prevX), 0));
            prevX = player.transform.position.x;
        }

        if (ratioY != 0) {
            currY = player.transform.position.y;
            this.transform.Translate(new Vector2(0, ratioY * (currY - prevY)));
            prevY = player.transform.position.y;
        } 
	}
}