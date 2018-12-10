using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

    private GameObject player;
    private Vector3 cameraOffset = new Vector3(0, 2, -10);
    private Vector2 levelExit;
    private bool levelExitExists;

    private float startingPosX;

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
    }

    // Update is called once per frame
    void Update () {
        if (levelExitExists && player != null)
        {
            float horizontalDistanceToExit = levelExit.x - player.transform.position.x;

            if (player.transform.position.x >= startingPosX
                && horizontalDistanceToExit > 3 && player.transform.position.x > 1)
            {
                transform.position = player.transform.position + cameraOffset;
            }
            else
            {
                float x = transform.position.x;
                float y = player.transform.position.y + cameraOffset.y;

                transform.position = new Vector3(x, y, transform.position.z);
            }
        }
    }

    void OnPlayerSpawned()
    {
        this.player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnLevelExitCreated()
    {
        levelExit = GameObject.FindGameObjectWithTag("LevelExit").transform.position;
        levelExitExists = true;
    }
}
