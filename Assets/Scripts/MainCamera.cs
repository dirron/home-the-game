using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MainCamera : MonoBehaviour {

    public GameObject backgroundSprite;

    private GameObject player;
    private Vector3 cameraOffset = new Vector3(0, 2, -10);
    private GameObject levelExit;
    private bool levelExitExists;

    private float startingPosX;

    private float camW;
    private float camH;
    private float endCamBoundary;
    private float topCamBoundary;

    void OnEnable ()
    {
        LevelEventManager.StartListening("LevelExitCreated", OnLevelExitCreated);
        LevelEventManager.StartListening("PlayerSpawned", OnPlayerSpawned);
    }

    void OnDisable ()
    {
        LevelEventManager.StopListening("LevelExitCreated", OnLevelExitCreated);
        LevelEventManager.StartListening("PlayerSpawned", OnPlayerSpawned);
    }

    // Use this for initialization
    void Start () {
        startingPosX = transform.position.x;
        camH = Camera.main.orthographicSize * 2f;
        camW = camH * Camera.main.aspect;
        topCamBoundary = backgroundSprite.transform.position.y
            + backgroundSprite.GetComponent<SpriteRenderer>().bounds.size.y / 2;
    }

    // Update is called once per frame
    void Update () {
        Move();
    }

    void OnPlayerSpawned()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnLevelExitCreated()
    {
        levelExit = GameObject.FindGameObjectWithTag("LevelExit");

        endCamBoundary = levelExit.transform.position.x + levelExit.GetComponent<SpriteRenderer>().bounds.size.x / 2;

        levelExitExists = true;
    }

    void Move()
    {
        if (levelExitExists && player != null)
        {
            float distToEndX = endCamBoundary - player.transform.position.x;
            float distToEndY = topCamBoundary - (player.transform.position.y + cameraOffset.y);
            float x;
            float y;

            if (player.transform.position.x > startingPosX && distToEndX > camW / 2)
            {
                x = player.transform.position.x;
            }
            else
            {
                x = transform.position.x;
            }

            if (distToEndY > camH / 2)
            {
                y = player.transform.position.y + cameraOffset.y;
            }
            else
            {
                y = Mathf.Min(transform.position.y, topCamBoundary - camH / 2);
            }

            transform.position = new Vector3(x, y, cameraOffset.z);
        }
    }
}
