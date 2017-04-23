using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Immediate {

    public int iRarity;
    public int iEffect;
    private int iNumEffects;

	// Use this for initialization
	void Start () {
		
	}

    public int GenerateEffect()
    {
        iNumEffects = GameManager.instance.iNumEffects;
        iEffect = (int)Random.Range(0, iNumEffects - float.Epsilon);

        //Set up rarity based on effect
        if (iEffect == 0)
        {
            iRarity = 0;
        }
        else if (iEffect == 1)
        {
            iRarity = 1;
        }
        else if (iEffect == 2)
        {
            iRarity = 2;
        }

        return iRarity;
    }



    ///////////////////
    //LIST OF EFFECTS//
    ///////////////////
    /*
    
    0 = No Effect!

    1 = Gain Score

    2 = Gain HP

    */
}
