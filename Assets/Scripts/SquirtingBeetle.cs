using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirtingBeetle : MonoBehaviour, Item {

    public int numCharges = 999999;
    public float duration = 30f;
    public GameObject laserPrefab;

    private GameObject player;
    private GameManager gameManager;

    private SpriteRenderer spriterenderer;

    public void Destroy()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    public string GetName()
    {
        return "Squirting Beetle";
    }

    public void Use()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 worldPoint2D = new Vector3(worldPoint.x, worldPoint.y);
        Vector3 direction = Vector3.Normalize(worldPoint2D - player.transform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(player.transform.position, direction))
        {
            if (hit.collider.gameObject.tag == "Enemy")
            {
                hit.collider.gameObject.GetComponent<Enemy>().Die();
            }
        }

        GameObject laser = Instantiate(laserPrefab, player.transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
        Destroy(laser, 0.67f);
    }

    // Use this for initialization
    void Start () {
        spriterenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (spriterenderer.enabled
            && player != null
            && Input.GetKeyDown(KeyCode.E)
            && Vector3.Distance(player.transform.position, transform.position) < 2f)
        {
            Debug.Log("Picked up mushroom");
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            gameManager.PickupItem(this, numCharges, true, duration);

            spriterenderer.enabled = false;

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            player = col.gameObject;
        }
    }
}
