using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEnemy : MonoBehaviour, Enemy {

    public int directionX = -1;

    private Rigidbody2D rb;

    private Vector3 prevPosition;

    private Vector3 targetPosition;

    private float moveSpeed = 1.5f;

    private bool awake = false;

    public const int LEFT = -1;
    public const int RIGHT = 1;

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
            if (directionX == LEFT)
            {
                transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);
            }
            else
            {
                transform.Translate(Vector2.right * Time.deltaTime * moveSpeed);
            }
        }
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

    public void SetDirection(int direction)
    {
        if (direction != SpiderEnemy.LEFT && direction != SpiderEnemy.RIGHT)
        {
            Debug.LogError("Invalid direction specified!");
        }
        else
        {
            directionX = direction;
        }
    }

    public void ChangeDirection()
    {
        if (directionX == LEFT)
        {
            directionX = RIGHT;
        }
        else
        {
            directionX = LEFT;
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
