using System.Collections;
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

    [HideInInspector]
    public List<GameObject> objectsHandle = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> enemyHandle = new List<GameObject>();

    private List<Vector3> traversablePositions = new List<Vector3>();    //!< List of traversable positions

    public List<int> levelGrid = new List<int>();
    public List<List<int>> cavesList = new List<List<int>>();

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

        ////Force fill empty -- TEST
        //for (int i = 0; i <= ((iLevelWidth) * (iLevelHeight)) - 1; i++)
        //{
        //    levelGrid[i] = 0;
        //}

        ////Force fill pattern border
        //for (int i = 0; i <= ((iLevelWidth) * (iLevelHeight)) - 1; i++)
        //{
        //    Vector2 coords = getCoord(i, iLevelWidth);
        //    if (coords.y == 5) levelGrid[i] = 1;
        //    if (coords.x == 2) levelGrid[i] = 1;
        //}

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

    public List<int> iVisited = new List<int>();

    bool isVisited(int index)
    {
        //Check if index has been visited
        for (int i = 0; i < iVisited.Count; i++)
        {
            if (iVisited[i] == index) return true;
        }
        return false;
    }

    public int iLargestCave = 0;
    public int iNumCaves = 0;

    bool isWithinBounds(Vector2 pos)
    {
        if (pos.x < 0) return false;
        else if (pos.x > iLevelWidth - 1) return false;
        else if (pos.y < 0) return false;
        else if (pos.y > iLevelHeight - 1) return false;
        else return true;
    }

    bool isWithinBounds(int x, int y)
    {
        if (x < 0) return false;
        else if (x > iLevelWidth - 1) return false;
        else if (y < 0) return false;
        else if (y > iLevelHeight - 1) return false;
        else return true;
    }

    bool isWithinBounds(int index)
    {
        Vector2 pos = getCoord(index, iLevelWidth);
        if (pos.x < 0) return false;
        else if (pos.x > iLevelWidth - 1) return false;
        else if (pos.y < 0) return false;
        else if (pos.y > iLevelHeight - 1) return false;
        else return true;
    }

    void floodFill()
    {
        List<List<int>> iCaves = new List<List<int>>();   //Unconnected Caves
        iCaves.Clear();
        //Loop through level grid
        for (int x = 0; x < iLevelWidth; x++)
        {
            for (int y = 0; y < iLevelHeight; y++)
            {
                int index = getIndex(x, y, iLevelWidth);
                if (levelGrid[index] == 0)  //Unvisited
                {
                    List<int> cave = new List<int>();         //Unconnected caves //Open List
                    List<int> totalCaves = new List<int>();
                    cave.Add(index);
                    totalCaves.Add(index);
                    //levelGrid[index] = 2;        //Visited
                    while (cave.Count > 0)
                    {
                        int iCurrent = cave[0];
                        levelGrid[iCurrent] = 2;        //Visited
                        cave.RemoveAt(0);

                        int iCurrIndex = -1;

                        Vector2 p = getCoord(iCurrent, iLevelWidth);
                        int iX = (int)p.y;
                        int iY = (int)p.x;

                        //Add Adjacent/Neighbour cells
                        if (getAdjacent(iX, iY, "right", ref iCurrIndex))
                        {
                            if (levelGrid[iCurrIndex] == 0)
                            {
                                if (!isInList(iCurrIndex, cave))
                                {
                                    cave.Add(iCurrIndex);
                                    totalCaves.Add(iCurrIndex);
                                }
                            }
                        }
                        if (getAdjacent(iX, iY, "up", ref iCurrIndex))
                        {
                            if (levelGrid[iCurrIndex] == 0)
                            {
                                if (!isInList(iCurrIndex, cave))
                                {
                                    cave.Add(iCurrIndex);
                                    totalCaves.Add(iCurrIndex);
                                }
                            }
                        }
                        if (getAdjacent(iX, iY, "down", ref iCurrIndex))
                        {
                            if (levelGrid[iCurrIndex] == 0)
                            {
                                if (!isInList(iCurrIndex, cave))
                                {
                                    cave.Add(iCurrIndex);
                                    totalCaves.Add(iCurrIndex);
                                }
                            }
                        }
                        if (getAdjacent(iX, iY, "left", ref iCurrIndex))
                        {
                            if (levelGrid[iCurrIndex] == 0)
                            {
                                if (!isInList(iCurrIndex, cave))
                                {
                                    cave.Add(iCurrIndex);
                                    totalCaves.Add(iCurrIndex);
                                }
                            }
                        }
                    }
                    iCaves.Add(totalCaves); //Add Cave
                }
            }
        }

        iNumCaves = iCaves.Count;

        iLargestCave = -999;
        int iLargestIndex = -1;
        //--Get Largest Cave--//
        for (int i = 0; i < iCaves.Count; i++)
        {
            for (int j = 0; j < iCaves[i].Count; j++)
            {
                if (iCaves[i][j] >= iLargestCave)
                {
                    iLargestIndex = i;
                    iLargestCave = iCaves[i][j];
                }
            }
        }
        
        if (iLargestIndex > -1)
        {
            //Check minumum traversable
            int iMinTraversable = (int)(levelGrid.Count * 0.1);
            if (iCaves[iLargestIndex].Count > iMinTraversable)
            {
                //Fill in smaller caves
                for (int i = 0; i < iCaves.Count; i++)
                {
                    if (i != iLargestIndex)
                    {
                        for (int j = 0; j < iCaves[i].Count; j++)
                        {
                            levelGrid[iCaves[i][j]] = 1;      //Turn into wall
                        }
                    }
                }
            }
            else
            {
                //Rebuild
                SetupLevel();
            }
        }
        else
        {
            //Rebuild
                SetupLevel();
        }
    }

    bool getAdjacent(int x, int y, string direction, ref int index)
    {
        int iX = x;
        int iY = y;
        
        if (direction == "up") iY += 1;
        else if (direction == "down") iY -= 1;
        else if (direction == "right") iX += 1;
        else if (direction == "left") iX -= 1;
        
        
        if (isWithinBounds(iX,iY)) {
            int i = getIndex(iX, iY, iLevelWidth);
            if (levelGrid[i] == 0)  //Unvisited cell
            {
                index = i;
                return true;
            }
            return false;
        }
        else {
            index = -1;
            return false;
        }
    }

    int getAdjacent(int index,string direction)
    {
        int iAdjacent = -1;
        Vector2 v = getCoord(index, iLevelWidth);
        int i = -1;
        if (direction == "up") v.y += 1;
        else if (direction == "down") v.y -= 1;
        else if (direction == "right") v.x += 1;
        else if (direction == "left") v.x -= 1;
        
        if (isWithinBounds(v))
        {
            i = getIndex((int)v.x, (int)v.y, iLevelWidth);
            if (levelGrid[i] == 0) iAdjacent = i;
        }

        return iAdjacent;
    }

    bool isInList(int integer, List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (integer == list[i]) return true;
        }
        return false;
    }

    void placeObject(GameObject[] objectArray,Vector3 position)
    {
        //Place Random game object from within array -- provides variation --
        if (objectArray.Length > 1)
        {
            int iRandIndex = Random.Range(0, objectArray.Length - 1);
            GameObject objectChoice = objectArray[iRandIndex];
            if (objectChoice != null)
            {
                objectsHandle.Add(Instantiate(objectChoice, position, Quaternion.identity));
            }
        }
        else if (objectArray.Length > 0)
        {
            GameObject objectChoice = objectArray[0];
            if (objectChoice != null)
            {
                objectsHandle.Add(Instantiate(objectChoice, position, Quaternion.identity));
            }
        }
    }

    void placeEnemy(GameObject[] objectArray, Vector3 position)
    {
        //Place Random game object from within array -- provides variation --
        if (objectArray.Length > 1)
        {
            int iRandIndex = Random.Range(0, objectArray.Length - 1);
            GameObject objectChoice = objectArray[iRandIndex];
            if (objectChoice != null)
            {
                enemyHandle.Add(Instantiate(objectChoice, position, Quaternion.identity));
            }
        }
        else if (objectArray.Length > 0)
        {
            GameObject objectChoice = objectArray[0];
            if (objectChoice != null)
            {
                enemyHandle.Add(Instantiate(objectChoice, position, Quaternion.identity));
            }
        }
    }

    void buildLevel()
    {
        //Destroy created objects in the scene
        for (int i = 0; i < objectsHandle.Count; i++)
        {
            Destroy(objectsHandle[i]);
        }
        objectsHandle.Clear();

        //Destroy created enemies in the scene
        for (int i = 0; i < enemyHandle.Count; i++)
        {
            Destroy(enemyHandle[i]);
        }
        enemyHandle.Clear();

        traversablePositions.Clear();
        for (int i = 0; i < (iLevelWidth * iLevelHeight); i++)
        {
           if (levelGrid[i] == 2)
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
            int i = Random.Range(0, traversablePositions.Count - 1);
            traversablePositions.RemoveRange(i, 1);  //Remove position - so no other object can be placed there 
            return traversablePositions[i];
        }
        else return new Vector3(-1f, 0f, 0f);
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
        if (hasTraversable())
        {
            playerStartPos = getRandomTraversable();
        }

        //Place num Enemies
        for (int i = 0; i < iEnemyCount; i++)
        {
            placeEnemy(enemies, getRandomTraversable());
        }

        //Place num Pickups
        for (int i = 0; i < iPickupCount; i++)
        {
            placeObject(pickups, getRandomTraversable());
        }
    }

    public void SetupLevel()
    {
        generateRandGrid(); //Fill grid with random states
        applyCA();          //Apply Cellular automata to the random grid
        floodFill();
        buildLevel();       //Create object instances
        placeObjects();     //Place pickups enemies etc
    }
}
