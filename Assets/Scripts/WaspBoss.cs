using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaspBoss : MonoBehaviour, IBossEnemy {

    public float speed;
    public GameObject sporeExplosionPrefab;
    public int hitPoints = 3;

    private GameObject player;
    private GameObject monsterSpawner;
    private Animator animator;

    private Coroutine attack;

    // camera stuff
    private float camH;
    private float camW;

    // Use this for initialization
    void Start () {
        camH = Camera.main.orthographicSize * 2f;
        camW = camH * Camera.main.aspect;

        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {

    }

    IEnumerator AttackPattern()
    {
        for (int i = 0; i < Random.Range(2, 4); i++)
        {
            yield return StartCoroutine(Charge(player.transform.position, 1f));
            yield return new WaitForSeconds(0.5f);
        }

        for (int i = 0; i < Random.Range(1, 4); i++)
        {
            yield return StartCoroutine(TeleportExplosion());
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("Wasp is tired");

        // restart
        if (hitPoints > 0)
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(AttackPattern());
        }
    }

    IEnumerator TeleportExplosion()
    {
        Vector3 up = new Vector3(transform.position.x,
            transform.position.y + camH);

        // disappear
        yield return StartCoroutine(SetVulnerable(false));
        yield return StartCoroutine(Charge(up, 3f));
        yield return new WaitForSeconds(1.5f);

        // reappear
        transform.position = player.transform.position + new Vector3(0, -camH, 0);
        yield return StartCoroutine(Charge(player.transform.position, 2f));

        // explode
        yield return new WaitForSeconds(0.5f);
        Explode();
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(SetVulnerable(true));
    }

    void Explode()
    {
        GameObject explosion = Instantiate(sporeExplosionPrefab, transform.position, Quaternion.identity);
        float explosionRadius = explosion.GetComponent<SpriteRenderer>().bounds.size.x / 2 + 1f;

        if (Vector3.Distance(player.transform.position, explosion.transform.position) <= explosionRadius)
        {
            Vector3 knockback = Vector3.Normalize(player.transform.position - explosion.transform.position);

            player.GetComponent<Rigidbody2D>().AddForce(knockback * 5f, ForceMode2D.Impulse);

            LevelEventManager.TriggerEvent("PlayerDamageTaken");
        }
        Destroy(explosion, 1f);
    }

    // Charge the wasp to a target position over a duration
    IEnumerator Charge(Vector3 targetPos, float chargeSpeed = -1)
    {
        // default
        if (chargeSpeed == -1) { chargeSpeed = speed; }

        Vector3 extension = targetPos - Vector3.Normalize(transform.position - targetPos) * 3f;

        while (Vector3.Distance(transform.position, extension) > 1f)
        {
            transform.position = Vector3.Lerp(transform.position, extension, Time.deltaTime * chargeSpeed);

            yield return null; // update every frame
        }
    }

    IEnumerator Awaken(Vector3 landingPoint)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        monsterSpawner = GameObject.FindGameObjectWithTag("MonsterSpawner");

        // stop spawning wasps
        monsterSpawner.SetActive(false);

        yield return StartCoroutine(Charge(landingPoint, 3f));
        yield return StartCoroutine(SetVulnerable(false));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(SetVulnerable(true));

        attack = StartCoroutine(AttackPattern());
    }

    public void Damage()
    {
        hitPoints--;
        Debug.Log("Boss has been damaged");

        if (hitPoints > 0)
        {
            StartCoroutine(Recover());
        }
        else if (hitPoints <= 0)
        {
            Die();
        }
    }

    IEnumerator SetVulnerable(bool isVulnerable)
    {
        transform.Find("BossDamageTrigger").gameObject.SetActive(isVulnerable);
        GetComponent<Collider2D>().enabled = isVulnerable;

        yield return null;
    }

    IEnumerator Recover()
    {
        Debug.Log("Paused attacks");
        StopCoroutine(attack);

        yield return StartCoroutine(SetVulnerable(false));
        animator.SetBool("IsHurt", true);

        yield return new WaitForSeconds(2f);

        animator.SetBool("IsHurt", false);
        yield return StartCoroutine(SetVulnerable(true));

        yield return StartCoroutine(TeleportExplosion());

        attack = StartCoroutine(AttackPattern());
        Debug.Log("Restarted attack");
    }

    public void Die()
    {
        StartCoroutine(DieRoutine());
    }

    public IEnumerator DieRoutine()
    {
        Debug.Log("Boss has been slain");
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1;

        StopCoroutine(attack);
        StopAllCoroutines();

        yield return new WaitForSeconds(2f);

        LevelEventManager.TriggerEvent("LevelComplete");

        yield return null;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject other = col.gameObject;

        if (other.tag == "Player")
        {
            LevelEventManager.TriggerEvent("PlayerDamageTaken");
        }
    }
}
