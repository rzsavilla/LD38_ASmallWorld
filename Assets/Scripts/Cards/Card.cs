using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {

    public int iRarity;
    public Immediate cardImmediate;
    public bool bSet;

	public Card EmptyCard()
    {
        iRarity = 0;
        bSet = false;
        return this;
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

        iRarity = cardImmediate.GenerateEffect();
        return true;
    }
}
