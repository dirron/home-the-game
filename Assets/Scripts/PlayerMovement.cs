using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float walkSpeed;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

    private int jumpCount;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        rb.freezeRotation = true;        
    }

    // Update is called once per frame
    void Update()
    {
        Jump(); // put jump here to avoid input getting missed
    }

    void FixedUpdate()
    {
        Move();

        animator.SetFloat("VelocityY", rb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.tag.Equals("Ground") && animator != null)
        {
            animator.SetBool("IsOnGround", true);
            jumpCount = 0;
        }
        else if (other.tag == "Enemy")
        {
            LevelEventManager.TriggerEvent("PlayerDamageTaken");
        }
    }

    void Move()
    {
        if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("IsMoving", true);
            transform.Translate(Vector2.right * Time.deltaTime * walkSpeed);
            spriteRenderer.flipX = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("IsMoving", true);
            transform.Translate(Vector2.left * Time.deltaTime * walkSpeed);
            spriteRenderer.flipX = false;
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }   
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpCount < 2)
            {
                animator.SetBool("IsOnGround", false);
                jumpCount++;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * 8f, ForceMode2D.Impulse);
            }
        }
    }
}
