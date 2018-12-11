using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomCap : MonoBehaviour {

    private GameObject player;
    private Rigidbody2D playerRb;
    private bool isTouching = false;
    private Vector3 prev;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (player != null && isTouching)
        {
            player.transform.position += new Vector3(transform.position.x - prev.x, 0, 0);
        }
        prev = transform.position;
	}


    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject other = col.gameObject;

        if (other.tag == "Player")
        {
            player = other;
            playerRb = player.GetComponent<Rigidbody2D>();
            isTouching = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        GameObject other = col.gameObject;

        if (other.tag == "Player")
        {
            isTouching = false;
            Vector3 newVelocity = new Vector3(0, playerRb.velocity.y);
            playerRb.velocity = newVelocity;
        }
    }
}
