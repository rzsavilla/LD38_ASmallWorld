using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class Passive : MonoBehaviour {
    
    public int iRarity;
    public int iEffect;
    public float fEffectAmount;
    public string sEffectName;
    public string sEffectText;

    //Total number of passive effects (Manually Change)
    private int iNumPassive = 6;

    // Use this for initialization
    void Start()
    {
        
    }

    public void Generate()
    {
        //Recursive function, just in case can't get a workable integer
        SetPassiveEffect();

        //Load Text of the effect
        SetEffectText();
    }

        void SetPassiveEffect()
    {
        iEffect = (int)Random.Range(0f, iNumPassive - float.Epsilon);
        if (iEffect >= iNumPassive)
        {
            SetPassiveEffect();
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
            else if (iEffect == 1 || iEffect == 2 || iEffect == 3 || iEffect == 4 || iEffect == 5)
            {
                fEffectAmount = (float)Math.Floor(Random.Range(0, 4 - float.Epsilon));

                //1 - Max HP - (5, 10, 15, 20)
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
                //2 - Max HP Decrease - (-5, -10, -15, -20)
                else if (iEffect == 2)
                {
                    if ((int)fEffectAmount == 0)
                        iRarity = -1;
                    else if ((int)fEffectAmount == 1)
                        iRarity = -2;
                    else if ((int)fEffectAmount == 2)
                        iRarity = -3;
                    else if ((int)fEffectAmount == 3)
                        iRarity = -4;
                }
                //3 - Score Multiplier - (0.8, 0.9, 1.1, 1.2)
                else if (iEffect == 3)
                {
                    if ((int)fEffectAmount == 0)
                        iRarity = -3;
                    else if ((int)fEffectAmount == 1)
                        iRarity = -1;
                    else if ((int)fEffectAmount == 2)
                        iRarity = 1;
                    else if ((int)fEffectAmount == 3)
                        iRarity = 3;
                }
                //4 - HP on level complete - (-3, -1, 1, 3)
                //5 - Score on level complete - (-50, -20, 20, 50)
                else if (iEffect == 4 || iEffect == 5)
                {
                    if ((int)fEffectAmount == 0)
                        iRarity = -5;
                    else if ((int)fEffectAmount == 1)
                        iRarity = -2;
                    else if ((int)fEffectAmount == 2)
                        iRarity = 2;
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
            sEffectText = "This card has no passive effect!";
        }
        else if (iEffect == 1)
        {
            sEffectName = "Max HP ";
            sEffectText = "Your Max HP increases by ";
        }
        else if (iEffect == 2)
        {
            sEffectName = "Max HP ";
            sEffectText = "Your Max Hp decreases by ";
        }
        else if (iEffect == 3)
        {
            sEffectName = "Score Multiplier ";
            sEffectText = "You have a score multiplier of ";
        }
        else if (iEffect == 4)
        {
            sEffectName = "HP Recovery ";
            sEffectText = "When you complete a level, your HP recovers by ";
        }
        else if (iEffect == 5)
        {
            sEffectName = "Bonus Score ";
            sEffectText = "When you complete a level, your Score increases by ";
        }
    }

}
