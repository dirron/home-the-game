using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour {

    public float duration = 3f;

    private float endTime;

	// Use this for initialization
	void Start () {
        endTime = Time.time + duration;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > endTime)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
	}
}
