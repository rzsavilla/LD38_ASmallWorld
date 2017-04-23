using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public LevelGenerator levelGenerator;

    public int iPlayerHP = 100;
    public int iPlayerMaxHP = 100;
    public int iScore = 0;
    public int iNumEffects = 3;
    public int iNumCards = 3;
    private List<Card> cards;
    public int iCurrentCard;

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
            //1st time setup, only played at beginning of game
            instance = this;

            iCurrentCard = 1;

            cards = new List<Card>();
            //Generate an empty hand of cards
            for (int i = 0; i < iNumCards; i++)
            {
                Card newCard = new Card();
                cards.Add(newCard.EmptyCard());
            }
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();

        levelGenerator = GetComponent<LevelGenerator>();
    }

    public List<Card> GetCards()
    {
        return cards;
    }

    public void SetCards(List<Card> newCards)
    {
        cards = newCards;
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
        //Update all enemies on screen
		for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].DecSkipMove();
            if (enemies[i].iSkipMove <= 0)
            {
                enemies[i].MoveEnemy();
            }
        }
        
        ////////////////////
        //Input manager. Keys are checked here, and the desired effect, or the desired
        //effect in another class is called
        ////////////////////

        //Movement (Only with WASD) (Will always be called, for animations sake)
        player.Movement(
                Input.GetKey(KeyCode.W),
                Input.GetKey(KeyCode.A),
                Input.GetKey(KeyCode.S),
                Input.GetKey(KeyCode.D));

        //Switching Cards (with Q and E)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            player.SwitchCard(KeyCode.Q);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            player.SwitchCard(KeyCode.E);
        }

        //Switching Cards (with numbers)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            player.SwitchCard(KeyCode.Alpha1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            player.SwitchCard(KeyCode.Alpha2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            player.SwitchCard(KeyCode.Alpha3);
        }

        //Attack with Hook
        if (Input.GetKeyDown(KeyCode.J))
        {
            player.Attack();
        }
        //Return with Hook
        if (Input.GetKeyUp(KeyCode.J))
        {
            player.ReturnHook();
        }

        //Use Card
        if (Input.GetKeyDown(KeyCode.K))
        {
            //Return true if the usage is successful
            if (player.UseCard())
            {

            }
            //Returns false if card is unusable or none-existent
            else
            {

            }
        }

        UpdateCamera();

        //Reload Level
        //Will be removed when done
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InitGame();
        }
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    private void UpdateCamera()
    {
        //Update camera to follow player
        //Must be done AFTER player moves
        if (player != null && activeCamera != null)
        {
            Vector3 newPos = player.transform.position;
            newPos.z = activeCamera.transform.position.z;
            activeCamera.transform.position = newPos;
        }
    }
}
