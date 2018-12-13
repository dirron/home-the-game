using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject playerPrefab;
    public GameObject spawnPoint;
    public GameObject entityContainer;
    public float invulnerabilityTime;

    private GameObject player;
    private EntityContainer entityContainerScript;
    private int hitPoints;
    private bool isHittable = true;

    // item pickup stuff
    private Item item;
    private int itemCharges;
    private bool itemIgnoreCollisionOnMouse = false;

    // UI stuff //
    public GameObject[] hitPointObjects;
    // menu
    public GameObject menu;
    public Text titleText;
    public int enemiesKilled = 0;
    public Text enemiesKilledText;
    public Text currentItemText;
    public int totalItems = 0;
    public Text totalItemsText;
    // transition
    public GameObject transitionScreen;
    public Text transitionTitle;

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

        if (Input.GetMouseButtonDown(1))
        {
            UseItem();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menu.activeSelf)
            {
                OpenMenu("PAUSED");
            }
            else if (menu.activeSelf && titleText.text == "PAUSED")
            {
                CloseMenu();
            }
        }
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
            KillPlayer();
        }
        else
        {
            Debug.Log("Player has taken damage! " + hitPoints + " remaining!");
            StartCoroutine(GracePeriod());
        }
    }

    void KillPlayer()
    {
        Debug.Log("Player has died");
        OpenMenu("You have died.");
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting");
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PickupItem(Item newItem, int numCharges, bool ignoreCollision = false)
    {
        item = newItem;
        itemCharges = numCharges;
        itemIgnoreCollisionOnMouse = ignoreCollision;

        totalItems++;
        totalItemsText.text = "Total items found " + totalItems;
    }

    public void UseItem()
    {
        if (!itemIgnoreCollisionOnMouse)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

            if (hit.collider != null && hit.collider.gameObject.tag == "Ground")
            {
                return;
            }
        }

        if (item != null && itemCharges > 0)
        {
            itemCharges--;
            item.Use();

            if (itemCharges == 0)
            {
                item.Destroy();
                item = null;
            }
        }
    }

    void CountEnemyDeath()
    {
        enemiesKilled++;
        enemiesKilledText.text = "Enemies killed: " + enemiesKilled;
    }

    void OnEnable()
    {
        LevelEventManager.StartListening("PlayerDamageTaken", DamagePlayer);
        LevelEventManager.StartListening("PlayerInstantDeath", KillPlayer);
        LevelEventManager.StartListening("EnemyKilled", CountEnemyDeath);
        LevelEventManager.StartListening("LevelComplete", LoadNextLevel);
    }

    void OnDisable()
    {
        LevelEventManager.StopListening("PlayerDamageTaken", DamagePlayer);
        LevelEventManager.StopListening("PlayerInstantDeath", KillPlayer);
        LevelEventManager.StopListening("EnemyKilled", CountEnemyDeath);
        LevelEventManager.StopListening("LevelComplete", LoadNextLevel);
    }

    public IEnumerator GracePeriod()
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

    void OpenMenu(string title = "Home")
    {
        titleText.text = title;
        menu.SetActive(true);
        Time.timeScale = 0;
    }

    void CloseMenu()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
    }

    void LoadNextLevel()
    {
        StartCoroutine(LoadNextLevelRoutine());
    }

    IEnumerator LoadNextLevelRoutine()
    {
        transitionTitle.text = "Level Complete";
        transitionScreen.SetActive(true);
        yield return new WaitForSeconds(2f);

        int index = SceneManager.GetActiveScene().buildIndex;
        if (index < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            // go to main menu
        }
    }
}
