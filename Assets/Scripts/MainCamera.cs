using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MainCamera : MonoBehaviour {

    public GameObject backgroundSprite;
    public bool verticalOffset;
    public bool trackPlayer = true;

    private GameObject player;
    private Vector3 cameraOffset;
    private GameObject levelExit;
    private bool levelExitExists;

    private float startingPosX;

    private float camW;
    private float camH;
    private float camBoundRight;
    private float camBoundTop;
    private float camBoundBot;

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
        camBoundTop = backgroundSprite.transform.position.y
            + backgroundSprite.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        camBoundBot = backgroundSprite.transform.position.y 
            - backgroundSprite.GetComponent<SpriteRenderer>().bounds.size.y / 2;

        if (verticalOffset)
        {
            cameraOffset = new Vector3(0, 2, -10);
        }
        else
        {
            cameraOffset = new Vector3(0, 0, -10);
        }
    }

    // Update is called once per frame
    void Update () {
        if (trackPlayer)
        {
            Move();
        }
    }

    void OnPlayerSpawned()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnLevelExitCreated()
    {
        levelExit = GameObject.FindGameObjectWithTag("LevelExit");

        if (levelExit.GetComponent<SpriteRenderer>() != null)
        {
            camBoundRight = levelExit.transform.position.x + levelExit.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        }
        else if (levelExit.GetComponent<BoxCollider2D>() != null)
        {
            camBoundRight = levelExit.transform.position.x + levelExit.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        }
        else if (levelExit.GetComponentInChildren<Tilemap>() != null)
        {
            Tilemap exitTiles = levelExit.GetComponentInChildren<Tilemap>();
            camBoundRight = levelExit.transform.position.x 
                + exitTiles.size.x / (1 / exitTiles.transform.localScale.x) / 2;
        }

        levelExitExists = true;
    }

    void Move()
    {
        if (levelExitExists && player != null)
        {
            float distToEndX = camBoundRight - player.transform.position.x;
            float distToTop = camBoundTop - (player.transform.position.y + cameraOffset.y);
            float distToBot = (player.transform.position.y + cameraOffset.y) - camBoundBot;
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

            if (distToTop > camH / 2)
            {
                if (distToBot < camH / 2)
                {
                    y = Mathf.Max(transform.position.y, camBoundBot + camH / 2);
                }
                else
                {
                    y = player.transform.position.y + cameraOffset.y;
                }
            }
            else
            {
                y = Mathf.Min(transform.position.y, camBoundTop - camH / 2);
            }

            transform.position = new Vector3(x, y, cameraOffset.z);
        }
    }
}
