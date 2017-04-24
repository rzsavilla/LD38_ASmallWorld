using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUpdate : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Text score = GameObject.Find("Score").GetComponent<Text>();
        Debug.Log("Score:" + PlayerPrefs.GetInt("Score"));
        score.text = PlayerPrefs.GetInt("Score").ToString();
    }
}
