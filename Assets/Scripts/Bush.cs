using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour {

    public Sprite[] sprites;
    public Sprite[] highlighted;

    private SpriteRenderer spriteRenderer;

    int spriteIndex;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();

        for (int i = 0; i < sprites.Length; i++)
        {
            if (spriteRenderer.sprite == sprites[i])
            {
                spriteIndex = i;
                break;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            spriteRenderer.sprite = highlighted[spriteIndex];
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }
}
