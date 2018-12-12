using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MushroomLevelCreator : MonoBehaviour
{

    public Tilemap tilemap;
    public Tile[] tiles;
    public GameObject[] environmentObjects;
    public int width;
    public int height;
    public GameObject environmentContainer;
    public GameObject exit;

    private float scaleMultiplier;
    private GameObject lastObjectGenerated;

    // Use this for initialization
    void Start()
    {
        //tilemap.SetTile(Vector3Int.zero, tiles[0]);
        scaleMultiplier = 1 / tilemap.transform.localScale.x;

        StartCoroutine(GenerateLevel());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator GenerateTrees()
    {

        SpriteRenderer treeRenderer = environmentObjects[0].GetComponent<SpriteRenderer>();
        float treeHeight = treeRenderer.bounds.size.y;

        float platformStagger = 0f;
        float step = 2.5f;
        float treeSpacing = 5f;
        int consecutiveTreeCounter = 0;
        int minNumTreesForGap = 1;

        for (float x = 5; x < width / scaleMultiplier; x += treeSpacing)
        {
            bool shouldMakeGap = Random.Range(0, 2) == 0;
            if (consecutiveTreeCounter >= minNumTreesForGap && shouldMakeGap
                && x + treeSpacing < width / scaleMultiplier)
            {
                x += treeSpacing;
                consecutiveTreeCounter = 0;

                float randomY = Random.Range(-treeHeight / 4, treeHeight / 4);

                GameObject movingMushroom = Instantiate(environmentObjects[2],
                    new Vector3(x - treeSpacing / scaleMultiplier, randomY, 0), Quaternion.identity);
                movingMushroom.transform.SetParent(environmentContainer.transform);

                lastObjectGenerated = movingMushroom;
            }
            else
            {
                consecutiveTreeCounter++;

                // generate tree trunk
                GameObject tree = Instantiate(environmentObjects[0], new Vector3(x, 0, 0), Quaternion.identity);
                tree.transform.SetParent(environmentContainer.transform);

                float treeCenterY = tree.transform.position.y;
                float mushroomHeightBound = treeHeight / 3;
                float randomOffsetY = Random.Range(-0.5f, 0.5f);

                // bounce between two values to stagger platforms between trees
                platformStagger = platformStagger == 0 ? step / 2 : 0;

                // generate mushroom platforms
                for (float y = treeCenterY - mushroomHeightBound + platformStagger;
                    y < treeCenterY + mushroomHeightBound;
                    y = y + step + randomOffsetY)
                {
                    float offsetX = Random.Range(-1f, 1f); // give some variation to the mushroom platform placement along x-axis
                    GameObject mushroomPlatform = Instantiate(environmentObjects[1], new Vector3(x + offsetX, y, 0), Quaternion.identity);
                    mushroomPlatform.transform.SetParent(environmentContainer.transform);
                }

                lastObjectGenerated = tree;
            }
        }

        Debug.Log("Generated trees");

        yield return null;
    }

    IEnumerator GenerateExit()
    {
        Vector3 offset = new Vector3(3f, 0, 0);
        Instantiate(exit, lastObjectGenerated.transform.position + offset, Quaternion.identity);

        Debug.Log("Generated exit");
        LevelEventManager.TriggerEvent("LevelExitCreated");

        yield return null;
    }

    IEnumerator GenerateLevel()
    {
        yield return StartCoroutine(GenerateTrees());
        yield return StartCoroutine(GenerateExit());
    }
}
