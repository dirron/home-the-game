using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaspBossArena : MonoBehaviour {

    public GameObject bossPrefab;
    public GameObject cordycepsRodPrefab;

    // camera stuff
    private Camera mainCamera;
    private float camH;
    private float camW;
    private bool doneMoving = false;
    
    private float leftBound;
    private GameObject player;
    private GameObject boss;

    private List<GameObject> platforms;


    // Use this for initialization
    void Start () {
        mainCamera = Camera.main;
        camH = mainCamera.orthographicSize * 2f;
        camW = camH * mainCamera.aspect;

        leftBound = transform.position.x - camW / 2;

        platforms = new List<GameObject>();
        foreach (Transform child in transform.Find("Environment"))
        {
            if (child.gameObject.tag == "Ground")
            {
                platforms.Add(child.gameObject);
            }
        }
        Debug.Assert(platforms.Count > 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (player != null && player.transform.position.x > leftBound && !doneMoving)
        {
            CenterCamera();
        }
        if (boss == null && doneMoving)
        {
            SpawnBoss();
            SpawnCordycepsRod();
        }
	}

    void SpawnBoss()
    {
        Vector3 spawnPoint = new Vector3(transform.position.x + camW,
            transform.position.y + camH, 0);
        Vector3 landingPoint = transform.position + new Vector3(camW / 4, 0, 0);

        boss = Instantiate(bossPrefab, spawnPoint, Quaternion.identity);
        boss.transform.SetParent(transform);

        BroadcastMessage("Awaken", landingPoint);

        Debug.Log("Boss spawned");
    }

    void CenterCamera()
    {
        if (mainCamera.transform.position.x == transform.position.x)
        {
            doneMoving = true;
            return;
        }

        float speed = 5f;
        mainCamera.GetComponent<MainCamera>().trackPlayer = false;
        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, 
            new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z),
            Time.deltaTime * speed);
    }

    void SpawnCordycepsRod()
    {
        int index = Random.Range(0, platforms.Count);
        GameObject platform = platforms[index];
        SpriteRenderer spriteRenderer = platform.GetComponent<SpriteRenderer>();
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), spriteRenderer.bounds.size.y / 3);

        GameObject cordycepsRod = Instantiate(cordycepsRodPrefab, platform.transform.position + offset, Quaternion.identity);
        cordycepsRod.transform.SetParent(transform);

        Invoke("SpawnCordycepsRod", Random.Range(5f, 10f));
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (player != null)
        {
            return;
        }

        LevelEventManager.TriggerEvent("EnteredBossArena");

        GameObject other = col.gameObject;
        if (other.tag == "Player")
        {
            player = other;
        }
    }
}
