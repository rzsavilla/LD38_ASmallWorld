using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(GameObject.Find("GameManager"));
        Destroy(GameObject.Find("SoundManager"));
	}
}
