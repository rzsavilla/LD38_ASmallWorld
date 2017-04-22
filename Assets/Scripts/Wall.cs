using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    //private BoxCollider2D boxCollider;

	// Use this for initialization
	void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //boxCollider = GetComponent<BoxCollider2D>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
