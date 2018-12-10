using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour {

    public GameObject[] backgroundSprites;
    public float speed;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private float spriteWidth;

    private Vector3 camLast;
    private Vector3 camCur;

    int direction = -1;

    // Use this for initialization
    void Start () {

        spriteWidth = backgroundSprites[0]
            .GetComponent<SpriteRenderer>().bounds.size.x;

        startPosition = backgroundSprites[0].transform.position;
        endPosition = backgroundSprites[backgroundSprites.Length - 1].transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        camCur = Camera.main.transform.position;

        float diff = camCur.x - camLast.x;
        if (Mathf.Abs(diff) > 0.02)
        {

            if (diff > 0)
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }

            for (int i = 0; i < backgroundSprites.Length; i++)
            {
                backgroundSprites[i].transform.Translate(Vector3.right * Time.deltaTime * speed * direction);
                if (backgroundSprites[i].transform.position.x <= startPosition.x - spriteWidth)
                {
                    backgroundSprites[i].transform.position = endPosition;
                }
            }
        }

        camLast = camCur;
	}
}
