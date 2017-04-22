using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour {

    public int iLevelWidth;
    public int iLevelHeight;

    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] collectables;
    public GameObject[] enemies;
    private Transform levelHolder;  //!< Level Position
    private List<Vector3> gridPositions = new List<Vector3>();   //!< List of active positions on the level

    private List<int> levelGrid = new List<int>();

    //CA Variables
    public float fRockPercent;
    public int iCAIterations;
    public int iRockDefinition;
    public int iMooreNeighbouhood;

    //! Add grid positions to list
    void InitializeList()
    {
        gridPositions.Clear();
        for (int x = 0; x < iLevelWidth; x++)
        {
            for (int y = 0; y < iLevelHeight; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        } 
    }

    private int getIndex(int x, int y, int width)
    {
        return x + (y * width);
    }

    private void generateRandGrid()
    {
        InitializeList();
        for (int i = 0; i < (iLevelWidth  * iLevelHeight); i++) {
            int iChance = Random.Range(0, 100);
            int iState = 0; //floor
            if (iChance <= fRockPercent) {
                iState = 1; //Rock
            }
            levelGrid.Add(iState);  //!< States floor (0) space and solid (1)
            Debug.Log("State:" + iState);
        }
    }

    private void placeObject(GameObject[] objectArray,Vector3 position)
    {
        //Place Random game object from within array -- provides variation --
        //int iRandIndex = Random.Range(0, objectArray.Length - 1);
        GameObject objectChoice = objectArray[0];
        Instantiate(objectChoice, position, Quaternion.identity);
        Debug.Log(position);
    }

    private void buildLevel()
    {
        for (int i = 0; i < (iLevelWidth * iLevelHeight); i++)
        {
           if (levelGrid[i] == 0)
            {
                //Place floor tile
                placeObject(floorTiles, gridPositions[i]);
            }
           else if (levelGrid[i] == 1)
            {
                //Place wall tile
                placeObject(wallTiles, gridPositions[i]);
            }
        }
    }

    public void CreateLevel()
    {

        generateRandGrid();
        buildLevel();
        Debug.Log("Level Created");
    }

	// Use this for initialization
	void Start () {
        CreateLevel();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
