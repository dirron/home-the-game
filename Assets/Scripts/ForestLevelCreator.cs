using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class ForestLevelCreator : MonoBehaviour {

    public GameObject levelExitPrefab;
    public GameObject environmentContainer;
    public GameObject[] environment;

    public int width;
    public int height;

    public TileBase[] tiles;

    public Tilemap generatedTilemap;
    public Tilemap boundaryTilemap;

    public int minDistanceToLastSpawn;


    private int[,] map;

	// Use this for initialization
	void Start () {
        map = new int[width, height];
        map = GenerateRandomWalkArray(map, UnityEngine.Random.value);

        StartCoroutine(LevelGenerationSeq(map, generatedTilemap));
    }

    // Update is called once per frame
    void Update () {
		
	}

    int[,] GenerateRandomWalkArray(int[,] map, float seed)
    {
        System.Random rand = new System.Random(seed.GetHashCode());

        int lastHeight = 1;
        int minSegmentLength = 0;
        int maxSegmentLength = 5;
        int segmentLength = 0;
        bool nearEnd = false;

        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            // roll for the next tile's chance of increasing in height
            int nextMove = rand.Next(10);

            // smooth out the map
            if (segmentLength < maxSegmentLength)
            {
                minSegmentLength = UnityEngine.Random.Range(5, 8);
            }

            // flatten end of map for exit spawn
            if (map.GetUpperBound(0) - x < levelExitPrefab.GetComponent<SpriteRenderer>().size.x)
            {
                nearEnd = true;
            }

            // decrease height
            if (nextMove < 2 && lastHeight > 2 && segmentLength > minSegmentLength && !nearEnd)
            {
                lastHeight--;
                segmentLength = 0;
            }
            // increase height
            else if (nextMove >= 2 && lastHeight < (map.GetUpperBound(1) - 2) && segmentLength > minSegmentLength && !nearEnd)
            {
                lastHeight++;
                segmentLength = 0;
            }

            segmentLength++;

            // choose 2 or 3 (tiles index based on this minus 1) to be a random non-corner top tile
            map[x, lastHeight] = UnityEngine.Random.Range(0, 2) + 2;

            // rest of tiles are just undeground tiles
            for (int y = lastHeight - 1; y >= 0; y--)
            {
                map[x, y] = 1;
            }
        }

        return map;
    }

    IEnumerator RenderLevel(int[,] map, Tilemap tilemap)
    {
        tilemap.ClearAllTiles();

        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (map[x, y] > 0)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);

                    if (x > 0 && map[x - 1, y] == 0)
                    {
                        // left corner tile
                        tilemap.SetTile(position, tiles[3]);
                    }
                    else if (x < map.GetUpperBound(0) && map[x + 1, y] == 0)
                    {
                        // right corner tile
                        tilemap.SetTile(position, tiles[4]);
                    }
                    else
                    {
                        // set rest of tiles as ground or non-corner top layer
                        tilemap.SetTile(position, tiles[map[x, y] - 1]);
                    }
                }
            }
        }

        yield return null;
    }

    IEnumerator CreateExit()
    {
        SpriteRenderer treeSprite = levelExitPrefab.GetComponentInChildren<SpriteRenderer>();
        float scaleMultiplier = (1 / generatedTilemap.transform.localScale.x);
        float yOffset = treeSprite.bounds.size.y / 2 - treeSprite.bounds.size.y / 100; // subtract to avoid pixel line between sprite and ground
        float xOffset = treeSprite.bounds.size.x / 2;

        int xSpawnLocation = (int) (Mathf.Round(map.GetUpperBound(0) / scaleMultiplier) - xOffset);

        RaycastHit2D hit = Physics2D.Raycast(new Vector2(xSpawnLocation, 100), Vector2.down);
        if (hit.collider != null)
        {
            GameObject exit = Instantiate(levelExitPrefab, new Vector3(hit.point.x, hit.point.y + yOffset, 0), Quaternion.identity);
            LevelEventManager.TriggerEvent("LevelExitCreated");
            Debug.Log("Spawned exit at " + exit.transform.position);
        }

        yield return null;
    }

    IEnumerator SpawnEnvironment()
    {
        float endX = boundaryTilemap.size.x;
        RaycastHit2D hit;

        for (int x = 0; x < endX; x += UnityEngine.Random.Range(5, 20))
        {
            int itemIndex = UnityEngine.Random.Range(0, environment.Length);
            hit = Physics2D.Raycast(new Vector2(x, transform.position.y), Vector2.down);

            if (hit.collider != null)
            {
                GameObject environmentObject = Instantiate(environment[itemIndex], 
                    new Vector3(hit.point.x, hit.point.y), Quaternion.identity);
                environmentObject.transform.SetParent(environmentContainer.transform);
            }
        }

        yield return null;
    }

    IEnumerator LevelGenerationSeq(int[,] map, Tilemap tilemap)
    {
        yield return StartCoroutine(RenderLevel(map, tilemap));
        yield return StartCoroutine(CreateExit());
        yield return StartCoroutine(SpawnEnvironment());
    }
}
