using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomMonsterSpawner : MonoBehaviour {

    public GameObject[] monsters;
    public GameObject entityContainer;
    public GameObject playerSpawnPoint;

    private float camH;
    private float camW;
    private bool isActive = false;

    private GameObject player;

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
        RaycastHit2D hit;
        Vector2 rayOrigin;
        Vector2 rayDirection = Vector2.down;
        float spawnDirectionX = Random.Range(0, 2) == 0 ? -1 : 1;
        float spawnDirectionY = Random.Range(0, 2) == 0 ? -1 : 1;
        float maxDistance = camH * 0.7f;

        rayOrigin = player.transform.position + new Vector3(camW * spawnDirectionX, camH * spawnDirectionY);

        hit = Physics2D.Raycast(rayOrigin, rayDirection * spawnDirectionY, maxDistance);

        GameObject monster;
        if (hit.collider != null)
        {
            monster = Instantiate(monsters[0], hit.point, Quaternion.identity);
        }
        else
        {
            monster = Instantiate(monsters[0], rayDirection * maxDistance * spawnDirectionY, Quaternion.identity);
        }
        monster.transform.SetParent(entityContainer.transform);

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
