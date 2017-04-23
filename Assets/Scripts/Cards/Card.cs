using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {

    public int iRarity;
    public Immediate cardImmediate;
    public bool bSet;

	void Awake () {
		
	}
	
    //Destroy the selected card, returning a bool if done or failed
	public bool DestroyCard()
    {
        //If no card, then failed 
        if (!bSet)
        {
            return false;
        }

        //Set the card as destroyed
        bSet = false;

        return true;
    }

    public bool GenerateCard()
    {
        //If have a card, then failed
        if (bSet)
        {
            return false;
        }

        return true;
    }
}
