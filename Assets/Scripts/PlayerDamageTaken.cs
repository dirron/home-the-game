using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageTaken : MonoBehaviour {

    private Rigidbody2D rb;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.tag.Equals("SpiderEnemy"))
        {
            Vector3 direction = Vector3.Normalize(transform.position - other.transform.position);

            if (collision.collider.GetType() == typeof(CapsuleCollider2D))
            {
                rb.AddForce(Vector2.up * 6f, ForceMode2D.Impulse);

                SpiderEnemy spiderEnemy = other.GetComponent<SpiderEnemy>();
                spiderEnemy.Die();
            }
            else if (collision.collider.GetType() == typeof(BoxCollider2D))
            {
                rb.AddForce(direction * 4f, ForceMode2D.Impulse);
                LevelEventManager.TriggerEvent("PlayerDamageTaken");
            }
        }
    }


}
