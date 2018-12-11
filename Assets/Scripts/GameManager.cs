using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject playerPrefab;
    public GameObject spawnPoint;
    public GameObject entityContainer;
    public GameObject[] hitPointObjects;
    public float invulnerabilityTime;

    private GameObject player;
    private EntityContainer entityContainerScript;
    private int hitPoints;
    private bool isHittable = true;

	// Use this for initialization
	void Start () {
        player = Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity);
        player.transform.SetParent(entityContainer.transform);

        hitPoints = hitPointObjects.Length;
        Debug.Log("Player has " + hitPoints + " hit points");

        entityContainerScript = entityContainer.GetComponent<EntityContainer>();

        LevelEventManager.TriggerEvent("PlayerSpawned");


	}
	
	// Update is called once per frame
	void Update () {
        entityContainerScript.BroadcastPlayerPosition(player.transform.position);
	}

    void DamagePlayer()
    {
        if (!isHittable)
        {
            return;
        }
        else
        {
            isHittable = false;
        }

        Image heart = hitPointObjects[hitPoints - 1].GetComponent<Image>();
        Color faded = heart.color;
        faded.a = 0.5f;
        heart.color = faded;

        hitPoints--;

        if (hitPoints == 0)
        {
            Debug.Log("Player has died");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.Log("Player has taken damage! " + hitPoints + " remaining!");
            StartCoroutine(GracePeriod());
        }
    }

    void OnEnable()
    {
        LevelEventManager.StartListening("PlayerDamageTaken", DamagePlayer);
    }

    void OnDisable()
    {
        LevelEventManager.StopListening("PlayerDamageTaken", DamagePlayer);
    }

    IEnumerator GracePeriod()
    {
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        Physics2D.IgnoreLayerCollision(9, 10, true); // ignore between player and mob
        Debug.Log("Player is temporarily invulnerable");

        Color normal = spriteRenderer.color;

        float endTime = Time.time + invulnerabilityTime;
        float prevAlpha = normal.a;
        while (Time.time < endTime)
        {
            Color temp = normal;
            temp.a = prevAlpha == 0.5f ? 1f : 0.5f;
            spriteRenderer.color = temp;
            prevAlpha = temp.a;

            yield return new WaitForSeconds(0.2f);
        }

        spriteRenderer.color = normal;
        Physics2D.IgnoreLayerCollision(9, 10, false);
        isHittable = true;
        Debug.Log("Player is vulnerable again");
    }
}
