using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {

    public Player player;
    public int iRarity;
    public Immediate cardImmediate;
    public bool bSet;

	public Card EmptyCard()
    {
        cardImmediate = new Immediate();
        iRarity = 0;
        bSet = false;
        return this;
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }
	
    //Destroy the selected card, returning a bool if done or failed
    //Will also do the effect here
	public bool DestroyCard()
    {
        //If no card, then failed 
        if (!bSet)
        {
            return false;
        }

        //Set the card as destroyed
        bSet = false;

        if (cardImmediate.iEffect == 0)
        {
            //Does nothing
        }
        else if (cardImmediate.iEffect == 1)
        {
            //Gain score
            player.GainScore(50);
        }
        else if (cardImmediate.iEffect == 2)
        {
            //Gain HP
            player.GainHP(10);
        }

        cardImmediate.iEffect = -1;

        return true;
    }

    //Generate the effect of the card
    public bool GenerateCard()
    {
        //If have a card, then failed
        if (bSet)
        {
            return false;
        }

        iRarity = cardImmediate.GenerateEffect();
        bSet = true;
        return true;
    }

    public string GetName()
    {
        if (cardImmediate.iEffect == 0)
        {
            //Does nothing
            return "Nothing!";
        }
        else if (cardImmediate.iEffect == 1)
        {
            //Gain score
            return "Gain Score!";
        }
        else if (cardImmediate.iEffect == 2)
        {
            //Gain HP
            return "Heal HP!";
        }
        else
        {
            return "None";
        }
    }

    public string GetEffect()
    {
        if (cardImmediate.iEffect == 0)
        {
            //Does nothing
            return  "This card does\n" + 
                    "nothing! Discard\n" +
                    "it!";
        }
        else if (cardImmediate.iEffect == 1)
        {
            //Gain score
            return  "This card gives\n" +
                    "you 50 points!";
        }
        else if (cardImmediate.iEffect == 2)
        {
            //Gain HP
            return  "This card heals\n" +
                    "you for 10 HP!";
        }
        else
        {
            return "NA";
        }
    }
}
