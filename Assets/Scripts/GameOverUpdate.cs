using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUpdate : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Text score = GameObject.Find("Score").GetComponent<Text>();
        score.text = GameStats.Score.ToString();
    }
}
