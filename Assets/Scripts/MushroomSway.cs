using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomSway : MonoBehaviour {
    public float maxRotation;
    public float speed;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Euler(0f, 0f, maxRotation * Mathf.Sin(Time.time * speed));
    }
}
