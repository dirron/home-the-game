using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour {

    public GameObject[] monsters;
    public GameObject entityContainer;
    public GameObject playerSpawnPoint;

    private GameObject player;

    private float camH;
    private float camW;
    private bool isActive = false;

	// Use this for initialization
	void Start () {
        camH = Camera.main.orthographicSize * 2f;
        camW = camH * Camera.main.aspect;
	}
	
	// Update is called once per frame
	void Update () {
        if (player != null
            && player.transform.position.x > playerSpawnPoint.transform.position.x + camW / 2
            && !isActive)
        {
            Debug.Log("MonsterSpawner is active!");
            isActive = true;
            Invoke("Spawn", 3f);
        }
    }

    void Spawn()
    {
        bool isFromLeft = Random.Range(0, 10) < 3; // bias towards right
        Vector2 rayOrigin;
        RaycastHit2D hit;
        int direction;

        if (isFromLeft)
        {
            rayOrigin = player.transform.position + new Vector3(-camW, camH);
            direction = SpiderEnemy.RIGHT;
        }
        else
        {
            rayOrigin = player.transform.position + new Vector3(camW, camH);
            direction = SpiderEnemy.LEFT;
        }
       
        hit = Physics2D.Raycast(rayOrigin, Vector2.down);

        if (hit.collider != null)
        {
            GameObject jumpingSpider = Instantiate(monsters[0], hit.point, Quaternion.identity);
            jumpingSpider.transform.SetParent(entityContainer.transform);
            jumpingSpider.GetComponent<SpiderEnemy>().SetDirection(direction);
        }

        // limit number of enemies at once
        if (entityContainer.transform.childCount < 30)
        {
            Invoke("Spawn", Random.Range(3f, 6f));
        }
    }

    void OnPlayerSpawned()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnEnable()
    {
        LevelEventManager.StartListening("PlayerSpawned", OnPlayerSpawned);
    }

    void OnDisable()
    {
        LevelEventManager.StartListening("PlayerSpawned", OnPlayerSpawned);
    }
}
