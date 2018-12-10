using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject playerPrefab;
    public GameObject spawnPoint;
    public GameObject entityContainer;

    private GameObject player;
    private EntityContainer entityContainerScript;

	// Use this for initialization
	void Start () {
        player = Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity);
        player.transform.SetParent(entityContainer.transform);

        entityContainerScript = entityContainer.GetComponent<EntityContainer>();

        LevelEventManager.TriggerEvent("PlayerSpawned");
	}
	
	// Update is called once per frame
	void Update () {
        entityContainerScript.BroadcastPlayerPosition(player.transform.position);
	}
}
