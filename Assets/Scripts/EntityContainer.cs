using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityContainer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BroadcastPlayerPosition(Vector3 position)
    {
        BroadcastMessage("Awaken", position, SendMessageOptions.DontRequireReceiver);
    }
}
