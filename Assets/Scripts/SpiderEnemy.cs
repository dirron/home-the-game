using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEnemy : MonoBehaviour {

    public int directionX = -1;

    private Rigidbody2D rb;

    private Vector3 prevPosition;

    private Vector3 targetPosition;

    private float moveSpeed = 1.5f;

    private bool awake = false;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
	}
	
	// Update is called once per frame
	void Update () {
        // move once awake
        if (awake)
        {
            if (directionX == -1)
            {
                transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);
            }
            else
            {
                transform.Translate(Vector2.right * Time.deltaTime * moveSpeed);
            }
        }
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
    }

    public void Die()
    {
        Destroy(gameObject);
        Debug.Log("Spider has been murdered");
    }

    void Jump()
    {
        if (rb.velocity.y < 0.01)
        {
            // if stuck on something, turn around
            if (Vector3.Distance(prevPosition, transform.position) < 1)
            {
                ChangeDirection();
            }

            prevPosition = transform.position;
            rb.AddForce(Vector2.up * 6f, ForceMode2D.Impulse);
        }
    }

    public void ChangeDirection()
    {
        if (directionX == -1)
        {
            directionX = 1;
        }
        else
        {
            directionX = -1;
        }
    }

    public void Awaken(Vector3 position)
    {
        if (Vector3.Distance(transform.position, position) < 30 && !awake)
        {
            awake = true;
            InvokeRepeating("Jump", 2f, 2f);
            Debug.Log("Spider has AWOKEN!!");
        }
    }
}
