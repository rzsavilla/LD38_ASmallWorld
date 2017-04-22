using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class Immediate : MonoBehaviour {
    
    public int iRarity;
    public int iEffect;
    public float fEffectAmount;
    public string sEffectName;
    public string sEffectText;

    //Total number of immediate effects (Manually Change)
    private int iNumPassive = 3;

	// Use this for initialization
	void Start ()
    {
        
    }

    public void Generate()
    {
        //Recursive function, just in case can't get a workable integer
        SetImmediateEffect();

        //Load Text of the effect
        SetEffectText();
    }

    void SetImmediateEffect()
    {
        iEffect = (int)Random.Range(0f, iNumPassive - float.Epsilon);
        if (iEffect >= iNumPassive)
        {
            SetImmediateEffect();
        }
        else
        {
            //Setup effect amount, and the rarity of the effect (-5 to 5)

            //0 - No Effect!
            if (iEffect == 0)
            {
                fEffectAmount = 0;
                iRarity = 0;
            }
            //Integer between 0-3 
            else if (iEffect == 1 || iEffect == 2)
            {
                fEffectAmount = (float)Math.Floor(Random.Range(0, 4 - float.Epsilon));

                //1 - Gain HP - (10, 20, 30, 40)
                if (iEffect == 1)
                {
                    if ((int)fEffectAmount == 0)
                        iRarity = 1;
                    else if ((int)fEffectAmount == 1)
                        iRarity = 2;
                    else if ((int)fEffectAmount == 2)
                        iRarity = 3;
                    else if ((int)fEffectAmount == 3)
                        iRarity = 4;
                }
                //2 - Gain Score - (50, 100, 200, 250)
                else if (iEffect == 2)
                {
                    if ((int)fEffectAmount == 0)
                        iRarity = 1;
                    else if ((int)fEffectAmount == 1)
                        iRarity = 2;
                    else if ((int)fEffectAmount == 2)
                        iRarity = 4;
                    else if ((int)fEffectAmount == 3)
                        iRarity = 5;
                }
            }
            
        }
    }

    void SetEffectText()
    {
        if (iEffect == 0)
        {
            sEffectName = "Nothing!";
            sEffectText = "This card has no immediate effect!";
        }
        else if (iEffect == 1)
        {
            sEffectName = "HP Potion ";
            sEffectText = "Your HP recovers by ";
        }
        else if (iEffect == 2)
        {
            sEffectName = "Score Potion ";
            sEffectText = "Your Score increases by ";
        }
    }

}
