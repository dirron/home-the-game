using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaspEnemy : MonoBehaviour, Enemy {

    public float speed;
    public GameObject waspProjectile;
    public float projectileSpeed;

    private bool awake = false;
    private Vector3 playerPosition;
    private bool shouldShoot;
    private float distanceToPlayer;

	// Use this for initialization
	void Start () {
        shouldShoot = Random.Range(0, 2) == 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (awake)
        {
            FacePlayer();

            distanceToPlayer = Vector3.Distance(transform.position, playerPosition);

            if (!shouldShoot)
            {
                MoveTowardsPlayer();
            }
            else if (shouldShoot && distanceToPlayer > 10)
            {
                MoveTowardsPlayer();
            }
        }
	}

    public void Die()
    {
        Destroy(gameObject);
        Debug.Log("Wasp has ceased evil activities");

        LevelEventManager.TriggerEvent("EnemyKilled");
    }

    public void Awaken(Vector3 position)
    {
        playerPosition = position;
        if (Vector3.Distance(transform.position, position) < 30 && !awake)
        {
            Debug.Log("Wasp has awoken! AHHHH");
            awake = true;

            if (shouldShoot)
            {
                Shoot();
            }
        }
    }

    void FacePlayer()
    {
        if ((transform.position.x - playerPosition.x) < 0)
        {
            Vector3 flipX = transform.localScale;
            flipX.x = -1;
            transform.localScale = flipX;
        }
        else
        {
            Vector3 flipX = transform.localScale;
            flipX.x = 1;
            transform.localScale = flipX;
        }
    }

    void MoveTowardsPlayer()
    {
        float speedBoost = 2f;
        Vector3 direction = Vector3.Normalize(playerPosition - transform.position);

        if (distanceToPlayer > 15)
        {
            transform.Translate(direction * Time.deltaTime * speed * speedBoost);
        }
        else
        {
            transform.Translate(direction * Time.deltaTime * speed);
        }
    }

    void Shoot()
    {
        if (distanceToPlayer <= 10)
        {
            Vector3 direction = Vector3.Normalize(playerPosition - transform.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            GameObject projectile = Instantiate(waspProjectile, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));

            projectile.GetComponent<Rigidbody2D>().AddForce(direction * projectileSpeed, ForceMode2D.Impulse);
        }

        Invoke("Shoot", 2f);
    }
}
