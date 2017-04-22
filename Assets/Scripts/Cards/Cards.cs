using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cards : MonoBehaviour {

    //The immediate and passive card effect of this one card
    public Immediate cardImmediate;
    public Passive cardPassive;
    public int iRarity;

    public bool bSet = false;

    // Use this for initialization
    void Awake()
    {
        bSet = false;
    }

    public void GenerateCard()
    {
        bSet = true;

        //Generate the new card effects
        //TODO pass the level number of difficulty to change the card?
        cardImmediate = new Immediate();
        cardPassive = new Passive();
        cardImmediate.Generate();
        cardPassive.Generate();

        iRarity = cardImmediate.iRarity + cardPassive.iRarity;
    }

}
