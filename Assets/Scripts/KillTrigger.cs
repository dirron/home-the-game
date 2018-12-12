using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        GameObject other = col.gameObject;
        if (other.tag == "Player")
        {
            Animator animator = other.GetComponent<Animator>();

            if (!animator.GetBool("IsHiding"))
            {
                Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();

                transform.parent.GetComponent<Enemy>().Die();

                playerRb.velocity = Vector2.zero;
                playerRb.AddForce(Vector2.up * 6f, ForceMode2D.Impulse);
            }
        }
    }
}
