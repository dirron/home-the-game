using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CordycepsRod : MonoBehaviour, Item {

    public int numCharges = 5;
    public GameObject sporeExplosionPrefab;

    private GameObject player;
    private GameManager gameManager;

    private SpriteRenderer spriterenderer;

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
            gameManager.PickupItem(this, numCharges);

            spriterenderer.enabled = false;
        }
	}

    public void Use()
    {
        Teleport();
    }

    public void Destroy()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    void Teleport()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldPoint2D = new Vector2(worldPoint.x, worldPoint.y);

        GameObject explosion = Instantiate(sporeExplosionPrefab, worldPoint2D, Quaternion.identity);
        Destroy(explosion, 1f);

        player.transform.position = worldPoint2D;
        player.GetComponent<PlayerMovement>().ResetJump();
        gameManager.StartCoroutine(gameManager.GracePeriod());
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            player = col.gameObject;
        }
    }

    public string GetName()
    {
        return "Cordyceps Rod";
    }
}
