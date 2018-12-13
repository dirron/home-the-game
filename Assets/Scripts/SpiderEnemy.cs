using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEnemy : MonoBehaviour, Enemy {

    public int directionX = -1;
    public GameObject ballPrefab;

    private Rigidbody2D rb;

    private Vector3 prevPosition;

    private Vector3 targetPosition;

    private float moveSpeed = 1.5f;

    private bool awake = false;

    public const int LEFT = -1;
    public const int RIGHT = 1;

    private Animator animator;

    private bool shouldshoot;

    private GameObject player;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
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

        if (player == null && shouldshoot)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
	}

    public void Die()
    {
        Destroy(gameObject);
        Debug.Log("Spider has been murdered");

        LevelEventManager.TriggerEvent("EnemyKilled");
    }

    void Attack()
    {
        if (shouldshoot)
        {
            Shoot();
        }
        Invoke("Jump", 1f);
        Invoke("Attack", 3f);
    }

    void Shoot()
    {
        if (player != null && Vector3.Distance(player.transform.position, transform.position) <= 10)
        {
            Vector3 direction = Vector3.Normalize(player.transform.position - transform.position);
            GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);

            ball.GetComponent<Rigidbody2D>().AddForce(direction * 10f, ForceMode2D.Impulse);

            Destroy(ball, 5f);
        }
    }

    void Jump()
    {
        animator.SetBool("IsJumping", true);
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
        Vector3 flipX = transform.localScale;
        if (directionX == LEFT)
        {
            flipX.x = -1;
            transform.localScale = flipX;
            directionX = RIGHT;
        }
        else
        {
            flipX.x = 1;
            transform.localScale = flipX;
            directionX = LEFT;
        }
    }

    public void Awaken(Vector3 position)
    {
        if (Vector3.Distance(transform.position, position) < 30 && !awake)
        {
            shouldshoot = Random.Range(0, 2) == 0;
            awake = true;
            Attack();
            Debug.Log("Spider has AWOKEN!!");
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            animator.SetBool("IsJumping", false);
        }
    }
}
