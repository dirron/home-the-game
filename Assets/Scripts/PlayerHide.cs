using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHide : MonoBehaviour {

    private GameObject player;
    private Animator animator;
    private bool canHide = false;

	// Use this for initialization
	void Start () {
        player = transform.parent.gameObject;
        animator = player.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        Hide();
    }

    void Hide()
    {
        if (Input.GetKey(KeyCode.S) && !animator.GetBool("IsMoving") && canHide)
        {
            animator.SetBool("IsHiding", true);
            Physics2D.IgnoreLayerCollision(9, 10, true);
        }
        else
        {
            animator.SetBool("IsHiding", false);
            Physics2D.IgnoreLayerCollision(9, 10, false);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        GameObject other = col.gameObject;

        if (other.tag == "Hideable")
        {
            Debug.Log("Entered hiding area");
            canHide = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        GameObject other = col.gameObject;

        if (other.tag == "Hideable")
        {
            Debug.Log("Exited hiding area");
            canHide = false;
        }
    }
}
