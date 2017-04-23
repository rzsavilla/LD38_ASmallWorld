using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    public LevelGenerator levelGenerator;

    public int iPlayerHP = 10;
    public int iScore = 0;

    private Text levelText;
    private int iLevel = 0;
    private List<Enemy> enemies;
    private bool doingSetup;

    private Player player;
    private GameObject activeCamera;

    // Use this for initialization
    void Awake () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();

        levelGenerator = GetComponent<LevelGenerator>();
    }

    //This is called each time a scene is loaded.
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //Add one to our level number
        iLevel++;

        //Call InitGame to initialize our level
        InitGame();
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene
        //TerrainChangedFlags event AspectRatioFitter soon as this script is enabled
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene
        //change event as soon as this script is disabled
        //Remember to always have an unsubscription for every delegate you subscribe to
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void InitGame()
    {
        //doingSetup = true;

        //levelImage = GameObject.Find("LevelImage");
        //levelText = GameObject.Find("LevelText").GetComponent<Text>();
        //levelText.text = "Day " + level;
        //levelImage.SetActive(true);
        //Invoke("HideLevelImage", levelStartDelay);
        enemies.Clear();
        levelGenerator.SetupLevel();
        placeGameObjects();
        activeCamera = GameObject.Find("Main Camera");
    }
    void placeGameObjects()
    {
        //Place Player character onto traversable space
        player = GameObject.Find("Player").GetComponent<Player>();
        player.transform.position = levelGenerator.getPlayerStartPos();
    }

    private void HideLevelImage()
    {
        //levelImage.SetActive(false);
        //doingSetup = false;
    }

    public void GameOver()
    {
        //levelText.text = "After " + level + " days, you starved.";
        //levelImage.SetActive(true);
        //enabled = false;
    }

    // Update is called once per frame
    void Update () {
		for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].DecSkipMove();
            if (enemies[i].iSkipMove <= 0)
            {
                enemies[i].MoveEnemy();
            }
        }

        //Update camera to follow player
        if (player != null && activeCamera != null)
        {
            Vector3 newPos = player.transform.position;
            newPos.z = activeCamera.transform.position.z;
            activeCamera.transform.position = newPos;
        }


        //Reload Level
        if (Input.GetKeyDown("space"))
        {
            InitGame();
        }
	}

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }
}
