﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [System.Serializable]
    public class Count
    {
        public int min;
        public int max;
        public Count(int mn,int mx)
        {
            min = mn;
            max = mx;
        }
    }

    public int iLevelWidth = 10;
    public int iLevelHeight = 10;

    public int iPickupCount = 1;
    public int iEnemyCount = 1;

    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] pickups;
    public GameObject[] enemies;
    private Transform levelHolder;  //!< Level Position
    private List<Vector3> gridPositions = new List<Vector3>();   //!< List of active positions on the level

    private List<Vector3> traversablePositions = new List<Vector3>();    //!< List of traversable positions

    public List<int> levelGrid = new List<int>();

    private Vector3 playerStartPos = new Vector3(0f,0f,0f);

    //CA Variables
    public float fRockPercent = 50;
    public int iCAIterations = 3;
    public int iRockDefinition = 5;
    public int iMooreNeighbouhood = 1;

    //! Add grid positions to list
    void InitializeList()
    {
        gridPositions.Clear();
        for (int y = 0; y < iLevelHeight; y++)
        {
            for (int x = 0; x < iLevelWidth; x++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        } 
    }

    int getIndex(int x, int y, int width)
    {
        return x + (y * width);
    }

    Vector2 getCoord(int index, int width)
    {
        int x = (int)(index / width);   
        int y = index % width;
        Vector2 coord = new Vector2(x, y);
        return coord;
    }

    void generateRandGrid()
    {
        levelGrid.Clear();
        InitializeList();
        for (int i = 0; i < (iLevelWidth  * iLevelHeight); i++) {
            int iChance = Random.Range(0, 100);
            int iState = 0; //floor
            if (iChance <= fRockPercent) {
                iState = 1; //Rock
            }
            levelGrid.Add(iState);  //!< States floor (0) space and solid (1)
            //Debug.Log("State:" + iState);
        }
    }

    int calcNeighbourhood(int index)
    {
        //index = 0;
        int RangeX = iMooreNeighbouhood;
        int RangeY = iMooreNeighbouhood;
        
        Vector2 coord = getCoord(index, iLevelWidth);
        
        int x = (int)coord.x;
        int y = (int)coord.y;
        int startX = x - RangeX;
        int startY = y - RangeY;
        int endX = x + RangeX;
        int endY = y + RangeY;

        if (startX < 0) startX = 0;
        if (startY < 0) startY = 0;
        if (startX > iLevelWidth - 1) startX = iLevelWidth - 1;
        if (startY > iLevelHeight - 1) startY = iLevelHeight - 1;

        if (endX < 0) endX = 0;
        if (endY < 0) endY = 0;
        if (endX > iLevelWidth - 1) endX = iLevelWidth - 1;
        if (endY > iLevelHeight - 1) endY = iLevelHeight - 1;

        int iLiving = 0;

        for (int iX = startX; iX <= endX; iX++)
        {
            for (int iY = startY; iY <= endY; iY++)
            {
                if (levelGrid[getIndex(iX, iY, iLevelWidth)] == 1)
                {
                    iLiving++;
                }
            }
        }
        return iLiving;
    }

    void applyCA()
    {
        Debug.Log("Applying CA");
        int gridSize = (iLevelWidth * iLevelHeight) - 1;

        for (int iCounter = 0; iCounter < iCAIterations; iCounter++)
        {
            List<int> iLivingCount = new List<int>();
            //Calculate Neighbourhood value of each cell
            for (int j = 0; j < gridSize; j++)
            {
                int iLiving = calcNeighbourhood(j);
                iLivingCount.Add(iLiving);
            }

            //Change each cell based on its neighbourhood value
            for (int i = 0; i < gridSize; i++)
            {
                if (iLivingCount[i] <= iRockDefinition)
                {
                    levelGrid[i] = 1; //Wall
                }
                else
                {
                    levelGrid[i] = 0; //Floor
                }
            }
        }

        //Force outer border
        for (int i = 0; i <= ((iLevelWidth) * (iLevelHeight)) - 1; i++)
        {
            Vector2 coords = getCoord(i, iLevelWidth);
            if (coords.x == 0) levelGrid[i] = 1;
            if (coords.y == 0) levelGrid[i] = 1;
            if (coords.x == (iLevelWidth - 1)) levelGrid[i] = 1;
            if (coords.y == (iLevelHeight - 1)) levelGrid[i] = 1;
        }
    }

    void placeObject(GameObject[] objectArray,Vector3 position)
    {
        //Place Random game object from within array -- provides variation --
        //if (objectArray.Length > 0)
        //{
            int iRandIndex = Random.Range(0, objectArray.Length - 1);
            GameObject objectChoice = objectArray[iRandIndex];
            if (objectChoice != null)
            {
                Instantiate(objectChoice, position, Quaternion.identity);
            }
        //}
    }

    void buildLevel()
    {
        traversablePositions.Clear();
        for (int i = 0; i < (iLevelWidth * iLevelHeight); i++)
        {
           if (levelGrid[i] == 0)
            {
                //Place floor tile
                placeObject(floorTiles, gridPositions[i]);
                traversablePositions.Add(gridPositions[i]);
            }
           else if (levelGrid[i] == 1)
            {
                //Place wall tile
                placeObject(wallTiles, gridPositions[i]);
            }
           else
            {

            }
        }
    }

    //Check for available traversable positions
    public bool hasTraversable()
    {
        if (traversablePositions.Count > 0) return true;
        else return false;
    }

    public Vector3 getRandomTraversable()
    {
        if (hasTraversable())
        {
            int i = Random.Range(0, traversablePositions.Count);
            traversablePositions.RemoveRange(i, 1);  //Remove position - so no other object can be placed there 
            return traversablePositions[i];
        }
        else return new Vector3(-1f, -1f, 0f);
    }

    //! Return player start position
    public Vector3 getPlayerStartPos()
    {
        return playerStartPos;
    }

    //! Place Game objects - enemies/pickups
    public void placeObjects()
    {
        //Set Player start position
        playerStartPos = getRandomTraversable();

        //Place num Enemies
        for (int i = 0; i < iEnemyCount; i++)
        {
            placeObject(enemies, getRandomTraversable());
        }

        //Place num Pickups
        for (int i = 0; i < iPickupCount; i++)
        {
            placeObject(pickups, getRandomTraversable());
        }
    }

    public void SetupLevel()
    {
        Debug.Log("Creating Level");
        generateRandGrid(); //Fill grid with random states
        applyCA();          //Apply Cellular automata to the random grid
        buildLevel();       //Create object instances
        placeObjects();     //Place pickups enemies etc
        Debug.Log("Level Created");
    }
}
